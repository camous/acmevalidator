using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace acmevalidator
{
    public class Validator
    {
        enum Modifier
        {
            NoModifier = 0,
            Negation = '!',
            Like = '%',
            CaseInsensitive = '~'
        }

        public JObject Rules { get; internal set; }
        private readonly string requiredkeywork = "$required";
        private readonly string requiredornullkeywork = "$requiredOrNull";

        public Validator()
        {
        }

        public Validator(JObject rules)
        {
            Rules = rules;
        }

        public bool Validate(JObject input, bool ignoreRequired = false)
        {
            var errors = new Dictionary<JToken, JToken>();
            return Validate(input, Rules, out errors, ignoreRequired);
        }

        public bool Validate(JObject input, out Dictionary<JToken, JToken> errors, bool ignoreRequired = false)
        {
            return Validate(input, Rules, out errors, ignoreRequired);
        }

        public bool Validate(JObject input, JObject rules, bool ignoreRequired = false)
        {
            var errors = new Dictionary<JToken, JToken>();
            return Validate(input, rules, out errors, ignoreRequired);
        }

        public bool Validate(JObject input, JObject rules, out Dictionary<JToken, JToken> errors, bool ignoreRequired = false)
        {
            if (rules == null)
            {
                throw new Exception("No rules were defined");
            }

            var _rules = (JObject)rules.DeepClone();
            if (ignoreRequired)
            {
                RemoveRequiredProperties(_rules);
            }

            errors = new Dictionary<JToken, JToken>();

            var tokens = input.DescendantsAndSelf().OfType<JProperty>().Where(x => x.Value.Type != JTokenType.Object);
            foreach (var token in tokens)
            {
                // specific for check requiredkeywork at any depth, we look parent after parent until we find requiredkeywork
                JToken tokenrule = GetNextParentRequired(_rules, token);

                // prevents comparison of full first level Object (see tests nestedproperties)
                tokenrule = tokenrule ?? _rules.SelectToken(token.Path);

                if (token.Value.Type != JTokenType.Object || (tokenrule != null && tokenrule.Type == JTokenType.Null))
                {
                    if (tokenrule != null)
                    {
                        if (!ValidateToken(tokenrule, token))
                        {
                            errors.Add(
                                new JObject {
                                    {
                                        tokenrule.Path, tokenrule
                                    }},
                                new JObject
                                {
                                    {
                                        token.Path, token.Value
                                    }
                                });
                        }
                        // either matching or not, we remove the matched token
                        tokenrule.Parent.Remove();
                    }
                }
            }

            // all remaining JValue token left in the rules object means that it's missing
            foreach (var leftover in _rules.Descendants().OfType<JValue>())
                errors.Add(new JObject { { leftover.Path, leftover } }, null);

            return !errors.Any();
        }

        // loop from token to root to find any requiredkeyword, return null otherwise
        private JToken GetNextParentRequired(JObject rules, JToken token)
        {
            string path = token.Parent.Path;
            do
            {
                var parenttoken = rules.SelectToken(path);

                // if parent token is required, we return it
                if (parenttoken != null && parenttoken.Type == JTokenType.String && parenttoken.Value<string>() == requiredkeywork)
                    return parenttoken;

                // we loop one level up
                token = token.Parent.Parent;
                path = token?.Path;

            } while (!String.IsNullOrEmpty(path));

            return null;
        }

        public static bool HasAllTheRequiredProperties(Dictionary<JToken, JToken> errors)
        {
            bool hasAllTheRequiredProperties = true;
            if (errors.Count == 0)
            {
                hasAllTheRequiredProperties = true;
            }
            else
            {
                foreach (KeyValuePair<JToken, JToken> error in errors)
                {
                    if (error.Value == null)
                    {
                        hasAllTheRequiredProperties = false;
                        break;
                    }

                    foreach (JProperty child in error.Value.Children<JProperty>())
                    {
                        if (string.IsNullOrEmpty(child.Value.ToString()))
                        {
                            hasAllTheRequiredProperties = false;
                        }
                    }
                    if (!hasAllTheRequiredProperties)
                    {
                        break;
                    }
                }
            }
            return hasAllTheRequiredProperties;
        }

        private bool ValidateToken(JToken rule, JProperty input)
        {
            // get operator if existing
            Modifier modifier = Modifier.NoModifier;

            if (rule.Type == JTokenType.Object)
            {
                foreach (var modifiertype in Enum.GetValues(typeof(Modifier)).OfType<Modifier>())
                {
                    var modifierkey = ((char)modifiertype).ToString();

                    if (rule[modifierkey] != null)
                    {
                        modifier = modifiertype; // we found a modifier
                        rule = rule[modifierkey]; // we move the value up
                        break; // we stop searching for modifier
                    }
                }
            }

            if (rule.Type == JTokenType.Array) // supporting rule with array pattern ["FR","AT","DE"] (OR operator), AND operator if negation detected
            {
                var match = rule.Any(x => Equals(x, input.Value));
                return modifier == Modifier.Negation ? !match : match;
            }
            else
            {
                return ApplyModifier(rule, input.Value, modifier);
            }
        }

        private bool ApplyModifier(JToken rule, JToken input, Modifier modifier)
        {
            bool match;

            // we shouldn't have any json object at that point
            if (rule.Type == JTokenType.Object)
                return false;

            switch (input.Type)
            {
                case JTokenType.String:
                    if (rule.Type == JTokenType.Null && input.Type != JTokenType.Null)
                        match = false;
                    else
                        match = rule.Value<string>().Equals(input.Value<string>(), modifier == Modifier.CaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

                    // $required / $requiredOrnull
                    ApplyWildcardOperators(ref match, rule, input);

                    if (modifier == Modifier.Like)
                        match = input.Value<string>().Contains(rule.Value<string>());
                    break;

                case JTokenType.Null:
                    match = rule.Type == input.Type;

                    // $requiredornull
                    if (rule.Type == JTokenType.String && rule.Value<string>() == requiredornullkeywork)
                        match = true;
                    break;

                default:
                    match = JValue.DeepEquals(rule, input);

                    // $required / $requiredOrnull
                    ApplyWildcardOperators(ref match, rule, input);

                    break;
            }

            if (modifier == Modifier.Negation)
                match = !match;

            return match;
        }

        private void ApplyWildcardOperators(ref bool match, JToken rule, JToken input)
        {
            // $required
            if (rule.Value<string>() == requiredkeywork && input.Type != JTokenType.Null)
                match = true;

            // $requiredornull
            if (rule.Value<string>() == requiredornullkeywork && (input.HasValues || input.Type == JTokenType.Null))
                match = true;
        }

        private void RemoveRequiredProperties(JObject rules)
        {
            // if ignoring $required and $requiredOrNull, removing upfront from rules for not touching comparison methods
            var stringproperties = rules.DescendantsAndSelf()
                .OfType<JProperty>()
                .Where(x => x.Value.Type == JTokenType.String);

            foreach (var requiredtoken in stringproperties.Where(x =>
                    new string[] { requiredkeywork, requiredornullkeywork }.Contains(x.Value.Value<string>())).ToList())
                requiredtoken.Remove();
        }
    }
}
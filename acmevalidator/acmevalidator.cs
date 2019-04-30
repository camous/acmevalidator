using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace acmevalidator
{
    public class Validator
    {
        public JObject Rules { get; internal set; }
        private readonly string requiredkeywork = "$required";
        private readonly string requiredornullkeywork = "$requiredOrNull";

        public Validator()
        {
        }

        public Validator(JObject rules)
        {
            this.Rules = rules;
        }

        public bool Validate(JObject input, bool ignoreRequired = false)
        {
            var errors = new Dictionary<JToken, JToken>();
            return Validate(input, this.Rules, out errors, ignoreRequired);
        }

        public bool Validate(JObject input, out Dictionary<JToken, JToken> errors, bool ignoreRequired = false)
        {
            return Validate(input, this.Rules, out errors, ignoreRequired);
        }

        public bool Validate(JObject input, JObject rules, bool ignoreRequired = false)
        {
            var errors = new Dictionary<JToken, JToken>();
            return Validate(input, rules, out errors, ignoreRequired);
        }

        public bool Validate(JObject input, JObject rules, out Dictionary<JToken, JToken> errors, bool ignoreRequired = false)
        {
            if (rules == null)
                throw new Exception("No rules were defined");

            if (ignoreRequired)
                RemoveRequiredProperties(rules);

            errors = new Dictionary<JToken, JToken>();

            // today we get as input ACME format which is only 2 level deep, let's not over engineer for now
            foreach (var token in input.DescendantsAndSelf().OfType<JProperty>())
            {
                // prevents comparison of full first level Object (see tests nestedproperties)
                var tokenrule = rules.SelectToken(token.Path);
                if (token.Value.Type != JTokenType.Object || (tokenrule != null && tokenrule.Type == JTokenType.Null))
                {
                    if (tokenrule != null)
                    {
                        if (!ValidateToken(tokenrule, token))
                            errors.Add(tokenrule.Parent, token);

                        // either matching or not, we remove the matched token
                        tokenrule.Parent.Remove();
                    }
                }
            }

            foreach (var leftover in rules.OfType<JProperty>())
                if (leftover.Value.Count() != 0 || leftover.Value.Type != JTokenType.Object)
                    errors.Add(leftover, null);

            return !errors.Any();
        }

        private bool ValidateToken(JToken rule, JProperty input)
        {
            if (rule.Type == JTokenType.Array) // supporting rule with array pattern ["FR","AT","DE"] (OR operator)
            {
                if (!rule.Any(x => JToken.Equals(x, input.Value)))
                    return false;
            }
            else
            {
                if (!JToken.DeepEquals(rule.Parent, input)) // either object deep clone is equal
                {
                    if (rule.Value<string>() == requiredkeywork && input.HasValues && input.Value.Type != JTokenType.Null) // if rule is $required, input need value and not being null
                            return true;
                    else if (rule.Value<string>() == requiredornullkeywork && input.HasValues)
                            return true;
                    else
                        return false;
                }

            }

            return true;
        }

        private void RemoveRequiredProperties(JObject rules)
        {
            // if ignoring $required and $requiredOrNull, removing upfront from rules for not touching comparison methods
            var stringproperties = rules.DescendantsAndSelf()
                .OfType<JProperty>().
                Where(x => x.Value.Type == JTokenType.String);

            foreach (var requiredtoken in stringproperties.Where(x =>
                    new string[] { requiredkeywork, requiredornullkeywork }.Contains(x.Value.Value<string>())).ToList())
                requiredtoken.Remove();
        }
    }
}

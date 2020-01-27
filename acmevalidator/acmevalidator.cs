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

            // today we get as input ACME format which is only 2 level deep, let's not over engineer for now
            foreach (var token in input.DescendantsAndSelf().OfType<JProperty>())
            {
                // prevents comparison of full first level Object (see tests nestedproperties)
                var tokenrule = _rules.SelectToken(token.Path);

                var notTokenrule = (tokenrule as JObject)?.Property("!")?.Value;
                tokenrule = notTokenrule ?? tokenrule; // if a negative token rule, we pass only the value but flag we are in not mode with notTokenrule != null

                if (token.Value.Type != JTokenType.Object || (tokenrule != null && tokenrule.Type == JTokenType.Null))
                {
                    if (tokenrule != null)
                    {
                        if (!ValidateToken(tokenrule, token, notTokenrule != null))
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
                        
                        if (notTokenrule != null)
                            tokenrule.Parent.Parent.Parent.Remove();
                        else
                            tokenrule.Parent.Remove();
                    }
                }
            }

            foreach (var leftover in _rules.OfType<JProperty>())
            {
                if (leftover.Value.Count() != 0 || leftover.Value.Type != JTokenType.Object)
                {
                    errors.Add(new JObject { { leftover.Path, leftover.Value } }, null);
                }
            }
            return !errors.Any();
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

        private bool ValidateToken(JToken rule, JProperty input, bool invert = false)
        {
            if (rule.Type == JTokenType.Array) // supporting rule with array pattern ["FR","AT","DE"] (OR operator)
            {
                if (!rule.Any(x => Equals(x, input.Value)))
                {
                    return invert;
                } else
                {
                    if (invert)
                        return false;
                }
            }
            else
            {
                if (!JToken.DeepEquals(rule, input.Value)) // either object deep clone is equal
                {
                    if (rule.Type == JTokenType.String)
                    {
                        if (rule.Value<string>() == requiredkeywork && input.HasValues
                            && input.Value.Type != JTokenType.Null
                            // if rule is $required, input need value and not being null
                            || rule.Value<string>() == requiredornullkeywork && input.HasValues)
                        {
                            return true;
                        } else
                        {
                            return invert;
                        }
                    }
                    else
                        return false;
                } else
                {
                    return !invert;
                }
            }
            return true;
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
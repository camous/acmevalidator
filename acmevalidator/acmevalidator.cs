using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace acmevalidator
{
    public class acmevalidator
    {
        public JObject Rules { get; internal set; }

        public acmevalidator()
        {
        }

        public acmevalidator(JObject rules)
        {
            this.Rules = rules;
        }

        public bool Validate(JObject input)
        {
            var errors = new Dictionary<JToken, JToken>();
            return Validate(input, this.Rules, out errors);
        }

        public bool Validate(JObject input, out Dictionary<JToken, JToken> errors)
        {
            return Validate(input, this.Rules, out errors);
        }

        public bool Validate(JObject input, JObject rules)
        {
            var errors = new Dictionary<JToken, JToken>();
            return Validate(input, rules, out errors);
        }

        public bool Validate(JObject input, JObject rules, out Dictionary<JToken, JToken> errors)
        {
            errors = new Dictionary<JToken, JToken>();

            // today we get as input ACME format which is only 2 level deep, let's not over engineer for now
            foreach (var token in input.DescendantsAndSelf().OfType<JProperty>())
            {
                // prevents comparison of full first level Object (see tests nestedproperties)
                if (token.Value.Type != JTokenType.Object)
                {
                    var tokenrule = rules.SelectToken(token.Path);
                    if (tokenrule != null)
                    {
                        if (!ValidateToken(tokenrule, token))
                            errors.Add(token, tokenrule.Parent);
                    }
                }
            }

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
                if (!JToken.DeepEquals(rule.Parent, input))
                    return false;
            }

            return true;
        }
    }
}

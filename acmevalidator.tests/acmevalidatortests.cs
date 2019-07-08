using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace acmevalidator.tests
{
    [TestClass]
    public class AcmevalidatorTests
    {
        [TestMethod]
        public void NullProperty()
        {
            var acmevalidator = new Validator();
            var input = new JObject { { "property", null } };
            var rule = new JObject { { "property", null } };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void NullPropertyAgainstObject()
        {
            var acmevalidator = new Validator();
            var input = new JObject { { "property", new JObject { { "subproperty", "value" } } } };
            var rule = new JObject { { "property", null } };
            _ = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(errors.Count == 1);
        }

        [TestMethod]
        public void NullPropertyAgainstNullObject()
        {
            var acmevalidator = new Validator();
            var input = new JObject { { "property", new JObject { { "subproperty", "value" } } } };
            var rule = new JObject { };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void NestedProperty()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject { { "country", "France" } }
            };
            var rule = new JObject
            {
                ["property"] = new JObject { { "country", "France" } }
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void NestedProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "country", "France" },
                    { "countryiso", "FR" },
                    { "region", "FBA" }
            }
            };

            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "country", "France" },
                    { "countryiso", "FR" } }
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void NestedPropertiesArray()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "country", "France" },
                    { "countryiso", "FR" },
                    { "region", "FBA" }
            }
            };

            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "country", "France" },
                    { "countryiso", new JArray("DE","FR") } }
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void MismatchNestedProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "country", "France" },
                    {"countryiso", "FR" } }
            };

            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "country", "France" },
                    { "countryiso", "DE" } }
            };

            Assert.IsFalse(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void PropertyArray()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                { "property", "FR" }
            };

            var rule = new JObject
            {
                { "property", new JArray("FR","AT")}
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void PropertyArrayInteger()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                { "property", 1 }
            };

            var rule = new JObject
            {
                { "property", new JArray(1,2,3)}
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void MismatchPropertyArrayInteger()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                { "property", 4 }
            };

            var rule = new JObject
            {
                { "property", new JArray(1,2,3)}
            };

            Assert.IsFalse(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void MismatchPropertyArray()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                { "property", "FR" }
            };

            var rule = new JObject
            {
                { "property", new JArray("DE","AT")}
            };

            Assert.IsFalse(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void MismatchNestedPropertyArray()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    {"countryiso", "FR" } }
            };

            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "countryiso",new JArray("DE","AT") } }
            };

            Assert.IsFalse(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void PropertyArrayMix()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                { "property", null }
            };

            var rule = new JObject
            {
                { "property", new JArray(null,false,3)}
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void MixNestedProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "country", "France" },
                    { "countryiso", "FR" },
                    { "region", "FBA" }
                },
                ["property2"] = 2
            };

            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "country", "France" },
                    { "countryiso", "FR" } },
                ["property2"] = 2
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void ErrorMessage()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    {"countryiso", "FR" } }
            };

            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "countryiso",new JArray("DE","AT") } }
            };

            acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(errors.Count != 0);
        }

        [TestMethod]
        public void TestCaseSensitive()
        {
            var acmevalidator = new Validator();
            var input = new JObject { { "property", "france" } };
            var rule = new JObject { { "property", "France" } };

            Assert.IsFalse(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void NoOneRules()
        {
            var acmevalidator = new Validator();

            Assert.ThrowsException<Exception>(() => acmevalidator.Validate(new JObject()));
        }

        [TestMethod]
        public void PartialMissingProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA","value" } },
                ["property2"] = "2"
            };
            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", "value" },
                    { "subpropertyB", "value" } },
                ["property2"] = "2",
                ["property3"] = "3"
            };

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);

            // property.subpropertyB & property3 are missing
            Assert.IsTrue(equals == false && errors.Count == 2);
        }

        [TestMethod]
        public void MissingProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA","value" },
                    { "subpropertyB", "value" }
                },
                ["property2"] = "2"
            };
            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", "value" },
                    { "subpropertyB", "value" } },
                ["property2"] = "2",
                ["property3"] = "3"
            };

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);

            // property.subpropertyB & property3 are missing
            Assert.IsTrue(equals == false && errors.Count == 1);
        }

        [TestMethod]
        public void RequiredProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA","whatever" },
                    { "subpropertyB", "value" }
                },
                ["property2"] = "whatever",
                ["property3"] = "3"
            };
            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", "$required" },
                    { "subpropertyB", "value" } },
                ["property2"] = "$required",
                ["property3"] = "3"
            };
            var equals = acmevalidator.Validate(input, rule, out _);

            // property.subpropertyB & property3 are missing
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void MissingRequiredProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyB", "value" }
                },
                ["property3"] = "3"
            };
            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", "$required" },
                    { "subpropertyB", "value" } },
                ["property2"] = "$required",
                ["property3"] = "3"
            };

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);

            // property.subpropertyB & property3 are missing
            Assert.IsTrue(equals == false && errors.Count == 2);
        }

        [TestMethod]
        public void RequiredMissingNonNullProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", null },
                    { "subpropertyB", "value" }
                },
                ["property2"] = null,
                ["property3"] = "3"
            };
            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", "$required" },
                    { "subpropertyB", "value" } },
                ["property2"] = "$required",
                ["property3"] = "3"
            };

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);

            // property.subpropertyB & property3 are missing
            Assert.IsTrue(equals == false && errors.Count == 2);
        }

        [TestMethod]
        public void RequiredMissingNonNullPropertiesIgnored()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyB", "value" }
                },
                ["property3"] = "3"
            };
            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", "$requiredOrNull" },
                    { "subpropertyB", "value" } },
                ["property2"] = "$required",
                ["property3"] = "3"
            };

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors, true);

            // property.subpropertyB & property3 are missing, but we don't care.
            Assert.IsTrue(equals == true && errors.Count == 0);
        }

        [TestMethod]
        public void RequiredNonNullProperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", null },
                    { "subpropertyB", "value" }
                },
                ["property2"] = null,
                ["property3"] = "3"
            };
            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "subpropertyA", "$requiredOrNull" },
                    { "subpropertyB", "value" } },
                ["property2"] = "$requiredOrNull",
                ["property3"] = "3"
            };
            var equals = acmevalidator.Validate(input, rule, out _);

            Assert.IsTrue(equals == true);
        }

        [TestMethod]
        public void NonRequiredRequiredFollow()
        {
            // we had a bug in version 2.2.0 where 2 tests of required field in a row were not consistent

            var rule = new JObject { { "property", "$required" } };
            var validator = new Validator(rule);

            var input1 = new JObject { { "property", "value" } };
            var input2 = new JObject();

            Assert.IsTrue(validator.Validate(input1));
            Assert.IsFalse(validator.Validate(input2, out _));
        }

        [TestMethod]
        public void NestedRequiredAgainstNull()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = null
            };
            var rule = new JObject
            {
                ["property"] = new JObject {
                    { "subproperty", "$required" }
                }
            };
            var equals = acmevalidator.Validate(input, rule, out _);
            Assert.IsTrue(equals == false);
        }

        [TestMethod]
        public void HasAllTheRequiredPropertiesMethod_IsMissingRequiredSubproperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", null },
                        { "subpropertyB", null },
                        { "subpropertyC", "value" }
                },
                ["property2"] = null
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", "$required" },
                        { "subpropertyB", "$required" },
                        { "subpropertyC", "$required" }
                },
                ["property2"] = "$required"
            };
            acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsFalse(acmevalidator.HasAllTheRequiredProperties(errors));
        }

        [TestMethod]
        public void HasAllTheRequiredPropertiesMethod_HasAllTheRequiredSubproperties()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", "value" },
                        { "subpropertyB", "value" },
                        { "subpropertyC", "value" }
                },
                ["property2"] = "value"
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", "$required" },
                        { "subpropertyB", "$required" },
                        { "subpropertyC", "$required" }
                },
                ["property2"] = "$required"
            };
            acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(acmevalidator.HasAllTheRequiredProperties(errors));
        }
    }
}
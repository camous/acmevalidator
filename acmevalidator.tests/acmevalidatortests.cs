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
        public void Level3NestedProperty()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject
                {
                    ["country"] = new JObject
                    {
                        ["city"] = "chaville"
                    },
                    ["subproperty"] = "hello"
                }
            };
            var rule = new JObject
            {
                ["property"] = new JObject
                {
                    ["country"] = new JObject
                    {
                        ["city"] = "chaville"
                    }
                }
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void MistmatchLevel3NestedArrayProperty()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject
                {
                    ["country"] = new JObject
                    {
                        ["city"] = "paris"
                    }
                }
            };
            var rule = new JObject
            {
                ["property"] = new JObject
                {
                    ["country"] = new JObject
                    {
                        ["city"] = new JArray { "chaville", "puteaux" }
                    }
                }
            };

            Assert.IsFalse(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void MismatchLevel3NestedProperty()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property"] = new JObject
                {
                    ["country"] = new JObject
                    {
                        ["city"] = "chaville"
                    }
                }
            };
            var rule = new JObject
            {
                ["property"] = new JObject
                {
                    ["country"] = new JObject
                    {
                        ["city"] = "puteaux",
                        ["city2"] = "puteaux"
                    }
                }
            };

            Assert.IsFalse(acmevalidator.Validate(input, rule));
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
        public void TestCaseInsensitive()
        {
            var acmevalidator = new Validator();
            var input = new JObject { { "property", "france" } };
            var rule = new JObject { ["property"] = new JObject { { "~", "France" } } };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
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
        public void RequiredOnIntegerw()
        {
            // test for critical regression on 2.6.0 where $required/$requiredOrNull not working on non string/null type

            var rule = new JObject { { "property", "$required" } };
            var validator = new Validator(rule);

            var input = new JObject { { "property", 10 } };

            Assert.IsTrue(validator.Validate(input));
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

            Assert.IsFalse(Validator.HasAllTheRequiredProperties(errors));
        }

        [TestMethod]
        public void LikeSimpleProperty()
        {
            var rule = new JObject
            {
                ["property"] = new JObject
                {
                    ["%"] = "fabien"
                }
            };
            var validator = new Validator(rule);

            var input1 = new JObject { { "property", "hello fabien" } };

            Assert.IsTrue(validator.Validate(input1));
        }

        [TestMethod]
        public void LikeSimplePropertyMismatch()
        {
            var rule = new JObject
            {
                ["property"] = new JObject
                {
                    ["%"] = "wrongfirstname"
                }
            };
            var validator = new Validator(rule);

            var input1 = new JObject { { "property", "hello fabien" } };

            Assert.IsFalse(validator.Validate(input1));
        }

        [TestMethod]
        public void LikeWithArray()
        {
            var rule = new JObject
            {
                ["property"] = new JObject
                {
                    ["%"] = new JArray { "fabien", "value" }
                }
            };
            var validator = new Validator(rule);

            var input = new JObject { { "property", "hello fabien" } };

            Assert.IsFalse(validator.Validate(input));
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

            Assert.IsTrue(Validator.HasAllTheRequiredProperties(errors));
        }

        [TestMethod]
        public void HasAllTheRequiredPropertiesMethod_RequiredFieldMissingFromInput()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", "value" },
                        { "subpropertyB", "value" },
                        { "subpropertyC", "value" }
                }
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

            Assert.IsFalse(Validator.HasAllTheRequiredProperties(errors));
        }

        [TestMethod]
        public void NotOperatorSimpleValue()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "value"
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "!", "value" }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotOperatorNullValue()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = null
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "!", "value" }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleValueSuccess()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "not_value"
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "!", "value" }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleIntegerValue()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = 1000
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "!", 1000 }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleIntegerValueSuccess()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = 999
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "!", 1000 }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleIntegerArray()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = 1000
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "!", new JArray
                            {
                        1000,
                        999
                        }
                    }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleIntegerArraySuccess()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = 999
            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "!", new JArray
                            {
                        1000,
                        998
                        }
                    }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleValueMany()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "not_value",
                ["property2"] = "value",

            };
            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        { "!", "value" }
                },
                ["property2"] = "value"
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleValueNested()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", "value" },
                        { "subpropertyB", "value" },
                        { "subpropertyC", "value" }
                }
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        {   "subpropertyA", new JObject {
                                { "!", "value" }
                            }
                        },
                        { "subpropertyB" , "value" }

                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleValueNestedSuccess()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", "value" },
                        { "subpropertyB", "value" }
                }
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        {   "subpropertyA", new JObject {
                                { "!", "not_value" }
                            }
                        }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleValueNestedSuccessMany()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", "value" },
                        { "subpropertyB", "value" },
                        { "subpropertyC", "value" }
                }
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                        {   "subpropertyA", new JObject {
                                { "!", "not_value" }
                            }
                        },
                        { "subpropertyB" , "value" }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleArray()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "value"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    { "!", new JArray
                    {
                        "value",
                        "value2"
                    } }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotOperatorSimpleArraySuccess()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "value"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    { "!", new JArray
                    {
                        "not_value1",
                        "not_value2"
                    } }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotOperatorNestedArray()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                        { "subpropertyA", "value" }
                }
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    ["subpropertyA"] = new JObject
                    { { "!", new JArray
                    {
                        "value",
                        "not_value2"
                    } } }
                }
            };
            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);

            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void TokenVersusProperty()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "hello"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    ["subpropertyA"] = "hello"
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotNullWithNull()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = null
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    ["!"] = null
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void NotNullWithNonNullString()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "non null"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    ["!"] = null
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotNullWithNonNullInteger()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = 10
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    ["!"] = null
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotNullObject()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                    ["subproperty"] = "hello"
                }
            };

            var rules = new JObject
            {
                ["property1"] = "$required"
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotNullObjectParent()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                    ["subproperty"] = new JObject
                    {
                        ["subsubproperty"] = "hello"
                    }
                }
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    ["subproperty"] = "$required"
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void NotNullObjectNested()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = new JObject
                {
                    ["subproperty"] = new JObject
                    {
                        ["subsubproperty"] = "hello"
                    }
                }
            };

            var rules = new JObject
            {
                ["property1"] = "$required"
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void SuperiorDateTrue()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "2021-10-30T10:00:00.000Z"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    [">"] = "2021-10-29T10:00:00.000Z"
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void SuperiorDateFalse()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "2021-10-28T10:00:00.000Z"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    [">"] = "2021-10-29T10:00:00.000Z"
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void InferiorDateFalse()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "2021-10-30T10:00:00.000Z"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    ["<"] = "2021-10-29T10:00:00.000Z"
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void InferiorDateTrue()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "2021-10-28T10:00:00.000Z"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    ["<"] = "2021-10-29T10:00:00.000Z"
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void FaultySuperiorDate()
        {
            var acmevalidator = new Validator();
            var input = new JObject
            {
                ["property1"] = "not a date"
            };

            var rules = new JObject
            {
                ["property1"] = new JObject
                {
                    [">"] = "still not a date"
                }
            };

            var equals = acmevalidator.Validate(input, rules, out Dictionary<JToken, JToken> errors);
            Assert.IsFalse(equals);
        }
    }
}
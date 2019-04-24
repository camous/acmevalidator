using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace acmevalidator.tests
{
    [TestClass]
    public class acmevalidatortests
    {
        [TestMethod]
        public void nullproperty()
        {
            var acmevalidator = new global::acmevalidator.Validator();
            var input = new JObject { { "property", null } };
            var rule = new JObject { { "property", null } };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void nullpropertyagainstobject()
        {
            var acmevalidator = new global::acmevalidator.Validator();
            var input = new JObject { { "property", new JObject { { "subproperty", "value" } } } };
            var rule = new JObject { { "property", null } };

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);

            Assert.IsTrue(errors.Count == 1);
        }

        [TestMethod]
        public void nullpropertyagainstnullobject()
        {
            var acmevalidator = new global::acmevalidator.Validator();
            var input = new JObject { { "property", new JObject { { "subproperty", "value" } } } };
            var rule = new JObject {  };
            
            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void nestedproperty()
        {
            var acmevalidator = new global::acmevalidator.Validator();
            var input = new JObject {
                ["property"] = new JObject { { "country", "France" } }
            };
            var rule = new JObject
            {
                ["property"] = new JObject { { "country", "France" } }
            };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void nestedproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void nestedpropertiesarray()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void mismatchnestedproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void propertyarray()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void propertyarrayinteger()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void mismatchpropertyarrayinteger()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void mismatchpropertyarray()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void mismatchnestedpropertyarray()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void propertyarraymix()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void mixnestedproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);
            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void errormessage()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void testcasesensitive()
        {
            var acmevalidator = new global::acmevalidator.Validator();
            var input = new JObject { { "property", "france" } };
            var rule = new JObject { { "property", "France" } };

            Assert.IsFalse(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void noonerules()
        {
            var acmevalidator = new global::acmevalidator.Validator();

            Assert.ThrowsException<Exception>(() => acmevalidator.Validate(new JObject()));
        }

        [TestMethod]
        public void partialmissingproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void missingproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void requiredproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);

            // property.subpropertyB & property3 are missing
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void missingrequiredproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void requiredmissingnonnullproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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
        public void requirednonnullproperties()
        {
            var acmevalidator = new global::acmevalidator.Validator();
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

            var equals = acmevalidator.Validate(input, rule, out Dictionary<JToken, JToken> errors);

            // property.subpropertyB & property3 are missing
            Assert.IsTrue(equals == true);
        }
    }
}

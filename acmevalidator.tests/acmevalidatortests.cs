using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace acmevalidator.tests
{
    [TestClass]
    public class acmevalidatortests
    {
        [TestMethod]
        public void nullproperty()
        {
            var acmevalidator = new global::acmevalidator.acmevalidator();
            var input = new JObject { { "property", null } };
            var rule = new JObject { { "property", null } };

            Assert.IsTrue(acmevalidator.Validate(input, rule));
        }

        [TestMethod]
        public void nestedproperty()
        {
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
            var acmevalidator = new global::acmevalidator.acmevalidator();
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
    }
}

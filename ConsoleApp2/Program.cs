using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var rule = JObject.Parse(System.IO.File.ReadAllText("json1.json"));
            var json = JObject.Parse(System.IO.File.ReadAllText("json2.json"));

            var equals = new acmevalidator.Validator().Validate(json, rule, out Dictionary<JToken,JToken> delta);
        }
    }
}

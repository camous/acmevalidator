[![Nuget](https://img.shields.io/nuget/v/acmevalidator.svg)](https://www.nuget.org/packages/acmevalidator)
# acmevalidator
basic json filtering &amp; query library

Provide ability to validate if a json object complies to a set of properties.

input
```json
{
    "firstname" : "Manon",
    "lastname" : "Dupont",
    "country" : "France"
}
```

rules
```json
{
    "country" : "France"
}
```

```csharp
    new acmevalidator().Validate(input, rule); // return True
```

`acmevalidator` supports several properties rule and nested objects (2 levels). All properties are forming `AND` operator.

input
```json
{
    "firstname" : "Manon",
    "lastname" : "Dupont",
    "location" : {
        "country" : "France",
        "office" : "Paris",
        "floor" : 13,
        "zipcode" : "75001"
    },
    "fulltime" : true
}
```

rules (location.country = `France` AND location.office = `Paris` AND fulltime = `true`)
```json
{
    "location" : {
        "country" : "France",
        "office" : "Paris"
    },
    "fulltime" : true
}
```

```csharp
    new acmevalidator().Validate(input, rule); // return True
```

`acmevalidator` supports `OR` operator for one or several properties with json array form

rules
```json
{
    "location" : {
        "country" : ["France", "Austria", "Germany"]
    },
    "fulltime" : true
}
```

```csharp
    new acmevalidator().Validate(input, rule); // return True
```

`acmevalidator` needs all rules provided to match in order to return a successful comparison
rules
```json
{
    "propertynotininput" : "value"
}
``` 

```csharp
    new acmevalidator().Validate(input, rule, out Dictionary<JToken,JToken> delta); // return False
    // delta : {"propertynotininput" : "value", null}
```

`acmevalidator` supports `$required` and `$requiredOrNull` for checking minimum set of fields

input
```json
{
    "lastname" : "Dupont",
    "prefixname" : null
}
```

rules
```json
{
    "firstname" : "$required",
    "lastname" : "$required",
    "prefixname" : "$requiredOrNull"
}
```

```csharp
    new acmevalidator().Validate(input, rule, out Dictionary<JToken,JToken> delta); // return False
    // delta : {"firstname" : "$required", null}
```

support for negation
```json
{
    "country" : {
        "!" : "France"
    }
}
```

```json
{
    "country" : {
        "!" : ["France", "Austria", "Romania"]
    }
}
```


Not implemented *yet*

* `or` operator for several properties

## Delta description

`Validate` methods return `false` if input json doesn't validate rule one. An optional parameter `out Dictionary<JToken,JToken> deltas` return all delta. 

* Key = input token
* Value = rule token

```csharp
    new acmevalidator().Validate(input, rule, out Dictionary<JToken, JToken> deltas); // return False
    foreach (var delta in deltas)
    {
        delta.Key // rule/expected { country = [Austria, Germany]}
        delta.Value // input { country = France }
    }
```
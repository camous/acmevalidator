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

`acmevalidator` supports several property rule and nested object (2 levels). All properties are forming `AND` operator.

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

Not implemented yet

* `not` operator support
* `or` operator for several properties

## Error description

`Validate` methods return `false` if input json doesn't validate rule one. An optional parameter `out Dictionary<JToken,JToken> errors` return all errors. 

* Key = input token
* Value = rule token

```csharp
    new acmevalidator().Validate(input, rule, out Dictionary<JToken, JToken> errors); // return False
    foreach (var error in errors)
    {
        error.Key // input { country = France }
        error.Value // rule/expected { country = [Austria, Germany]}
    }
```
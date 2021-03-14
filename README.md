[![Nuget](https://img.shields.io/nuget/v/acmevalidator.svg)](https://www.nuget.org/packages/acmevalidator)
# acmevalidator
simple json query library

Provide ability to validate if a json object complies to a set of properties. Focus is done on simplicity & readibility rather a full query language.

`acmevalidator` supports

* any json depth
* wildcard values ($required / $requiredOrNull)
* OR operator (array [])
* negation operator (!)
* contains operator (%)
* case insensitive operator (~)

operators can't be accumulated except OR.

input
```json
{
    "firstname" : "Manon",
    "lastname" : "Dupont",
    "country" : "France"
}
```

rule
```json
{
    "country" : "France"
}
```

```csharp
    new acmevalidator().Validate(input, rule); // returns True
```

`acmevalidator` supports several properties rule and nested objects. All properties are forming `AND` operator.

input
```json
{
    "firstname" : "Manon",
    "lastname" : "Dupont",
    "location" : {
        "country" : "France",
        "office" : "Paris",
        "floor" : 13,
        "address" : {
            "line1" : "1 rue de Rivoli",
            "line2" : null,
            "zipcode" : "75001"
        }
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
    new acmevalidator().Validate(input, rule); // returns True
```

## delta between rules & input

`Validate` methods return `false` if input json doesn't validate rules. An optional parameter `out Dictionary<JToken,JToken> deltas` return all delta usefull for troubleshooting and monitoring purpose. 

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

## operators
### OR operator ([])
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

### negation operator (!)

single value
```json
{
    "location" : {
        "country" : {
            "!" : "France"
        }
    }
}
```

several values : country NOT France `AND` country NOT Austria `AND` country NOT Romania
```json
{
    "location" : {
        "country" : {
            "!" : ["France", "Austria", "Romania"]
        }
    }
}
```

### wildcard values ($required|$requiredOrNull)

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
    new acmevalidator().Validate(input, rule, out Dictionary<JToken,JToken> delta); // returns False
    // delta : {"firstname" : "$required", null}
```

### contains operator (%)

contains is case sensitive following C# String.Contains default behavior. Operators can't be accumulated.

input
```json
{
    "lastname" : "Dupont"
}
```

rules
```json
{
    "firstname" : {
        "%" : "Dupo"
    }
}
```
 > returns true



### case insensitive operator (~)

input
```json
{
    "lastname" : "Dupont"
}
```

rules
```json
{
    "firstname" : {
        "~" : "dupont"
    }
}
```
> returns true
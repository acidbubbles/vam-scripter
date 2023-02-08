# Virt-A-Mate Scripter Plugin

A scripting engine to write some code inside Virt-A-Mate without having to write a new plugin. The language is greatly inspired from JavaScript.

## Language Features

- Variables (`var`, `let`)
- Types (`string`, `float`, `int`, `bool`, `undefined`, `function`)
- Boolean operators (`||`, `&&`, `==`, `!=`, `<`, `>`, `<=`, `>=`)
- Math operators (`+`, `-`, `*`, `/`, `%`)
- String concatenation (`+`)
- Assignment operators (`++`, `--`, `+=`, `-=`, `*=`, `/=`)
- Comments (`//`, `/* */`)
- Keywords (`if`, `else`, `for`, `while`, `throw`)
- Static variables (`static`)
- Functions (`function`, `return`)
- Arrays (`var x = []`, `var x = [values]`, `x.add`, `x[index]`, `x.length`)
- Code blocks and lexical scopes (`{`, `}`)
- DateTime (`DateTime.now.format("")`)

## Notable Omissions

- No support for dictionaries (maps)
- No `break` and `continue` statements
- No `foreach`
- Code blocks without brackets, like `if(condition) something();`
- Variable values are locally scoped, but you cannot reuse them in other functions of the same script

## Globals

- [`console`](#console)
- [`scene`](#scene)
- [`Time`](#time)
- [`Random`](#random)
- [`DateTime`](#dateTime)

## Classes

### `Console`

| Property | Type                       | Notes                                             |
|----------|----------------------------|---------------------------------------------------|
| `log`    | function(string) => `void` | Uses `SuperController.LogMessage`                 |
| `error`  | function(string) => `void` | Uses `SuperController.LogError`                   |
| `clear`  | function() => `void`       | Uses `SuperController.singleton.ClearMessages();` |

### `Scene`

| Property | Type                                | Notes      |
|----------|-------------------------------------|------------|
| getAtom  | function(string) => [`Atom`](#atom) | Atom by ID |

### `Atom`

| Property    | Type                                        | Notes          |
|-------------|---------------------------------------------|----------------|
| getStorable | function(string) => [`Storable`](#storable) | Storable by ID |

### `Storable`

| Property         | Type                                                            | Notes                          |
|------------------|-----------------------------------------------------------------|--------------------------------|
| getFloat         | function(string) => [`FloatParam`](#floatParam)                 | Param trigger by ID            |
| getString        | function(string) => [`StringParam`](#stringParam)               | Param trigger by ID            |
| getBool          | function(string) => [`BoolParam`](#boolParam)                   | Param trigger by ID            |
| getStringChooser | function(string) => [`StringChooserParam`](#stringChooserParam) | Param trigger by ID            |
| trigger          | function(string) => `void`                                      | Invoke an action trigger by ID |

### `FloatParam`

| Property | Type  | Notes           |
|----------|-------|-----------------|
| val      | float | The param value |

### `StringParam`

| Property | Type   | Notes           |
|----------|--------|-----------------|
| val      | string | The param value |

### `BoolParam`

| Property | Type | Notes           |
|----------|------|-----------------|
| val      | bool | The param value |

### `StringChooserParam`

| Property | Type   | Notes           |
|----------|--------|-----------------|
| val      | string | The param value |

### `Time`

| Property    | Type  | Notes                                                                          |
|-------------|-------|--------------------------------------------------------------------------------|
| `time`      | float | [Time.time](https://docs.unity3d.com/ScriptReference/Time-time.html)           |
| `deltaTime` | float | [Time.deltaTime](https://docs.unity3d.com/ScriptReference/Time-deltaTime.html) |

### `Random`

| Property | Type                            | Notes                                                                      |
|----------|---------------------------------|----------------------------------------------------------------------------|
| `value`  | float                           | [Random.value](https://docs.unity3d.com/ScriptReference/Random-value.html) |
| `Range`  | function(float, float) => float | [Random.Range](https://docs.unity3d.com/ScriptReference/Random.Range.html) |

### `DateTime`

| Property | Type                       | Notes                                                                                                               |
|----------|----------------------------|---------------------------------------------------------------------------------------------------------------------|
| `now`    | `DateTime`                 | Static                                                                                                              |
| `format` | function(string) => string | [DateTime.Now.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tostring?view=netframework-4.8) |

## Examples

Here is a simple example to get you started with the kind of code you can write:

```js
// Welcome to Scripter!
static var alpha = scene.getAtom("Cube").getStorable("CubeMat").getFloat("Alpha Adjust");
if (alpha.val == 0) {
    logMessage("The cube is fully transparent");
} else {
    logMessage("The cube alpha is: " + alpha.val);
}
```

You can also declare functions:

```js
static var bubbleText = scene.getAtom("Person").getStorable("SpeechBubble").getString("bubbleText");

function say(message) {
    bubbleText.val = message;
}

say("Hello, world!");
```

# License

[GNU GPLv3](LICENSE.md)

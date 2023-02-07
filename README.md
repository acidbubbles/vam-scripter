# Virt-A-Mate Scripter Plugin

A scripting engine to write some code inside Virt-A-Mate without having to write a new plugin. The language is greatly inspired from JavaScript.

## Language Features

- Variables (`var`, `let`)
- Types (`string`, `float`, `int`, `bool`, `undefined`)
- Boolean operators (`||`, `&&`, `==`, `!=`, `<`, `>`, `<=`, `>=`)
- Math operators (`+`, `-`, `*`, `/`)
- String concatenation (`+`)
- Assignment operators (`++`, `--`, `+=`, `-=`, `*=`, `/=`)
- Comments (`//`, `/* */`)
- Keywords (`if`, `else`, `for`, `while`, `throw`)
- Static variables (`static`)
- Functions (`function`, `return`)
- Code blocks and lexical scopes (`{`, `}`)

## Notable Omissions

- Objects, arrays and maps cannot be created
- Code blocks without brackets, like `if(condition) something();`
- Variable values are locally scoped, but you cannot reuse them in other functions of the same script

## Virt-A-Mate Functions

- `getDateTime(format)` (.NET: `DateTime.Now.ToString(format)`)
- `logMessage(message)` (VAM: `SuperController.LogMessage`)
- `logError(message)` (VAM: `SuperController.LogError`)
- `getTime()` (Unity: `Time.time`)
- `getDeltaTime()` (Unity: `Time.deltaTime`)
- `getRandom` (Unity: `Random.value` when no arguments are provided, `Random.Range(min, max)` when two arguments are provided)
- `getFloatParamValue(atomName, storableName, paramName)`
- `setFloatParamValue(atomName, storableName, paramName, value)`
- `getBoolParamValue(atomName, storableName, paramName)`
- `setBoolParamValue(atomName, storableName, paramName, value)`
- `getStringParamValue(atomName, storableName, paramName)`
- `setStringParamValue(atomName, storableName, paramName, value)`
- `getStringChooserParamValue(atomName, storableName, paramName)`
- `setStringChooserParamValue(atomName, storableName, paramName, value)`
- `invokeTrigger(atomName, storableName, paramName)`
- `invokeKeybinding(bindingName)`

## Examples

Here is a simple example to get you started with the kind of code you can write:

```js
// Welcome to Scripter!
var alpha = getFloatParamValue("Cube", "CubeMat", "Alpha Adjust", 0.5);
if(alpha == 0) {
    logMessage("The cube is fully transparent");
} else {
    logMessage("The cube alpha is: " + alpha);
}
```

You can use static values:

```js
static var initialized = false;
static var counter = 0;
if(!initialized) {
    // Do some work
}
counter++;
logMessage("Called " + counter + " times");
```

And functions:

```js
function say(message) {
    invokeTrigger("Person", "SpeechBubble", "bubbleText", message);
}

say("Hello, world!");
```

# License

[GNU GPLv3](LICENSE.md)

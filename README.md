# Virt-A-Mate Scripter Plugin

A scripting engine to write some code inside Virt-A-Mate without having to write a new plugin.

## Language Features

- Types (`string`, `float`, `int`, `bool`)
- Boolean operators (`||`, `&&`, `==`, `!=`, `<`, `>`, `<=`, `>=`)
- Math operators (`+`, `-`, `*`, `/`)
- String concatenation (`+`)
- Increment operators (`++`, `--`)
- Comments (`//`, `/* */`)
- Keywords (`var`, `if`, `else`, `for`, `while`, `true`, `false`, `throw`, `static`)
- Code blocks (`{`, `}`)
- Lexical scope (scoped variables)

## Notable Omissions

- Objects
- Custom functions
- Order of precedence

## Virt-A-Mate Functions

- `getDateTime(format)`
- `logMessage(message)`
- `logError(message)`
- `getFloatParamValue(atomName, storableName, paramName)`
- `setFloatParamValue(atomName, storableName, paramName, value)`
- `invokeTrigger(atomName, storableName, paramName)`
- `invokeKeybinding(bindingName)`

## Examples

Here is a simple example to get you started with the kind of code you can write:

```c#
// Welcome to Scripter!
var alpha = getFloatParamValue("Cube", "CubeMat", "Alpha Adjust", 0.5);
if(alpha == 0) {
    logMessage("The cube is fully transparent");
} else {
    logMessage("The cube alpha is: " + alpha);
}
```

You can use static values:

```c#
static var initialized = false;
static var counter = 0;
if(!initialized) {
    // Do some work
}
counter++;
logMessage("Called " + counter + " times");
```

# License

[GNU GPLv3](LICENSE.md)

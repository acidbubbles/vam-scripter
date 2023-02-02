# Virt-A-Mate Scripter Plugin

A scripting engine to write some code inside Virt-A-Mate without having to write a new plugin.

### Features:

- [ ] Maths (`+`, `-`, `*`, `/`)
- [ ] String concatenation (`+`)
- [ ] Invoking Virt-A-Mate triggers (`trigger()`)

### Example:

```c#
// Welcome to Scripter!
var alpha = getFloatParamValue("Cube", "CubeMat", "Alpha Adjust", 0.5);
if(alpha == 0) {
    logMessage("The cube is fully transparent");
} else {
    logMessage("The cube alpha is: " + alpha);
}
```

## License

[GNU GPLv3](LICENSE.md)

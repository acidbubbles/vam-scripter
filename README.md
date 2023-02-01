# Virt-A-Mate Scripter Plugin

A scripting engine to write some code inside Virt-A-Mate without having to write a new plugin.

### Features:

- [ ] Maths (`+`, `-`, `*`, `/`)
- [ ] String concatenation (`+`)
- [ ] Invoking Virt-A-Mate triggers (`trigger()`)

### Example:

```c#
if(value != "") {
    triggerString('AtomName', 'StorableName', 'TriggerName', 'Hello ' + value);
} else {
    logMessage('A name must be provided to the trigger');
}
```

## Credits

The code is pretty much a Virt-A-Mate port of this project: https://www.codemag.com/article/1607081/How-to-Write-Your-Own-Programming-Language-in-C

## License

[GNU GPLv3](LICENSE.md)

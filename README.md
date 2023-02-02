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

## License

[GNU GPLv3](LICENSE.md)

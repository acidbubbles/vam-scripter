You can listen to key presses.

```js
scripter.declareFloatParam({
    name: "Intensity",
    min: 0,
    max: 1,
    onChange: value => {
        console.log("Intensity is now: " + value);
    }
});
```

If you need to update the value, you can declare the param and listen to the event separately:

```js
let intensity = scripter.declareFloatParam({
    name: "Intensity",
    default: 0,
    min: 0,
    max: 1,
    constrain: true
});

intensity.onChange(value => {
    intensity.valNoCallback = Math.round(value);
});
```

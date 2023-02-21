```js
let intensity = self.declareFloatParam({
    name: "Intensity",
    default: 0,
    min: 0,
    max: 1,
    constrain: true
});

intensity.onChange(value => {
    console.log("Intensity is now: " + value);
});
```

```js
let intensity = self.declareFloatParam({
    name: "Intensity",
    min: 0,
    max: 1,
    onChange: value => {
        console.log("Intensity is now: " + value);
    }
});
```

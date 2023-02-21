```js
// Calls the function later
var timeout = setTimeout(() => {
    console.log("Shown a second later");
}, 1000);

// Cancels the timeout
clearTimeout(timeout);
```

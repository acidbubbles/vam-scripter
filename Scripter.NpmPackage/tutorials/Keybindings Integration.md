```js
import { self } from "vam-scripter";

// Available as Scripter.MyCommand
self.declareKeybinding("MyCommand", () => {
    console.log("Hello, world!");
});
```

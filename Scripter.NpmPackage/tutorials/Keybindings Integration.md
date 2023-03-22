```js
import { scripter} from "vam-scripter";

// Available as Scripter.MyCommand
scripter.declareKeybinding("MyCommand", () => {
    console.log("Hello, world!");
});
```

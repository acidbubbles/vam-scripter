You can declare actions that can be triggered by other scripts. You can also declare float params,string params and bool params.

```js
import { scripter } from "vam-scripter";

scripter.declareAction("Click", () => {
    console.log("Hello, world!");
});
```

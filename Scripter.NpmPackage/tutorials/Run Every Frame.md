You can run code every frame using `onUpdate` method.

```js
import { scripter, Time } from "vam-scripter";

scripter.onUpdate(function() {
    console.log(Time.time, "ding");
});
```

```js
import { scripter } from "vam-scripter";

let counter = 0;
const co = scripter.startCoroutine(i => {
    counter++;
    if(counter == 1) {
        return i.nextFrame;
    } else if(counter == 2) {
        return i.waitForSeconds(1);
    } else {
        return i.stop;
    }
});

// Cancels the coroutine
scripter.stopCoroutine(co);
```

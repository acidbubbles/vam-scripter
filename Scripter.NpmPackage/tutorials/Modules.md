You can reference things from separate modules:

`index.js`:

```js
import { say } from "./lib1.js";

say("Hello, world!");
```

`lib1.js`:

```js
export function say(message) {
    console.log("The user said: " + message);
}
```

If you just want to write separate modules:

`index.js`:

```js
import "./lib1.js";
```

`lib1.js`:

```js
console.log("This script will run because index.js references it");
```

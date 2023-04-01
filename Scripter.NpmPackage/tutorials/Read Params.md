You can access storable values from other atoms.

```js
import { scene } from "vam-scripter";

var alpha = scene.getAtom("Cube").getStorable("CubeMat").getFloat("Alpha Adjust");
if (alpha.val == 0) {
    console.log("The cube is fully transparent");
} else {
    console.log("The cube alpha is: " + alpha.val);
}
```

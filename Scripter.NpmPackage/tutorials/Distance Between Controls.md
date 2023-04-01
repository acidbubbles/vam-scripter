You can find the distance between two controls using the `distance` method.

```js
import { scene } from "vam-scripter";

var rightHand = scene.getAtom("Person").getController("rHandControl");
var ball = scene.getAtom("SPhere").getController("control");
var distance = rightHand.distance(ball);

console.log("The distance between the hand and the ball is " + distance);
```

```js
import { scene } from "vam-scripter";

var bubbleText = scene.getAtom("Person").getStorable("SpeechBubble").getString("bubbleText");

function say(message) {
    bubbleText.val = message;
}

say("Hello, world!");
```

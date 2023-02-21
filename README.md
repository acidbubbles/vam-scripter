# Virt-A-Mate Scripter Plugin

A scripting engine to write some code inside Virt-A-Mate without having to write a new plugin. The language is greatly inspired from JavaScript.

See [the documentation](https://acidbubbles.github.io/vam-scripter/) for more information.

## Example

```js
import { scene, scripter } from "vam-scripter";

const surprisedSound = scene.getAudioClip("URL", "web", "surprised.wav");
const person = scene.getAtom("Person");
const personVoice = person.getStorable("HeadAudioSource").getAudioAction("PlayNow");
const head = person.getController("head");
const ball = scene.getAtom("Ball").getController("control");
const timeline = person.getStorable("plugin#0_VamTimeline.AtomPlugin");

scripter.onUpdate(() => {
  if (ball.distance(head) < 0.5) {
      personVoice.play(surprisedSound);
      timeline.invokeAction("Play Surprised");
  }
});
```

# License

[GNU GPLv3](LICENSE.md)

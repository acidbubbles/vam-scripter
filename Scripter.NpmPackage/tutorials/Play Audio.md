You can refer to audio clips by name, and play them using the `play` method on the atom of your choice.

```js
import { scene } from "vam-scripter";

var laugh = scene.getAudioClip("URL", "web", "laugh.wav");
var music = scene.getAudioClip("Embedded", "Music", "CyberPetrifiedFull");

var speaker = scene.getAtom("AudioSource").getStorable("AudioSource").getAudioClipAction("PlayNow");
var person = scene.getAtom("Person").getStorable("HeadAudioSource").getAudioClipAction("PlayNow");

speaker.play(music);
person.play(laugh);
```

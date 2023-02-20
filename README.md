# Virt-A-Mate Scripter Plugin

A scripting engine to write some code inside Virt-A-Mate without having to write a new plugin. The language is greatly inspired from JavaScript.

See [the documentation](https://acidbubbles.github.io/vam-scripter/) for more information.

### `Player`

| Property     | Type                                          | Notes                                         |
|--------------|-----------------------------------------------|-----------------------------------------------|
| isVR         | boolean                                       | Whether the user currently wears a VR headset |

### `Keybindings`

| Property        | Type                                                                                         | Notes                                                                           |
|-----------------|----------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------|
| invokeCommand   | function(commandName: string) => void                                                        | Invoke a Keybinding command, if it exists                                       |
| declareCommand  | function(commandName: string, function) => [`KeybindingDeclaration`](#keybindingdeclaration) | Invoke a [Keybindings](https://github.com/acidbubbles/vam-keybindings) trigger  |

### `KeybindingDeclaration`

| Property | Type | Notes |
|----------|------|-------|
| N/A      | N/A  | N/A   |

### `Time`

| Property         | Type  | Notes                                                                                 |
|------------------|-------|---------------------------------------------------------------------------------------|
| `time`           | float | [Time.time](https://docs.unity3d.com/ScriptReference/Time-time.html)                  |
| `deltaTime`      | float | [Time.deltaTime](https://docs.unity3d.com/ScriptReference/Time-deltaTime.html)        |
| `fixedDeltaTime` | float | [Time.deltaTime](https://docs.unity3d.com/ScriptReference/Time-fixedDeltaTime.html)   |

### `Random`

| Property | Type                            | Notes                                                                      |
|----------|---------------------------------|----------------------------------------------------------------------------|
| `value`  | float                           | [Random.value](https://docs.unity3d.com/ScriptReference/Random-value.html) |
| `range`  | function(float, float) => float | [Random.Range](https://docs.unity3d.com/ScriptReference/Random.Range.html) |

### `DateTime`

| Property | Type                       | Notes                                                                                                               |
|----------|----------------------------|---------------------------------------------------------------------------------------------------------------------|
| `now`    | `DateTime`                 | Static                                                                                                              |
| `format` | function(string) => string | [DateTime.Now.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tostring?view=netframework-4.8) |

## Examples

Here is a simple example to get you started with the kind of code you can write:

```js
import { scene } from "scripter";

var alpha = scene.getAtom("Cube").getStorable("CubeMat").getFloat("Alpha Adjust");
if (alpha.val == 0) {
    console.log("The cube is fully transparent");
} else {
    console.log("The cube alpha is: " + alpha.val);
}
```

You can also declare functions:

```js
import { scene } from "scripter";

var bubbleText = scene.getAtom("Person").getStorable("SpeechBubble").getString("bubbleText");

function say(message) {
    bubbleText.val = message;
}

say("Hello, world!");
```

You can declare action triggers:

```js
import { self } from "scripter";

self.declareAction("Click", () => {
    console.log("Hello, world!");
});
```

You can declare params:

```js
let intensity = self.declareFloatParam({
    name: "Intensity",
    default: 0,
    min: 0,
    max: 1,
    constrain: true
});

intensity.onChange(value => {
    console.log("Intensity is now: " + value);
});
```

Or simply:

```js
let intensity = self.declareFloatParam({
    name: "Intensity",
    min: 0,
    max: 1,
    onChange: value => {
        console.log("Intensity is now: " + value);
    }
});
```

Run every frame:

```js
import { self, time } from "scripter";

self.onUpdate(function() {
    console.log(time.time, "ding");
});
```

Play audio:

```js
import { scene } from "scripter";

var laugh = scene.getAudioClip("URL", "web", "laugh.wav");
var music = scene.getAudioClip("Embedded", "Music", "CyberPetrifiedFull");

var speaker = scene.getAtom("AudioSource").getStorable("AudioSource").getAudioAction("PlayNow");
var person = scene.getAtom("Person").getStorable("HeadAudioSource").getAudioAction("PlayNow");

speaker.play(music);
person.play(laugh);
```

Find out the distance between two objects

```js
import { scene } from "scripter";

var rightHand = scene.getAtom("Person").getController("rHandControl");
var ball = scene.getAtom("SPhere").getController("control");
var distance = rightHand.distance(ball);

console.log("The distance between the hand and the ball is " + distance);
```

Create an event that you can call from [Keybindings](https://github.com/acidbubbles/vam-keybindings):

```js
import { self } from "scripter";

// Available as Scripter.MyCommand
self.declareKeybinding("MyCommand", () => {
    console.log("Hello, world!");
});
```
})

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

# License

[GNU GPLv3](LICENSE.md)

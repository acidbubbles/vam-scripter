# Virt-A-Mate Scripter Plugin

A scripting engine to write some code inside Virt-A-Mate without having to write a new plugin. The language is greatly inspired from JavaScript.

## Language Features

- Variables (`var`, `let`)
- Types (`string`, `float`, `int`, `bool`, `undefined`, `function`)
- Boolean operators (`||`, `&&`, `==`, `!=`, `<`, `>`, `<=`, `>=`)
- Math operators (`+`, `-`, `*`, `/`, `%`)
- String concatenation (`+`)
- Assignment operators (`++`, `--`, `+=`, `-=`, `*=`, `/=`)
- Comments (`//`, `/* */`)
- Keywords (`if`, `else`, `for`, `while`, `throw`, `break`, `continue`)
- Functions (`function`, `return`)
- Arrow functions (`(x) => { code; }`, `x => code`)
- Arrays (`var x = []`, `var x = [values]`, `x.add`, `x[index]`, `x.length`)
- Maps (`var x = {}`, `x["key"] = value`, `x.key = value`)
- Code blocks and lexical scopes (`{`, `}`)
- Try blocks (`try`, `catch`, `finally`)
- Export and import values (`import { x } from "Script Name"`, `export var x = 0;`)

## Notable Omissions

- No `foreach`
- No classes, prototypes
- No way to use http, tcp, udp, etc.
- No string interpolation (`${value}`)

## Globals

- [`console`](#console)
- [`setTimeout`](#settimeout)
- [`clearTimeout`](#settimeout)

## Modules

### Scripter

```js
import {
    self,
    time,
    random,
    scene,
    player,
    keybindings,
    datetime
} from "scripter";
```

- [`self: ScripterPlugin`](#scripterplugin)
- [`time: Time`](#time)
- [`random: Random`](#random)
- [`scene: Scene`](#scene)
- [`player: Player`](#player)
- [`keybindings: Keybindings`](#keybindings)
- [`datetime: DateTime`](#datetime)

## Native

### `console`

Writes to the Scripter log window. If the log window is not visible, errors will be sent to the Virt-A-Mate errors window.

```js
console.log("Hello World");
console.log("Hello", 1);
```

| Property | Type                            | Notes                           |
|----------|---------------------------------|---------------------------------|
| `log`    | function(string, ...) => `void` | Logs information to the console |
| `error`  | function(string, ...) => `void` | Writes logs in red              |
| `clear`  | function() => `void`            | Clears the console              |

### `setTimeout`

```js
// Calls the function later
var timeout = setTimeout(() => {
    console.log("Shown a second later");
}, 1000);
// Cancels the timeout
clearTimeout(timeout);
```

### `[]`

| Property  | Type                      | Notes                                             |
|-----------|---------------------------|---------------------------------------------------|
| `length`  | number                    | How many entries in the array                     |
| `add`     | function(value) => void   | Add a new value to the list                       |
| `indexOf` | function(value) => number | Returns the index in the list, or -1 if not found |
| `[]`      | `[number] => any`         | Get or set a value in the array                   |

## Classes

### `ScripterPlugin`

| Property           | Type                                                                                                                                                                                | Notes                                                                          |
|--------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------|
| declareFloatParam  | function({ name: string, default?: number, min?: number, max?: number, constrain?: bool, onChange: function(number) => void }) => [`FloatParamDeclaration`](#floatparamdeclaration) | Param trigger by ID                                                            |
| declareStringParam | function({ name: string, default?: string, onChange: function(string) => void }}) => [`StringParamDeclaration`](#stringparamdeclaration)                                            | Param trigger by ID                                                            |
| declareBoolParam   | function({ name: string, default?: string, onChange: function(bool) => void }) => [`BoolParamDeclaration`](#boolparamdeclaration)                                                   | Param trigger by ID                                                            |
| declareAction      | function(string, function) => [`ActionDeclaration`](#actionparamdeclaration)                                                                                                        | Invoke an action trigger by ID                                                 |
| onUpdate           | function(function) => void                                                                                                                                                          | Called every frame                                                             |
| onFixedUpdate      | function(function) => void                                                                                                                                                          | Called every physics frame                                                     |


### `FloatParamDeclaration`

| Property | Type                                       | Notes           |
|----------|--------------------------------------------|-----------------|
| val      | number                                     | The param value |
| onChange | function(function(number) => void) => void | The param value |

### `StringParamDeclaration`

| Property | Type                                       | Notes           |
|----------|--------------------------------------------|-----------------|
| val      | string                                     | The param value |
| onChange | function(function(string) => void) => void | The param value |

### `BoolParamDeclaration`

| Property | Type                                     | Notes           |
|----------|------------------------------------------|-----------------|
| val      | bool                                     | The param value |
| onChange | function(function(bool) => void) => void | The param value |

### `ActionParamDeclaration`

| Property | Type | Notes |
|----------|------|-------|
| N/A      | N/A  | N/A   |

### `Scene`

| Property     | Type                                          | Notes      |
|--------------|-----------------------------------------------|------------|
| getAtom      | function(string) => [`Atom`](#atom)           | Atom by ID |
| getAtomIds   | function() => `string[]`                      | Atom by ID |
| getAudioClip | function(string) => [`AudioClip`](#audioclip) | Atom by ID |

### `Atom`

| Property      | Type                                            | Notes          |
|---------------|-------------------------------------------------|----------------|
| getStorable   | function(string) => [`Storable`](#storable)     | Storable by ID |
| getController | function(string) => [`Controller`](#controller) | Storable by ID |

### `Storable`

| Property         | Type                                                            | Notes                          |
|------------------|-----------------------------------------------------------------|--------------------------------|
| invokeAction     | function(string) => `void`                                      | Invoke an action trigger by ID |
| getFloat         | function(string) => [`FloatParam`](#floatparam)                 | Param trigger by ID            |
| getString        | function(string) => [`StringParam`](#stringparam)               | Param trigger by ID            |
| getBool          | function(string) => [`BoolParam`](#boolparam)                   | Param trigger by ID            |
| getStringChooser | function(string) => [`StringChooserParam`](#stringchooserparam) | Param trigger by ID            |
| getAudioAction   | function(string) => [`AudioActionParam`](#audioactionparam)     | Param trigger by ID            |

### `FloatParam`

| Property | Type  | Notes           |
|----------|-------|-----------------|
| val      | float | The param value |

### `StringParam`

| Property | Type   | Notes           |
|----------|--------|-----------------|
| val      | string | The param value |

### `BoolParam`

| Property | Type | Notes           |
|----------|------|-----------------|
| val      | bool | The param value |

### `StringChooserParam`

| Property | Type   | Notes           |
|----------|--------|-----------------|
| val      | string | The param value |

### `AudioActionParam`

| Property | Type                              | Notes                  |
|----------|-----------------------------------|------------------------|
| play     | function(clip: AudioClip) => void | The audio clip to play |

### `AudioClip`

| Property | Type | Notes                                       |
|----------|------|---------------------------------------------|
| N/A      | N/A  | A named audio clip from the Scene Audio tab |

### `Controller`

| Property      | Type                                             | Notes                                          |
|---------------|--------------------------------------------------|------------------------------------------------|
| distance      | function(Controller) => `number`                 | Finds out the distance between two controllers |
| moveTowards   | function(Controller, maxDistanceDelta) => `void` | Moves towards another control                  |
| rotateTowards | function(Controller, maxDegreesDelta) => `void`  | Rotates towards another control                |

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

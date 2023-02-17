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
- DateTime (`DateTime.now.format("")`)
- Export and import values (`import { x } from "Script Name"`, `export var x = 0;`)

## Notable Omissions

- No `foreach`
- No classes, prototypes
- No `try`/`catch`
- No way to use http, tcp, udp, etc.
- No string interpolation (`${value}`)

## Globals

- [`console`](#console)

## Modules

### Scripter

```js
import {
    self,
    time,
    random,
    scene,
    datetime
} from "scripter";
```

- [`self: ScripterPlugin`](#scripterplugin)
- [`time: Time`](#time)
- [`random: Random`](#random)
- [`scene: Scene`](#scene)
- [`datetime: DateTime`](#datetime)

## Classes

### `Console`

| Property | Type                       | Notes                                             |
|----------|----------------------------|---------------------------------------------------|
| `log`    | function(string) => `void` | Uses `SuperController.LogMessage`                 |
| `error`  | function(string) => `void` | Uses `SuperController.LogError`                   |
| `clear`  | function() => `void`       | Uses `SuperController.singleton.ClearMessages();` |

### `ScripterPlugin`

| Property      | Type                                                                                                                                        | Notes                          |
|---------------|---------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------|
| declareFloat  | function({ name: string, default: number, min: number, max: number, constrain: bool }) => [`FloatParamDeclaration`](#floatparamdeclaration) | Param trigger by ID            |
| declareString | function({ name: string, default: string }}) => [`StringParamDeclaration`](#stringparamdeclaration)                                         | Param trigger by ID            |
| declareBool   | function({ name: string, default: string }) => [`BoolParamDeclaration`](#boolparamdeclaration)                                              | Param trigger by ID            |
| declareAction | function(string, function) => [`ActionDeclaration`](#actionparamdeclaration)                                                                | Invoke an action trigger by ID |

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
| getAudioClip | function(string) => [`AudioClip`](#audioclip) | Atom by ID |

### `Atom`

| Property    | Type                                        | Notes          |
|-------------|---------------------------------------------|----------------|
| getStorable | function(string) => [`Storable`](#storable) | Storable by ID |

### `Storable`

| Property         | Type                                                            | Notes                          |
|------------------|-----------------------------------------------------------------|--------------------------------|
| invokeTrigger    | function(string) => `void`                                      | Invoke an action trigger by ID |
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

### `Time`

| Property    | Type  | Notes                                                                          |
|-------------|-------|--------------------------------------------------------------------------------|
| `time`      | float | [Time.time](https://docs.unity3d.com/ScriptReference/Time-time.html)           |
| `deltaTime` | float | [Time.deltaTime](https://docs.unity3d.com/ScriptReference/Time-deltaTime.html) |

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

You can declare storables:

```js
import { self } from "scripter";

self.declareAction("Say", function(message) {
    say(message);
});

let intensity = self.declareFloatParam({
    name: "Intensity",
    default: 0,
    min: 0,
    max: 1,
    constrain: true
});

intensity.onChange(function(value) {
    say("Intensity is now: " + value);
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

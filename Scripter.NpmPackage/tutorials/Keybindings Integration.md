You can integrate with the [Keybindings](https://hub.virtamate.com/resources/keybindings.4400/) plugin to either create new bindings or invoke existing ones.

```js
import { keybindings } from "vam-scripter";

// Available as Scripter.MyCommand
keybindings.declareCommand("MyCommand", () => {
    console.log("Hello, world!");
});

// You can also call other commands
keybindings.invokeCommand("Scripter.OpenUI");
```

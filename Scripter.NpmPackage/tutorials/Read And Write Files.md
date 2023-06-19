You can read and write files that will only be available for the current scene, and the current machine (e.g. save files)

```js
import { fs } from "vam-scripter";

var content = fs.readSceneFileAsync('counter.txt');
if(!content) content = '0';
content = parseInt(content) + 1;
console.log('File loaded ' + content + ' times')
fs.writeSceneFileAsync('counter.txt', content.toString());
```

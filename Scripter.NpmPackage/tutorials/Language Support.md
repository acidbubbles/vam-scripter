### Language Features

- Variables (`var`, `let`)
- Types (`string`, `float`, `int`, `bool`, `undefined`, `function`)
- Boolean operators (`||`, `&&`, `==`, `!=`, `<`, `>`, `<=`, `>=`)
- Math operators (`+`, `-`, `*`, `/`, `%`)
- String concatenation (`+`)
- Assignment operators (`++`, `--`, `+=`, `-=`, `*=`, `/=`)
- Comments (`//`, `/* */`)
- Keywords (`if`, `else`, `for`, `while`, `throw`, `break`, `continue`)
- Ternary operators (`condition ? true : false`)
- Functions (`function`, `return`)
- Arrow functions (`(x) => { code; }`, `x => code`)
- Arrays (`var x = []`, `var x = [values]`, `x.add`, `x[index]`, `x.length`)
- Maps (`var x = {}`, `x["key"] = value`, `x.key = value`)
- Code blocks and lexical scopes (`{`, `}`)
- Try blocks (`try`, `catch`, `finally`)
- Export and import values (`import { x } from "Script Name"`, `export var x = 0;`)
- Built in functions (`JSON.parse()`, `JSON.stringify()`, `parseFloat()`, `parseInt()`)
- Base prototype functions (`.toString()`)

### Notable Omissions

- No `foreach`
- No classes, prototypes
- No way to use http, tcp, udp, etc.
- No string interpolation (`${value}`)
- No iterators
- No promises
- No prototypes system
- `JSON` only supports flat, strings-only JSON objects

### Arrays

| Property  | Type                    | Notes                                             |
| --------- | ----------------------- | --------------------------------------------------|
| `length`  | `number`                | How many entries in the array                     |
| `add`     | `function(*) => void`   | Add a new value to the list                       |
| `indexOf` | `function(*) => number` | Returns the index in the list, or -1 if not found |
| `[]`      | `[number] => any`       | Get or set a value in the array                   |

### Strings

| Property       | Type                          | Notes                                                 |
| -------------- | ----------------------------- | ----------------------------------------------------- |
| `length`       | `number`                      | How many characters in string                         |
| `startsWith`   | `function(string) => boolean` | Whether the string starts with the provided substring |
| `endsWith`     | `function(string) => boolean` | Whether the string ends with the provided substring   |
| `contains`     | `function(string) => boolean` | Whether the string contains the provided substring    |

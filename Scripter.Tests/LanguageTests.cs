using ScripterLang;

namespace Scripter.Tests;

public class LanguageTests
{
    private Program _program;

    [SetUp]
    public void SetUp()
    {
        _program = new Program();
    }

    [Test]
    public void Variables()
    {
        _program.Register("index.js", """
            var x = 1;
            const y = 2;
            return x + y;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("3"));
    }

    [Test]
    public void Globals()
    {
        _program.globalContext.DeclareGlobal("v", 1);
        _program.Register("index.js", """
            return v + 1;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("2"));
    }

    [Test]
    public void Undefined()
    {
        _program.Register("index.js", """
            var x = undefined;
            return undefined == undefined;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void ControlFlow()
    {
        _program.Register("index.js", """
            var x = 1 + 1;
            var result;
            if(x == 1) {
                result = "not ok";
            } else if(x == 2) {
                result = "ok";
            }
            return result;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void ReturnExits()
    {
        _program.Register("index.js", """
            {
                return "ok";
            }
            throw "Did not return!";
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void ReturnExitsNoStatement()
    {
        _program.Register("index.js", """
            {
                return;
            }
            throw "Did not return!";
            """);
        var result = _program.Run();

        Assert.That(result, Is.EqualTo(Value.Undefined));
    }

    [Test]
    public void CustomFunctions()
    {
        _program.globalContext.DeclareGlobal("MyFunction", Value.CreateFunction((context, args) =>
        {
            Assert.That(context.GetVariable("x").GetValue().RawInt, Is.EqualTo(1));
            Assert.That(args[0].ToString(), Is.EqualTo("1"));
            Assert.That(args[1].ToString(), Is.EqualTo("ab"));
            Assert.That(args[2].ToString(), Is.EqualTo("true"));
            return Value.CreateString("ok");
        }));
        _program.Register("index.js", """
            var x = 1;
            return MyFunction(1 * 1, "a" + "b", true == true);
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void Errors()
    {
        _program.Register("index.js", """
            let previous;
            try {
                throw "Previous";
            } catch(e) {
                previous = e.message;
            } finally {
                previous += "!";
            }
            try {} catch {}
            try {} finally {}
            throw "Error: " + previous;
            """);

        var exc = Assert.Throws<ScripterRuntimeException>(() => _program.Run());
        Assert.That(exc!.Message, Is.EqualTo("Error: Previous!"));
    }

    [Test]
    public void Maths()
    {
        _program.Register("index.js", """
            var x = 1;
            var y = ++x;
            x = (x + y) * 2;
            x += 1;
            return x++;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("9"));
    }

    [Test]
    public void Precedence()
    {
        Assert.DoesNotThrow(() => _program.Register("index.js", """
            if((1 + 1 * 2) != 3) { throw "* before +"; }
            if(!(false || true && true)) { throw "&& before ||"; }
            if(1 < 2 && false) { throw "< before &&"; }
            """));
    }

    [Test]
    public void Strings()
    {
        _program.Register("index.js", """
            return "a" + 2 + true;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("a2true"));
    }

    [Test]
    public void ConditionsAndThrow()
    {
        _program.Register("index.js", """
            if(false) { throw "false"; }
            if(!true) { throw "!true"; }
            if(false) throw "no brackets";
            if(true) throw "success";
            """);
        var exc = Assert.Throws<ScripterRuntimeException>(() => _program.Run());
        Assert.That(exc?.Message, Is.EqualTo("success"));
    }

    [Test]
    public void Loops()
    {
        _program.Register("index.js", """
            var x = 0;
            while(x < 5) {
                x++;
            }
            for(var y = 0; y < 5; y++) {
                x++;
            }
            for(let i= 0; i < 2; i++) {
                if(i < 1) continue;
                if(i < 2) break;
                throw "break failed";
            }
            while(true) {
                break;
                throw "break failed";
            }
            return x;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("10"));
    }

    [Test]
    public void MultipleRuns()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        _program.Register("index.js", source);
        _program.Register("index.js", source);
        var result1 = _program.Run();
        var result2 = _program.Run();

        Assert.That(result1.ToString(), Is.EqualTo("1"));
        Assert.That(result2.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void Functions()
    {
        _program.Register("index.js", """
            var x = 0;
            run();
            function run() {
                let y = 0;
                y += 1;
                x += y;
                function increment(v) {
                    return ++v;
                }
                x = increment(x);
            }
            var wrap = function() { return run(); };
            wrap();
            return x;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("4"));
    }

    [Test]
    public void ArrowFunctions()
    {
        _program.Register("index.js", """
            var x = (i) => { return i + 1; };
            var y = i => i + 1;
            return x(2) + y(10);
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("14"));
    }

    [Test]
    public void UnaryOperators()
    {
        _program.Register("index.js", """
            return -1 + -2;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("-3"));
    }

    [Test]
    public void Arrays()
    {
        _program.globalContext.DeclareGlobal("getThing", Value.CreateFunction((context, args) => new MyThing { value = args[0].AsInt }));
        _program.Register("index.js", """
            var x = [];
            x.add(1);
            x[0] = x[0] + 1;
            x[0]++;
            x[0] += 1;
            x[0] = ++x[0];
            return [x[0], x.length, x.indexOf(5), x.indexOf(3)];
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("[5, 1, 0, -1]"));
    }

    [Test]
    public void Maps()
    {
        _program.Register("index.js", """
            var x = {};
            var y = { key1: 1, key2: "2", key3: x };
            y.key1 = y.key1 + 1;
            y["key2"] = y["key2"] + "!";
            y.key3.test = "ok";
            return y;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("{ key1: 2, key2: \"2!\", key3: { test: \"ok\" } }"));
    }

    [Test]
    public void Objects()
    {
        _program.globalContext.DeclareGlobal("getThing", Value.CreateFunction((context, args) => new MyThing { value = args[0].AsInt }));
        _program.Register("index.js", """
            var x = getThing(10);
            var o = getThing(1);
            o.value = o.value + 1;
            return o.value
                + o.increment(2)
                + x.value
                + x.increment(3)
                + o
                    .createAndAdd(100)
                    .increment(1);
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("134"));
    }

    [Test]
    public void Deep()
    {
        var thing = new MyThing
        {
            deep = new MyThing
            {
                deep = new MyThing
                {
                    deep = new MyThing
                    {
                        value = 1
                    }
                }
            }
        };
        _program.globalContext.DeclareGlobal("getThing", Value.CreateFunction((context, args) => thing));
        _program.Register("index.js", """
            var o = getThing(1);
            o.deep.deep.deep.value = o.deep.deep.deep.value + 1;
            o.deep.deep.deep.increment(1);
            o.deep.deep.deep.value += 1;
            return o.deep.deep.deep.value;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("4"));
    }

    [Test]
    public void ImportExport()
    {
        _program.Register("lib.js", """
            export var x = 1;
            export function fn(y) { return x + y; }
            """);
        _program.Register("index.js", """
            import { x, fn } from "./lib.js";
            return x + fn(1);
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("3"));
    }

    [Test]
    public void ImportNoVariables()
    {
        var called = Value.Undefined;
        _program.globalContext.DeclareGlobal("test", new FunctionReference((context, args) => called = args[0]));
        _program.Register("lib.js", """
            test("ok");
            """);
        _program.Register("index.js", """
            import "./lib.js";
            """);
        _program.Run();

        Assert.That(called.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void ImportExportNoOrder()
    {
        _program.Register("index.js", """
            import { x } from "./lib.js";
            return x;
            """);
        _program.Register("lib.js", """
            export var x = 1;
            """);
        var result = _program.Run();

        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void RefreshScripts()
    {
        _program.Register("index.js", """
            import { x } from "./lib.js";
            return x;
            """);
        _program.Register("lib.js", """
            export var x = 1;
            """);
        var result1 = _program.Run();
        Assert.That(result1.ToString(), Is.EqualTo("1"));

        _program.Register("index.js", """
            import { x } from "./lib.js";
            return x * 3;
            """);
        var result2 = _program.Run();
        Assert.That(result2.ToString(), Is.EqualTo("3"));

        _program.Register("lib.js", """
            export var x = 2;
            """);
        var result3 = _program.Run();
        Assert.That(result3.ToString(), Is.EqualTo("6"));
    }

    private class MyThing : ObjectReference
    {
        public int value;
        public MyThing deep;

        public override Value GetProperty(string name)
        {
            switch (name)
            {
                case "value": return value;
                case "deep": return deep;
                case "increment": return Func(Increment);
                case "createAndAdd": return Func(CreateAndAdd);
                default: return base.GetProperty(name);
            }
        }

        public override void SetProperty(string name, Value value)
        {
            switch (name)
            {
                case "value":
                    this.value = value.AsInt;
                    break;
                default:
                    base.SetProperty(name, value);
                    break;
            }
        }

        private Value Increment(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(Increment), args, 1);
            return  (value = value + args[0].AsInt);
        }

        private Value CreateAndAdd(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(CreateAndAdd), args, 1);
            return new MyThing { value = value + args[0].AsInt };
        }
    }

    [Test]
    public void PerfTests()
    {
        // Latest results:
        // Scripter: 0.2382ms
        // Native: 0.0015ms
        // Ratio: 155.1 times slower (0.64% of native speed)
        PerfTest.Run();
    }
}

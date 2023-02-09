using System;
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
        _program.Add("script", """
            var x = 1;
            return x;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void Globals()
    {
        _program.GlobalContext.DeclareHoisted("v", 1, Location.Empty);
        _program.Add("script", """
            return v + 1;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("2"));
    }

    [Test]
    public void Undefined()
    {
        _program.Add("script", """
            var x = undefined;
            return undefined == undefined;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void ControlFlow()
    {
        _program.Add("script", """
            var x = 1 + 1;
            var result;
            if(x == 1) {
                result = "not ok";
            } else if(x == 2) {
                result = "ok";
            }
            return result;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void ReturnExits()
    {
        _program.Add("script", """
            {
                return "ok";
            }
            throw "Did not return!";
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void ReturnExitsNoStatement()
    {
        _program.Add("script", """
            {
                return;
            }
            throw "Did not return!";
            """);
        var result = _program.Run("script");

        Assert.That(result, Is.EqualTo(Value.Undefined));
    }

    [Test]
    public void CustomFunctions()
    {
        _program.GlobalContext.DeclareHoisted("MyFunction", Value.CreateFunction((context, args) =>
        {
            Assert.That(context.GetVariableValue("x").RawInt, Is.EqualTo(1));
            Assert.That(args[0].ToString(), Is.EqualTo("1"));
            Assert.That(args[1].ToString(), Is.EqualTo("ab"));
            Assert.That(args[2].ToString(), Is.EqualTo("true"));
            return Value.CreateString("ok");
        }));
        _program.Add("script", """
            var x = 1;
            return MyFunction(1 * 1, "a" + "b", true == true);
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void Errors()
    {
        _program.Add("script", """
            throw "Error!";
            """);

        var exc = Assert.Throws<ScripterRuntimeException>(() => _program.Run("script"));
        Assert.That(exc!.Message, Is.EqualTo("Error!"));
    }

    [Test]
    public void Maths()
    {
        _program.Add("script", """
            var x = 1;
            var y = ++x;
            x = (x + y) * 2;
            x += 1;
            return x++;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("9"));
    }

    [Test]
    public void Precedence()
    {
        Assert.DoesNotThrow(() => _program.Add("script", """
            if((1 + 1 * 2) != 3) { throw "* before +"; }
            if(!(false || true && true)) { throw "&& before ||"; }
            if(1 < 2 && false) { throw "< before &&"; }
            """));
    }

    [Test]
    public void Strings()
    {
        _program.Add("script", """
            return "a" + 2 + true;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("a2true"));
    }

    [Test]
    public void ConditionsAndThrow()
    {
        _program.Add("script", """
            if(false) { throw "false"; }
            if(!true) { throw "!true"; }
            return "nice";
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("nice"));
    }

    [Test]
    public void Loops()
    {
        _program.Add("script", """
            var x = 0;
            while(x < 5) {
                x++;
            }
            for(var y = 0; y < 5; y++) {
                x++;
            }
            return x;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("10"));
    }

    [Test]
    public void MultipleRuns()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        _program.Add("script", source);
        _program.Add("script", source);
        var result1 = _program.Run("script");
        var result2 = _program.Run("script");

        Assert.That(result1.ToString(), Is.EqualTo("1"));
        Assert.That(result2.ToString(), Is.EqualTo("1"));
    }

    #warning Replace static by export or bind to event, e.g. param.onChange(function() { ... }), plugin

    [Test]
    public void Functions()
    {
        _program.Add("script", """
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
            run();
            return x;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("4"));
    }

    #warning No unary operators (+1, -2)

    [Test]
    public void Arrays()
    {
        _program.GlobalContext.DeclareHoisted("getThing", Value.CreateFunction((context, args) => new MyThing { Value = args[0].AsInt }));
        _program.Add("script", """
            var x = [];
            x.add(1);
            x[0] = x[0] + 1;
            x[0]++;
            x[0] += 1;
            x[0] = ++x[0];
            return [x[0], x.length];
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("[5, 1]"));
    }

    [Test]
    public void Objects()
    {
        _program.GlobalContext.DeclareHoisted("getThing", Value.CreateFunction((context, args) => new MyThing { Value = args[0].AsInt }));
        _program.Add("script", """
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
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("134"));
    }

    [Test]
    public void Deep()
    {
        var thing = new MyThing
        {
            Deep = new MyThing
            {
                Deep = new MyThing
                {
                    Deep = new MyThing
                    {
                        Value = 1
                    }
                }
            }
        };
        _program.GlobalContext.DeclareHoisted("getThing", Value.CreateFunction((context, args) => thing));
        _program.Add("script", """
            var o = getThing(1);
            o.deep.deep.deep.value = o.deep.deep.deep.value + 1;
            o.deep.deep.deep.increment(1);
            o.deep.deep.deep.value += 1;
            return o.deep.deep.deep.value;
            """);
        var result = _program.Run("script");

        Assert.That(result.ToString(), Is.EqualTo("4"));
    }

    [Test]
    public void ImportExport()
    {
        _program.Add("lib", """
            export var x = 1;
            export function fn(y) { return x + y; }
            """);
        _program.Add("main", """
            import { x, fn } from "lib";
            return x + fn(1);
            """);
        var result = _program.Run("main");

        Assert.That(result.ToString(), Is.EqualTo("3"));
    }

    [Test]
    public void ImportExportNoOrder()
    {
        _program.Add("main", """
            import { x } from "lib";
            return x;
            """);
        _program.Add("lib", """
            export var x = 1;
            """);
        var result = _program.Run("main");

        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    private class MyThing : ObjectReference
    {
        public int Value;
        public MyThing Deep;

        public override Value Get(string name)
        {
            switch (name)
            {
                case "value": return Value;
                case "deep": return Deep;
                case "increment": return fn(Increment);
                case "createAndAdd": return fn(CreateAndAdd);
                default: return base.Get(name);
            }
        }

        public override void Set(string name, Value value)
        {
            switch (name)
            {
                case "value":
                    Value = value.AsInt;
                    break;
                default:
                    base.Set(name, value);
                    break;
            }
        }

        private Value Increment(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(Increment), args, 1);
            return  (Value = Value + args[0].AsInt);
        }

        private Value CreateAndAdd(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(CreateAndAdd), args, 1);
            return new MyThing { Value = Value + args[0].AsInt };
        }
    }

    [Test]
    public void PerfTestStructure()
    {
        _program.Add("script", """
            var x1 = 1;
            function test(x2) {
                for(var i = 0; i < 5; i++) {
                    x2++;
                }
                return x2;
            }
            {
                for(var i = 0; i < 5; i++) {
                    x1 = test(x1);
                }
            }
            return x1;
            """);
        var result = _program.Run("script");

        Assert.That(result.RawInt, Is.EqualTo(26));
    }
}

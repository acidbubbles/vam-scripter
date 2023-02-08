using ScripterLang;

namespace Scripter.Tests;

public class LanguageTests
{
    private GlobalLexicalContext _globalLexicalContext;

    [SetUp]
    public void SetUp()
    {
        _globalLexicalContext = new GlobalLexicalContext();
    }

    [Test]
    public void Variables()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void Globals()
    {
        const string source = """
            return v + 1;
            """;
        _globalLexicalContext.Declare("v", Location.Empty);
        var expression = Parser.Parse(source, _globalLexicalContext);
        _globalLexicalContext.Globals.Add("v", 1);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("2"));
    }

    [Test]
    public void Undefined()
    {
        const string source = """
            var x = undefined;
            return undefined == undefined;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void ControlFlow()
    {
        const string source = """
            var x = 1 + 1;
            var result;
            if(x == 1) {
                result = "not ok";
            } else if(x == 2) {
                result = "ok";
            }
            return result;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void ReturnExits()
    {
        const string source = """
            {
                return "ok";
            }
            throw "Did not return!";
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void CustomFunctions()
    {
        const string source = """
            var x = 1;
            return MyFunction(1 * 1, "a" + "b", true == true);
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        _globalLexicalContext.Globals.Add("MyFunction", Value.CreateFunction((d, args) =>
        {
            Assert.That(d.GetVariableValue("x").RawInt, Is.EqualTo(1));
            Assert.That(args[0].ToString(), Is.EqualTo("1"));
            Assert.That(args[1].ToString(), Is.EqualTo("ab"));
            Assert.That(args[2].ToString(), Is.EqualTo("true"));
            return Value.CreateString("ok");
        }));
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void Errors()
    {
        const string source = """
            throw "Error!";
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var exc = Assert.Throws<ScripterRuntimeException>(() => expression.Evaluate(domain));
        Assert.That(exc!.Message, Is.EqualTo("Error!"));
    }

    [Test]
    public void Maths()
    {
        const string source = """
            var x = 1;
            var y = ++x;
            x = (x + y) * 2;
            x += 1;
            return x++;
            ;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("9"));
    }

    [Test]
    public void Precedence()
    {
        const string source = """
            if((1 + 1 * 2) != 3) { throw "* before +"; }
            if(!(false || true && true)) { throw "&& before ||"; }
            if(1 < 2 && false) { throw "< before &&"; }
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);

        Assert.DoesNotThrow(() => expression.Evaluate(domain));
    }

    [Test]
    public void Strings()
    {
        const string source = """
            return "a" + 2 + true;
            ;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("a2true"));
    }

    [Test]
    public void ConditionsAndThrow()
    {
        const string source = """
            if(false) { throw "false"; }
            if(!true) { throw "!true"; }
            return "nice";
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("nice"));
    }

    [Test]
    public void Loops()
    {
        const string source = """
            var x = 0;
            while(x < 5) {
                x++;
            }
            for(var y = 0; y < 5; y++) {
                x++;
            }
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("10"));
    }

    [Test]
    public void MultipleRuns()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result1 = expression.Evaluate(domain);
        var result2 = expression.Evaluate(domain);

        Assert.That(result1.ToString(), Is.EqualTo("1"));
        Assert.That(result2.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void StaticValues()
    {
        const string source = """
            static var x = 1 + 1;
            x++;
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result1 = expression.Evaluate(domain);
        var result2 = expression.Evaluate(domain);

        Assert.That(result1.ToString(), Is.EqualTo("3"));
        Assert.That(result2.ToString(), Is.EqualTo("4"));
    }

    [Test]
    public void Functions()
    {
        const string source = """
            var x = 0;
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
            run();
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("4"));
    }

    #warning No unary operators (+1, -2)

    [Test]
    public void Arrays()
    {
        const string source = """
            static var x = [];
            x.add(1);
            x[0] = x[0] + 1;
            return x[0];
            """;
        _globalLexicalContext.Globals.Add("getThing", Value.CreateFunction((d, args) => new MyThing { Value = args[0].AsInt }));
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("134"));
    }

    [Test]
    public void Objects()
    {
        const string source = """
            static var x = getThing(10);
            var o = getThing(1);
            o.value = o.value + 1;
            return o.value
                + o.increment(2)
                + x.value
                + x.increment(3)
                + o
                    .createAndAdd(100)
                    .increment(1);
            """;
        _globalLexicalContext.Globals.Add("getThing", Value.CreateFunction((d, args) => new MyThing { Value = args[0].AsInt }));
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("134"));
    }

    [Test]
    public void Deep()
    {
        const string source = """
            var o = getThing(1);
            o.deep.deep.deep.value = o.deep.deep.deep.value + 1;
            o.deep.deep.deep.increment(1);
            o.deep.deep.deep.value += 1;
            return o.deep.deep.deep.value;
            """;
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
        _globalLexicalContext.Globals.Add("getThing", Value.CreateFunction((d, args) => thing));
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.ToString(), Is.EqualTo("4"));
    }

    private class MyThing : ObjectReference
    {
        public int Value;
        public MyThing Deep;

        public override Value Get(string name)
        {
            if (name == "value") return Value;
            if (name == "deep") return Deep;
            return base.Get(name);
        }

        public override void Set(string name, Value value)
        {
            Value = value.AsInt;
        }

        public override Value InvokeMethod(string name, Value[] args)
        {
            if (name == "increment")
            {
                ValidateArgumentsLength(name, args, 1);
                return  (Value = Value + args[0].AsInt);
            }
            if (name == "createAndAdd")
            {
                ValidateArgumentsLength(name, args, 1);
                return Deep = new MyThing { Value = Value + args[0].AsInt };
            }
            return base.InvokeMethod(name, args);
        }
    }

    [Test]
    public void PerfTestStructure()
    {
        const string source = """
            var x1 = 1;
            function test(x2) {
                for(var i = 0; i < 5; i++) {
                    x2++;
                }
                return x2;
            }
            {
                for(var j = 0; j < 5; j++) {
                    x1 = test(x1);
                }
            }
            return x1;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var domain = new RuntimeDomain(_globalLexicalContext);
        var result = expression.Evaluate(domain);

        Assert.That(result.RawInt, Is.EqualTo(26));
    }
}

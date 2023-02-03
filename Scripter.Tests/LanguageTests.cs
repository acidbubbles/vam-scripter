using ScripterLang;

namespace Scripter.Tests;

public class Tests
{
    private GlobalLexicalContext _globalLexicalContext;
    private Runtime _runtime;

    [SetUp]
    public void SetUp()
    {
        _globalLexicalContext = new GlobalLexicalContext();
        _runtime = new Runtime(_globalLexicalContext);
    }
    [Test]
    public void Variables()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var expressions = Parser.Parse(source, _globalLexicalContext);
        var result = _runtime.Evaluate(expressions);

        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void ControlFlow()
    {
        const string source = """
            var x = 1;
            var result;
            if(x == 1) {
                result = "ok";
            }
            return result;
            """;
        var expressions = Parser.Parse(source, _globalLexicalContext);
        var result = _runtime.Evaluate(expressions);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void CustomFunctions()
    {
        const string source = """
            return MyFunction(1, "2", true);
            """;
        var expressions = Parser.Parse(source, _globalLexicalContext);
        _globalLexicalContext.Functions.Add("MyFunction", args =>
        {
            Assert.That(args[0].ToString(), Is.EqualTo("1"));
            Assert.That(args[1].ToString(), Is.EqualTo("2"));
            Assert.That(args[2].ToString(), Is.EqualTo("true"));
            return Value.CreateString("ok");
        });
        var result = _runtime.Evaluate(expressions);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void MultipleRuns()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var expressions = Parser.Parse(source, _globalLexicalContext);
        var result1 = _runtime.Evaluate(expressions);
        var result2 = _runtime.Evaluate(expressions);

        Assert.That(result1.ToString(), Is.EqualTo("1"));
        Assert.That(result2.ToString(), Is.EqualTo("1"));
    }
}

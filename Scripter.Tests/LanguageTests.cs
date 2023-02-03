using ScripterLang;

namespace Scripter.Tests;

public class Tests
{
    private GlobalLexicalContext _globalLexicalContext;
    private RuntimeDomain _domain;

    [SetUp]
    public void SetUp()
    {
        _globalLexicalContext = new GlobalLexicalContext();
        _domain = new RuntimeDomain();
    }
    [Test]
    public void Variables()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

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
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void CustomFunctions()
    {
        const string source = """
            return MyFunction(1 * 1, "a" + "b", true == true);
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        _globalLexicalContext.Functions.Add("MyFunction", args =>
        {
            Assert.That(args[0].ToString(), Is.EqualTo("1"));
            Assert.That(args[1].ToString(), Is.EqualTo("ab"));
            Assert.That(args[2].ToString(), Is.EqualTo("true"));
            return Value.CreateString("ok");
        });
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void Errors()
    {
        const string source = """
            throw "Error!";
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var exc = Assert.Throws<ScripterRuntimeException>(() => expression.Evaluate(_domain));
        Assert.That(exc!.Message, Is.EqualTo("Error!"));
    }


        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void MultipleRuns()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result1 = expression.Evaluate(_domain);
        var result2 = expression.Evaluate(_domain);

        Assert.That(result1.ToString(), Is.EqualTo("1"));
        Assert.That(result2.ToString(), Is.EqualTo("1"));
    }
}

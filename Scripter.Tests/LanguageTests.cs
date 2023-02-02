using System.Linq;
using ScripterLang;

namespace Scripter.Tests;

public class Tests
{
    [Test]
    public void Variables()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var tokens = Tokenizer.Tokenize(source).ToList();
        var expressions = Parser.Parse(tokens);
        var result = new Runtime().Evaluate(expressions);

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
        var tokens = Tokenizer.Tokenize(source).ToList();
        var expressions = Parser.Parse(tokens);
        var result = new Runtime().Evaluate(expressions);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void CustomFunctions()
    {
        const string source = """
            return MyFunction(1, "2", true);
            """;
        var tokens = Tokenizer.Tokenize(source).ToList();
        var expressions = Parser.Parse(tokens);
        var runtime = new Runtime();
        runtime.GlobalLexicalContext.Functions.Add("MyFunction", args =>
        {
            Assert.That(args[0].ToString(), Is.EqualTo("1"));
            Assert.That(args[1].ToString(), Is.EqualTo("2"));
            Assert.That(args[2].ToString(), Is.EqualTo("true"));
            return Value.CreateString("ok");
        });
        var result = runtime.Evaluate(expressions);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }
}

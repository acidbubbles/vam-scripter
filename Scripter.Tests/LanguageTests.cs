using System.Linq;
using ScripterLang;

namespace Scripter.Tests;

public class Tests
{
    [Test]
    public void DeclareVar()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var tokens = Tokenizer.Tokenize(source).ToList();
        var expressions = Parser.ParseExpressions(tokens);
        var result = new Runtime().Evaluate(expressions);

        Assert.That(result.ToString(), Is.EqualTo("1"));
    }
}

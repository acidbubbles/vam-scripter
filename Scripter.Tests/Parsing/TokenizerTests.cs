using System.Linq;
using ScripterLang;

namespace Scripter.Tests.Parsing;

[TestFixture]
public class TokenizerTests
{
    [Test]
    public void VariableDeclaration()
    {
        var tokens = Tokenizer.Tokenize("""
            var x = 12;
            """);

        Assert.That(tokens.Select(t => t.Value), Is.EqualTo(new[] { "var", "x", "=", "12", ";" }));
    }

    [Test]
    public void AssignationsDeclaration()
    {
        var tokens = Tokenizer.Tokenize("""
            x = 1;
            y = 2;
            """);

        Assert.That(tokens.Select(t => t.Value), Is.EqualTo(new[] { "x", "=", "1", ";", "y", "=", "2", ";" }));
    }

    [Test]
    public void FunctionCall()
    {
        var tokens = Tokenizer.Tokenize("""
            log("something", 56, false);
            """);

        Assert.That(tokens.Select(t => t.Value), Is.EqualTo(new[] { "log", "(", "something", ",", "56", ",", "false", ")", ";" }));
    }

    [Test]
    public void IfStatement()
    {
        var tokens = Tokenizer.Tokenize("""
            if(x == 1) {
                ok();
            } else if (!x) {
                fail();
            }
            """);

        Assert.That(tokens.Select(t => t.Value), Is.EqualTo(new[] { "if", "(", "x", "==", "1", ")", "{", "ok", "(", ")", ";", "}", "else", "if", "(", "!", "x", ")", "{", "fail", "(", ")", ";", "}" }));
    }
}

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
            var x = 12.34;
            """);

        Assert.That(tokens.Select(t => t.value), Is.EqualTo(new[] { "var", "x", "=", "12.34", ";" }));
    }

    [Test]
    public void AssignationsDeclaration()
    {
        var tokens = Tokenizer.Tokenize("""
            x = 1;
            y = 2;
            """);

        Assert.That(tokens.Select(t => t.value), Is.EqualTo(new[] { "x", "=", "1", ";", "y", "=", "2", ";" }));
    }

    [Test]
    public void FunctionCall()
    {
        var tokens = Tokenizer.Tokenize("""
            log("something", 56, false);
            """);

        Assert.That(tokens.Select(t => t.value), Is.EqualTo(new[] { "log", "(", "something", ",", "56", ",", "false", ")", ";" }));
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

        Assert.That(tokens.Select(t => t.value), Is.EqualTo(new[] { "if", "(", "x", "==", "1", ")", "{", "ok", "(", ")", ";", "}", "else", "if", "(", "!", "x", ")", "{", "fail", "(", ")", ";", "}" }));
    }

    [Test]
    public void Comments()
    {
        var tokens = Tokenizer.Tokenize("""
            // Comment
            var x = 1;
            /*
            x = x + 1;
            */
            return x;
            """);

        Assert.That(tokens.Select(t => t.value), Is.EqualTo(new[] { "var", "x", "=", "1", ";", "return", "x", ";" }));
    }

    [Test]
    public void Operators()
    {
        var tokens = Tokenizer.Tokenize("""
            x /= 2.3;
            """);

        Assert.That(tokens.Select(t => t.value), Is.EqualTo(new[] { "x", "/=", "2.3", ";" }));
    }
}

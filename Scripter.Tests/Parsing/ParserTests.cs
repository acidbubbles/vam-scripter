using System.Linq;
using ScripterLang;

namespace Scripter.Tests.Parsing;

public class ParserTests
{
    [Test]
    public void VariableDeclaration()
    {
        var tokens = new[]
        {
            new Token(TokenType.Keyword, "var", 0),
            new Token(TokenType.Identifier, "x", 0),
            new Token(TokenType.Assignment, "=", 0),
            new Token(TokenType.Number, "123", 0),
        };

        var expressions = Parser.ParseExpressions(tokens).ToList();

        Assert.That(expressions, Has.Count.EqualTo(1));
        Assert.That(expressions[0], Is.TypeOf<DeclareExpression>());
        var declare = (DeclareExpression)expressions[0];
        Assert.That(declare.Name, Is.EqualTo("x"));
        Assert.That(declare.Expression, Is.TypeOf<NumberExpression>());
    }
}

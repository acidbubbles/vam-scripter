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
            new Token(TokenType.Integer, "123", 0),
            new Token(TokenType.SemiColon, ";", 0),
        };

        var expression = (CodeBlockExpression)Parser.Parse(tokens);

        Assert.That(expression.Expressions, Has.Count.EqualTo(1));
        Assert.That(expression.Expressions[0], Is.TypeOf<VariableDeclarationExpression>());
        var declare = (VariableDeclarationExpression)expression.Expressions[0];
        Assert.That(declare.Name, Is.EqualTo("x"));
        Assert.That(declare.Expression, Is.TypeOf<FloatExpression>());
        var number = (FloatExpression)declare.Expression;
        Assert.That(number.Value, Is.EqualTo(123));
    }

    [Test]
    public void IfStatement()
    {
        var tokens = new[]
        {
            new Token(TokenType.Keyword, "if", 0),
            new Token(TokenType.LeftParenthesis, "(", 0),
            new Token(TokenType.Identifier, "x", 0),
            new Token(TokenType.Operator, "==", 0),
            new Token(TokenType.Integer, "1", 0),
            new Token(TokenType.RightParenthesis, ")", 0),
            new Token(TokenType.LeftBrace, "{", 0),
            new Token(TokenType.Identifier, "MyFunc", 0),
            new Token(TokenType.LeftParenthesis, "(", 0),
            new Token(TokenType.RightParenthesis, ")", 0),
            new Token(TokenType.SemiColon, ";", 0),
            new Token(TokenType.RightBrace, "}", 0),
        };

        var expression = (CodeBlockExpression)Parser.Parse(tokens);

        Assert.That(expression.Expressions, Has.Count.EqualTo(1));
        Assert.That(expression.Expressions[0], Is.TypeOf<IfExpression>());
    }
}

using ScripterLang;

namespace Scripter.Tests.Parsing;

public class ParserTests
{
    private GlobalLexicalContext _globalLexicalContext;

    [SetUp]
    public void SetUp()
    {
        _globalLexicalContext = new GlobalLexicalContext();
    }

    [Test]
    public void VariableDeclaration()
    {
        var tokens = new[]
        {
            new Token(TokenType.Keyword, "var", new Location()),
            new Token(TokenType.Identifier, "x", new Location()),
            new Token(TokenType.Assignment, "=", new Location()),
            new Token(TokenType.Integer, "123", new Location()),
            new Token(TokenType.SemiColon, ";", new Location()),
        };

        var expression = Parser.Parse(tokens, _globalLexicalContext);

        Assert.That(expression, Is.TypeOf<FunctionBlockExpression>());
    }

    [Test]
    public void IfStatement()
    {
        var tokens = new[]
        {
            new Token(TokenType.Keyword, "if", new Location()),
            new Token(TokenType.LeftParenthesis, "(", new Location()),
            new Token(TokenType.Identifier, "x", new Location()),
            new Token(TokenType.Operator, "==", new Location()),
            new Token(TokenType.Integer, "1", new Location()),
            new Token(TokenType.RightParenthesis, ")", new Location()),
            new Token(TokenType.LeftBrace, "{", new Location()),
            new Token(TokenType.Identifier, "MyFunc", new Location()),
            new Token(TokenType.LeftParenthesis, "(", new Location()),
            new Token(TokenType.RightParenthesis, ")", new Location()),
            new Token(TokenType.SemiColon, ";", new Location()),
            new Token(TokenType.RightBrace, "}", new Location()),
        };

        var expression = Parser.Parse(tokens, _globalLexicalContext);

        Assert.That(expression, Is.TypeOf<FunctionBlockExpression>());
    }
}

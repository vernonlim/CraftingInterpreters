namespace CraftingInterpreters.Lox;

using static TokenType;

class Scanner
{
    private readonly string source;
    private readonly IList<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private static readonly Dictionary<string, TokenType> keywords = new()
    {
        {"and", AND},
        {"class", CLASS},
        {"else", ELSE},
        {"false", FALSE},
        {"for", FOR},
        {"fun", FUN},
        {"if", IF},
        {"nil", NIL},
        {"or", OR},
        {"print", PRINT},
        {"return", RETURN},
        {"super", SUPER},
        {"this", THIS},
        {"var", VAR},
        {"while", WHILE},
    };

    public Scanner(string source)
    {
        this.source = source;
    }

    public IList<Token> ScanTokens()
    {
        while (!IsAtEnd)
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(EOF, "", null, line));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(LEFT_PAREN); break;
            case ')': AddToken(RIGHT_PAREN); break;
            case '{': AddToken(LEFT_BRACE); break;
            case '}': AddToken(RIGHT_BRACE); break;
            case ',': AddToken(COMMA); break;
            case '.': AddToken(DOT); break;
            case '-': AddToken(MINUS); break;
            case '+': AddToken(PLUS); break;
            case ';': AddToken(SEMICOLON); break;
            case '*': AddToken(STAR); break;
            case '!': AddToken(Match('=') ? BANG_EQUAL : BANG); break;
            case '=': AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
            case '<': AddToken(Match('=') ? LESS_EQUAL : LESS); break;
            case '>': AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;
            case '/':
                if (Match('/'))
                {
                    // A comment goes until the end of line
                    while (Peek() != '\n' && !IsAtEnd) Advance();
                }
                else
                {
                    AddToken(SLASH);
                }
                break;

            case ' ': break;
            case '\r': break;
            case '\t': break;
            case '\n': line++; break;
            case '"': ScanString(); break;
            default:
                if (char.IsDigit(c))
                {
                    ScanNumber();
                }
                else if (IsAlpha(c))
                {
                    ScanIdentifier();
                }
                else
                {
                    Lox.Error(line, "Unexpected Character.");
                }
                break;
        }
    }

    private void ScanIdentifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = source[start..current];

        if (!keywords.TryGetValue(text, out TokenType type))
        {
            type = IDENTIFIER;
        }

        AddToken(type);
    }

    private void ScanNumber()
    {
        while (char.IsDigit(Peek())) Advance();

        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            Advance();

            while (char.IsDigit(Peek())) Advance();
        }

        AddToken(NUMBER,
            double.Parse(source[start..current]));
    }

    private void ScanString()
    {
        while (Peek() != '"' && !IsAtEnd)
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd)
        {
            Lox.Error(line, "Unterminated String.");
            return;
        }

        // Closing '"'
        Advance();

        // Trim the surrounding quotes
        string value = source[(start + 1)..(current - 1)];
        AddToken(STRING, value);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    private char Peek() => IsAtEnd ? '\0' : source[current];

    private char PeekNext() => (current + 1 >= source.Length) ? '\0' : source[current + 1];

    private bool IsAlpha(char c)
    {
        return char.IsAsciiLetter(c) || c == '_';
    }

    private bool IsAlphaNumeric(char c)
    {
        return char.IsAsciiLetter(c) || char.IsAsciiDigit(c);
    }

    private bool IsAtEnd => current >= source.Length;

    private char Advance()
    {
        return source[current++];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object? literal)
    {
        string text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }
}
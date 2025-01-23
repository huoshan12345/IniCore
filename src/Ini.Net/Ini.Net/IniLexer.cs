namespace Ini.Net;

public class IniLexer
{
    private readonly string _input; // the string being scanned
    private int _start; // start position of the current item
    private int _position; // current position in the input
    private int _line; // current line number in the input
    private int _column; // current column number in the current line
    private NextState? _state; // current state
    private readonly Queue<IniToken> _tokens = []; // channel of scanned items

    public IniLexer(string input)
    {
        _input = input;
        _state = LexStart;
    }

    public IEnumerable<IniToken> LexTokens()
    {
        while (true)
        {
            if (_tokens.Count > 0)
            {
                var item = _tokens.Dequeue();
                if (item.Type == IniTokenType.Eof)
                {
                    yield break;
                }
                else
                {
                    yield return item;
                }
            }
            else if (_state == null)
            {
                yield break;
            }
            else
            {
                _state = _state();
            }
        }
    }

    private NextState? EmitEof()
    {
        Emit(IniTokenType.Eof);
        return null;
    }

    private IniToken EnqueueToken(IniTokenType type, string value)
    {
        var token = new IniToken(type, value, _position, _line, _column);
        _tokens.Enqueue(token);
        return token;
    }

    /// <summary>
    /// Emits a token
    /// </summary>
    /// <param name="type"></param>
    private void Emit(IniTokenType type)
    {
        Debug.Assert(_start <= _position);
        Emit(type, _input[_start.._position]);
    }

    private void Emit(IniTokenType type, string value)
    {
        Debug.Assert(_start <= _position);
        EnqueueToken(type, value);
        _start = _position;
    }

    private void EmitLineFeed()
    {
        Emit(IniTokenType.LineFeed, "\n");
        _line++;
        _column = 0;
    }

    /// <summary>
    /// Returns the next rune in the input
    /// </summary>
    /// <returns></returns>
    private char? Next()
    {
        var ch = Peek();
        if (ch is not null)
        {
            _position++;
            _column++;
        }
        return ch;
    }

    /// <summary>
    /// Returns but does not consume the next rune in the input.
    /// </summary>
    /// <returns></returns>
    private char? Peek()
    {
        return _position >= _input.Length
            ? null
            : _input[_position];
    }

    /// <summary>
    /// Skip space and tab characters.
    /// </summary>
    private void SkipBlankSpaces()
    {
        SkipWhile(m => m.IsBlankSpace());
    }

    /// <summary>
    /// Skip white space characters.
    /// </summary>
    private void SkipWhiteSpaces()
    {
        SkipWhile(m => m.IsWhiteSpace());
    }

    /// <summary>
    /// Skip chars until a specified condition is false
    /// </summary>
    /// <param name="predicate"></param>
    private void SkipWhile(Func<char?, bool> predicate)
    {
        while (true)
        {
            var ch = Peek();
            if (predicate(ch) == false)
                break;

            if (ch.IsLineFeed())
                EmitLineFeed();

            _position++;
            _column++;
        }
        _start = _position;
    }

    /// <summary>
    /// Returns an error token and terminates the scan by passing back
    /// a null pointer that will be the next state, terminating Run.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private NextState? Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
    {
        EnqueueToken(IniTokenType.Error, string.Format(format, args));
        return null;
    }

    private NextState? LexStart()
    {
        SkipWhiteSpaces();

        var ch = Peek();
        if (ch.IsCommentStarter())
            return LexComment();
        if (ch is '[')
            return LexSection();
        if (ch.IsEof())
            return EmitEof();
        return LexKeyValuePair();
    }

    private NextState? LexComment()
    {
        Debug.Assert(Peek().IsCommentStarter());

        while (true)
        {
            var ch = Next();
            if (ch.IsNewLine())
            {
                _position--;
                break;
            }
            else if (ch.IsEof())
            {
                break;
            }
        }
        Emit(IniTokenType.Comment);
        return LexStart();
    }

    private NextState? LexSection()
    {
        return LexString(IniTokenType.SectionName);
    }

    private NextState? LexKeyValuePair()
    {
        return LexString(IniTokenType.Key, () => LexString(IniTokenType.Value));
    }

    private NextState? LexString(IniTokenType type, NextState? next = null)
    {
        if (type == IniTokenType.SectionName)
        {
            var ch = Next();
            Debug.Assert(ch == '[');
        }

        SkipBlankSpaces();

        var hasLineFeed = false;
        var inQuotes = false;
        var quotes = new Stack<char>();
        bool? ended = type is IniTokenType.SectionName or IniTokenType.Key
            ? false
            : null; // null means don't need to check.
        var trailingSpaceCount = 0;
        var disallowEmpty = type.IsIdentifier();

        using var disposable = StringBuilderHelper.GetCached();
        var builder = disposable.Value;

        while (true)
        {
            // TODO: add support for escape. The quote ' or " can be escaped by \
            var ch = Next();
            if (ch.IsQuote())
            {
                if (quotes.Count > 0 && ch == quotes.Peek())
                {
                    quotes.Pop();
                    inQuotes = false;
                }
                else
                {
                    quotes.Push(ch.Value);
                    inQuotes = true;
                }

                if (type == IniTokenType.SectionName)
                    builder.Append(ch.Value);
            }
            else if (ch.IsEof())
            {
                if (inQuotes)
                    return Error("unterminated quoted string");

                break;
            }
            else
            {
                if (inQuotes == false)
                {
                    if (ch.IsCommentStarter())
                    {
                        _position--;
                        next = LexComment + next;

                        break;
                    }
                    else if (ch.IsNewLine())
                    {
                        if (ch.IsLineFeed())
                            hasLineFeed = true;

                        break;
                    }
                    else if (ch == ']' && type.IsSectionNamePart())
                    {
                        ended = true;
                        break;
                    }
                    else if (ch == '.' && type.IsSectionNamePart())
                    {
                        // For relative nesting, there is a leading dot expresses nesting to the previous section.
                        // The section name is empty at this time.
                        if (type == IniTokenType.SectionName)
                            disallowEmpty = false;

                        next = LexSubsectionName + next;
                        ended = true;
                        break;

                        NextState? LexSubsectionName() => LexString(IniTokenType.SubsectionName);
                    }
                    else if (type == IniTokenType.Key && ch.IsKeyValueDelimiter())
                    {
                        ended = true;
                        break;
                    }

                    if (ch.IsBlankSpace())
                    {
                        if (builder.Length == 0)
                            continue;

                        trailingSpaceCount++;
                    }
                    else
                    {
                        trailingSpaceCount = 0;
                    }
                }
                else
                {
                    trailingSpaceCount = 0;
                }

                builder.Append(ch.Value);
            }
        }

        if (ended == false)
        {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            var error = type switch
            {
                IniTokenType.SectionName => "section name is not ended due to missing ']'",
                IniTokenType.Key => "key is not ended due to missing '=' or ':'",
                _ => throw new InvalidOperationException("Unexpected type: " + type),
            };
            return Error(error);
        }

        var span = builder.ToString(0, builder.Length - trailingSpaceCount).AsSpan();

        if (type.IsIdentifier())
            span = TrimQuotePair(span);

        if (span.Length == 0 && disallowEmpty)
            return Error($"{type} cannot be empty");

        var str = span.ToString();
        Emit(type, str);

        if (hasLineFeed)
            EmitLineFeed();

        return next ?? LexStart();

        static ReadOnlySpan<char> TrimQuotePair(ReadOnlySpan<char> span)
        {
            var i = 0;
            var j = span.Length - 1;
            for (; i < span.Length && i <= j; i++, j--)
            {
                if (IsQuotePair(span[i], span[j]) == false)
                    break;
            }
            return span.Slice(i, j - i + 1);
        }

        static bool IsQuotePair(char value, char other)
        {
            return value.IsQuote() && value == other;
        }
    }
}

file static class Extensions
{
    /// <summary>
    /// Check a char is space, tab, or newline.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsWhiteSpace([NotNullWhen(true)] this char? value)
    {
        return value is { } ch && char.IsWhiteSpace(ch);
    }

    /// <summary>
    /// Check a char is space, tab.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsBlankSpace([NotNullWhen(true)] this char? value)
    {
        return value.IsWhiteSpace() && value.IsNewLine() == false;
    }

    public static bool IsQuote(this char value)
    {
        return ((char?)value).IsQuote();
    }

    public static bool IsQuote([NotNullWhen(true)] this char? value)
    {
        return value is '\'' or '"';
    }

    public static bool IsCarriageReturn([NotNullWhen(true)] this char? value)
    {
        return value is '\r';
    }

    public static bool IsLineFeed([NotNullWhen(true)] this char? value)
    {
        return value is '\n';
    }

    public static bool IsNewLine([NotNullWhen(true)] this char? value)
    {
        return value.IsCarriageReturn() || value.IsLineFeed();
    }

    public static bool IsEof([NotNullWhen(false)] this char? value)
    {
        return value is null;
    }

    public static bool IsCommentStarter([NotNullWhen(true)] this char? value)
    {
        return value is '#' or ';';
    }

    public static bool IsKeyValueDelimiter([NotNullWhen(true)] this char? value)
    {
        return value is '=' or ':';
    }
}
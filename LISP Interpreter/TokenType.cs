public enum TokenType
{
    // Single-character tokens.
    LEFT_PAREN, RIGHT_PAREN,
    COMMA, MINUS, PLUS, SLASH, STAR,

    // One or two character tokens.
    EQUAL, EQUAL_EQUAL,
    GREATER, GREATER_EQUAL,
    LESS, LESS_EQUAL,

    // Literals.
    IDENTIFIER, STRING, NUMBER,

    // Keywords.
    AND, ELSE, FALSE, NIL, OR,
    PRINT, TRUE, COND, CONS, DEFINE, CAR, CDR,

    EOF
}


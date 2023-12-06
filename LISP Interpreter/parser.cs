using System;
using System.Security.Claims;
using static Stmt;

class Parser
{
    private class ParseError : Exception { }
    private List<Token> tokens;
    private int current = 0;
    public Parser(List<Token> newtoken)
    {
        this.tokens = newtoken;
    }

    public List<Stmt> parse()
    {
        List<Stmt> statements = new List<Stmt>();
        while (!isAtEnd())
        {
            statements.Add(declaration());
        }

        return statements;
    }

    private Expr expression()
    {
        return assignment();
    }
    private Stmt declaration()
    {
        try
        {

            if (match(TokenType.LEFT_PAREN)) return varDeclaration();

            return statement();
        }
        catch (ParseError error)
        {
            synchronize();
            return null;
        }
    }
    
    private Stmt statement()
    {
        if (match(TokenType.COND)) return condStatement();
        return expressionStatement();
    }
    private Stmt condStatement()
    {
        consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
        Expr condition = expression();
        consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

        Stmt thenBranch = statement();
        Stmt elseBranch = null;
        if (match(TokenType.ELSE))
        {
            elseBranch = statement();
        }

        return new Stmt.cond(condition, thenBranch, elseBranch);
    }
    
    private Stmt returnStatement()
    {
        Token keyword = previous();
        Expr value = null;
        if (!check(TokenType.SEMICOLON))
        {
            value = expression();
        }

        consume(TokenType.SEMICOLON, "Expect ';' after return value.");
        return new Stmt.Return(keyword, value);
    }
    private Stmt varDeclaration()
    {
        Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");

        Expr initializer = null;
        if (match(TokenType.EQUAL))
        {
            initializer = expression();
        }

        consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
        return new Stmt.Var(name, initializer);
    }
   
    private Stmt expressionStatement()
    {
        Expr expr = expression();
        consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new Stmt.Expression(expr);
    }
    private Stmt.Function function(String kind)
    {
        Token name = consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
        consume(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name.");
        List<Token> parameters = new List<Token>();
        if (!check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    error(peek(), "Can't have more than 255 parameters.");
                }

                parameters.Add(
                    consume(TokenType.IDENTIFIER, "Expect parameter name."));
            } while (match(TokenType.COMMA));
        }
        consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");
        
        List<Stmt> body = block();
        return new Stmt.Function(name, parameters, body);
    }

    private List<Stmt> block()
    {
        List<Stmt> statements = new List<Stmt>();

        while (!check(TokenType.RIGHT_PAREN) && !isAtEnd())
        {
            statements.Add(declaration());
        }

        consume(TokenType.RIGHT_PAREN, "Expect ')' after block.");
        return statements;
    }
    private Expr assignment()
    {
        Expr expr = or();

        if (match(TokenType.EQUAL))
        {
            Token equals = previous();
            Expr value = assignment();

            if (expr is Expr.Variable)
            {
                Token name = ((Expr.Variable)expr).name;
                return new Expr.Assign(name, value);
            }
            else if (expr is Expr.Get)
            {
                Expr.Get get = (Expr.Get)expr;
                return new Expr.Set(get.obj, get.name, value);
            }

            error(equals, "Invalid assignment target.");
        }

        return expr;
    }
    private Expr or()
    {
        Expr expr = and();

        while (match(TokenType.OR))
        {
            Token op = previous();
            Expr right = and();
            expr = new Expr.Logical(expr, op, right);
        }

        return expr;
    }
    private Expr and()
    {
        Expr expr = equality();

        while (match(TokenType.AND))
        {
            Token op = previous();
            Expr right = equality();
            expr = new Expr.Logical(expr, op, right);
        }

        return expr;
    }
    private Expr equality()
    {
        Expr expr = comparison();

        while (match(TokenType.EQUAL_EQUAL))
        {
            Token op = previous();
            Expr right = comparison();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }
    private Expr comparison()
    {
        Expr expr = term();

        while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = previous();
            Expr right = term();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }
    private Expr term()
    {
        Expr expr = factor();

        while (match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = previous();
            Expr right = factor();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }
    private Expr factor()
    {
        Expr expr = unary();

        while (match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = previous();
            Expr right = unary();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }
    private Expr unary()
    {
        if (match(TokenType.MINUS))
        {
            Token op = previous();
            Expr right = unary();
            return new Expr.Unary(op, right);
        }

        return call();
    }
    private Expr call()
    {
        Expr expr = primary();

        while (true)
        {
            if (match(TokenType.LEFT_PAREN))
            {
                expr = finishCall(expr);
            }
            
            else
            {
                break;
            }
        }

        return expr;
    }
    private Expr finishCall(Expr callee)
    {
        List<Expr> arguments = new List<Expr>();
        if (!check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    error(peek(), "Can't have more than 255 arguments.");
                }
                arguments.Add(expression());
            } while (match(TokenType.COMMA));
        }

        Token paren = consume(TokenType.RIGHT_PAREN,
                              "Expect ')' after arguments.");

        return new Expr.Call(callee, paren, arguments);
    }
    private Expr primary()
    {
        if (match(TokenType.FALSE)) return new Expr.Literal(false);
        if (match(TokenType.TRUE)) return new Expr.Literal(true);
        if (match(TokenType.NIL)) return new Expr.Literal(null);

        if (match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(previous().literal);
        }
       
        

        if (match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable(previous());
        }
        if (match(TokenType.LEFT_PAREN))
        {
            Expr expr = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }
        throw error(peek(), "Expect expression.");
    }

    private Boolean match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (check(type))
            {
                advance();
                return true;
            }
        }

        return false;
    }
    private Token consume(TokenType type, String message)
    {
        if (check(type)) return advance();

        throw error(peek(), message);
    }
    private Boolean check(TokenType type)
    {
        if (isAtEnd()) return false;
        return peek().type == type;
    }
    private Token advance()
    {
        if (!isAtEnd()) current++;
        return previous();
    }
    private Boolean isAtEnd()
    {
        return peek().type == TokenType.EOF;
    }

    private Token peek()
    {
        // Console.WriteLine(tokens[current].type);
        return tokens[current];
    }

    private Token previous()
    {
        return tokens[current - 1];
    }
    private ParseError error(Token token, String message)
    {
        Lisp.error(token, message);
        return new ParseError();
    }
    private void synchronize()
    {
        advance();

        while (!isAtEnd())
        {
            if (previous().type == TokenType.SEMICOLON) return;

            switch (peek().type)
            {
                case TokenType.COND


                    return;
            }

            advance();
        }
    }
}


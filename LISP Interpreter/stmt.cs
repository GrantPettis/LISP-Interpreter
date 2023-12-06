using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using static Expr;

public abstract class Stmt
{
    public interface Visitor<R>
    {
        R visitExpressionStmt(Expression stmt);
        R visitPrintStmt(Print stmt);
        R visitVarStmt(Var stmt);

        R visitBlockStmt(Block stmt);
        R visitcondStmt(cond stmt);

        R visitWhileStmt(While stmt);
        R visitFunctionStmt(Function stmt);

        R visitReturnStmt(Return stmt);

      

    }
    public abstract R accept<R>(Visitor<R> visitor);
    public class Expression : Stmt
    {
        public Expression(Expr expression)
        {
            this.expression = expression;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitExpressionStmt(this);
        }

        public Expr expression;
    }
    public class Print : Stmt
    {
        public Print(Expr expression)
        {
            this.expression = expression;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitPrintStmt(this);
        }

        public Expr expression;
    }
    public class Var : Stmt
    {
        public Var(Token name, Expr initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitVarStmt(this);
        }

        public Token name;
        public Expr initializer;
    }
    public class Block : Stmt
    {
        public Block(List<Stmt> statements)
        {
            this.statements = statements;
        }
        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitBlockStmt(this);
        }
        public List<Stmt> statements;
    }
    public class cond : Stmt
    {
        public cond(Expr condition, Stmt thenBranch, Stmt elseBranch)
        {
            this.condition = condition;
            this.thenBranch = thenBranch;
            this.elseBranch = elseBranch;
        }

        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitcondStmt(this);
        }

        public Expr condition;
        public Stmt thenBranch;
        public Stmt elseBranch;
    }
    public class While : Stmt
    {
        public While(Expr condition, Stmt body)
        {
            this.condition = condition;
            this.body = body;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitWhileStmt(this);
        }

        public Expr condition;
        public Stmt body;
    }
    public class Function : Stmt
    {
        public Function(Token name, List<Token> par, List<Stmt> body)
        {
            this.name = name;
            this.par = par;
            this.body = body;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitFunctionStmt(this);
        }

        public Token name;
        public List<Token> par;
        public List<Stmt> body;
    }
    public class Return : Stmt
    {
        public Return(Token keyword, Expr value)
        {
            this.keyword = keyword;
            this.value = value;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitReturnStmt(this);
        }

        public Token keyword;
        public Expr value;
    }
   


}


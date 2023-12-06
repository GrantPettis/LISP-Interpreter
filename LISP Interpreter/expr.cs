public abstract class Expr
{
    public interface Visitor<R>
    {

        R visitAssignExpr(Assign expr);
        R visitBinaryExpr(Binary expr);
        R visitGroupingExpr(Grouping expr);
        R visitLiteralExpr(Literal expr);
        R visitUnaryExpr(Unary expr);
        R visitVariableExpr(Variable expr);

        R visitLogicalExpr(Logical expr);
        R visitCallExpr(Call expr);

        R visitGetExpr(Get expr);

      

       
        R visitSetExpr(Set expr);

    }
    public abstract R accept<R>(Visitor<R> visitor);
    public class Assign : Expr
    {
        public Assign(Token name, Expr value)
        {
            this.name = name;
            this.value = value;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitAssignExpr(this);
        }

        public Token name;
        public Expr value;
    }
    public class Binary : Expr
    {
        public Binary(Expr left, Token op, Expr right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitBinaryExpr(this);
        }

        public Expr left;
        public Token op;
        public Expr right;
    }
    public class Grouping : Expr
    {
        public Grouping(Expr expression)
        {
            this.expression = expression;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitGroupingExpr(this);
        }

        public Expr expression;
    }
    public class Literal : Expr
    {
        public Literal(Object value)
        {
            this.value = value;
        }


        public override R accept<R>(Visitor<R> visitor)
        {

            return visitor.visitLiteralExpr(this);
        }

        public Object value;
    }
    public class Unary : Expr
    {
        public Unary(Token op, Expr right)
        {
            this.op = op;
            this.right = right;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitUnaryExpr(this);
        }

        public Token op;
        public Expr right;
    }
    public class Variable : Expr
    {
        public Variable(Token name)
        {
            this.name = name;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitVariableExpr(this);
        }

        public Token name;
    }
    public class Logical : Expr
    {
        public Logical(Expr left, Token op, Expr right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitLogicalExpr(this);
        }

        public Expr left;
        public Token op;
        public Expr right;
    }
    public class Call : Expr
    {
        public Call(Expr callee, Token paren, List<Expr> arguments)
        {
            this.callee = callee;
            this.paren = paren;
            this.arguments = arguments;
        }

        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitCallExpr(this);
        }

        public Expr callee;
        public Token paren;
        public List<Expr> arguments;
    }

    public class Get : Expr
    {
        public Get(Expr obj, Token name)
        {
            this.obj = obj;
            this.name = name;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitGetExpr(this);
        }

        public Expr obj;
        public Token name;
    }
   
   
    public class Set : Expr
    {
        public Set(Expr obj, Token name, Expr value)
        {
            // Console.WriteLine("obj is " + obj);
            this.obj = obj;
            this.name = name;
            this.value = value;
        }


        public override R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitSetExpr(this);
        }

        public Expr obj;
        public Token name;
        public Expr value;
    }


}


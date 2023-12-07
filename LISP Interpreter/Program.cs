using System;
using System.IO;
using System.Runtime.InteropServices;

class Lisp
{
  
    static Boolean hadError = false;
   
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: c#LISP [script]");
            System.Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            runFile(args[0]);
        }
        else
        {
            runPrompt();
        }

    }
    static void runFile(String path)
    {
        string strings = File.ReadAllText(path);
        if (hadError) System.Environment.Exit(65);
       
        run(strings);
    }
    static void runPrompt()
    {

        StreamReader input = new StreamReader(Console.OpenStandardInput());

        for (; ; )
        {
            Console.WriteLine("> ");
            String? line = input.ReadLine();
            if (line == null) break;
            run(line);
            hadError = false;

        }
    }
    static void run(String source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();
        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.parse();


        if (hadError) return;
    }
    public static void error(int line, String message)
    {
        report(line, "", message);

    }

    private static void report(int line, String where, String message)
    {
        Console.Error.WriteLine(
            "[line " + line + "] Error" + where + ": " + message);
        hadError = true;
    }

    public static void error(Token token, String message)
    {
        if (token.type == TokenType.EOF)
        {
            report(token.line, " at end", message);
        }
        else
        {
            report(token.line, " at '" + token.lexeme + "'", message);
        }
    }
  
}






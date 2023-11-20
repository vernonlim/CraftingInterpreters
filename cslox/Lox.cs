using System.Text;

namespace CraftingInterpreters.Lox;

public class Lox
{
    internal static bool HadError = false;
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        string characters = File.ReadAllText(Path.GetFullPath(path), Encoding.Default);
        Run(characters);

        // indicating an error
        if (HadError)
        {
            Environment.Exit(65);
        }
    }

    private static void RunPrompt()
    {
        for (; ; )
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line is null)
            {
                break;
            }
            Run(line);
            HadError = false;
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new(source);
        IList<Token> tokens = scanner.ScanTokens();

        // For now, just print the tokens
        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    internal static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }
}
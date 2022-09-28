using System.Runtime.CompilerServices;

namespace Main.Util
{
    public static class Logger
    {
        private static void Print(ConsoleColor consoleColor, string level, string message, string callerName)
        {
            callerName = Path.GetFileNameWithoutExtension(callerName);
            Console.ForegroundColor = consoleColor;
            Console.Write($"[{DateTime.Now.ToString("HH:mm:ss")}] [{Path.GetFileNameWithoutExtension(callerName)}] [{level}] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
        }

        public static void Info(string message, [CallerFilePath] string callerName = "")
        {
            Print(ConsoleColor.Green, "INFO", message, callerName);
        }

        public static void Warn(string message, [CallerFilePath] string callerName = "")
        {
            Print(ConsoleColor.Yellow, "WARN", message, callerName);
        }

        public static void Error(string message, [CallerFilePath] string callerName = "")
        {
            Print(ConsoleColor.Red, "ERROR", message, callerName);
        }
    }
}

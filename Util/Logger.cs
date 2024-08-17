namespace Util
{
    public enum LogLevel
    {
        ERROR,
        WARN,
        INFO
    }

    public class Logger
    {
        public static LogLevel LogLevel { get; set; } = LogLevel.INFO;

        static void Print(LogLevel logLevel, String message)
        {
            if (logLevel > LogLevel)
            {
                return;
            }

            TextWriter textWriter = logLevel switch
            {
                LogLevel.ERROR => Console.Error,
                _ => Console.Out,
            };

            textWriter.WriteLine($"[{logLevel}] [{DateTime.Now}] {message}");
        }

        public static void Error(String message)
        {
            Print(LogLevel.ERROR, message);
        }

        public static void Warn(String message)
        {
            Print(LogLevel.WARN, message);
        }

        public static void Info(String message)
        {
            Print(LogLevel.INFO, message);
        }
    }
}

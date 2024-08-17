namespace Util
{
    public class Logger
    {
        public enum LogLevel
        {
            ERROR,
            WARN,
            INFO
        }

        private LogLevel _logLevel;

        public Logger(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        void Print(LogLevel logLevel, String message)
        {
            if (logLevel > _logLevel)
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

        public void Error(String message)
        {
            Print(LogLevel.ERROR, message);
        }

        public void Warn(String message)
        {
            Print(LogLevel.WARN, message);
        }

        public void Info(String message)
        {
            Print(LogLevel.INFO, message);
        }
    }
}

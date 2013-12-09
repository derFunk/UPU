namespace UpuCore.Config
{
    public interface ILogger
    {
        void LogDebug(string message, params object[] parameters);
        void LogInfo(string message, params object[] parameters);
        void LogWarn(string message, params object[] parameters);
        void LogError(string message, params object[] parameters);
        void LogFatal(string message, params object[] parameters);
    }
}

﻿namespace Contracts
{
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string messgae);
        void LogError(string message);
    }
}

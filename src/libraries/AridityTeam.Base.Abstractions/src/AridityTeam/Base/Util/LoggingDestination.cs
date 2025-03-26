namespace AridityTeam.Base.Util;

public enum LoggingDestination
{
    LogNone = 0,
    LogToFile = 1,
    LogToSystemDebug = 2,
    LogToStderr = 3,
    LogToAll = LogToFile | LogToSystemDebug | LogToStderr,
    
    LogDefault = LogToAll
}
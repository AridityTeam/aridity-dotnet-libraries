namespace AridityTeam.Base.Util;

public static class LoggingDestinationExtensions
{
    public static bool IsEnabled(this LoggingDestination destination, LoggingDestination check)
    {
        return (destination & check) == check;
    }
}
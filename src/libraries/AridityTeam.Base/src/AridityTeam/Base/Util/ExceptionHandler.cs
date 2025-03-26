using System;

namespace AridityTeam.Base.Util;

public static class ExceptionHandler
{
    public static void LogException(Exception ex)
    {
        // Here you can implement your logging logic (e.g., log to a file, database, etc.)
        Console.WriteLine($"Exception: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    }

    public static Result<T> HandleException<T>(Func<Result<T>> func)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            LogException(ex);
            return Result<T>.Failure("An error occurred while processing your request.");
        }
    }
}
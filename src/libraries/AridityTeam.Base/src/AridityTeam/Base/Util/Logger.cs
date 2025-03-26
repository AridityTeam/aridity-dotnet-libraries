/*
 * Copyright (c) 2025 The Aridity Team
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 */
using System;
using System.Threading;
using System.Runtime.CompilerServices;
using Sentry;

namespace AridityTeam.Base.Util;

/// <summary>
/// Ah yes. The so-called "Chromium" logger in ".NET".
/// <para>
/// This kind of replicates Chromium's logging format + some of the code were just copied from the logger code of Chromium. (just some C++ keywords were changed)
/// </para>
/// </summary>
public class Logger : ILogger
{
    private LogMessageHandlerFunction? _logMessageHandler = null;
    private LoggingSettings? _settings = null;
    private static Logger? _instance = null;
    private IDisposable? _sentry = null;
    //private RemovableConcurrentBag<LoggerBuilder> _loggingExts = new RemovableConcurrentBag<LoggerBuilder>();
    private static readonly Lock _lock = new Lock();
    private LogMessage? _logMsg = null;

    public static Logger Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null) _instance = new Logger();
                return _instance;
            }
        }
    }
    
    public int GetMinLogLevel()
    {
        return (int)LogSeverity.LogInfo;
    }

    public int GetVLogLevel(string idk)
    {
        return int.MaxValue;
    }

    public void SetLogMessage(LogMessage msg)
    {
        _logMsg = msg;
    }

    public bool InitLogging(LoggingSettings settings)
    {
        _settings = settings;

        if (settings.EnableSentry == true)
        {
            if (settings.SentryOptions == null) return false;
            
            _sentry = SentrySdk.Init(settings.SentryOptions);
        }
        
        return true;
    }

    public void Log(LogSeverity severity, string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int line = 0)
    {
        if (_logMsg == null) _logMsg = new LogMessage("", filePath, line, (int)severity);
        _logMsg?.GetWriter()?.WriteLine(message);
        if (_settings?.EnableSentry == true)
            SentrySdk.CaptureMessage(_logMsg?.GetLastOutput() + message, severity.ToSentryLevel());
        _logMsg?.Dispose();
        _logMsg = null;
    }

    public void VLog(string message, int verbosity,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int line = 0)
    {
        if (_logMsg == null) _logMsg = new LogMessage("", filePath, line, verbosity);
        _logMsg?.GetWriter()?.WriteLine(message);
        _logMsg?.Dispose();
        _logMsg = null;
    }

    public void Dispose()
    {
        _sentry?.Dispose();
        _logMsg?.Dispose();
        _logMsg = null;
    }
    
    public LoggingDestination GetDestination()
    {
        if (_settings != null) return _settings.Destination;
        return LoggingDestination.LogDefault;
    }

    public void SetLogMessageHandler(LogMessageHandlerFunction handler) => _logMessageHandler = handler;
    public LogMessageHandlerFunction? GetLogMessageHandler() => _logMessageHandler;
}
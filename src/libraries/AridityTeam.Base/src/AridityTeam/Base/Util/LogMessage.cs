using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AridityTeam.Base.Util;

/// <summary>
/// <para>https://github.com/chromium/mini_chromium/blob/main/base/logging.cc</para>
/// <para>https://github.com/chromium/mini_chromium/blob/main/base/logging.h#L95</para>
/// </summary>
public class LogMessage : IDisposable
{
    private readonly string[] _logSeverityNames = {
        "INFO",
        "WARNING",
        "ERROR",
        "ERROR_REPORT",
        "FATAL"
    };

    private readonly System.Threading.Lock _lock = new System.Threading.Lock();
    
    private TextWriter? _writer = null;
    private readonly string _filePath;
    private readonly int _line = 0;
    private readonly LogMessageHandlerFunction? _handler = Logger.Instance.GetLogMessageHandler();
    private readonly LogSeverity _severity;
    private int? _messageStart;
    private string? _output = null;
    
    public LogMessage(string function, string filePath, int line, int severity)
    {
        _writer = Console.Out;
        _filePath = filePath;
        _messageStart = 0;
        _output = null;
        _line = line;
        _severity = (LogSeverity)severity;
        Init(function);
    }
    public LogMessage(string function, string filePath, int line, string result)
    {
        _writer = Console.Out;
        _filePath = filePath;
        _line = line;
        _messageStart = 0;
        _output = null;
        _severity = LogSeverity.LogFatal;
        Init(function);
        _writer.WriteLine("Check failed: " + result + ".");
    }

    public void Dispose()
    {
        Flush();
        GC.SuppressFinalize(this);
    }

    public void SetWriter(TextWriter? writer)
    {
        _writer = writer;
    }

    public TextWriter? GetWriter()
    {
        return _writer;
    }

    private void Init(string function)
    {
        lock (_lock)
        {
            var destination = Logger.Instance.GetDestination();
            var strBuilder = new StringBuilder();
            string fileName = _filePath;

            _writer = new MultiTextWriter(Console.Out);

            if (destination.IsEnabled(LoggingDestination.LogToFile))
            {
                if (_writer is MultiTextWriter writer)
                {
                    writer.AddWriter(new FileTextWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log"), true));
                }
                else 
                {
                    _writer = new FileTextWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log"), true);
                }
            }
            
            if (destination.IsEnabled(LoggingDestination.LogToStderr))
            {
                if (_writer is MultiTextWriter writer)
                {
                    writer.AddWriter(Console.Error);
                }
                else 
                {
                    _writer = Console.Error;
                }
            }

            int lastSlash;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                lastSlash = fileName.LastIndexOf("\\");
            else
                lastSlash = fileName.LastIndexOf('/');

            if (lastSlash != -1)
            {
                fileName = fileName.Substring(lastSlash + 1);
            }

            int pid = Process.GetCurrentProcess().Id;
            int thread = Environment.CurrentManagedThreadId;

            strBuilder.Append("[" +
                           pid +
                           ":" +
                           thread +
                           ":");
            
            DateTime localTime = DateTime.Now;
            
            strBuilder.Append(localTime.Year +
                           localTime.Month +
                           localTime.Day +
                           "," +
                           localTime.Hour +
                           localTime.Minute +
                           localTime.Second +
                           "." + localTime.Millisecond +
                           ":");

            if ((int)_severity >= 0)
            {
                strBuilder.Append(_logSeverityNames[(int)_severity]);
            }
            else
            {
                strBuilder.Append("VERBOSE" + -(int)_severity);
            }

            strBuilder.Append(" " +
                           fileName +
                           ":" +
                           _line +
                           "] ");
            
            _output = strBuilder.ToString();
            Debug.WriteLine("The output of the log message: "+_output,"Logger");
            _writer?.Write(_output);

            _messageStart = _writer?.ToString()?.Length;
        }
    }

    public string? GetLastOutput() => _output;

    private void Flush()
    {
        lock (_lock)
        {
            var destination = Logger.Instance.GetDestination();

            string? strNewLine = _writer?.NewLine;

            if (_handler != null && strNewLine != null &&
                _handler(_severity, _filePath, _line, _messageStart, strNewLine))
                return;

            if (destination.IsEnabled(LoggingDestination.LogToStderr))
            {
                Console.Error.Write("{0}", strNewLine);
                Console.Error.Flush();
            }

            if (destination.IsEnabled(LoggingDestination.LogToSystemDebug))
            {
                Debug.WriteLine(strNewLine); 
            }
            
            _writer?.Close();

            if (_severity == LogSeverity.LogFatal)
                BaseFunctions.ImmediateCrash("Fatal error detected!");// "base::ImmediateCrash" they say.
        }
    }
}
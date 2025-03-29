using System.Diagnostics;
using System.Runtime.Versioning;
using AridityTeam.Base;
using AridityTeam.Base.Util;

namespace ConCommandExample;

/// <summary>
/// This is a Linux-only program since I've just made a whole terminal shell by accident LMAO.
/// But you could still use some code from this or from FortressInstaller's ConsoleWindow.xaml.cs
/// </summary>
[SupportedOSPlatform("linux")]
static class Program
{
    private static readonly Logger Log = new();
    private static readonly Dictionary<string,object> LocalVars = [];
    //private static bool _commandRunning = false;
    
    static void Main(string[] args)
    {
        Log.Log(LogSeverity.LogInfo, "Welcome to the example app for managing ConVar and ConCommand!");
        Console.CancelKeyPress += (s, e) =>
        {
            // Set the Cancel property to true to prevent the process from terminating.
            e.Cancel = true;

            // only print the message if any command isn't running.
        };

        var quit = new ConCommand("quit", QuitCmdExecute);
        var exit = new ConCommand("exit", QuitCmdExecute);
        var help = new ConCommand("help", HelpCmdExecute);
        var cd = new ConCommand("cd", CdCmdExecute);
        var set = new ConCommand("set", SetCmdExecute);
        var get = new ConCommand("get", GetVarExecute);
        var setVar = new ConCommand("setVar", SetCmdExecute);
        var getVar = new ConCommand("getVar", GetVarExecute);
        Console.WriteLine("welcome to aridsh!");
        Console.WriteLine($"logged in as {Environment.UserName}");
        Console.WriteLine($"hostname is {Environment.MachineName}");
        InitInteractive();
    }

    static void GetVarExecute(ConCommandArgs? args)
    {
        var input = args?.AppliedArgs?.Trim();
        var parts = input?.Split([' ', '=', ':'], 2);
        if (string.IsNullOrEmpty(input)
            || parts?.Length < 1)
        {
            Console.WriteLine("getVar: you must enter an variable name!!!");
            return;
        }

        if (parts?.Length == 0) return;

        var name = parts?[0].Trim();

        if (string.IsNullOrEmpty(name)) return;
        
        var variable = LocalVars[name];
        Console.WriteLine($"{name}: {variable}");
    }

    static void SetCmdExecute(ConCommandArgs? args)
    {
        var input = args?.AppliedArgs?.Trim();
        var parts = input?.Split([' ', '=', ':'], 2);
        if (string.IsNullOrEmpty(input)
            || parts?.Length < 2)
        {
            Console.WriteLine("set: you must enter an variable name and value!!!");
            return;
        }

        if (parts?.Length == 0) return;

        var name = parts?[0].Trim();
        var val = parts?[1].Trim();

        if (string.IsNullOrEmpty(name)) return;
        if (string.IsNullOrEmpty(val)) return;
        
        if (LocalVars.ContainsKey(name))
        {
            LocalVars.Remove(name);
        }
        
        LocalVars.Add(name, val);
    }

    static void InitInteractive()
    {
        string directory = Environment.CurrentDirectory.Replace($"/home/{Environment.UserName}", "~");
        Console.Write($"{Environment.UserName}@{Environment.MachineName}:{directory}\n>> ");
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            Log.Log(LogSeverity.LogWarning, "You must enter a command!");
            InitInteractive();
            return;
        }

        if (input.Contains(';'))
        {
            Log.Log(LogSeverity.LogWarning, "command separator isn't supported on \"aridsh\""); 
            InitInteractive();
            return;
        }
        
        string[] parts = input.Split([' ', '=', ':'], 2); // Split into at most 2 parts
        if (parts.Length == 0) return;

        string name = parts[0].Trim();
        object? val = parts.Length > 1 ? parts[1].Trim() : null; // Get the value if it exists
        var conCmd = CommandManager.Instance.GetConCommandByName(name);
        var conVar = CommandManager.Instance.GetConVarByName(name);

        //_commandRunning = true;
        if (conCmd != null)
        {
            conCmd.Execute(val?.ToString());
        }
        else if (conVar != null)
        {
            conVar.SetValue(val);
        }
        else
        {
            var envVar = Environment.GetEnvironmentVariable("PATH")?.Trim().Split([':'], StringSplitOptions.RemoveEmptyEntries);
            if (envVar == null) return;
            foreach (var path in envVar)
            {
                string cmd = Path.Combine(path.Trim(), name);
                if (File.Exists(cmd))
                {
                    Process p = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = cmd,
                            Arguments = val?.ToString(),
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardInput = true,
                        }
                    };
                    
                    p.Start();
                    p.WaitForExit();
                    InitInteractive();
                    return;
                }
            }

            if (name.StartsWith("./"))
            {
                var cmd = name;
                if (File.Exists(cmd))
                {
                    Process p = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = cmd,
                            Arguments = val?.ToString(),
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardInput = true,
                        }
                    };
                    
                    p.Start();
                    p.WaitForExit();
                    InitInteractive();
                    return;
                }
            }

            Log.Log(LogSeverity.LogError, "Command '" + name + "' not found!");
        }

        InitInteractive();
        //_commandRunning = false;
    }

    static void QuitCmdExecute(ConCommandArgs? conCommandArgs)
    {
        Log.Log(LogSeverity.LogInfo, "Exiting...");
        Environment.Exit(0);
    }
    
    static void CdCmdExecute(ConCommandArgs? args)
    {
        string? input = args?.AppliedArgs?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("cd: you must enter an directory!!!");
            return;
        }

        string[] parts = input.Split([' ', '=', ':'], 2);
        if (parts.Length == 0) return;

        string dir = parts[0].Trim();

        if (dir.Contains('~', StringComparison.OrdinalIgnoreCase))
        {
            dir = dir.Replace("~", $"/home/{Environment.UserName}");
        }
        
        if (!Directory.Exists(dir))
        {
            
            Log.Log(LogSeverity.LogError, "Directory '" + dir + "' does not exist!");
            return;
        }
        
        Directory.SetCurrentDirectory(dir);
    }
    
    static void HelpCmdExecute(ConCommandArgs? args)
    {
        string? input = args?.AppliedArgs?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("help: you must enter an ConVar name!!!");
            return;
        }

        string[] parts = input.Split([' ', '=', ':'], 2);
        if (parts.Length == 0) return;

        string? name = parts[0].Trim();

        IConCommand? cmd = CommandManager.Instance.GetConCommandByName(name);
        IConVar? var = CommandManager.Instance.GetConVarByName(name);

        if (cmd != null)
        {
            Console.WriteLine("{0}:\n" +
                              "   {1}", cmd.GetName(), cmd.GetHelpString());
        }
        else if (var != null)
        {
            Console.WriteLine("{0}:\n" +
                              "   {1}", var.GetName(), var.GetHelpString());
        }
        else
        {
            Console.WriteLine("help: error getting command: Could not find \"{0}\".", name);
        }
    }
}

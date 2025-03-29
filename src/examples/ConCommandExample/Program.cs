using System.Diagnostics;
using AridityTeam.Base;
using AridityTeam.Base.Util;

namespace ConCommandExample;

static class Program
{
    private static readonly Logger Log = new();
    
    static void Main(string[] args)
    {
        Log.Log(LogSeverity.LogInfo, "Welcome to the example app for managing ConVar and ConCommand!");

        var quit = new ConCommand("quit", QuitCmdExecute);
        var exit = new ConCommand("exit", QuitCmdExecute);
        var help = new ConCommand("help", HelpCmdExecute);
        var cd = new ConCommand("cd", CdCmdExecute);
        Console.WriteLine("welcome to aridsh!");
        Console.WriteLine($"logged in as {Environment.UserName}");
        InitInteractive();
    }

    static void InitInteractive()
    {
        string directory = Environment.CurrentDirectory.Replace($"/home/{Environment.UserName}", "~");
        Console.Write($"{Environment.UserName}@{Environment.MachineName}:{directory} > ");
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            Log.Log(LogSeverity.LogError, "You must enter a command!");
            InitInteractive();
            return;
        }

        if (input.Contains(';'))
        {
            Console.WriteLine("command separator isn't supported on \"aridsh\""); 
            InitInteractive();
            return;
        }
        
        string[] parts = input.Split(new char[] { ' ', '=', ':' }, 2); // Split into at most 2 parts
        if (parts.Length == 0) return;

        string name = parts[0].Trim();
        object? val = parts.Length > 1 ? parts[1].Trim() : null; // Get the value if it exists
        var conCmd = CommandManager.Instance.GetConCommandByName(name);
        var conVar = CommandManager.Instance.GetConVarByName(name);

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
            var envVar = Environment.GetEnvironmentVariable("PATH")?.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
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

            Log.Log(LogSeverity.LogError, "Command '" + name + "' not found!");
        }

        InitInteractive();
        return;
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

        string[] parts = input.Split(new char[] { ' ', '=', ':' }, 2);
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

        string[] parts = input.Split(new char[] { ' ', '=', ':' }, 2);
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

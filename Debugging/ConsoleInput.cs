using Cobbs_Engine.Input;
using Microsoft.Xna.Framework;
using Pastel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cobbs_Engine
{
    public partial class MainGame : Game
    {
        public partial void TakeConsoleInput()
        {
#if DEBUG
            Program.Console.SetForegroundWindow(Program.Console.GetConsoleWindow());
#endif
            string help = @"
List of Commands
===================
- flushlog       : Flushes the log, outputting it to the diagnostics folder
- showlog        : Flushes log, then opens it in default web browser
- manuallog      : Allows the user to add a manual log message. Message is all proceeding characters after the space following the command.
- prunelogs      : Deletes all log files
- showexecutable : Opens the folder of the executable
- triggeraction  : Triggers an InputAction, requires a proceding InputAction argument (case sensitive)
- quit           : Closes the program";

            Console.Write("\n");
            Console.Write("Enter a command: ".Pastel("#FFFFFF"));
            string input = Console.ReadLine();

            if (input == null || input.Length == 0)
            {
                return;
            }
            string[] inputArguments = input.Split(' ');
            List<string> otherArguments = inputArguments.Skip(1).ToList();

            switch (inputArguments[0].ToLower())
            {
                case "showlog":
                    if (!otherArguments.Contains("--help"))
                    {
                        Diagnostics.LogMessage("Console intiated flush", false);
                        string path = Diagnostics.LogPath;
                        Diagnostics.Flush();
                        Process process = new()
                        {
                            StartInfo = new ProcessStartInfo(path)
                            {
                                UseShellExecute = true,
                            }
                        };
                        process.Start();
                    }
                    else
                    {
                        Diagnostics.LogMessage("command: showlog\nFlushes log, then opens it in default web browser", false);
                    }
                    break;
                case "flushlog":
                    if (!otherArguments.Contains("--help"))
                    {
                        Diagnostics.LogMessage("Console intiated flush", false);
                        Diagnostics.Flush();
                    }
                    else
                    {
                        Diagnostics.LogMessage("command: flushlog\nAllows the user to add a manual log message. Message is all proceeding characters after the space following the command.", false);
                    }
                    break;
                case "prunelogs":
                    if (!otherArguments.Contains("--help"))
                    {
                        Diagnostics.LogMessage("Console intiated prune", false);
                        Diagnostics.Prune(true);
                    }
                    else
                    {
                        Diagnostics.LogMessage("command: prunelogs\nDeletes all log files", false);
                    }
                    break;
                case "quit":
                    if (!otherArguments.Contains("--help"))
                    {
                        Diagnostics.LogMessage("Console intiated quit", false);
                        Exit();
                    }
                    else
                    {
                        Diagnostics.LogMessage("command: quit\nCloses the program", false);
                    }
                    break;
                case "manuallog":
                    if (!otherArguments.Contains("--help"))
                    {
                        if (otherArguments.Count != 0)
                        {
                            string message = string.Empty;
                            foreach (string fragment in otherArguments)
                            {
                                message += fragment + " ";
                            }
                            message.Trim();
                            Diagnostics.LogMessage(message);
                        }
                    }
                    else
                    {
                        Diagnostics.LogMessage("command: manuallog\nAllows the user to add a manual log message. Message is all proceeding characters after the space following the command.", false);
                    }
                    break;
                case "showexecutable":
                    if (!otherArguments.Contains("--help"))
                    {
                        string path = IO.Paths[PathType.Executable];
                        Process process = new()
                        {
                            StartInfo = new ProcessStartInfo(path)
                            {
                                UseShellExecute = true,
                            }
                        };
                        process.Start();
                    }
                    else
                    {
                        Diagnostics.LogMessage("command: showexecutable\nOpens the folder of the executable", false);
                    }
                    break;
                case "triggeraction":
                    if (!otherArguments.Contains("--help"))
                    {
                        List<string> inputActions = Enum.GetNames(typeof(InputAction)).ToList();

                        if (inputArguments.Length > 1 && inputActions.Contains(inputArguments[1]))
                        {
                            InputAction actionToInvoke;
                            Enum.TryParse(inputArguments[1], out actionToInvoke);

                            Input.InvokeInputActionTriggered(actionToInvoke);
                        }
                        else
                        {
                            Diagnostics.LogMessage("Invalid input", false);
                        }
                    }
                    else
                    {
                        Diagnostics.LogMessage("command: triggeraction\nTriggers an InputAction, requires a proceding InputAction argument (case sensitive)", false);
                    }
                    break;
                case "commands":
                case "list":
                case "info":
                case "man":
                case "manual":
                case "h":
                case "help":
                    Diagnostics.LogMessage($"{help}", false);
                     break;
                default:
                    Diagnostics.LogMessage("Argument not recognized", false);
                    break;
            }
        }
    }
}
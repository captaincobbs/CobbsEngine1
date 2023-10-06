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
            const string help = @"
List of Commands
===================
- flushlog          : Flushes the log, outputting it to the diagnostics folder
- showlog           : Flushes log, then opens it in default web browser
- manuallog         : Allows the user to add a manual log message. Message is all proceeding characters after the space following the command.
- prunelogs         : Deletes all log files
- showexecutable    : Opens the folder of the executable
- triggeraction     : Triggers an InputAction, requires a proceding InputAction argument (case sensitive)
- showlocalization  : Shows an input localization pair (following argument), displays all loaded languages unless followed by a third argument of a valid language
- quit              : Closes the program";

            Console.Write("\n");
            Console.Write("Enter a command: ".Pastel("#FFFFFF"));
            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
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
                        Diagnostics.LogMessage("command: flushlog\nAllows the user to add a manual log message. Message is all proceding characters after the space following the command.", false);
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
                            Diagnostics.LogMessage(message.Trim());
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
                            Enum.TryParse(inputArguments[1], out InputAction actionToInvoke);

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
                case "showlocalization":
                    if (!otherArguments.Contains("--help"))
                    {
                        if (inputArguments.Length == 2)
                        {
                            bool exists = false;
                            string output = $"Key: '{inputArguments[1]}'\n================\n";
                            if (Localization.LoadedLanguages == 0)
                            {
                                Diagnostics.LogMessage("There are no loaded languages to search", false);
                            }

                            foreach (Language language in Enum.GetValues(typeof(Language)))
                            {
                                if (language == Language.None)
                                {
                                    continue;
                                }

                                if (Localization.LocalizationPairExists(inputArguments[1], language))
                                {
                                    exists = true;
                                    string value = Localization.GetLocalizationPair(inputArguments[1], language);
                                    if (value != inputArguments[1])
                                    {
                                        output += $"{Enum.GetName(language)} : '{Localization.GetLocalizationPair(inputArguments[1], language)}'\n";
                                    }
                                }
                            }

                            if (exists)
                            {
                                Diagnostics.LogMessage(output.Trim(), false);
                            }
                            else
                            {
                                Diagnostics.LogMessage($"Key '{inputArguments[1]}' not found", false);
                            }
                        }
                        else if (inputArguments.Length == 3)
                        {
                            string output = $"Key: '{inputArguments[1]}'\n";

                            bool casted = Enum.TryParse(inputArguments[2], out Language language);

                            if (casted)
                            {
                                string value = Localization.GetLocalizationPair(inputArguments[1], language, false);

                                if (value == inputArguments[1])
                                {
                                    Diagnostics.LogMessage($"{output}Value '{value.Pastel(Diagnostics.Colors[Diagnostics.MessageType.Error])}'", false, true, false);
                                }

                                output += $"Value: '{value}'";
                                Diagnostics.LogMessage(output, false, false);
                            }
                            else
                            {
                                Diagnostics.LogMessage($"Language '{inputArguments[2]}' not found", false);
                            }
                        }
                        else
                        {
                            Diagnostics.LogMessage("Invalid input", false);
                        }
                    }
                    else
                    {
                        Diagnostics.LogMessage("command: showlocalization\nShows an input localization pair (following argument), displays all loaded languages unless followed by a third argument of a valid language", false);
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ValorantAgentPicker
{
    internal class Program
    {
        private static string ver = CurFileVersion();
        private static bool enableControllers = true;
        private static bool enableInitiators = true;
        private static bool enableDuelists = true;
        private static bool enableSentinels = true;
        private static List<Agent> AgentList = new List<Agent>
            {
                new Agent("Astra", AgentRole.Controller, true),
                new Agent("Brimstone",AgentRole.Controller, true),
                new Agent("Harbor",AgentRole.Controller, true),
                new Agent("Omen",AgentRole.Controller, true),
                new Agent("Viper",AgentRole.Controller, true),
                new Agent("Chamber",AgentRole.Sentinel, true),
                new Agent("Cypher",AgentRole.Sentinel, true),
                new Agent("Deadlock",AgentRole.Sentinel, true),
                new Agent("Killjoy",AgentRole.Sentinel, true),
                new Agent("Sage",AgentRole.Sentinel, true),
                new Agent("Breach",AgentRole.Initiator, true),
                new Agent("Fade",AgentRole.Initiator, true),
                new Agent("Gekko",AgentRole.Initiator, true),
                new Agent("KAY/O",AgentRole.Initiator, true),
                new Agent("Skye",AgentRole.Initiator, true),
                new Agent("Sova",AgentRole.Initiator, true),
                new Agent("Iso",AgentRole.Duelist, true),
                new Agent("Jett",AgentRole.Duelist, true),
                new Agent("Neon",AgentRole.Duelist, true),
                new Agent("Pheonix",AgentRole.Duelist, true),
                new Agent("Raze",AgentRole.Duelist, true),
                new Agent("Reyna",AgentRole.Duelist, true),
                new Agent("Yoru",AgentRole.Duelist, true)
            };
        static void Main(string[] args)
        {
            while (true)
            {
                LoadMenu();
            }
        }

        private static void LoadMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Valorant Agent Picker - v{ver}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Avaiable Operations:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("1 - Roll an Agent");
            Console.WriteLine("2 - Settings");
            Console.WriteLine("3 - Clear Terminal");
            Console.WriteLine("q - Quit");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Enter Choice:");
            Console.ResetColor();
            while (true)
            {
                Console.Write(">");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        string agent = GetAgent();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{agent}");
                        Console.ResetColor();
                        break;
                    case "2":
                        SettingsMenu();
                        return;
                    case "3":
                        Console.Clear();
                        return;
                    case "q":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid");
                        break;
                }
            }
        }

        private static string GetAgent()
        {
            List<Agent> EnabledList = new List<Agent>();
            Random rnd = new Random();
            int index;
            Agent agent;

            foreach (Agent enabledAgent in AgentList)
            {
                if (enabledAgent.Enabled)
                {
                    EnabledList.Add(enabledAgent);
                }
            }

            if (EnabledList.Count == 0)
            {
                return "No Enabled Agents";
            }

            index = rnd.Next(EnabledList.Count);

            agent = EnabledList[index];

            string AgentName = agent.Name;

            return AgentName;
        }

        private static void SettingsMenu()
        {
            bool Invalid = false;
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Settings:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Enter Number to Change Setting");
                Console.ForegroundColor= ConsoleColor.Gray;
                Console.WriteLine("*Note if you manually change all of a certain role manually this will not update");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"1: Controllers: {enableControllers}");
                Console.WriteLine($"2: Sentinels: {enableSentinels}");
                Console.WriteLine($"3: Initiators: {enableInitiators}");
                Console.WriteLine($"4: Duelists: {enableDuelists}");
                Console.WriteLine($"5: Indvidual Agents Menu");
                Console.WriteLine("q - Return");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Enter Choice:");
                Console.ResetColor();
                if (Invalid)
                {
                    Console.WriteLine("Invalid Input");
                    Console.Write(">");
                    Invalid = false;
                }
                else
                {
                    Console.Write(">");
                }
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        enableControllers = !enableControllers;
                        ToggleAgents(AgentRole.Controller, enableControllers);
                        break;
                    case "2":
                        enableSentinels = !enableSentinels;
                        ToggleAgents(AgentRole.Sentinel, enableSentinels);
                        break;
                    case "3":
                        enableInitiators = !enableInitiators;
                        ToggleAgents(AgentRole.Initiator, enableInitiators);
                        break;
                    case "4":
                        enableDuelists = !enableDuelists;
                        ToggleAgents(AgentRole.Duelist, enableDuelists);
                        break;
                    case "5":
                        bool returnToMain = AgentMenu();
                        if (returnToMain)
                        {
                            Console.Clear();
                            return;
                        }
                        break;
                    case "q":
                        Console.Clear();
                        return;
                    default:
                        Invalid = true;
                        break;
                }
            }
        }

        private static bool AgentMenu()
        {
            bool OutofRange = false;
            bool Invalid = false;
            while (true)
            {
                Console.Clear();
                int index;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Agents:");
                Console.ForegroundColor = ConsoleColor.White;
                PrintAgents();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Enter Number to toggle agent,\ns to return to settings,\nq to return to main screen");
                Console.ResetColor();
                if (OutofRange)
                {
                    Console.WriteLine("Input Out of Range");
                    Console.Write(">");
                    OutofRange = false;
                }
                else if (Invalid)
                {
                    Console.WriteLine("Invalid Input");
                    Console.Write(">");
                    Invalid = false;
                }
                else
                {
                    Console.Write(">");
                }
                
                string input = Console.ReadLine();

                if (input == "q")
                {
                    return true;
                }

                if (input == "s")
                {
                    return false;
                }

                if (input == "5") { }

                bool agentNum = int.TryParse(input, out index);

                if (agentNum)
                {
                    if (index >= 0 && index < AgentList.Count)
                    {
                        AgentList[index].Enabled = !AgentList[index].Enabled;
                    }
                    else
                    {
                        OutofRange = true;
                    }
                }
                else
                {
                    Invalid = true;
                }
            }
        }

        private static void PrintAgents()
        {
            int i = 0;
            foreach (Agent agent in AgentList)
            {
                Console.WriteLine($"{i} - {agent.Name} - {agent.Role} - {agent.Enabled}");
                i++;
            }
        }

        private static string CurFileVersion()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);
            string version = fileVersionInfo.FileVersion;

            return version;
        }

        private static void ToggleAgents(AgentRole role, bool state)
        {
            foreach (Agent agent in AgentList)
            {
                if (agent.Role == role)
                {
                    agent.Enabled = state;
                }
            }
        }
    }
}

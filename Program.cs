using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace ValorantAgentPicker
{
    internal class Program
    {
        private static string ver;
        private static bool enableControllers = true;
        private static bool enableInitiators = true;
        private static bool enableDuelists = true;
        private static bool enableSentinels = true;
        private static List<Agent> AgentList = new List<Agent>();
        private static List<Strat> StratList = new List<Strat>();
        private static bool isAgentsLoaded;
        private static bool isStratsLoaded = true;
        private static AppSettings userSettings;
        private static Map chosenMap = Map.Any;
        private static TeamSide chosenSide = TeamSide.Both;
        private static bool inDeeper = true;
        private static string message = "";
        static void Main(string[] args)
        {
            StartUp();
            while (true)
            {
                LoadMenu();
            }
        }

        private static void StartUp()
        {
            string agentCSVPath = "";
            string stratCSVPath = "";
            try
            {
                ver = CurFileVersion();
                Global.appDataRoamingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "REclipsent");
                Global.roamingFolder = Path.Combine(Global.appDataRoamingPath, "ValorantAgentPicker");
                Global.settingsFile = Path.Combine(Global.roamingFolder, "settings.json");
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                agentCSVPath = Path.Combine(directoryPath, "Agents.csv");
                stratCSVPath = Path.Combine(directoryPath, "Strats.csv");
            }
            catch
            {
                AnsiConsole.WriteLine("[red]Can't find folder locations app will now exit[/]");
                Environment.Exit(1);
            }
            userSettings = Settings.ReadSettings();
            CleanConsole();

            Console.WriteLine("Startup Started");

            Console.Clear();

            Directory.CreateDirectory(Global.appDataRoamingPath);
            Directory.CreateDirectory(Global.roamingFolder);

            
            AgentList = ReadAgentsFromCsv(agentCSVPath);
            StratList = ReadStratsFromCsv(stratCSVPath);
        }

        private static void LoadMenu()
        {
            string strAgentRoul = "[white]Agent Roulette[/]";
            string strStratRoul = "[white]Strat Roulette[/]";
            string strSetting = "[white]Settings[/]";
            string strQuit = "[white]Quit[/]";
            AnsiConsole.MarkupLine($"[maroon]REclipsent's Valorant Companion App - v{ver}[/]");
            var selections = new SelectionPrompt<string>();

            if (isAgentsLoaded)
            {
                selections.AddChoice(strAgentRoul);
            }
            else
            {
                selections.AddChoice("[red3_1]Agent Roulette - Unavailable[/]");
            }
            if (isStratsLoaded)
            {
                selections.AddChoice(strStratRoul);
            }
            else
            {
                selections.AddChoice("[red]Strat Roulette - Unavailable[/]");
            }
            selections.AddChoices(
                strSetting, strQuit
                );

            AnsiConsole.MarkupLine("[red]Main Menu:[/]");
            AnsiConsole.MarkupLine($"[white]{message}[/]");

            string input = AnsiConsole.Prompt(selections
                        .HighlightStyle(Color.Red));

            if (input == strAgentRoul)
            {
                message = "";
                inDeeper = true;
                while (inDeeper)
                {
                    AgentMenu();
                }
                Console.Clear();
                return;
            }
            else if (input == strStratRoul)
            {
                message = "";
                inDeeper = true;
                while (inDeeper)
                {
                    StratRouletteMenu();
                }
                Console.Clear();
            }
            else if (input == strSetting)
            {
                message = "";
                AppSettingsMenu();
            }
            else if (input == strQuit)
            {
                Settings.WriteSettings(userSettings);
                Environment.Exit(0);
            }
            else
            {
                message = "Unavaliable";
            }
            Console.Clear();
        }

        private static void AppSettingsMenu()
        {
            bool Invalid = false;
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Settings:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Enter Number to Change Setting");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"1: Color of Window");
                Console.WriteLine("q - Return");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Enter Choice:");
                CleanConsole();
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
                        ChangeConsoleColor();
                        Settings.WriteSettings(userSettings);
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

        private static void ChangeConsoleColor()
        {
            while (true)
            {
                Console.Clear();
                int i = 0;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Color Change Settings:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Enter Number to Change Color");
                Console.ForegroundColor = ConsoleColor.White;
                ConsoleColor[] consoleColors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));
                foreach (ConsoleColor color in consoleColors)
                {
                    Console.WriteLine($"{i} - {color}");
                    i++;
                }
                Console.WriteLine("q - Return");
                Console.ForegroundColor= ConsoleColor.Cyan;
                Console.WriteLine("Enter Number:");
                CleanConsole();
                Console.Write(">");
                string input = Console.ReadLine();

                if (input == "q")
                {
                    return;
                }

                bool isNum = int.TryParse(input, out int value);

                if (isNum)
                {
                    if (value >= 0 && value < consoleColors.Length)
                    {
                        ConsoleColor chosenColor = consoleColors[value];
                        userSettings.userBackgroundColor = chosenColor;
                        Console.BackgroundColor = userSettings.userBackgroundColor;
                    }
                }
            }
        }

        private static void CleanConsole()
        {
            Console.ResetColor();
            Console.BackgroundColor = userSettings.userBackgroundColor;
        }

        private static void AgentMenu()
        {
            var layout = new Layout("Root").SplitColumns(new Layout("Left"),new Layout("Right"));

            Agent agent = null;
            while (true)
            {
                Console.Clear();

                string strRoll = "[white]Roll an Agent[/]";
                string strAgentSettings = "[white]Agent Settings[/]";
                string strClear = "[white]Clear Terminal[/]";
                string strReturn = "[white]Return to Main Menu[/]";

                //AnsiConsole.MarkupLine($"[maroon]Valorant Agent Roulette Menu[/]");
                var selections = new SelectionPrompt<string>();

                var panel = new Panel("").Border(BoxBorder.None);

                var thibng = panel.
                if (agent != null)
                {
                    layout["Left"].Update(panel);
                    layout["Right"].Update(panel);
                    AnsiConsole.Write(layout);
                }

                selections.AddChoices(
                    strRoll, strAgentSettings, strClear, strReturn
                    );

                AnsiConsole.MarkupLine("[red]Valorant Agent Roulette Menu:[/]");
                AnsiConsole.MarkupLine($"[white]{message}[/]");


                string input = AnsiConsole.Prompt(selections
                        .HighlightStyle(Color.Red));

                if (input == strRoll)
                {
                    message = "";
                    agent = GetAgent();
                    if (agent != null)
                    {
                        layout["Right"].Update(panel);
                        //layout["Right"].Update(new Panel(Align.Center(new Markup($"[yellow]{agent.Name}[/]"))).Expand()));
                    }
                    else
                    {
                        message = "No Enabled Agents";
                    }

                    //AnsiConsole.MarkupLine($"[yellow]{agent}[/]");
                }
                else if (input == strAgentSettings)
                {
                    message = "";
                    AgentSettingsMenu();
                    Settings.WriteSettings(userSettings);
                    return;
                }
                else if (input == strClear)
                {
                    message = "";
                    Console.Clear();
                    return;
                }
                else if (input == strReturn)
                {
                    message = "";
                    inDeeper = false;
                    return;
                }
                else
                {
                    message = "Unavaliable";
                }
            }
            

            //Console.Clear();
            //Console.ForegroundColor = ConsoleColor.DarkRed;
            //Console.WriteLine("Valorant Agent Roulette Menu");
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine("Choose Operation");
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine("1 - Roll an Agent");
            //Console.WriteLine("2 - Agent Settings");
            //Console.WriteLine("c - Clear Terminal");
            //Console.WriteLine("q - Return to Main Menu");
            //Console.ForegroundColor = ConsoleColor.Cyan;
            //Console.WriteLine("Enter Choice:");
            //CleanConsole();
            //while (true)
            //{
            //    Console.Write(">");
            //    string input = Console.ReadLine();
            //    switch (input)
            //    {
            //        case "1":
            //            string agent = GetAgent();
            //            Console.ForegroundColor = ConsoleColor.Yellow;
            //            Console.WriteLine($"{agent}");
            //            CleanConsole();
            //            break;
            //        case "2":
            //            AgentSettingsMenu();
            //            Settings.WriteSettings(userSettings);
            //            return;
            //        case "c":
            //            Console.Clear();
            //            return;
            //        case "q":
            //            inDeeper = false;
            //            return;
            //        default:
            //            Console.WriteLine("Invalid");
            //            break;
            //    }
            //}
        }

        private static Agent GetAgent()
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
                return null;
            }

            index = rnd.Next(EnabledList.Count);

            agent = EnabledList[index];

            return agent;

            //string agentName = agent.Name;

            //string agentRole = agent.Role.ToString();

            ////if (userSettings.returnRole)
            ////{
            ////    return $"{agentName} - {agentRole}";
            ////}
            ////else
            ////{
            ////    return agentName;
            ////}
        }

        private static void AgentSettingsMenu()
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
                Console.WriteLine($"6: Print Agents Role: {userSettings.returnRole}");
                Console.WriteLine("q - Return");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Enter Choice:");
                CleanConsole();
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
                        bool returnToMain = ToggleAgentMenu();
                        if (returnToMain)
                        {
                            Console.Clear();
                            return;
                        }
                        break;
                    case "6":
                        userSettings.returnRole = !userSettings.returnRole;
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

        private static bool ToggleAgentMenu()
        {
            bool OutofRange = false;
            bool Invalid = false;
            while (true)
            {
                Console.Clear();
                int index;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Agents:");
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("Enter Number to toggle agent");
                Console.ForegroundColor = ConsoleColor.White;
                PrintAgents();
                Console.WriteLine("s - Return to settings");
                Console.WriteLine("q - Return to main screen");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Enter Choice:");
                CleanConsole();
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

        private static void StratRouletteMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Valorant Strat Roulette Menu");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Choose Operation");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("1 - Roll Strat");
            Console.WriteLine("2 - Strat Settings");
            Console.WriteLine("c - Clear Terminal");
            Console.WriteLine("q - Return to Main Menu");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Enter Choice:");
            CleanConsole();
            while (true)
            {
                Console.Write(">");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        RollStrat();
                        break;
                    case "2":
                        StratSettingsMenu();
                        return;
                    case "c":
                        Console.Clear();
                        return;
                    case "q":
                        inDeeper = false;
                        return;
                    default:
                        Console.WriteLine("Invalid");
                        break;
                }
            }
        }

        private static void RollStrat()
        {
            Random rnd = new Random();

            List<Strat> enabledList = new List<Strat>();

            foreach (Strat strat in StratList)
            {
                if ((strat.Map == chosenMap | strat.Map == Map.Any) && (strat.Side == chosenSide | strat.Side == TeamSide.Both))
                {
                    enabledList.Add(strat);
                }
            }

            int index = rnd.Next(enabledList.Count);

            Strat chosenStrat = enabledList[index];

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(chosenStrat.Name);
            Console.WriteLine(chosenStrat.Description);
            Console.WriteLine($"Map: {chosenStrat.Map}");
            Console.WriteLine($"Side: {chosenStrat.Side}");
            CleanConsole();
        }

        private static void StratSettingsMenu()
        {
            bool isInvalid = false;
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Strat Roulette Settings");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Avaliable Settings:");
                Console.ForegroundColor= ConsoleColor.White;
                Console.WriteLine($"1 - Chosen Map: {chosenMap}");
                Console.WriteLine($"2 - Chosen Side: {chosenSide}");
                Console.WriteLine($"q - Return");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Enter Choice:");
                CleanConsole();
                if ( isInvalid )
                {
                    Console.WriteLine("Invalid");
                    isInvalid = false;
                }
                Console.Write(">");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ChoseMap();
                        break;
                    case "2":
                        ChoseSide();
                        break;
                    case "q":
                        return;
                    default:
                        isInvalid = true;
                        break;
                }
            }
        }

        private static void ChoseMap()
        {
            while (true)
            {
                Console.Clear();
                int i = 0;

                List<Map> mapList = new List<Map>();
                foreach (Map map in Enum.GetValues(typeof(Map)))
                {
                    mapList.Add(map);
                }
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Map Selecter");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Options:");
                Console.ForegroundColor = ConsoleColor.White;
                foreach (Map map in mapList)
                {
                    Console.WriteLine($"{i} - {map}");
                    i++;
                }
                Console.WriteLine("q - Return");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Currently Selected: {chosenMap}");
                Console.WriteLine("Enter Option:");
                CleanConsole();

                Console.Write(">");
                string input = Console.ReadLine();
                if (input == "q")
                {
                    return;
                }

                bool isNum = int.TryParse( input, out int value);

                if (isNum)
                {
                    if (value >= 0 && value < mapList.Count)
                    {
                        chosenMap = mapList[value];
                    }
                }
            }
        }

        private static void ChoseSide()
        {
            while (true)
            {
                Console.Clear();
                int i = 0;

                List<TeamSide> sideList = new List<TeamSide>();
                foreach (TeamSide side in Enum.GetValues(typeof(TeamSide)))
                {
                    sideList.Add(side);
                }
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Map Selecter");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Options:");
                Console.ForegroundColor = ConsoleColor.White;
                foreach (TeamSide side in sideList)
                {
                    Console.WriteLine($"{i} - {side}");
                    i++;
                }
                Console.WriteLine("q - Return");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Currently Selected: {chosenSide}");
                Console.WriteLine("Enter Option:");
                CleanConsole();

                Console.Write(">");
                string input = Console.ReadLine();
                if (input == "q")
                {
                    return;
                }

                bool isNum = int.TryParse(input, out int value);

                if (isNum)
                {
                    if (value >= 0 && value < sideList.Count)
                    {
                        chosenSide = sideList[value];
                    }
                }
            }
        }


        static List<Agent> ReadAgentsFromCsv(string filePath)
        {
            AnsiConsole.Markup("[yellow](1/2) Reading Agents from CSV Located at: [/]");
            var path = new TextPath(filePath);

            path.RootStyle = new Style(foreground: Color.Yellow);
            path.SeparatorStyle = new Style(foreground: Color.Yellow);
            path.StemStyle = new Style(foreground: Color.Yellow);
            path.LeafStyle = new Style(foreground: Color.Yellow);

            AnsiConsole.Write(path);

            List<Agent> agents = new List<Agent>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split(',');

                        if (values.Length == 2)
                        {
                            string name = values[0];
                            string stringRole = values[1];

                            Enum.TryParse(stringRole, out AgentRole role);

                            Agent agent = new Agent(name, role, true);
                            agents.Add(agent);
                        }
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                AnsiConsole.MarkupLine("[red]Agents.csv dosen't exist inside the executing folder [/]");
                isAgentsLoaded = false;
                return null;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Their was an unkown error reading CSV file:\n{ex}[/]");
                isAgentsLoaded = false;
                return null;
            }
            AnsiConsole.MarkupLine($"[yellow]Loaded {agents.Count} Agents from List[/]");
            isAgentsLoaded = true;
            return agents;
        }

        static List<Strat> ReadStratsFromCsv(string filePath)
        {
            AnsiConsole.Markup("[yellow](2/2) Reading Strats from CSV Located at: [/]");

            var path = new TextPath(filePath);

            path.RootStyle = new Style(foreground: Color.Yellow);
            path.SeparatorStyle = new Style(foreground: Color.Yellow);
            path.StemStyle = new Style(foreground: Color.Yellow);
            path.LeafStyle = new Style(foreground: Color.Yellow);

            AnsiConsole.Write(path);

            List<Strat> strats = new List<Strat>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split(',');

                        if (values.Length == 4)
                        {
                            string name = values[0];
                            string desc = values[1];
                            string mapString = values[2];
                            string sideString = values[3];

                            Enum.TryParse(mapString, out Map map);

                            Enum.TryParse(sideString, out TeamSide side);

                            Strat strat = new Strat(name, desc, map, side);
                            strats.Add(strat);
                        }
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                AnsiConsole.MarkupLine("[red]Strats.csv dosen't exist inside the executing folder [/]");
                isStratsLoaded = false;
                return null;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Their was an unkown error reading CSV file:\n{ex}[/]");
                isStratsLoaded = false;
                return null;
            }
            AnsiConsole.MarkupLine($"[yellow]Loaded {strats.Count} Strats from List[/]");
            isStratsLoaded = true;
            return strats;
        }
    }
}

//{
//    new Agent("Astra", AgentRole.Controller, true),
//    new Agent("Brimstone",AgentRole.Controller, true),
//    new Agent("Harbor",AgentRole.Controller, true),
//    new Agent("Omen",AgentRole.Controller, true),
//    new Agent("Viper",AgentRole.Controller, true),
//    new Agent("Chamber",AgentRole.Sentinel, true),
//    new Agent("Cypher",AgentRole.Sentinel, true),
//    new Agent("Deadlock",AgentRole.Sentinel, true),
//    new Agent("Killjoy",AgentRole.Sentinel, true),
//    new Agent("Sage",AgentRole.Sentinel, true),
//    new Agent("Breach",AgentRole.Initiator, true),
//    new Agent("Fade",AgentRole.Initiator, true),
//    new Agent("Gekko",AgentRole.Initiator, true),
//    new Agent("KAY/O",AgentRole.Initiator, true),
//    new Agent("Skye",AgentRole.Initiator, true),
//    new Agent("Sova",AgentRole.Initiator, true),
//    new Agent("Iso",AgentRole.Duelist, true),
//    new Agent("Jett",AgentRole.Duelist, true),
//    new Agent("Neon",AgentRole.Duelist, true),
//    new Agent("Pheonix",AgentRole.Duelist, true),
//    new Agent("Raze",AgentRole.Duelist, true),
//    new Agent("Reyna",AgentRole.Duelist, true),
//    new Agent("Yoru",AgentRole.Duelist, true)
//};
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace ValorantAgentPicker
{
    internal class Beta
    {
        public static bool betaBuild = true;
        public static string beta1Info = @"[maroon]Beta 1 Release Notes-
[red]CHANGES-
[white]Rewritten Main Menu, Agent Menu, and Agent Settings Menu with Spectre.Console library
[red]ISSUES-
[white]Changing the console's color through settings currently doesn't work[/][/][/][/][/]";

        public static void PrintBetaInfo()
        {
            Console.Clear();
            AnsiConsole.MarkupLine(beta1Info);
            AnsiConsole.MarkupLine("[grey]*Press Enter to Return[/]");
            Console.ReadLine();
        }
    }
}

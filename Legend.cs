using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantAgentPicker
{
    public enum SummonerRole
    {
        Top,
        Jungle,
        Mid,
        Bot,
        Support
    }
    public class Legend
    {
        string Name;
        SummonerRole Role;

    }
}

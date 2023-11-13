using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantAgentPicker
{
    public enum TeamSide
    {
        Attacking,
        Defending,
        Both
    }
    public class Strat
    {
        public string Name;
        string Description;
        TeamSide Side; 
        public Strat(string name, string desc, TeamSide side)
        {
            Name = name;
            Description = desc;
            Side = side;
        }
    }
}

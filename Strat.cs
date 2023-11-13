using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantAgentPicker
{
    public enum Map
    {
        Any,
        Breeze,
        Pearl,
        Split,
        Haven,
        Bind
    }
    public enum TeamSide
    {
        Both,
        Attacking,
        Defending
    }
    public class Strat
    {
        public string Name;
        public string Description;
        public Map Map;
        public TeamSide Side; 
        public Strat(string name, string desc, Map mapName, TeamSide side)
        {
            Name = name;
            Description = desc;
            Map = mapName;
            Side = side;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantAgentPicker
{
    public enum AgentRole
    {
        Controller,
        Sentinel,
        Initiator,
        Duelist
    }
    public class Agent
    {
        public string Name;
        public AgentRole Role;
        public bool Enabled;

        public Agent(string name, AgentRole role, bool enabled)
        {
            Name = name;
            Role = role;
            Enabled = enabled;
        }
    }
}

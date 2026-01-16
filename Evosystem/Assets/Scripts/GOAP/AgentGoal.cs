using System.Collections.Generic;

public class AgentGoal
{
    public string Name { get; }
    public float Priority { get; private set; }
    public HashSet<AgentBelief> DesiredEffects { get; } = new HashSet<AgentBelief>();
    AgentGoal(string name)
    {
        Name = name;
    }
    
    public class Builder
    {
        readonly AgentGoal goal;
        public Builder(string name)
        {
            goal = new AgentGoal(name);
        }
        public Builder WithPriority(float priority)
        {
            goal.Priority = priority;
            return this;
        }
        public Builder AddDesiredState(AgentBelief belief)
        {
            goal.DesiredEffects.Add(belief);
            return this;
        }
        public AgentGoal Build()
        {
            return goal;
        }
    }
}
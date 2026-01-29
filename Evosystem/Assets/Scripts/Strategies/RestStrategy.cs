using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

internal class RestStrategy : IActionStrategy
{
    GoapAgent agent;

    public bool CanPerform => true;

    public bool Complete => agent.status.Energy >= agent.entity.Stats.Energy;

    public RestStrategy(GoapAgent agent)
    {
        this.agent = agent;
    }

    public void Update(float deltaTime)
    { 
        float energyDelta = deltaTime * agent.entity.Stats.Energy;
        agent.status.Energy = Mathf.Clamp(agent.status.Energy + energyDelta, 0, agent.entity.Stats.Energy);

        float staminaDelta = deltaTime * agent.entity.Stats.StaminaRegenRate;
        agent.status.Stamina = Mathf.Clamp(agent.status.Stamina + staminaDelta, 0, agent.entity.Stats.Stamina);
    }
}
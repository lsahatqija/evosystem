using System;
using UnityEngine;
using UnityEngine.AI;

public class IdleStrategy : IActionStrategy
{
    GoapAgent agent;
    public bool CanPerform => true;
    public bool Complete { get; private set; }

    readonly CountdownTimer timer;

    public IdleStrategy(GoapAgent agent, float duration)
    {
        this.agent = agent;
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start() => timer.Start();
    public void Update(float deltaTime)
    {
        if (agent != null)
            agent.status.Stamina += deltaTime * agent.entity.Stats.StaminaRegenRate;
        timer.Tick(deltaTime);
    }
}
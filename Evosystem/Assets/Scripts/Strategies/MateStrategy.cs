using UnityEngine;
using System;

public class MateStrategy : IActionStrategy
{
    GoapAgent self;
    GoapAgent mate;

    CountdownTimer timer;

    public bool CanPerform => self != null && !self.entity.IsPregnant && mate != null && self.InRangeof(mate.transform.position, 1f) && self.potentialMate != null;
    public bool Complete { get; private set; }

    public MateStrategy(GoapAgent selfAgent)
    {
        this.self = selfAgent;

        timer = new CountdownTimer(10f);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    // massively simplified here. need to add a bunch more behaviour complexity here
    public void Start()
    {
        if (self == null || self.potentialMate == null)
            return;

        mate = self.potentialMate;
        timer.Start();
    }

    public void Update(float deltaTime)
    {
        self.animations.Mate();
        self.status.Desire = Mathf.Clamp(self.status.Desire - 10f * deltaTime, 0, self.entity.Stats.Desire);
        timer.Tick(deltaTime);
    }

    public void Stop()
    {
        if (!self.entity.IsMale && mate != null)
        {
            self.entity.mateEntity = ScriptableObject.Instantiate(mate.entity);
            self.entity.IsPregnant = true;
        }
        Complete = true;
    }
}
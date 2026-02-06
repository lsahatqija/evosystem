using UnityEngine;
using System;

public class MateStrategy : IActionStrategy
{
    GoapAgent self;
    GoapAgent mate;

    CountdownTimer timer;

    public bool CanPerform => self != null && !self.entity.IsPregnant && mate != null && self.InRangeof(mate.transform.position, 2f) && self.potentialMate != null;
    public bool Complete { get; private set; }

    public MateStrategy(GoapAgent selfAgent)
    {
        this.self = selfAgent;

        timer = new CountdownTimer(1f);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    // massively simplified here. need to add a bunch more behaviour complexity here
    public void Start()
    {
        if (self == null || self.potentialMate == null)
        {
            mate = null;
        }
        else
        {
            mate = self.potentialMate;
            timer.Start();
        }
    }

    public void Update(float deltaTime)
    {
        self.animations.Mate();
        self.status.Desire = Mathf.Clamp(self.status.Desire - 50f * deltaTime, 0, self.entity.Stats.Desire);
        timer.Tick(deltaTime);
    }

    public void Stop()
    {
        if (!self.entity.IsMale && mate != null)
        {
            self.entity.mateEntity = ScriptableObject.Instantiate(mate.entity);
            self.entity.IsPregnant = true;
            self.status.PregTime = 0;
        }
        self.status.Desire = 0;
        self.potentialMate = null;
        mate = null;
    }
}
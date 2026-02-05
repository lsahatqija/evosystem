using UnityEngine;

internal class BirthStrategy : IActionStrategy
{
    GoapAgent agent;
    CountdownTimer timer;

    public bool CanPerform => agent != null && !agent.entity.IsMale && agent.entity.IsPregnant && agent.entity.mateEntity != null;

    public bool Complete { get; private set; }


    public BirthStrategy(GoapAgent agent)
    {
        this.agent = agent;
        timer = new CountdownTimer(10f);

        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start()
    {
        agent.animations.Mate();
        timer.Start();
    }
    public void Update(float deltaTime)
    {
        timer.Tick(deltaTime);
    }

    public void Stop()
    {
        GameObject eggPrefab = Resources.Load("Egg") as GameObject;

        if (eggPrefab == null)
            return;

        for (int i = 0; i < agent.entity.Stats.ClutchSize; i++)
        {
            Vector3 posRand = UnityEngine.Random.insideUnitSphere;
            posRand.y = 0;
            GameObject egg = GameObject.Instantiate(eggPrefab, agent.transform.position + posRand, agent.transform.rotation);

            egg.TryGetComponent(out Egg eggComponent);
            if (eggComponent != null)
            {
                Entity eggEntity = ScriptableObject.Instantiate(agent.entity);
                eggEntity.Attributes = EntityUtils.CombineAttributes(agent.entity.mateEntity.Attributes, agent.entity.Attributes, agent.status.Stress);
                eggEntity.Stats = EntityUtils.CombineInitialStats(agent.entity.mateEntity.Stats, agent.entity.Stats);
                eggEntity.IsPregnant = false;
                eggEntity.mateEntity = null;
                eggComponent.InitializeEgg(eggEntity);
            }
        }

        agent.entity.IsPregnant = false;

        Complete = true;
    }
}
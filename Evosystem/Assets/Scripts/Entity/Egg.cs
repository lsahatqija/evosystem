using UnityEngine;

public class Egg : MonoBehaviour
{
    public Entity entity;

    CountdownTimer timer;

    public void InitializeEgg(Entity entity)
    {
        this.entity = entity;
        timer = new CountdownTimer(entity.Stats.EggDuration);
        timer.OnTimerStop += () => Hatch();
    }

    private void Update()
    {
        if (timer != null)
            timer.Tick(Time.deltaTime);
    }

    void Hatch()
    {
        GameObject agentPrefab = Resources.Load("AgentEntity") as GameObject;

        GameObject child = Instantiate(agentPrefab, transform.position, transform.rotation);
        child.TryGetComponent(out GoapAgent Agent);

        if (Agent)
        {
            Agent.entity = entity;
            Agent.InitializeAgent();
        }

        Destroy(gameObject, 1f);
    }

}
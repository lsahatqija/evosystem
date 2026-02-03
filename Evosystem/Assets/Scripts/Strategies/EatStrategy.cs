using Unity.VisualScripting;
using UnityEngine;

internal class EatStrategy : IActionStrategy
{
    GoapAgent agent;
    Consumable food;

    public bool CanPerform => food != null && agent != null && Vector3.Distance(food.transform.position, agent.transform.position) <= 2f;

    public bool Complete => agent.status.Hunger <= 0 || food == null;

    public EatStrategy(GoapAgent agent)
    {
        this.agent = agent;
    }

    public void Start()
    {
        agent.foodPositionCurrent.TryGetComponent(out this.food);
    }

    public void Update(float deltaTime)
    {
        agent.status.Hunger -= food.Consume(10f * deltaTime);
        float staminaDelta = deltaTime * agent.entity.Stats.StaminaRegenRate;
        agent.status.Stamina = Mathf.Clamp(agent.status.Stamina + staminaDelta, 0, agent.entity.Stats.Stamina);
        agent.animations.Eat();
    }
}
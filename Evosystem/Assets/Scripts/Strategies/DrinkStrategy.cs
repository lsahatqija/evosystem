using UnityEngine;

internal class DrinkStrategy : IActionStrategy
{
    GoapAgent agent;
    Consumable drink;

    public bool CanPerform => drink != null && agent != null && Vector3.Distance(drink.transform.position, agent.transform.position) <= 2f;

    public bool Complete => agent.status.Thirst <= 0 || drink == null;

    public DrinkStrategy(GoapAgent agent)
    {
        this.agent = agent;
    }

    public void Start()
    {
        agent.drinkPositionCurrent.TryGetComponent(out this.drink);
    }

    public void Update(float deltaTime)
    {
        agent.status.Thirst -= drink.Consume(10f * deltaTime);

        float staminaDelta = deltaTime * agent.entity.Stats.StaminaRegenRate;
        agent.status.Stamina = Mathf.Clamp(agent.status.Stamina + staminaDelta, 0, agent.entity.Stats.Stamina);

        agent.animations.Eat();
    }
}
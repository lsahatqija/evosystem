using UnityEngine;

internal class DrinkStrategy : IActionStrategy
{
    GoapAgent agent;
    Consumable drink;

    public bool CanPerform => drink != null && agent != null && Vector3.Distance(drink.transform.position, agent.transform.position) < 3f;

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
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Entity/Entity")]
public class Entity : ScriptableObject
{
    private string uuid;
    public string UUID
    {
        get
        {
            if (string.IsNullOrEmpty(uuid))
            {
                uuid = System.Guid.NewGuid().ToString();
            }
            return uuid;
        }
    }

    public Species species;

    public List<AgentBelief> initialBeliefs;
    public List<AgentAction> availableActions;
    public List<AgentGoal> initialGoals;

    public List<EntityTag> IsTags;
    public List<EntityTag> HasTags;
    public List<EntityTag> WantsTags;
    public List<EntityTag> AvoidTags;
    public EntityAttributes Attributes;
    public EntityStats Stats;

    public bool IsPregnant = false;
    public bool IsMale = false;

    public bool IsTagPresent(EntityTag tag)
    {
        return IsTags.Contains(tag) || HasTags.Contains(tag) || WantsTags.Contains(tag);
    }

    public EntityAttributes CombineAttributes(EntityAttributes M, EntityAttributes F, float stress)
    {
        // todo: sanitize stress input
        EntityAttributes attributes = new EntityAttributes();

        attributes.Strength = Random.Range(0f, 1f) > 0.5f ? M.Strength : F.Strength;
        attributes.Strength += Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        attributes.Agility = Random.Range(0f, 1f) > 0.5f ? M.Agility : F.Agility;
        attributes.Agility += Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        attributes.Intelligence = Random.Range(0f, 1f) > 0.5f ? M.Intelligence : F.Intelligence;
        attributes.Intelligence += Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        attributes.Charisma = Random.Range(0f, 1f) > 0.5f ? M.Charisma : F.Charisma;
        attributes.Charisma += Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        attributes.Endurance = Random.Range(0f, 1f) > 0.5f ? M.Endurance : F.Endurance;
        attributes.Endurance += Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        attributes.Perception = Random.Range(0f, 1f) > 0.5f ? M.Perception : F.Perception;
        attributes.Perception += Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        return attributes;
    }

    public EntityStats InitializeStats(EntityAttributes attributes = null, float stress = 0f)
    {
        EntityStats stats = new EntityStats();

        // Base stats + attributes modifiers + random stress induced variation
        stats.Health = Stats.Health * (attributes.Strength * .1f + attributes.Endurance * .1f) + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.Speed = Stats.Speed * attributes.Agility * .1f + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.Power = Stats.Power * (attributes.Strength * .1f) + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.Defense = Stats.Defense * (attributes.Endurance * .1f + attributes.Intelligence * .1f) + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.Age = Stats.Age * (attributes.Endurance * .1f + attributes.Intelligence * .1f) + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        stats.HealthRegenRate = Stats.HealthRegenRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.HealthConsumptionRate = Stats.HealthConsumptionRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        stats.Stamina = Stats.Stamina + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.StaminaRegenRate = Stats.StaminaRegenRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.StaminaConsumptionRate = Stats.StaminaConsumptionRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        stats.Energy = Stats.Energy + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.EnergyRegenRate = Stats.EnergyRegenRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.EnergyConsumptionRate = Stats.EnergyConsumptionRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        stats.Hunger = Stats.Hunger + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.HungerThreshold = Stats.HungerThreshold + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.HungerRate = Stats.HungerRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        stats.Thirst = Stats.Thirst + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.ThirstThreshold = Stats.ThirstThreshold + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.ThirstRate = Stats.ThirstRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        stats.Desire = Stats.Desire + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.DesireThreshold = Stats.DesireThreshold + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.DesireRate = Stats.DesireRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        stats.StressRate = Stats.StressRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.StressThreshold = Stats.StressThreshold + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));
        stats.StressRecoveryRate = Stats.StressRecoveryRate + Mathf.RoundToInt(stress * Random.Range(-1f, 1f));

        return stats;
    }

    public Dictionary<string, AgentBelief> InitializeBeliefs(GoapAgent agent, BeliefFactory factory)
    {
        Dictionary<string, AgentBelief>  beliefs = new Dictionary<string, AgentBelief>();
        factory = new BeliefFactory(agent, beliefs);
        factory.AddBelief("Nothing", () => false);
        factory.AddBelief("AgentIdle", () => !agent.navMeshAgent.hasPath);
        factory.AddBelief("AgentMoving", () => agent.navMeshAgent.hasPath);

        factory.AddBelief("HasTarget", () => agent.target != null);
        factory.AddBelief("NoTarget", () => agent.target == null);
        factory.AddBelief("TargetInRange", () => agent.target != null && agent.InRangeof(agent.target.transform.position, 5f));
        factory.AddBelief("DangerClose", () => agent.dangerSource != null && agent.InRangeof(agent.dangerSource.transform.position, 10f));
        factory.AddBelief("NoDanger", () => agent.dangerSource == null || !agent.InRangeof(agent.dangerSource.transform.position, 10f));

        factory.AddBelief("LowHealth", () => agent.status.Health <= agent.entity.Stats.Health * .1f);
        factory.AddBelief("AgentIsHealthy", () => agent.status.Health >= agent.entity.Stats.Health * .8f);

        factory.AddBelief("LowEnergy", () => agent.status.Energy <= agent.entity.Stats.Energy * .1f);
        factory.AddBelief("AgentIsRested", () => agent.status.Energy >= agent.entity.Stats.Energy * .8f);
        factory.AddBelief("AgentIsResting", () => agent.currentAction != null && agent.currentAction.Name == "Rest");
        factory.AddBelief("AgentNearRestSpot", () => agent.restPositionCurrent != null && agent.InRangeof(agent.restPositionCurrent.position, 3f));
        factory.AddBelief("!AgentNearRestSpot", () => agent.restPositionCurrent == null || !agent.InRangeof(agent.restPositionCurrent.position, 3f));

        factory.AddBelief("LowStamina", () => agent.status.Stamina <= agent.entity.Stats.Stamina * .1f);
        factory.AddBelief("HighStamina", () => agent.status.Stamina >= agent.entity.Stats.Stamina * .8f);

        factory.AddBelief("Hungry", () => agent.status.Hunger >= agent.entity.Stats.HungerThreshold);
        factory.AddBelief("Peckish", () => agent.status.Hunger < agent.entity.Stats.HungerThreshold);
        factory.AddBelief("NotHungry", () => agent.status.Hunger <= agent.entity.Stats.Hunger * .1f);
        factory.AddBelief("AgentIsEating", () => agent.currentAction != null && agent.currentAction.Name == "Eat");
        factory.AddBelief("AgentNearFood", () => agent.foodPositionCurrent != null && agent.InRangeof(agent.foodPositionCurrent.position, 2f));
        factory.AddBelief("!AgentNearFood", () => agent.foodPositionCurrent == null || !agent.InRangeof(agent.foodPositionCurrent.position, 2f));

        factory.AddBelief("Thirsty", () => agent.status.Thirst >= agent.entity.Stats.ThirstThreshold);
        factory.AddBelief("Parched", () => agent.status.Thirst < agent.entity.Stats.ThirstThreshold);
        factory.AddBelief("NotThirsty", () => agent.status.Thirst <= agent.entity.Stats.Thirst * .1f);
        factory.AddBelief("AgentIsDrinking", () => agent.currentAction != null && agent.currentAction.Name == "Drink");
        factory.AddBelief("AgentNearDrinkSpot", () => agent.drinkPositionCurrent != null && agent.InRangeof(agent.drinkPositionCurrent.position, 2f));
        factory.AddBelief("!AgentNearDrinkSpot", () => agent.drinkPositionCurrent == null || !agent.InRangeof(agent.drinkPositionCurrent.position, 2f));

        factory.AddBelief("Lusty", () => agent.status.Desire > agent.entity.Stats.DesireThreshold);
        factory.AddBelief("NotLusty", () => agent.status.Desire <= agent.entity.Stats.Desire * .1f);
        factory.AddBelief("CanMate", () => agent.entity.IsTagPresent(EntityTag.Adult) && !agent.entity.IsPregnant);
        factory.AddBelief("Pregnant", () => agent.entity.IsPregnant && !agent.entity.IsMale);

        factory.AddBelief("FoodLocationKnown", () => agent.foodPositionsKnown.Count > 0);
        factory.AddBelief("!FoodLocationKnown", () => agent.foodPositionsKnown.Count == 0);
        factory.AddBelief("RestLocationKnown", () => agent.restPositionsKnown.Count > 0);
        factory.AddBelief("!RestLocationKnown", () => agent.restPositionsKnown.Count == 0);
        factory.AddBelief("DrinkLocationKnown", () => agent.drinkPositionsKnown.Count > 0);
        factory.AddBelief("!DrinkLocationKnown", () => agent.drinkPositionsKnown.Count == 0);

        return beliefs;
    }

    public HashSet<AgentAction> InitializeActions(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
    {
        HashSet<AgentAction> actions = new HashSet<AgentAction>();
        // Actions would be initialized here similarly to the GoapAgent example
        actions.Add(new AgentAction.Builder("Relax")
            .WithCost(0)
            .AddPrecondition(beliefs["NotHungry"])
            .AddPrecondition(beliefs["NotThirsty"])
            .AddPrecondition(beliefs["AgentIsRested"])
            .AddPrecondition(beliefs["NoDanger"])
            .AddPrecondition(beliefs["NotLusty"])
            .AddPrecondition(beliefs["LowStamina"])
            .WithStrategy(new IdleStrategy(agent, 10))
            .AddEffect(beliefs["Nothing"])
            .AddEffect(beliefs["HighStamina"])
            .Build());

        //actions.Add(new AgentAction.Builder("Wander")
        //    .WithCost(1)
        //    .WithStrategy(new WanderStrategy(agent.navMeshAgent, 60f))
        //    .AddEffect(beliefs["AgentMoving"])
        //    .Build());


        //actions.Add(new AgentAction.Builder("Explore")
        //    .WithCost(10)
        //    .WithStrategy(new WanderStrategy(agent.navMeshAgent, 1200f))
        //    .AddEffect(beliefs["AgentMoving"])
        //    .AddEffect(beliefs["RestLocationKnown"])
        //    .AddEffect(beliefs["FoodLocationKnown"])
        //    .AddEffect(beliefs["DrinkLocationKnown"])
        //    .Build());

        #region IdleActions
        actions.Add(new AgentAction.Builder("Recover")
            .WithCost(0)
            .AddPrecondition(beliefs["LowStamina"])
            .WithStrategy(new IdleStrategy(agent, 5f))
            .AddEffect(beliefs["HighStamina"])
            .Build());

        #endregion

        #region RestActions
        actions.Add(new AgentAction.Builder("SeekRest")
            .WithCost(1)
            .AddPrecondition(beliefs["!RestLocationKnown"])
            .WithStrategy(new WanderStrategy(agent.navMeshAgent, 60f))
            .AddEffect(beliefs["RestLocationKnown"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToRestSpot")
            .WithCost(1)
            .WithStrategy(new MoveStrategy(agent.navMeshAgent, () => agent.restPositionCurrent.position))
            .AddPrecondition(beliefs["RestLocationKnown"])
            .AddPrecondition(beliefs["LowEnergy"])
            .AddEffect(beliefs["AgentNearRestSpot"])
            .Build());

        actions.Add(new AgentAction.Builder("Rest")
            .WithCost(1)
            .WithStrategy(new RestStrategy(agent))
            .AddPrecondition(beliefs["LowEnergy"])
            .AddPrecondition(beliefs["AgentNearRestSpot"])
            .AddEffect(beliefs["AgentIsResting"])
            .Build());

        actions.Add(new AgentAction.Builder("FinishResting")
            .WithCost(0)
            .AddPrecondition(beliefs["AgentIsResting"])
            .WithStrategy(new IdleStrategy(agent, 1))
            .AddEffect(beliefs["AgentIsRested"])
            .Build());
        #endregion

        #region FoodActions
        actions.Add(new AgentAction.Builder("SeekFood")
            .WithCost(1)
            .AddPrecondition(beliefs["!FoodLocationKnown"])
            .WithStrategy(new WanderStrategy(agent.navMeshAgent, 60f))
            .AddEffect(beliefs["FoodLocationKnown"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToFoodSpot")
            .WithCost(1)
            .WithStrategy(new MoveStrategy(agent.navMeshAgent, () => agent.foodPositionCurrent.position))
            .AddPrecondition(beliefs["FoodLocationKnown"])
            .AddPrecondition(beliefs["Hungry"])
            .AddEffect(beliefs["AgentNearFood"])
            .Build());

        actions.Add(new AgentAction.Builder("Eat")
            .WithCost(1)
            .WithStrategy(new EatStrategy(agent))
            .AddPrecondition(beliefs["Hungry"])
            .AddPrecondition(beliefs["AgentNearFood"])
            .AddEffect(beliefs["NotHungry"])
            .Build());
        #endregion

        #region DrinkActions
        actions.Add(new AgentAction.Builder("SeekDrink")
            .WithCost(1)
            .AddPrecondition(beliefs["!DrinkLocationKnown"])
            .WithStrategy(new WanderStrategy(agent.navMeshAgent, 60f))
            .AddEffect(beliefs["DrinkLocationKnown"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToDrinkSpot")
            .WithCost(1)
            .WithStrategy(new MoveStrategy(agent.navMeshAgent, () => agent.drinkPositionCurrent.position))
            .AddPrecondition(beliefs["DrinkLocationKnown"])
            .AddPrecondition(beliefs["Thirsty"])
            .AddEffect(beliefs["AgentNearDrinkSpot"])
            .Build());

        actions.Add(new AgentAction.Builder("Drink")
            .WithCost(1)
            .WithStrategy(new DrinkStrategy(agent))
            .AddPrecondition(beliefs["Thirsty"])
            .AddPrecondition(beliefs["AgentNearDrinkSpot"])
            .AddEffect(beliefs["NotThirsty"])
            .Build());
        #endregion

        //actions.Add(new AgentAction.Builder("Flee")
        //    .WithCost(0)
        //    .WithStrategy(new FleeStrategy())       // implement flee strategy    
        //    .AddPrecondition(beliefs["DangerClose"])
        //    .AddEffect(beliefs["NoDanger"])
        //    .Build());

        //actions.Add(new AgentAction.Builder("Mate")
        //    .WithCost(3)
        //    .WithStrategy(new MateStrategy())       // implement mate strategy
        //    .AddPrecondition(beliefs["Lusty"])
        //    .AddEffect(beliefs["NotLusty"])
        //    .AddEffect(beliefs["Pregnant"])
        //    .Build());

        //actions.Add(new AgentAction.Builder("Chase")
        //    .WithCost(5)
        //    .WithStrategy(new ChaseStrategy())      // implement chase strategy
        //    .AddPrecondition(beliefs["HasTarget"])
        //    .AddEffect(beliefs["AgentMoving"])
        //    .AddEffect(beliefs["TargetInRange"])
        //    .Build());

        //actions.Add(new AgentAction.Builder("Attack")
        //    .WithCost(4)
        //    .WithStrategy(new AttackStrategy())     // implement attack strategy
        //    .AddPrecondition(beliefs["HasTarget"])
        //    .AddEffect(beliefs["Nothing"])          // define appropriate effect
        //    .Build());

        //actions.Add(new AgentAction.Builder("Forage")
        //    .WithCost(2)
        //    .WithStrategy(new ForageStrategy())     // implement forage strategy
        //    .AddEffect(beliefs["FoodLocationKnown"])
        //    .Build());

        //actions.Add(new AgentAction.Builder("SearchWater")
        //    .WithCost(2)
        //    .WithStrategy(new SeekStrategy()) // implement search water strategy
        //    .AddEffect(beliefs["DrinkLocationKnown"])
        //    .Build());

        //actions.Add(new AgentAction.Builder("Follow")
        //    .WithCost(2)
        //    .WithStrategy(new FollowStrategy())     // implement follow strategy
        //    .AddPrecondition(beliefs["HasTarget"])
        //    .AddEffect(beliefs["AgentMoving"])
        //    .Build());

        //actions.Add(new AgentAction.Builder("Sleep")
        //    .WithCost(2)
        //    .WithStrategy(new SleepStrategy())      // implement sleep strategy
        //    .AddPrecondition(beliefs["LowEnergy"])
        //    .AddEffect(beliefs["AgentIsRested"])
        //    .Build());

        //actions.Add(new AgentAction.Builder("Birth")
        //    .WithCost(5)
        //    .AddPrecondition(beliefs["Pregnant"])
        //    .WithStrategy(new BirthStrategy())      // implement birth strategy
        //    .AddEffect(beliefs["Nothing"])          // define appropriate effect
        //    .Build());

        return actions;
    }

    public HashSet<AgentGoal> InitializeGoals(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
    {
        HashSet<AgentGoal> goals = new HashSet<AgentGoal>();
        // Goals would be initialized here similarly to the GoapAgent example
        goals.Add(new AgentGoal.Builder("Chill")
            .WithPriority(0)
            .AddDesiredState(beliefs["Nothing"])
            .Build());

        goals.Add(new AgentGoal.Builder("Know Rest Spot")
            .WithPriority(1)
            .AddDesiredState(beliefs["RestLocationKnown"])
            .Build());

        goals.Add(new AgentGoal.Builder("KeepHealthUp")
            .WithPriority(2)
            .AddDesiredState(beliefs["AgentIsHealthy"])
            .Build());

        goals.Add(new AgentGoal.Builder("KeepEnergyUp")
            .WithPriority(3)
            .AddDesiredState(beliefs["AgentIsRested"])
            .Build());

        goals.Add(new AgentGoal.Builder("RecoverStamina")
            .WithPriority(10)
            .AddDesiredState(beliefs["HighStamina"])
            .Build());

        goals.Add(new AgentGoal.Builder("Know Food Spot")
            .WithPriority(2)
            .AddDesiredState(beliefs["FoodLocationKnown"])
            .Build());

        goals.Add(new AgentGoal.Builder("SatisfyHunger")
            .WithPriority(6)
            .AddDesiredState(beliefs["NotHungry"])
            .Build());

        goals.Add(new AgentGoal.Builder("Know Drink Spot")
            .WithPriority(3)
            .AddDesiredState(beliefs["DrinkLocationKnown"])
            .Build());

        goals.Add(new AgentGoal.Builder("SatisfyThirst")
            .WithPriority(5)
            .AddDesiredState(beliefs["NotThirsty"])
            .Build());

        //goals.Add(new AgentGoal.Builder("SatisfyDesire")
        //    .WithPriority(7)
        //    .AddDesiredState(beliefs["NotLusty"])
        //    .Build());

        //goals.Add(new AgentGoal.Builder("Safe")
        //    .WithPriority(10)
        //    .AddDesiredState(beliefs["NoDanger"])
        //    .Build());

        return goals;
    }
}

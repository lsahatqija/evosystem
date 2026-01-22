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

    public List<EntityTag> Tags;
    public EntityAttributes Attributes;
    public EntityStats Stats;

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

    public Dictionary<string, AgentBelief> InitializeBeliefs(GoapAgent agent)
    {
        Dictionary<string, AgentBelief>  beliefs = new Dictionary<string, AgentBelief>();
        BeliefFactory factory = new BeliefFactory(agent, beliefs);
        factory.AddBelief("Nothing", () => false);
        factory.AddBelief("AgentIdle", () => !agent.navMeshAgent.hasPath);
        factory.AddBelief("AgentMoving", () => agent.navMeshAgent.hasPath);

        factory.AddBelief("HasTarget", () => agent.target != null);
        factory.AddBelief("DangerClose", () => agent.dangerSource != null && agent.InRangeof(agent.dangerSource.transform.position, 10f));
        factory.AddBelief("NoDanger", () => agent.dangerSource == null || !agent.InRangeof(agent.dangerSource.transform.position, 10f));

        factory.AddBelief("LowHealth", () => agent.status.Health <= agent.entity.Stats.Health * .1f);
        factory.AddBelief("AgentIsHealthy", () => agent.status.Health >= agent.entity.Stats.Health * .8f);

        factory.AddBelief("LowEnergy", () => agent.status.Energy <= agent.entity.Stats.Energy * .1f);
        factory.AddBelief("AgentIsRested", () => agent.status.Energy >= agent.entity.Stats.Energy * .8f);
        factory.AddBelief("AgentIsResting", () => agent.currentAction != null && agent.currentAction.Name == "Rest" && agent.InRangeof(beliefs["RestLocation"].Location, 1f));
        factory.AddBelief("AgentNearRestSpot", () => beliefs.ContainsKey("RestLocation") && agent.InRangeof(beliefs["RestLocation"].Location, 1f));

        factory.AddBelief("LowStamina", () => agent.status.Stamina <= agent.entity.Stats.Stamina * .1f);
        factory.AddBelief("HighStamina", () => agent.status.Stamina >= agent.entity.Stats.Stamina * .8f);

        factory.AddBelief("Hungry", () => agent.status.Hunger > agent.entity.Stats.HungerThreshold);
        factory.AddBelief("NotHungry", () => agent.status.Hunger <= agent.entity.Stats.Hunger * .1f);
        factory.AddBelief("AgentIsEating", () => agent.currentAction != null && agent.currentAction.Name == "Eat" && agent.InRangeof(beliefs["FoodLocation"].Location, 1f));
        factory.AddBelief("AgentNearFood", () => beliefs.ContainsKey("FoodLocation") && agent.InRangeof(beliefs["FoodLocation"].Location, 1f));

        factory.AddBelief("Thirsty", () => agent.status.Thirst > agent.entity.Stats.ThirstThreshold);
        factory.AddBelief("NotThirsty", () => agent.status.Thirst <= agent.entity.Stats.Thirst * .1f);
        factory.AddBelief("AgentIsDrinking", () => agent.currentAction != null && agent.currentAction.Name == "Drink" && agent.InRangeof(beliefs["DrinkLocation"].Location, 1f));

        factory.AddBelief("Lusty", () => agent.status.Desire > agent.entity.Stats.DesireThreshold);
        factory.AddBelief("NotLusty", () => agent.status.Desire <= agent.entity.Stats.Desire * .1f);

        factory.AddBelief("FoodLocationKnown", () => beliefs.ContainsKey("FoodLocation") && beliefs["FoodLocation"].Location != Vector3.zero);
        factory.AddBelief("RestLocationKnown", () => beliefs.ContainsKey("RestLocation") && beliefs["RestLocation"].Location != Vector3.zero);
        factory.AddBelief("DrinkLocationKnown", () => beliefs.ContainsKey("DrinkLocation") && beliefs["DrinkLocation"].Location != Vector3.zero);



        return beliefs;
    }

    public HashSet<AgentAction> InitializeActions(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
    {
        HashSet<AgentAction> actions = new HashSet<AgentAction>();
        // Actions would be initialized here similarly to the GoapAgent example
        return actions;
    }

    public HashSet<AgentGoal> InitializeGoals(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
    {
        HashSet<AgentGoal> goals = new HashSet<AgentGoal>();
        // Goals would be initialized here similarly to the GoapAgent example
        goals.Add(new AgentGoal.Builder("Chill")
            .WithPriority(1f)
            .AddDesiredState(beliefs["Nothing"])
            .Build());

        goals.Add(new AgentGoal.Builder("Explore")
            .WithPriority(1)
            .AddDesiredState(beliefs["AgentMoving"])
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
            .WithPriority(4)
            .AddDesiredState(beliefs["HighStamina"])
            .Build());

        goals.Add(new AgentGoal.Builder("SatisfyThirst")
            .WithPriority(5)
            .AddDesiredState(beliefs["NotThirsty"])
            .Build());

        goals.Add(new AgentGoal.Builder("SatisfyHunger")
            .WithPriority(6)
            .AddDesiredState(beliefs["NotHungry"])
            .Build());

        goals.Add(new AgentGoal.Builder("SatisfyDesire")
            .WithPriority(7)
            .AddDesiredState(beliefs["NotLusty"])
            .Build());

        goals.Add(new AgentGoal.Builder("Safe")
            .WithPriority(10)
            .AddDesiredState(beliefs["NoDanger"])
            .Build());

        return goals;
    }
}

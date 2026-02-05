using System.Collections.Generic;
using System.IO;
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

    public GameObject entityModel;

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
    public Entity mateEntity;
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

    public EntityStats InitializeStats(EntityStats stats, EntityAttributes attributes)
    {
        stats.Health = stats.Health * (attributes.Strength * .1f + attributes.Endurance * .1f);
        stats.HealthRegenRate = stats.HealthRegenRate + EntityUtils.StatVariation(stats.HealthRegenRate);
        stats.HealthConsumptionRate = stats.HealthConsumptionRate + EntityUtils.StatVariation(stats.HealthConsumptionRate);

        stats.Speed = stats.Speed * attributes.Agility * .1f + EntityUtils.StatVariation(stats.Speed);
        stats.Power = stats.Power * (attributes.Strength * .1f) + EntityUtils.StatVariation(stats.Power);
        stats.Defense = stats.Defense * (attributes.Endurance * .1f + attributes.Intelligence * .1f) + EntityUtils.StatVariation(stats.Defense);
        stats.Age = stats.Age * (attributes.Intelligence * .1f) + EntityUtils.StatVariation(stats.Age);
        stats.Size = stats.Size * (attributes.Strength * .1f) + EntityUtils.StatVariation(stats.Size);

        stats.Stamina = stats.Stamina + EntityUtils.StatVariation(stats.Stamina);
        stats.StaminaRegenRate = stats.StaminaRegenRate + EntityUtils.StatVariation(stats.StaminaRegenRate);
        stats.StaminaConsumptionRate = stats.StaminaConsumptionRate + EntityUtils.StatVariation(stats.StaminaConsumptionRate);

        stats.Energy = stats.Energy + EntityUtils.StatVariation(stats.Energy);
        stats.EnergyRegenRate = stats.EnergyRegenRate + EntityUtils.StatVariation(stats.EnergyRegenRate);
        stats.EnergyConsumptionRate = stats.EnergyConsumptionRate + EntityUtils.StatVariation(stats.EnergyConsumptionRate);

        stats.Hunger = stats.Hunger + EntityUtils.StatVariation(stats.Hunger);
        stats.HungerThreshold = stats.HungerThreshold + EntityUtils.StatVariation(stats.HungerThreshold);
        stats.HungerRate = stats.HungerRate + EntityUtils.StatVariation(stats.HungerRate);

        stats.Thirst = stats.Thirst + EntityUtils.StatVariation(stats.Thirst);
        stats.ThirstThreshold = stats.ThirstThreshold + EntityUtils.StatVariation(stats.ThirstThreshold);
        stats.ThirstRate = stats.ThirstRate + EntityUtils.StatVariation(stats.ThirstRate);

        stats.Desire = stats.Desire + EntityUtils.StatVariation(stats.Desire);
        stats.DesireThreshold = stats.DesireThreshold + EntityUtils.StatVariation(stats.DesireThreshold);
        stats.DesireRate = stats.DesireRate + EntityUtils.StatVariation(stats.DesireRate);

        stats.StressRate = stats.StressRate + EntityUtils.StatVariation(stats.StressRate);
        stats.StressThreshold = stats.StressThreshold + EntityUtils.StatVariation(stats.StressThreshold);
        stats.StressRecoveryRate = stats.StressRecoveryRate + EntityUtils.StatVariation(stats.StressRecoveryRate);

        stats.MaleChance = stats.MaleChance + EntityUtils.StatVariation(stats.MaleChance);
        stats.PregnancyDuration = stats.PregnancyDuration + EntityUtils.StatVariation(stats.PregnancyDuration);
        stats.EggDuration = stats.EggDuration + EntityUtils.StatVariation(stats.EggDuration);
        stats.ClutchSize = stats.ClutchSize + (int)EntityUtils.StatVariation(stats.ClutchSize);

        IsMale = Random.Range(0f, 1f) < stats.MaleChance;

        return stats;
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

        #region Targets
        factory.AddBelief("HasTarget", () => agent.target != null);
        factory.AddBelief("NoTarget", () => agent.target == null);
        factory.AddBelief("TargetInRange", () => agent.target != null && agent.InRangeof(agent.target.transform.position, 5f));
        factory.AddBelief("DangerClose", () => agent.dangerSource != null && agent.InRangeof(agent.dangerSource.transform.position, 10f));
        factory.AddBelief("NoDanger", () => agent.dangerSource == null || !agent.InRangeof(agent.dangerSource.transform.position, 10f));
        #endregion

        #region Health
        factory.AddBelief("LowHealth", () => agent.status.Health <= agent.entity.Stats.Health * .1f);
        factory.AddBelief("AgentIsHealthy", () => agent.status.Health >= agent.entity.Stats.Health * .8f);
        factory.AddBelief("LowStamina", () => agent.status.Stamina <= agent.entity.Stats.Stamina * .1f);
        factory.AddBelief("HighStamina", () => agent.status.Stamina >= agent.entity.Stats.Stamina * .8f);
        #endregion

        #region Energy
        factory.AddBelief("LowEnergy", () => agent.status.Energy <= agent.entity.Stats.Energy * .1f);
        factory.AddBelief("AgentIsRested", () => agent.status.Energy >= agent.entity.Stats.Energy * .8f);
        factory.AddBelief("AgentIsResting", () => agent.currentAction != null && agent.currentAction.Name == "Rest");
        factory.AddBelief("AgentNearRestSpot", () => agent.restPositionCurrent != null && agent.InRangeof(agent.restPositionCurrent.position, 3f));
        factory.AddBelief("!AgentNearRestSpot", () => agent.restPositionCurrent == null || !agent.InRangeof(agent.restPositionCurrent.position, 3f));
        #endregion

        #region Hunger
        factory.AddBelief("Hungry", () => agent.status.Hunger >= agent.entity.Stats.HungerThreshold);
        factory.AddBelief("Peckish", () => agent.status.Hunger < agent.entity.Stats.HungerThreshold);
        factory.AddBelief("NotHungry", () => agent.status.Hunger <= agent.entity.Stats.Hunger * .1f);
        factory.AddBelief("AgentIsEating", () => agent.currentAction != null && agent.currentAction.Name == "Eat");
        factory.AddBelief("AgentNearFood", () => agent.foodPositionCurrent != null && agent.InRangeof(agent.foodPositionCurrent.position, 2f));
        factory.AddBelief("!AgentNearFood", () => agent.foodPositionCurrent == null || !agent.InRangeof(agent.foodPositionCurrent.position, 2f));
        #endregion

        #region Thirst
        factory.AddBelief("Thirsty", () => agent.status.Thirst >= agent.entity.Stats.ThirstThreshold);
        factory.AddBelief("Parched", () => agent.status.Thirst < agent.entity.Stats.ThirstThreshold);
        factory.AddBelief("NotThirsty", () => agent.status.Thirst <= agent.entity.Stats.Thirst * .1f);
        factory.AddBelief("AgentIsDrinking", () => agent.currentAction != null && agent.currentAction.Name == "Drink");
        factory.AddBelief("AgentNearDrinkSpot", () => agent.drinkPositionCurrent != null && agent.InRangeof(agent.drinkPositionCurrent.position, 2f));
        factory.AddBelief("!AgentNearDrinkSpot", () => agent.drinkPositionCurrent == null || !agent.InRangeof(agent.drinkPositionCurrent.position, 2f));
        #endregion

        #region Mate
        factory.AddBelief("Lusty", () => agent.status.Desire > agent.entity.Stats.DesireThreshold);
        factory.AddBelief("NotLusty", () => agent.status.Desire <= agent.entity.Stats.Desire * .1f);
        factory.AddBelief("CanMate", () => agent.entity.IsTagPresent(EntityTag.Adult) && (!agent.entity.IsPregnant || agent.entity.IsMale));
        factory.AddBelief("Pregnant", () => agent.entity.IsPregnant && !agent.entity.IsMale);
        factory.AddBelief("!Pregnant", () => !agent.entity.IsPregnant || agent.entity.IsMale);
        factory.AddBelief("AgentNearMate", () => agent.potentialMate != null && agent.InRangeof(agent.potentialMate.transform.position, 2f));
        factory.AddBelief("!AgentNearMate", () => agent.potentialMate == null || !agent.InRangeof(agent.potentialMate.transform.position, 2f));
        #endregion

        #region Locations
        factory.AddBelief("FoodLocationKnown", () => agent.foodPositionsKnown.Count > 0);
        factory.AddBelief("!FoodLocationKnown", () => agent.foodPositionsKnown.Count == 0);
        factory.AddBelief("RestLocationKnown", () => agent.restPositionsKnown.Count > 0);
        factory.AddBelief("!RestLocationKnown", () => agent.restPositionsKnown.Count == 0);
        factory.AddBelief("DrinkLocationKnown", () => agent.drinkPositionsKnown.Count > 0);
        factory.AddBelief("!DrinkLocationKnown", () => agent.drinkPositionsKnown.Count == 0);
        factory.AddBelief("MateLocationKnown", () => agent.potentialMate != null);
        factory.AddBelief("!MateLocationKnown", () => agent.potentialMate == null);
        #endregion

        return beliefs;
    }

    public HashSet<AgentAction> InitializeActions(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
    {
        HashSet<AgentAction> actions = new HashSet<AgentAction>();
        // Actions would be initialized here similarly to the GoapAgent example
        actions.Add(new AgentAction.Builder("Chill")
            .WithCost(0)
            .WithStrategy(new IdleStrategy(agent, 10))
            .AddEffect(beliefs["Nothing"])
            .Build());

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
            .WithCost(5)
            .AddPrecondition(beliefs["!RestLocationKnown"])
            .WithStrategy(new WanderStrategy(agent.navMeshAgent, 60f))
            .AddEffect(beliefs["RestLocationKnown"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToRestSpot")
            .WithCost(3)
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
            .WithCost(5)
            .AddPrecondition(beliefs["!FoodLocationKnown"])
            .WithStrategy(new WanderStrategy(agent.navMeshAgent, 60f))
            .AddEffect(beliefs["FoodLocationKnown"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToFoodSpot")
            .WithCost(3)
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
            .WithCost(5)
            .AddPrecondition(beliefs["!DrinkLocationKnown"])
            .WithStrategy(new WanderStrategy(agent.navMeshAgent, 60f))
            .AddEffect(beliefs["DrinkLocationKnown"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToDrinkSpot")
            .WithCost(3)
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

        #region MateActions
        actions.Add(new AgentAction.Builder("SeekMate")
            .WithCost(1)
            .AddPrecondition(beliefs["!MateLocationKnown"])
            .WithStrategy(new WanderStrategy(agent.navMeshAgent, 60f))
            .AddEffect(beliefs["MateLocationKnown"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToMate")
            .WithCost(1)
            .AddPrecondition(beliefs["Lusty"])
            .AddPrecondition(beliefs["CanMate"])
            .AddPrecondition(beliefs["!Pregnant"])
            .AddPrecondition(beliefs["!AgentNearMate"])
            .AddPrecondition(beliefs["MateLocationKnown"])
            .WithStrategy(new MoveStrategy(agent.navMeshAgent, () => (agent.potentialMate.transform.position + agent.transform.position) / 2f)) //midpoint between the two
            .AddEffect(beliefs["AgentNearMate"])
            .Build());

        actions.Add(new AgentAction.Builder("Mate")
            .WithCost(1)
            .AddPrecondition(beliefs["Lusty"])
            .AddPrecondition(beliefs["CanMate"])
            .AddPrecondition(beliefs["!Pregnant"])
            .AddPrecondition(beliefs["AgentNearMate"])
            .WithStrategy(new MateStrategy(agent))
            .AddEffect(beliefs["NotLusty"])
            .Build());

        //actions.Add(new AgentAction.Builder("Birth")
        //    .WithCost(5)
        //    .AddPrecondition(beliefs["Pregnant"])
        //    .WithStrategy(new BirthStrategy(agent))      // implement birth strategy
        //    .AddEffect(beliefs["Nothing"])          // define appropriate effect
        //    .Build());
        #endregion

        //actions.Add(new AgentAction.Builder("Flee")
        //    .WithCost(0)
        //    .WithStrategy(new FleeStrategy())       // implement flee strategy    
        //    .AddPrecondition(beliefs["DangerClose"])
        //    .AddEffect(beliefs["NoDanger"])
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

        goals.Add(new AgentGoal.Builder("Know Mate")
            .WithPriority(4)
            .AddDesiredState(beliefs["MateLocationKnown"])
            .Build());

        goals.Add(new AgentGoal.Builder("SatisfyDesire")
            .WithPriority(10)
            .AddDesiredState(beliefs["NotLusty"])
            .Build());

        //goals.Add(new AgentGoal.Builder("Safe")
        //    .WithPriority(10)
        //    .AddDesiredState(beliefs["NoDanger"])
        //    .Build());

        return goals;
    }
}

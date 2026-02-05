using UnityEngine;
using System.Collections.Generic;

public class EntityUtils
{
    public static EntityStatus InitializeEntityStatus(EntityStats entityStats = null)
    {
        EntityStatus status = new EntityStatus
        {
            Health = 100f,
            Stamina = 100f,
            Energy = 100f,
            Hunger = 0f,
            Thirst = 0f,
            Desire = 0f,
            Age = 0f,
            Stress = 0f,
        };

        if (entityStats != null)
        {
            status.Health = entityStats.Health;
            status.Stamina = entityStats.Stamina;
            status.Energy = entityStats.Energy;
        }

        return status;
    }

    public static EntityStatus ProcessEntityStatus(EntityStats baseStats, EntityStatus currentStatus)
    {
        EntityStatus finalStatus = new EntityStatus
        {
            Health = currentStatus.Health,
            Stamina = currentStatus.Stamina,
            Energy = currentStatus.Energy,
            Hunger = currentStatus.Hunger,
            Thirst = currentStatus.Thirst,
            Desire = currentStatus.Desire,
            Age = currentStatus.Age,
        };

        if (baseStats == null)
            return finalStatus;

        finalStatus.Stamina = Mathf.Clamp(currentStatus.Stamina - baseStats.StaminaConsumptionRate, 0f, baseStats.Stamina);
        finalStatus.Energy = Mathf.Clamp(currentStatus.Energy - baseStats.EnergyConsumptionRate, 0f, baseStats.Energy);
        finalStatus.Hunger = Mathf.Clamp(currentStatus.Hunger + baseStats.HungerRate, 0f, baseStats.Hunger);
        finalStatus.Thirst = Mathf.Clamp(currentStatus.Thirst + baseStats.ThirstRate, 0f, baseStats.Thirst);
        finalStatus.Desire = Mathf.Clamp(currentStatus.Desire + baseStats.DesireRate, 0f, baseStats.Desire);
        finalStatus.Stress = Mathf.Clamp(currentStatus.Stress - baseStats.StressRecoveryRate, 0f, 2f * baseStats.StressThreshold);
        finalStatus.Age++;

        float healthDelta = baseStats.HealthRegenRate;
        float healthConsumptionTicks = 0f;
        if (finalStatus.Hunger >= baseStats.HungerThreshold) healthConsumptionTicks += .2f;
        if (finalStatus.Thirst >= baseStats.ThirstThreshold) healthConsumptionTicks += .2f;
        if (finalStatus.Desire >= baseStats.DesireThreshold) healthConsumptionTicks += .1f;
        if (finalStatus.Stamina < baseStats.Stamina * .1f) healthConsumptionTicks += 1f;
        if (finalStatus.Energy < baseStats.Energy * .1f) healthConsumptionTicks += 1f;
        if (finalStatus.Age > baseStats.Age) healthConsumptionTicks += 100f;

        healthDelta -= baseStats.HealthConsumptionRate * healthConsumptionTicks;
        finalStatus.Health = Mathf.Clamp(currentStatus.Health + healthDelta, 0f, baseStats.Health);

        return finalStatus;
    }

    public static EntityAttributes CombineAttributes(EntityAttributes M, EntityAttributes F, float stress)
    {
        // todo: sanitize stress input
        EntityAttributes attributes = new EntityAttributes();

        attributes.Strength = Random.Range(0f, 1f) > 0.5f ? M.Strength : F.Strength;
        attributes.Strength += Mathf.RoundToInt(stress * Random.Range(-.1f, .1f));

        attributes.Agility = Random.Range(0f, 1f) > 0.5f ? M.Agility : F.Agility;
        attributes.Agility += Mathf.RoundToInt(stress * Random.Range(-.1f, .1f));

        attributes.Intelligence = Random.Range(0f, 1f) > 0.5f ? M.Intelligence : F.Intelligence;
        attributes.Intelligence += Mathf.RoundToInt(stress * Random.Range(-.1f, .1f));

        attributes.Charisma = Random.Range(0f, 1f) > 0.5f ? M.Charisma : F.Charisma;
        attributes.Charisma += Mathf.RoundToInt(stress * Random.Range(-.1f, .1f));

        attributes.Endurance = Random.Range(0f, 1f) > 0.5f ? M.Endurance : F.Endurance;
        attributes.Endurance += Mathf.RoundToInt(stress * Random.Range(-.1f, .1f));

        attributes.Perception = Random.Range(0f, 1f) > 0.5f ? M.Perception : F.Perception;
        attributes.Perception += Mathf.RoundToInt(stress * Random.Range(-.1f, .1f));

        return attributes;
    }

    public static EntityStats CombineInitialStats(EntityStats M, EntityStats F)
    {
        EntityStats stats = new EntityStats();

        stats.Health = Random.Range(0f, 1f) > 0.5f ? M.Health : F.Health;
        stats.Speed = Random.Range(0f, 1f) > 0.5f ? M.Speed : F.Speed;
        stats.Power = Random.Range(0f, 1f) > 0.5f ? M.Power : F.Power;
        stats.Defense = Random.Range(0f, 1f) > 0.5f ? M.Defense : F.Defense;
        stats.Age = Random.Range(0f, 1f) > 0.5f ? M.Age : F.Age;
        stats.Size = Random.Range(0f, 1f) > 0.5f ? M.Size : F.Size;

        stats.HealthRegenRate = Random.Range(0f, 1f) > 0.5f ? M.HealthRegenRate : F.HealthRegenRate;
        stats.HealthConsumptionRate = Random.Range(0f, 1f) > 0.5f ? M.HealthConsumptionRate : F.HealthConsumptionRate;

        stats.Stamina = Random.Range(0f, 1f) > 0.5f ? M.Stamina : F.Stamina;
        stats.StaminaRegenRate = Random.Range(0f, 1f) > 0.5f ? M.StaminaRegenRate : F.StaminaRegenRate;
        stats.StaminaConsumptionRate = Random.Range(0f, 1f) > 0.5f ? M.StaminaConsumptionRate : F.StaminaConsumptionRate;

        stats.Energy = Random.Range(0f, 1f) > 0.5f ? M.Energy : F.Energy;
        stats.EnergyRegenRate = Random.Range(0f, 1f) > 0.5f ? M.EnergyRegenRate : F.EnergyRegenRate;
        stats.EnergyConsumptionRate = Random.Range(0f, 1f) > 0.5f ? M.EnergyConsumptionRate : F.EnergyConsumptionRate;

        stats.Hunger = Random.Range(0f, 1f) > 0.5f ? M.Hunger : F.Hunger;
        stats.HungerRate = Random.Range(0f, 1f) > 0.5f ? M.HungerRate : F.HungerRate;
        stats.HungerThreshold = Random.Range(0f, 1f) > 0.5f ? M.HungerThreshold : F.HungerThreshold;

        stats.Thirst = Random.Range(0f, 1f) > 0.5f ? M.Thirst : F.Thirst;
        stats.ThirstRate = Random.Range(0f, 1f) > 0.5f ? M.ThirstRate : F.ThirstRate;
        stats.ThirstThreshold = Random.Range(0f, 1f) > 0.5f ? M.ThirstThreshold : F.ThirstThreshold;

        stats.Desire = Random.Range(0f, 1f) > 0.5f ? M.Desire : F.Desire;
        stats.DesireThreshold = Random.Range(0f, 1f) > 0.5f ? M.DesireThreshold : F.DesireThreshold;
        stats.DesireRate = Random.Range(0f, 1f) > 0.5f ? M.DesireRate : F.DesireRate;

        stats.StressRate = Random.Range(0f, 1f) > 0.5f ? M.StressRate : F.StressRate;
        stats.StressThreshold = Random.Range(0f, 1f) > 0.5f ? M.StressThreshold : F.StressThreshold;
        stats.StressRecoveryRate = Random.Range(0f, 1f) > 0.5f ? M.StressRecoveryRate : F.StressRecoveryRate;

        stats.MaleChance = Random.Range(0f, 1f) > 0.5f ? M.MaleChance : F.MaleChance;

        return stats;
    }

    public static float StatVariation(float initialStat, float variance = .1f) => Mathf.RoundToInt(initialStat * Random.Range(-variance, variance));

}
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
            Desire = 0f
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
            Desire = currentStatus.Desire
        };

        if (baseStats == null)
            return finalStatus;

        finalStatus.Stamina = Mathf.Clamp(currentStatus.Stamina - baseStats.StaminaConsumptionRate, 0f, baseStats.Stamina);
        finalStatus.Energy = Mathf.Clamp(currentStatus.Energy - baseStats.EnergyConsumptionRate, 0f, baseStats.Energy);
        finalStatus.Hunger = Mathf.Clamp(currentStatus.Hunger + baseStats.HungerRate, 0f, baseStats.Hunger);
        finalStatus.Thirst = Mathf.Clamp(currentStatus.Thirst + baseStats.ThirstRate, 0f, baseStats.Thirst);
        finalStatus.Desire = Mathf.Clamp(currentStatus.Desire + baseStats.DesireRate, 0f, baseStats.Desire);
        finalStatus.Stress = Mathf.Clamp(currentStatus.Stress - baseStats.StressRecoveryRate, 0f, 2f * baseStats.StressThreshold);

        float healthDelta = baseStats.HealthRegenRate;
        float healthConsumptionTicks = 0f;
        if (finalStatus.Hunger >= baseStats.HungerThreshold) healthConsumptionTicks += .2f;
        if (finalStatus.Thirst >= baseStats.ThirstThreshold) healthConsumptionTicks += .2f;
        if (finalStatus.Desire >= baseStats.DesireThreshold) healthConsumptionTicks += .1f;
        if (finalStatus.Stamina < baseStats.Stamina * .1f) healthConsumptionTicks += 1f;
        if (finalStatus.Energy < baseStats.Energy * .1f) healthConsumptionTicks += 1f;

        healthDelta -= baseStats.HealthConsumptionRate * healthConsumptionTicks;
        finalStatus.Health = Mathf.Clamp(currentStatus.Health + healthDelta, 0f, baseStats.Health);

        return finalStatus;
    }
}
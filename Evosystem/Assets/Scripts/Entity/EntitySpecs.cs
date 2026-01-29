using System;
using System.Collections.Generic;
using Unity.VisualScripting;

//public class EntitySpecs
//{
//    public List<EntityTag> Tags { get; set; } = new List<EntityTag>();
//    public EntityAttributes Attributes { get; set; } = new EntityAttributes();
//    public EntityStats Stats { get; set; } = new EntityStats();
//}

[Serializable]
public enum EntityTag
{
    None,

    #region Age tags
    Juvenile,
    Youth,
    Adult,
    Elder,
    #endregion

    #region Species tags
    Mammal,
    Bird,
    Reptile,
    Amphibian,
    FishSpecies,
    InsectSpecies,
    #endregion

    #region Food tags
    Food,
    Meat,
    Insect,
    Fish,
    Poultry,
    Milk,
    Egg,
    Bone,

    Grain,
    Fruit,
    Plant,  // leaves and stems
    Root,
    Seed,
    Fungus,
    Water,
    #endregion

    #region Behavior tags
    Aggressive,
    Docile,
    Timid,
    Curious,
    Social,
    Solitary,
    Pack,
    Herd,
    Nocturnal,
    Diurnal,
    Crepuscular,
    Arboreal,
    Terrestrial,
    Aquatic,
    Amphibious,
    #endregion

    #region Food chain tags
    Herbivore,
    Carnivore,
    Omnivore,
    Scavenger,
    ApexPredator,
    Predator,
    Prey,
    #endregion

    #region Size tags
    Tiny,
    Small,
    Medium,
    Large,
    Huge,
    #endregion

    #region Location tags
    Rest,   // generic resting place
    Nest,
    Den,
    Burrow,
    Hive,
    Perch,
    #endregion
}

[Serializable]
public class EntityAttributes
{
    public int Strength;
    public int Agility;
    public int Intelligence;
    public int Charisma;
    public int Endurance;
    public int Perception;
}

[Serializable]
public class EntityStats
{
    public float Health;

    public float Speed;
    public float Power;
    public float Defense;
    public float Age;

    public float HealthRegenRate;
    public float HealthConsumptionRate;

    public int Stamina;             // maximum stamina
    public float StaminaRegenRate;  // rate at which stamina regenerates
    public float StaminaConsumptionRate; // rate at which stamina is consumed

    public int Energy;
    public float EnergyRegenRate;
    public float EnergyConsumptionRate;

    public int Hunger;             // maximum hunger
    public float HungerThreshold;   // the value at which the entity starts feeling hungry
    public float HungerRate;        // rate at which hunger increases

    public int Thirst;             // maximum thirst
    public float ThirstThreshold;   // the value at which the entity starts feeling thirsty
    public float ThirstRate;

    public int Desire;             // maximum desire
    public float DesireThreshold;   // the value at which the entity starts feeling desire
    public float DesireRate;

    public float StressThreshold;  // the value at which stress starts affecting attributes
    public float StressRate;
    public float StressRecoveryRate;
}

[Serializable]
public struct EntityStatus
{
    public float Health;
    public float Stamina;
    public float Energy;
    public float Hunger;
    public float Thirst;
    public float Desire;
    public float Stress;
}

[Serializable]
public enum Species
{
    None,
    Rabbit,
    Fox,
    Bear,
}
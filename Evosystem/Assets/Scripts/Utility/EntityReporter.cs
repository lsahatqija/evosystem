using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

public class EntityReporter : MonoBehaviour
{
    public Dictionary<Species, int> populationCount = new Dictionary<Species, int>();

    private void OnEnable()
    {
        EntityEvents.EntitySpawned += OnEntitySpawned;
        EntityEvents.EntityDestroyed += OnEntityDespawn;
    }

    private void OnDisable()
    {
        EntityEvents.EntitySpawned -= OnEntitySpawned;
        EntityEvents.EntityDestroyed -= OnEntityDespawn;
    }

    void OnEntitySpawned(Entity entity)
    {
        Species entitySpecies = entity.species;
        if (populationCount.ContainsKey(entitySpecies))
            populationCount[entitySpecies]++;
        else
            populationCount[entitySpecies] = 1;
    }

    void OnEntityDespawn(Entity entity)
    {
        Species entitySpecies = entity.species;
        if (populationCount.ContainsKey(entitySpecies))
        {
            populationCount[entitySpecies]--;
            if (populationCount[entitySpecies] == 0)
                populationCount.Remove(entitySpecies);
        }
        else
            Debug.LogWarning($"{entitySpecies.ToString()} has died without ever being recorded as existing");
    }

}

public class EntityEvents
{
    public static event Action<Entity> EntitySpawned = delegate { };
    public static event Action<Entity> EntityDestroyed = delegate { };

    public static void OnEntitySpawned(Entity entity)
    {
        EntitySpawned?.Invoke(entity);
    }

    public static void OnEntityDespawn(Entity entity)
    {
        EntityDestroyed?.Invoke(entity);
    }
}

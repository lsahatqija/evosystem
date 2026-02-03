using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlantGenerator : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;

    public float GenerationPeriod = 1f;
    public float GenerationRadius = 100f;

    Coroutine spawnCoroutine;

    List<GameObject> spawnedObjects = new List<GameObject>();
    public int maxSpawnCount = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnCoroutine = StartCoroutine(PlantSpawn());
    }

    IEnumerator PlantSpawn()
    {
        while (true)
        {
            spawnedObjects.RemoveAll(x => x == null);

            yield return new WaitForSeconds(GenerationPeriod);

            if (spawnedObjects.Count < maxSpawnCount)
            {
                GameObject go = Instantiate(plantPrefab, SpawnPosition(), Quaternion.identity);
                spawnedObjects.Add(go);
                go.transform.parent = transform;
            }
        }
    }

    Vector3 SpawnPosition()
    {
        Vector3 randomDirection = transform.position + Random.insideUnitSphere * GenerationRadius;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, GenerationRadius, -1);

        return  navHit.position;
    }
}

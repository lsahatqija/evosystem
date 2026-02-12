using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class DeathStrategy : IActionStrategy
{
    private GoapAgent agent;

    CountdownTimer timer;

    public bool CanPerform => agent != null && agent.status.Alive;

    public bool Complete { get; private set; }

    public DeathStrategy(GoapAgent agent)
    {
        this.agent = agent;
        timer = new CountdownTimer(1f);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start()
    {
        if (!agent.status.Alive)
            return;
        Debug.Log($"Death has come for {agent.name}");
        agent.animations.Death();
        agent.navMeshAgent.isStopped = true;
        //DropLoot(agent.entity.HasTags);
        timer.Start();
    }

    public void Update(float deltaTime)
    {
        timer.Tick(deltaTime);
    }

    public void Stop()
    {
        agent.animations.Death();
        DropLoot(agent.entity.HasTags);
        GameObject.Destroy(agent.gameObject, 3f);
        agent.status.Alive = false;
        Debug.Log($"The soul of {agent.name} was collected");

        EntityEvents.OnEntityDespawn(agent.entity);
    }

    public void DropLoot(List<EntityTag> agentTags)
    {
        // loot drops
        foreach (EntityTag tag in agentTags)
        {
            //GameObject loot = GameObject.Instantiate(new GameObject(), agent.transform.position + UnityEngine.Random.insideUnitSphere + Vector3.up, agent.transform.rotation);
            GameObject loot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            loot.transform.position = agent.transform.position + UnityEngine.Random.insideUnitSphere + Vector3.up * 1.5f;
            loot.transform.rotation = agent.transform.rotation;

            loot.AddComponent(typeof(Tags));
            loot.TryGetComponent(out Tags tags);
            tags.IsTags.Add(EntityTag.Food);
            tags.IsTags.Add(tag);

            loot.AddComponent(typeof(Consumable));
            loot.TryGetComponent(out Consumable consumable);
            consumable.ConsumableAmount = agent.entity.Stats.Size * 100f;

            loot.AddComponent(typeof(BoxCollider));

            loot.AddComponent(typeof(Rigidbody));
            loot.TryGetComponent(out Rigidbody rb);
            rb.useGravity = true;

            loot.name = $"Consumable - {tag.HumanName()}";
            loot.layer = LayerMask.NameToLayer("Consumable");
        }

    }
}
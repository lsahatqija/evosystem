using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DeathStrategy : IActionStrategy
{
    private GoapAgent agent;
    bool deathComplete;

    readonly CountdownTimer timer;

    public bool CanPerform => agent != null;

    public bool Complete => deathComplete;

    public DeathStrategy(GoapAgent agent)
    {
        this.agent = agent;
        timer = new CountdownTimer(1f);
    }

    public void Start()
    {
        if (deathComplete)
            return;

        agent.animations.Death();
        DropLoot(agent.entity.HasTags);
        GameObject.Destroy(agent.gameObject, 3f);
        deathComplete = true;
    }

    public void DropLoot(List<EntityTag> agentTags)
    {
        // loot drops
        foreach (EntityTag tag in agentTags)
        {
            //GameObject loot = GameObject.Instantiate(new GameObject(), agent.transform.position + UnityEngine.Random.insideUnitSphere + Vector3.up, agent.transform.rotation);
            GameObject loot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            loot.transform.position = agent.transform.position + UnityEngine.Random.insideUnitSphere + Vector3.up;
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
        }

    }
}
using Gaia;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BeliefFactory
{
    readonly GoapAgent agent;
    readonly Dictionary<string, AgentBelief> beliefs = new Dictionary<string, AgentBelief>();

    public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
    {
        this.agent = agent;
        this.beliefs = beliefs;
    }

    public void AddBelief(string key, Func<bool> condition)
    {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(condition)
            .Build());
    }

    public void AddSensorBelief(string key, Sensor sensor)
    {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(() => sensor.IsTargetInRange)
            .WithLocation(() => sensor.TargetPosition)
            .Build());
    }


    public void AddLocationBelief(string key, float distance, Transform location)
    {
        AddLocationBelief(key, distance, location.position);
    }

    public void AddLocationBelief(string key, float distance, Vector3 location)
    {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(() => InRangeOf(location, distance))
            .WithLocation(() => location)
            .Build());
    }

    bool InRangeOf(Vector3 target, float range) => Vector3.Distance(agent.transform.position, target) <= range;


}

public class AgentBelief
{
    public string Name { get; }

    Func<bool> condition = () => false;
    Func<Vector3> observedLocation = () => Vector3.zero;

    public Vector3 Location => observedLocation();

    AgentBelief(string name)
    {
        Name = name;
    }

    public bool Evaluate() => condition();

    public class Builder
    {
        readonly AgentBelief belief;

        public Builder(string name)
        {
            belief = new AgentBelief(name);
        }

        public Builder WithCondition(Func<bool> condition)
        {
            belief.condition = condition;
            return this;
        }

        public Builder WithLocation(Func<Vector3> location)
        {
            belief.observedLocation = location;
            return this;
        }

        public AgentBelief Build() => belief;
    }
}

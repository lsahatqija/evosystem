using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AnimationController))]
public class GoapAgent : MonoBehaviour
{
    [Header("Sensors")]
    [SerializeField] private Sensor[] sensors;
    [SerializeField] Sensor chaseSensor;
    [SerializeField] Sensor attackSensor;

    [Header("KnownLocations")]
    [SerializeField] Transform restPosition;
    [SerializeField] Transform foodPosition;

    NavMeshAgent navMeshAgent;
    AnimationController animations;
    Rigidbody rb;

    [Header("Stats")]
    public float health = 100f;
    public float energy = 100f;

    CountdownTimer statsTimer;

    GameObject target;
    Vector3 destination;

    AgentGoal lastGoal;
    public AgentGoal currentGoal { get; private set; }
    public ActionPlan actionPlan;
    public AgentAction currentAction;

    public Dictionary<string, AgentBelief> beliefs;
    public HashSet<AgentAction> actions;
    public HashSet<AgentGoal> goals;

    IGoapPlanner gPlanner;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animations = GetComponent<AnimationController>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        gPlanner = new GoapPlanner();
    }

    private void Start()
    {
        SetupTimers();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    private void SetupTimers()
    {
        statsTimer = new CountdownTimer(2f);
        statsTimer.OnTimerStop += () =>
        {
            UpdateStats();
            statsTimer.Start();
        };
        statsTimer.Start();
    }

    void SetupBeliefs()
    {
        beliefs = new Dictionary<string, AgentBelief>();
        BeliefFactory factory = new BeliefFactory(this, beliefs);
        factory.AddBelief("Nothing", () => false);
        factory.AddBelief("AgentIdle", () => !navMeshAgent.hasPath);
        factory.AddBelief("AgentMoving", () => navMeshAgent.hasPath);
        factory.AddBelief("LowHealth", () => health <= 30f);
        factory.AddBelief("AgentIsHealthy", () => health >= 80f);
        factory.AddBelief("LowEnergy", () => energy <= 30f);
        factory.AddBelief("AgentIsRested", () => energy >= 80f);

        factory.AddLocationBelief("AgentAtFood", 3f, foodPosition);
        factory.AddLocationBelief("AgentAtRest", 6f, restPosition);
    }

    void SetupActions()
    {
        actions = new HashSet<AgentAction>();

        actions.Add(new AgentAction.Builder("Relax")
            .WithStrategy(new IdleStrategy(5))
            .AddEffect(beliefs["Nothing"])
            .Build());

        actions.Add(new AgentAction.Builder("Wander")
            .WithStrategy(new WanderStrategy(navMeshAgent, 30))
            .AddEffect(beliefs["AgentMoving"])
            .Build());

        actions.Add(new AgentAction.Builder("Eat")
            .WithStrategy(new IdleStrategy(5))
            .AddPrecondition(beliefs["AgentAtFood"])
            .AddEffect(beliefs["AgentIsHealthy"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToFood")
            .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["AgentAtFood"].Location))
            .AddPrecondition(beliefs["LowHealth"])
            .AddEffect(beliefs["AgentAtFood"])
            .Build());

        actions.Add(new AgentAction.Builder("Rest")
            .WithStrategy(new IdleStrategy(15))
            .AddPrecondition(beliefs["AgentAtRest"])
            .AddEffect(beliefs["AgentIsRested"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToRest")
            .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["AgentAtRest"].Location))
            .AddPrecondition(beliefs["LowEnergy"])
            .AddEffect(beliefs["AgentAtRest"])
            .Build());
    }

    void SetupGoals()
    {
        goals = new HashSet<AgentGoal>();

        goals.Add(new AgentGoal.Builder("Chill")
            .WithPriority(0)
            .AddDesiredState(beliefs["Nothing"])
            .Build());

        goals.Add(new AgentGoal.Builder("Explore")
            .WithPriority(1)
            .AddDesiredState(beliefs["AgentMoving"])
            .Build());

        goals.Add(new AgentGoal.Builder("KeepEnergyUp")
            .WithPriority(2)
            .AddDesiredState(beliefs["AgentIsRested"])
            .Build());

        goals.Add(new AgentGoal.Builder("KeepHealthUp")
            .WithPriority(3)
            .AddDesiredState(beliefs["AgentIsHealthy"])
            .Build());

    }

    // set up a real stats system
    void UpdateStats()
    {
        // this is very rudimentary stat management for demo purposes
        energy += InRangeof(restPosition.position, 5f) ? 5f : -1.5f;
        health += InRangeof(foodPosition.position, 5f) ? 10f : -1f;
        energy = Mathf.Clamp(energy, 0f, 100f);
        health = Mathf.Clamp(health, 0f, 100f);
    }

    bool InRangeof(Vector3 pos, float range) =>  Vector3.Distance(transform.position, pos) <= range;

    private void OnEnable() => chaseSensor.OnTargetChanged += HandleTargetChanged;
    private void OnDisable() => chaseSensor.OnTargetChanged -= HandleTargetChanged;

    private void HandleTargetChanged()
    {
        currentAction = null;
        currentGoal = null;
    }

    private void Update()
    {
        statsTimer.Tick(Time.deltaTime);
        animations.SetSpeed(navMeshAgent.velocity.magnitude);

        if (currentAction == null)
        {
            CalculatePlan();

            if (actionPlan != null && actionPlan.Actions.Count > 0)
            {
                currentGoal = actionPlan.AgentGoal;
                currentAction = actionPlan.Actions.Pop();
                currentAction.Start();
            }
        }

        if (actionPlan != null && currentAction != null)
        {
            currentAction.Update(Time.deltaTime);
            if (currentAction.Complete)
            {
                currentAction.Stop();
                currentAction = null;
                if (actionPlan.Actions.Count == 0)
                {
                    lastGoal = currentGoal;
                    currentGoal = null;
                }
            }
        }
    }

    void CalculatePlan()
    {
        var priorityLevel = currentGoal?.Priority ?? 0;

        HashSet<AgentGoal> goalsToCheck = goals;

        if (currentGoal != null)
        {
            goalsToCheck = new HashSet<AgentGoal>(goals.Where(g => g.Priority >= priorityLevel));
        }

        var potentialPlan = gPlanner.CreatePlan(this, goalsToCheck, lastGoal);
        if (potentialPlan != null)
        {
            actionPlan = potentialPlan;
        }
    }
}

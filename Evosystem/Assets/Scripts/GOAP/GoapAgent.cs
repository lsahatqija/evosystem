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

    public NavMeshAgent navMeshAgent;
    public AnimationController animations;
    public Rigidbody rb;

    [Header("Stats")]
    public Entity entity;
    public EntityStatus status;

    CountdownTimer statsTimer;

    public GameObject target;
    public GameObject dangerSource;
    Vector3 destination;
    public Transform restPositionCurrent;
    public Transform foodPositionCurrent;

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
        SetupEntity();
        SetupTimers();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    private void SetupEntity()
    {
        // create a copy of the entity scriptable object
        entity = Instantiate(entity);

        // initialize stats with attributes
        entity.Stats = entity.InitializeStats(entity.Attributes);

        // initialize status with updated stats
        status = EntityUtils.InitializeEntityStatus(entity.Stats);
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
        beliefs = entity.InitializeBeliefs(this);
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
            .AddPrecondition(beliefs["AgentNearFood"])
            .AddEffect(beliefs["AgentIsHealthy"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToFood")
            .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["FoodLocationKnown"].Location))
            .AddPrecondition(beliefs["LowHealth"])
            .AddPrecondition(beliefs["FoodLocationKnown"])
            .AddEffect(beliefs["AgentNearFood"])
            .Build());

        actions.Add(new AgentAction.Builder("Rest")
            .WithStrategy(new IdleStrategy(15))
            .AddPrecondition(beliefs["AgentNearRestSpot"])
            .AddEffect(beliefs["AgentIsRested"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToRest")
            .WithStrategy(new MoveStrategy(navMeshAgent, () => beliefs["RestLocationKnown"].Location))
            .AddPrecondition(beliefs["LowEnergy"])
            .AddPrecondition(beliefs["RestLocationKnown"])
            .AddEffect(beliefs["AgentNearRestSpot"])
            .Build());
    }

    void SetupGoals()
    {
        goals = entity.InitializeGoals(this, beliefs);

        //goals = new HashSet<AgentGoal>();

        //goals.Add(new AgentGoal.Builder("Chill")
        //    .WithPriority(0)
        //    .AddDesiredState(beliefs["Nothing"])
        //    .Build());

        //goals.Add(new AgentGoal.Builder("Explore")
        //    .WithPriority(1)
        //    .AddDesiredState(beliefs["AgentMoving"])
        //    .Build());

        //goals.Add(new AgentGoal.Builder("KeepEnergyUp")
        //    .WithPriority(2)
        //    .AddDesiredState(beliefs["AgentIsRested"])
        //    .Build());

        //goals.Add(new AgentGoal.Builder("KeepHealthUp")
        //    .WithPriority(3)
        //    .AddDesiredState(beliefs["AgentIsHealthy"])
        //    .Build());

    }

    // set up a real stats system
    void UpdateStats()
    {
        status = EntityUtils.ProcessEntityStatus(entity.Stats, status);
    }

    public bool InRangeof(Vector3 pos, float range) =>  Vector3.Distance(transform.position, pos) <= range;

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

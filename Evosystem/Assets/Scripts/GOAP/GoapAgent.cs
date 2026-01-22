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
        actions = entity.InitializeActions(this, beliefs);
    }

    void SetupGoals()
    {
        goals = entity.InitializeGoals(this, beliefs);
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

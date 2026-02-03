using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(Tags))]
public class GoapAgent : MonoBehaviour
{
    [Header("Sensors")]
    [SerializeField] private Sensor[] sensors;

    public NavMeshAgent navMeshAgent;
    public AnimationController animations;
    public Rigidbody rb;

    [Header("Stats")]
    public Entity entity;
    public EntityStatus status;

    CountdownTimer statsTimer;

    public GameObject target;
    Vector3 destination;
    public Transform restPositionCurrent;
    public List<Transform> restPositionsKnown;
    public Transform foodPositionCurrent;
    public List<Transform> foodPositionsKnown;
    public Transform drinkPositionCurrent;
    public List<Transform> drinkPositionsKnown;
    public Transform dangerSource;
    public GoapAgent potentialMate;

    AgentGoal lastGoal;
    public AgentGoal currentGoal { get; private set; }
    public ActionPlan actionPlan;
    public AgentAction currentAction;

    public Dictionary<string, AgentBelief> beliefs;
    BeliefFactory beliefFactory;
    public HashSet<AgentAction> actions;
    public HashSet<AgentGoal> goals;

    IGoapPlanner gPlanner;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //animations = GetComponent<AnimationController>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        gPlanner = new GoapPlanner();
    }

    private void Start()
    {
        InitializeAgent();
    }

    public void InitializeAgent()
    {
        if (entity == null)
            return;
        SetupEntity();
        SetupTimers();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
        SetupTags();
        SetupModel();
    }

    private void SetupModel()
    {
        if (entity.entityModel == null)
        {
            Debug.LogWarning($"Entity model of species {entity.species} does not exist!");
            return;
        }

        GameObject model = Instantiate(entity.entityModel, transform);
        model.TryGetComponent(out Animator modelAnimator);

        TryGetComponent(out animations);
        if (animations != null && modelAnimator != null)
            animations.SetAnimator(modelAnimator);

        gameObject.name = $"{entity.species.ToString()} - {entity.UUID}";
    }

    private void SetupEntity()
    {
        // create a copy of the entity scriptable object
        entity = Instantiate(entity);

        // initialize stats with attributes
        entity.Stats = entity.InitializeStats(entity.Stats, entity.Attributes);

        // initialize status with updated stats
        status = EntityUtils.InitializeEntityStatus(entity.Stats);

        navMeshAgent.speed = entity.Stats.Speed;
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
        beliefs = entity.InitializeBeliefs(this, beliefFactory);
    }

    void SetupActions()
    {
        actions = entity.InitializeActions(this, beliefs);
    }

    void SetupGoals()
    {
        goals = entity.InitializeGoals(this, beliefs);
    }

    void SetupTags()
    {
        TryGetComponent<Tags>(out var tagsComponent);

        if (tagsComponent == null)
        {
            gameObject.AddComponent<Tags>();
        }

        tagsComponent.SetTags(entity.IsTags, entity.HasTags, entity.WantsTags, entity.AvoidTags);

        foreach (Sensor sensor in sensors)
        {
            sensor.InitializeTags(entity.WantsTags, entity.AvoidTags);
        }
    }

    // set up a real stats system
    void UpdateStats()
    {
        status = EntityUtils.ProcessEntityStatus(entity.Stats, status);
    }

    public bool InRangeof(Vector3 pos, float range) => Vector3.Distance(transform.position, pos) <= range;

    private void OnEnable()
    {
        foreach (Sensor sensor in sensors)
        {
            sensor.OnTargetChanged += HandleTargetChanged;
            sensor.OnTargetDetected += TargetDetected;
            sensor.OnDangerDetected += DangerDetected;
        }
    }

    private void OnDisable()
    {
        foreach (Sensor sensor in sensors)
        {
            sensor.OnTargetChanged -= HandleTargetChanged;
            sensor.OnTargetDetected -= TargetDetected;
            sensor.OnDangerDetected -= DangerDetected;
        }
    }

    private void HandleTargetChanged()
    {
        currentAction = null;
        currentGoal = null;
        navMeshAgent.destination = transform.position;
    }

    private void TargetDetected(Tags target)
    {
        BeliefFactory beliefFactory = new BeliefFactory(this, beliefs);

        if (target.Is(EntityTag.Rest))
        {
            if (!restPositionsKnown.Contains(target.transform))
                restPositionsKnown.Add(target.transform);
        }

        if (target.Is(EntityTag.Food))
        {
            if (!foodPositionsKnown.Contains(target.transform))
                foodPositionsKnown.Add(target.transform);
        }

        if (target.Is(EntityTag.Water))
        {
            if (!drinkPositionsKnown.Contains(target.transform))
                drinkPositionsKnown.Add(target.transform);
        }

        target.TryGetComponent(out GoapAgent targetAgent);
        if (targetAgent != null && targetAgent.entity.species == entity.species && entity.IsMale != targetAgent.entity.IsMale)
        {
            potentialMate = targetAgent;
        }

        currentAction = null;
        currentGoal = null;
    }

    private void DangerDetected(GameObject danger)
    {
        dangerSource = danger.transform;
        currentAction = null;
        currentGoal = null;
    }

    private void Update()
    {
        if (entity == null)
            { return; }

        statsTimer.Tick(Time.deltaTime);
        if (animations != null)
            animations.SetSpeed(navMeshAgent.velocity.magnitude / entity.Stats.Speed);

        if (currentAction == null)
        {
            OrderPossibleDestinations();
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

    void OrderPossibleDestinations()
    {
        // order rest spots
        if (restPositionsKnown.Count > 0)
        {
            restPositionsKnown.RemoveAll(t => t.gameObject == null);
            restPositionsKnown.OrderBy(t => Vector3.Distance(t.position, transform.position));
            restPositionCurrent = restPositionsKnown[0];
        }

        // order food spots

        if (foodPositionsKnown.Count > 0)
        {
            foodPositionsKnown.RemoveAll(t => t.gameObject == null);
            foodPositionsKnown.OrderBy(t => Vector3.Distance(t.position, transform.position));
            foodPositionCurrent = foodPositionsKnown[0];
        }

        // order food spots
        if (drinkPositionsKnown.Count > 0)
        {
            drinkPositionsKnown.RemoveAll(t => t.gameObject == null);
            drinkPositionsKnown.OrderBy(t => Vector3.Distance(t.position, transform.position));
            drinkPositionCurrent = drinkPositionsKnown[0];
        }
    }

}

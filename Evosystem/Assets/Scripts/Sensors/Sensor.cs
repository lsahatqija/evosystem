using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Sensor: MonoBehaviour
{
    [SerializeField] protected float SensingDistance = 5f;
    [SerializeField] protected float timerInterval = 1f;
    private SphereCollider detectionRange;

    public event Action OnTargetChanged = delegate { };

    public event Action<Tags> OnTargetDetected = delegate { };
    public event Action<GameObject> OnDangerDetected = delegate { };

    public Vector3 TargetPosition => target ? target.transform.position : Vector3.zero;
    public bool IsTargetInRange => TargetPosition != Vector3.zero;

    GameObject target;
    Vector3 lastKnownPosition;
    CountdownTimer timer;

    protected List<EntityTag> targetTags;
    protected List<EntityTag> dangerTags;

    void Awake()
    {
        detectionRange = GetComponent<SphereCollider>();
        detectionRange.isTrigger = true;
        detectionRange.radius = SensingDistance;

        targetTags = new List<EntityTag>();
        dangerTags = new List<EntityTag>();
    }

    private void Start()
    {
        timer = new CountdownTimer(timerInterval);
        timer.OnTimerStop += () =>
        {
            UpdateTargetPosition(target);
            DetectionTick();
            timer.Start();
        };
        timer.Start();
    }

    protected virtual void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    protected virtual void DetectionTick()
    {

    }

    void UpdateTargetPosition(GameObject target = null)
    {
        this.target = target;
        if (IsTargetInRange && (lastKnownPosition != TargetPosition || lastKnownPosition != Vector3.zero))
        {
            lastKnownPosition = TargetPosition;
            OnTargetChanged.Invoke();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Add logic for when an object enters the sensing radius
        if (!other.CompareTag("Targetable")) return;
        UpdateTargetPosition(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        // Add logic for when an object exits the sensing radius
        UpdateTargetPosition();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsTargetInRange ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, SensingDistance);
    }

    public void InitializeTags(List<EntityTag> targetTags, List<EntityTag> dangerTags)
    {
        foreach (EntityTag tag in targetTags)
            this.targetTags.Add(tag);
        foreach (EntityTag tag in dangerTags)
            this.dangerTags.Add(tag);
    }

    protected void ObjectDetected(Tags tags)
    {
        foreach (var tag in dangerTags)
        {
            if (tags.Is(tag))
            {
                OnDangerDetected.Invoke(tags.gameObject);
                return;
            }
        }

        foreach (var tag in targetTags)
        {
            if (tags.Is(tag) || tags.Has(tag))
            {
                OnTargetDetected.Invoke(tags);
                return;
            }
        }
    }
}
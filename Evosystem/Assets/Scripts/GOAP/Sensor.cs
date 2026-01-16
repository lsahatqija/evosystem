using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Sensor: MonoBehaviour
{
    [SerializeField] float SensingRadius = 5f;
    [SerializeField] float timerInterval = 1f;
    private SphereCollider detectionRange;

    public event Action OnTargetChanged = delegate { };

    public Vector3 TargetPosition => target ? target.transform.position : Vector3.zero;
    public bool IsTargetInRange => TargetPosition != Vector3.zero;

    GameObject target;
    Vector3 lastKnownPosition;
    CountdownTimer timer;

    void Awake()
    {
        detectionRange = GetComponent<SphereCollider>();
        detectionRange.isTrigger = true;
        detectionRange.radius = SensingRadius;
    }

    private void Start()
    {
        timer = new CountdownTimer(timerInterval);
        timer.OnTimerStop += () =>
        {
            UpdateTargetPosition(target);
            timer.Start();
        };
        timer.Start();
    }

    private void Update()
    {
        timer.Tick(Time.deltaTime);
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
        Gizmos.DrawWireSphere(transform.position, SensingRadius);
    }
}
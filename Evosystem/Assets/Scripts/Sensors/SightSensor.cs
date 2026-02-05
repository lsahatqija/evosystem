using UnityEngine;

public class SightSensor : Sensor
{
    public float sightAngle = 60f;
    public float verticalSightAngle = 30f;
    float angleStep = 3f;

    public bool drawDebug = false;

    protected override void DetectionTick()
    {
        base.DetectionTick();
        // Additional sight-specific logic can be added here
        LineOfSight();
    }

    void LineOfSight()
    {
        if (dangerTags.Count == 0 && targetTags.Count == 0)
        {
            Debug.LogWarning("No tags on this sensor");
            return;
        }

        int steps = (int)(sightAngle / angleStep);
        int verticalSteps = (int)(verticalSightAngle / angleStep);
        for (int i = (int)(-steps * 0.5f); i < (int)(steps * 0.5f) + 1; i++)
        {
            for (int j = (int)(-verticalSteps * 0.5f); j < (int)(verticalSteps * 0.5f) + 1; j++)
            {
                Vector3 lookDir = Quaternion.Euler(-angleStep * j, angleStep * i, 0) * transform.forward;
                Physics.Raycast(transform.position, lookDir, out RaycastHit hit, SensingDistance);

                if (hit.collider != null)
                {
                    // Check if the hit object has desired tags
                    var hitObject = hit.collider.gameObject;
                    hitObject.TryGetComponent(out Tags tagsComponent);
                    if (tagsComponent != null)
                    {
                        foreach (var tag in dangerTags)
                        {
                            if (tagsComponent.Is(tag)) ObjectDetected(tagsComponent);
                        }
                        foreach (var tag in targetTags)
                        {
                            if (tagsComponent.Is(tag) || tagsComponent.Has(tag)) ObjectDetected(tagsComponent);
                        }
                    }                    
                }
            }            
        }
    }    

    private void OnDrawGizmos()
    {
        if (!drawDebug)
            return;

        Gizmos.color = Color.orange;

        int steps = (int)(sightAngle / angleStep);
        int verticalSteps = (int)(verticalSightAngle / angleStep);
        for (int i = (int)(-steps * 0.5f); i < (int)(steps * 0.5f) + 1; i++)
        {
            for (int j = (int)(-verticalSteps * 0.5f); j < (int)(verticalSteps * 0.5f) + 1; j++)
            {
                Vector3 lookDir = Quaternion.Euler(-angleStep * j, angleStep * i, 0) * transform.forward;
                Gizmos.DrawLine(transform.position, transform.position + lookDir * SensingDistance);
            }
        }
    }
}

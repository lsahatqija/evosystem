using UnityEngine;

public class Consumable : MonoBehaviour
{
    public float ConsumableAmount = 100f;
    public bool Replenishing = false;

    public Consumable(float amount)
    {
        ConsumableAmount = amount;
    }

    public float Consume(float biteSize)
    {
        float biteTaken = biteSize;

        if (!Replenishing)
            ConsumableAmount -= biteTaken;

        if (ConsumableAmount < 0f)
        {
            biteTaken = Mathf.Abs(ConsumableAmount);
            Destroy(this.gameObject, Time.deltaTime * 2f);
        }

        return biteTaken;
    }
}
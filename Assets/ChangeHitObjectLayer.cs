using UnityEngine;
using System.Collections;

public class ChangeHitObjectLayer : MonoBehaviour
{
    public int LayerOnEnter; // Default
    public int LayerOnExit;  // HitObject

    public float suckForce = 10f;   // Pull objects toward the hole
    public float torqueForce = 5f;  // Strength of flipping torque

    public float maxAllowedSize = 3f;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EatableItem>(out var eatable))
        {
            // Only process if the item's size is within the hole's allowed limit.
            float max = transform.localScale.x * maxAllowedSize;
            if (eatable.Size <= max)
            {
                if (other.TryGetComponent(out Rigidbody rb))
                {
                    rb.isKinematic = false;
                }

                other.gameObject.layer = LayerOnEnter;
            }
            else
            {
                
                Debug.Log("Item too big to be eaten! Size: " + eatable.Size + " Max: " + max);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<EatableItem>(out var eatable))
        {
            float max = transform.localScale.x * maxAllowedSize;
            if (eatable.Size <= max)
            {
                if (other.TryGetComponent(out Rigidbody rb))
                {
                    // Pull object toward hole center.
                    Vector3 toCenter = transform.position - other.transform.position;
                    rb.AddForce(toCenter.normalized * suckForce, ForceMode.Force);

                    // Apply torque to flip the object's up axis inward.
                    Vector3 currentUp = rb.transform.up;
                    Vector3 desiredUp = -toCenter.normalized; // Flip inward.
                    float angle = Vector3.Angle(currentUp, desiredUp);
                    if (angle > 1f)
                    {
                        Vector3 rotationAxis = Vector3.Cross(currentUp, desiredUp).normalized;
                        float angleRad = angle * Mathf.Deg2Rad;
                        rb.AddTorque(rotationAxis * torqueForce * angleRad, ForceMode.Force);
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<EatableItem>(out var eatable))
        {
            float max = transform.localScale.x * maxAllowedSize;
            if (eatable.Size <= max)
            {
                other.gameObject.layer = LayerOnExit;
            }
        }
    }
}

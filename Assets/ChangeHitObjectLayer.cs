using UnityEngine;
using System.Collections;

public class ChangeHitObjectLayer : MonoBehaviour
{
    public int LayerOnEnter; // Default
    public int LayerOnExit; // HitObject

    void OnTriggerEnter(Collider other)
    {
        
            if (other.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }

            other.gameObject.layer = LayerOnEnter;
        
    }

    void OnTriggerExit(Collider other)
    {
          
            other.gameObject.layer = LayerOnExit;
        
    }
}
using UnityEngine;
using System.Collections;

public class ChangeHitObjectLayer : MonoBehaviour
{
    public int LayerOnEnter; // Default
    public int LayerOnExit; // HitObject

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }

            other.gameObject.layer = LayerOnEnter;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {   
            other.gameObject.layer = LayerOnExit;
        }
    }
}
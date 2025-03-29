// EatableItem.cs
using UnityEngine;

public class EatableItem : MonoBehaviour, IEatable
{
    [SerializeField] private int score = 1;
    public int Score => score;

    public void Eat()
    {
        Destroy(gameObject);
    }

    void Awake()
    {
        //If you have no rigidbody, add one
        if (!TryGetComponent(out Rigidbody rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            
        }
        
        //If you have no collider, add one
        if (!TryGetComponent(out MeshCollider col))
        {
            col = gameObject.AddComponent<MeshCollider>();
            col.convex = true;
        }
        
        
    }
}
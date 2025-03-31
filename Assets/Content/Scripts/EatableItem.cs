using UnityEngine;

public class EatableItem : MonoBehaviour, IEatable
{
    [SerializeField] private int score = 1;
    public int Score => score;

    private float size = 0;
    public float Size => size; 

    public void Eat()
    {
        Destroy(gameObject);
    }

    void Awake()
    {
        if (!TryGetComponent(out Rigidbody rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        
        if (!TryGetComponent(out MeshCollider col))
        {
            col = gameObject.AddComponent<MeshCollider>();
            col.convex = true;
            Vector3 s = col.bounds.size;
            size = Mathf.Max(s.x, s.z);
        }
    }
}
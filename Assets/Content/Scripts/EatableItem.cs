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
}
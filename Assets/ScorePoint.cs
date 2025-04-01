// ScorePoint.cs
using UnityEngine;
using System;

public class ScorePoint : MonoBehaviour
{
    public event Action<int, Transform> OnItemEaten;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IEatable>(out IEatable eatable))
        {
            OnItemEaten?.Invoke(eatable.Score, other.transform);
            eatable.Eat();
        }
    }
}
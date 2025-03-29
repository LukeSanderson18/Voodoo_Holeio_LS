// ScorePoint.cs
using UnityEngine;
using System;

public class ScorePoint : MonoBehaviour
{
    public event Action<int> OnItemEaten;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IEatable>(out IEatable eatable))
        {
            OnItemEaten?.Invoke(eatable.Score);
            StarUIManager.Instance.SpawnStar(other.transform.position);
            eatable.Eat();
        }
    }
}
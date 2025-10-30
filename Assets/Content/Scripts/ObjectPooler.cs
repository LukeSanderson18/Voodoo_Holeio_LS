using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public int amountToPool;
    public GameObject objectToPool;
    public bool shouldExpand;
}

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler Instance;
    public List<ObjectPoolItem> itemsToPool;

    public List<GameObject> pooledObjects;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                Create(item);
            }
        }
    }

    //retuns next good object
    public GameObject GetPooledObject(string n)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].name == n)
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.name == n)
            {
                if (item.shouldExpand)
                {
                    return Create(item);
                }
            }
        }
        return null;
    }

    GameObject Create(ObjectPoolItem item)
    {
        GameObject obj = Instantiate(item.objectToPool);
        obj.name = item.objectToPool.name;
        obj.SetActive(false);
        pooledObjects.Add(obj);
        obj.transform.parent = transform;
        return obj;
    }
}
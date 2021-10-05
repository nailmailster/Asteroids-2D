using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolItem
{
    public GameObject prefab;
    public int amount;
}

public class Pool : MonoBehaviour
{
    public static Pool singleton;

    public List<PoolItem> items;
    public List<GameObject> pooledItems;

    [SerializeField] GameObject parent;

    public int ActiveBulletsAmount
    {
        get
        {
            int count = 0;

            foreach (GameObject prefab in pooledItems)
                if (prefab.CompareTag("Bullet") && prefab.activeInHierarchy)
                    count++;

            return count;
        }
    }

    private void Awake()
    {
        singleton = this;
    }

    public GameObject Get(string tag)
    {
        for (int i = 0; i < pooledItems.Count; i++)
        {
            if (!pooledItems[i].activeInHierarchy && pooledItems[i].CompareTag(tag))
            {
                return pooledItems[i];
            }
        }
        return null;
    }

    void Start()
    {
        pooledItems = new List<GameObject>();
        foreach(PoolItem item in items)
        {
            for (int i = 0; i < item.amount; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(parent.transform);
                pooledItems.Add(obj);
            }
        }
    }
}

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

    Queue<Vector2> itemsVelocity;

    [SerializeField] GameObject parent;

    private void Awake()
    {
        singleton = this;
    }

    public GameObject Get(string tag)
    {
        for (int i = 0; i < pooledItems.Count; i++)
            if (!pooledItems[i].activeInHierarchy && pooledItems[i].CompareTag(tag))
                return pooledItems[i];

        return null;
    }

    public int ActiveObjectsCount(string tag)
    {
        int count = 0;

        foreach (GameObject obj in pooledItems)
            if (obj.activeInHierarchy && obj.CompareTag(tag))
                count++;
        
        return count;
    }

    public void DeactivateActiveObjects(string tag)
    {
        foreach (GameObject obj in pooledItems)
            if (obj.activeInHierarchy && obj.CompareTag(tag))
                obj.SetActive(false);
    }

    public void DeactivateAllActiveObjects()
    {
        foreach (GameObject obj in pooledItems)
            if (obj.activeInHierarchy)
                obj.SetActive(false);
    }

    public void FreezeActiveObjects()
    {
        itemsVelocity = new Queue<Vector2>();

        foreach (GameObject obj in pooledItems)
            if (obj.activeInHierarchy)
            {
                itemsVelocity.Enqueue(obj.GetComponent<Rigidbody2D>().velocity);
                obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
    }

    public void UnfreezeActiveObjects()
    {
        foreach (GameObject obj in pooledItems)
            if (obj.activeInHierarchy)
                obj.GetComponent<Rigidbody2D>().velocity = itemsVelocity.Dequeue();
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

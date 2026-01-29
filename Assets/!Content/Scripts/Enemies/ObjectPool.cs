using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    public void Configure(GameObject prefabToUse, int size)
    {
        prefab = prefabToUse;
        initialSize = size;
        Prewarm();
    }

    private void Awake()
    {
        if (prefab != null)
        {
            Prewarm();
        }
    }

    private void Prewarm()
    {
        if (prefab == null)
        {
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (prefab == null)
        {
            return null;
        }

        if (pool.Count == 0)
        {
            GameObject created = Instantiate(prefab, transform);
            created.SetActive(false);
            pool.Enqueue(created);
        }

        GameObject instance = pool.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    public void Release(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolConfig
{
    public GameObject prefab;
    public int initialPoolSize = 10;
    public int maxPoolSize = 50;
    public bool expandable = true;
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [Header("Pool Configurations")]
    [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();

    private Dictionary<GameObject, ObjectPool> pools = new Dictionary<GameObject, ObjectPool>();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();
    private Transform poolParent;

    private new void Awake()
    {
        base.Awake();

        poolParent = new GameObject("ObjectPools").transform;
        poolParent.SetParent(transform);

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (PoolConfig config in poolConfigs)
        {
            if (config.prefab == null)
            {
                Debug.LogWarning("ObjectPoolManager: Pool config has null prefab, skipping.", this);
                continue;
            }

            CreatePool(config);
        }
    }

    private void CreatePool(PoolConfig config)
    {
        GameObject poolObject = new GameObject($"Pool_{config.prefab.name}");
        poolObject.transform.SetParent(poolParent);

        ObjectPool pool = poolObject.AddComponent<ObjectPool>();
        pool.Initialize(config.prefab, config.initialPoolSize, config.maxPoolSize, config.expandable);

        pools[config.prefab] = pool;
    }

    public GameObject Get(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("ObjectPoolManager: Prefab is null.", this);
            return null;
        }

        if (!pools.TryGetValue(prefab, out ObjectPool pool))
        {
            Debug.LogWarning($"ObjectPoolManager: Pool for prefab '{prefab.name}' not found. Creating default pool.", this);
            pool = CreateDefaultPool(prefab);
        }

        GameObject instance = pool.Get();
        if (instance != null)
        {
            instanceToPrefab[instance] = prefab;
        }

        return instance;
    }

    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (!instanceToPrefab.TryGetValue(obj, out GameObject prefab))
        {
            Debug.LogWarning("ObjectPoolManager: Cannot determine prefab from instance. Destroying object.", this);
            Destroy(obj);
            return;
        }

        if (pools.TryGetValue(prefab, out ObjectPool pool))
        {
            pool.Return(obj);
            instanceToPrefab.Remove(obj);
        }
        else
        {
            Debug.LogWarning($"ObjectPoolManager: Pool for prefab '{prefab.name}' not found. Destroying object.", this);
            instanceToPrefab.Remove(obj);
            Destroy(obj);
        }
    }

    public void ReturnAll(GameObject prefab)
    {
        if (prefab == null)
        {
            return;
        }

        if (pools.TryGetValue(prefab, out ObjectPool pool))
        {
            pool.ReturnAll();
        }
    }

    public void ReturnAll()
    {
        foreach (ObjectPool pool in pools.Values)
        {
            pool.ReturnAll();
        }
    }

    public void Clear()
    {
        foreach (ObjectPool pool in pools.Values)
        {
            pool.Clear();
        }

        instanceToPrefab.Clear();
    }

    public bool HasPool(GameObject prefab)
    {
        return prefab != null && pools.ContainsKey(prefab);
    }

    public int GetPooledCount(GameObject prefab)
    {
        if (prefab != null && pools.TryGetValue(prefab, out ObjectPool pool))
        {
            return pool.PooledCount;
        }
        return 0;
    }

    public int GetActiveCount(GameObject prefab)
    {
        if (prefab != null && pools.TryGetValue(prefab, out ObjectPool pool))
        {
            return pool.ActiveCount;
        }
        return 0;
    }

    private ObjectPool CreateDefaultPool(GameObject prefab)
    {
        PoolConfig defaultConfig = new PoolConfig
        {
            prefab = prefab,
            initialPoolSize = 10,
            maxPoolSize = 50,
            expandable = true
        };

        CreatePool(defaultConfig);
        return pools[prefab];
    }


    private new void OnDestroy()
    {
        Clear();

        base.OnDestroy();
    }
}


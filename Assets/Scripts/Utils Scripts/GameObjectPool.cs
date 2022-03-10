using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    public GameObject ObjToPool;
    // This value NEEDS to be higher than the amount of objects you'll ever take from this pool or you'll get some weird behaviours
    public int PoolCount;

    private List<GameObject> _pool;
    private int _poolIndex = 0;
    public void Start()
    {
        // Initialise all objects in the pool
        _pool = new List<GameObject>();
        for (int i = 0; i < PoolCount; i++)
            _pool.Add(Instantiate(ObjToPool, transform));
        // Hide all objects in the pool
        _pool.ForEach(_ => _.SetActive(false));
    }

    public GameObject GetObjectFromPool()
        => _pool[_poolIndex++ % PoolCount];

    public void ReturnToPool(GameObject go)
    {
        if (!_pool.Contains(go))
            Debug.LogError("ERROR! Tried to return an object to a GameObjectPool that wasn't originally part of it!");

        go.transform.SetParent(transform, false);
        go.SetActive(false);
    }
}

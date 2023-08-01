using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkedObjectPool : IObjectPool
{
    List<GameObject> _pool;
    public GameObject _objectToPool = null;
    public NetworkedObjectPool(GameObject objectToPool, int startSize = 5)
    {
        _objectToPool = objectToPool;
        _pool = new List<GameObject>(startSize);
    }

    [Server]
    public void FillPool()
    {
        if (!NetworkServer.active)
        {
            return;
        }

        for (int i = 0; i < _pool.Capacity; i++)
        {
            GameObject obj = GameObject.Instantiate(_objectToPool);
            _pool.Add(obj);
            NetworkedObjectSpawner.Instance.CreateObject(obj, Vector3.zero);
            obj.GetComponent<PoolableObject>().SetPool(this);
        }
    }

    public void ExpandPool(int addition)
    {
        List<GameObject> newObjects = new List<GameObject>(_pool.Capacity + addition);
        for (int i = 0; i < addition; i++)
        {
            GameObject obj = GameObject.Instantiate(_objectToPool);
            newObjects.Add(obj);
            NetworkedObjectSpawner.Instance.CreateObject(obj, Vector3.zero);
            obj.GetComponent<PoolableObject>().SetPool(this);
        }
        newObjects.AddRange(_pool);
        _pool = newObjects;
    }

    public GameObject GetObject()
    {
        if (_pool.Count == 0)
        {
            ExpandPool(10);
        }
        GameObject pulled = _pool[0];
        _pool.Remove(pulled);

        return pulled;
    }

    public void ReturnObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            Debug.LogWarning("Returning null to pool!");
            return;
        }

        if (_pool.Contains(gameObject))
        {
            Debug.LogWarning("Returning already pooled object to pool!");
            return;
        }

        _pool.Add(gameObject);
        gameObject.SetActive(false);
    }
}

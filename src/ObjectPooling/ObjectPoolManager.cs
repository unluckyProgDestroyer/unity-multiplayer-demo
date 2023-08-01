using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : NetworkBehaviour
{
    public static ObjectPoolManager Instance { get { return instance; } }
    static ObjectPoolManager instance;
    static Dictionary<string, IObjectPool> _objectPools = new Dictionary<string, IObjectPool>();
    static bool _initialised = false;
    bool loaded = false;
    private void Awake()
    {
        instance = this;
        NetworkManager.singleton.spawnPrefabs.ForEach(prefab => {
            NetworkedObjectPool networkedObjectPool = new NetworkedObjectPool(prefab);
            _objectPools.Add(prefab.name, networkedObjectPool);
        });
    }

    [ContextMenu("FILL")]
    private void FIll()
    {
        GetObjectPool("HE").FillPool();
        GetObjectPool("762 bullet").FillPool();
    }

    private void Update()
    {
        if (_initialised) return;

        if (!NetworkServer.active) return;
        
        bool allReady = true;
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (!conn.isReady)
            {
                allReady = false;
            }
        }

        if (allReady)
        {
            _initialised = true;
            GetObjectPool("HE").FillPool();
            GetObjectPool("762 bullet").FillPool();
        }
        
    }

    public IObjectPool GetObjectPool(string prefabName)
    {
        return _objectPools[prefabName];
    }
}

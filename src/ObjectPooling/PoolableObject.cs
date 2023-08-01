using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolableObject : NetworkBehaviour
{
    [SerializeField]
    IObjectPool _pool;

    private void Awake()
    {
        if (gameObject.name.EndsWith("(Clone)"))
        {
            gameObject.name = gameObject.name.Substring(0,gameObject.name.Length-7);
        }

        if (!NetworkServer.active)
        {
            gameObject.SetActive(false);
        }
    }

    [Server] public bool ReturnSelf()
    {
        if (_pool == null) return false;
        _pool.ReturnObject(gameObject);
        gameObject.SetActive(false);
        DisableRcp();

        return true;
    }

    [Server] public void SetPool(IObjectPool pool)
    {
        _pool = pool;
        gameObject.SetActive(false);
        DisableRcp();
    }

    [ClientRpc] void DisableRcp()
    {
        gameObject.SetActive(false);
    }
}

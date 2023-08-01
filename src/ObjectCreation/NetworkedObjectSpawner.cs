using Mirror;
using Mirror.Examples.NetworkRoom;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkedObjectSpawner : NetworkBehaviour
{
    static NetworkedObjectSpawner intance;
    public static NetworkConnectionToClient ConnectionToHost;


    [SerializeField]
    GameObject heg;

    public static NetworkedObjectSpawner Instance { get { return intance; } }

    private void Awake()
    {
        intance = this;
    }

    private void Update()
    {
        if (!isServer) return;
        if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftControl))
        {
            SpawnHEG();
        }
    }

    public void CreateProjectile(GameObject prefab, float speed, float mass, float damage, Transform start)
    {
        if (!NetworkServer.active)
        {
            CreateProjectileServer(prefab, speed, mass, damage, start);
        }

        GameObject projectile = Instantiate(prefab, start.position, start.rotation);
        SpawnObject(projectile);
        if (projectile == null)
        {
            Debug.LogWarning("NULL PROJECTILE");
            return;
        }
        InitializeSpawnedProjectileServer(projectile, speed, mass, damage);
        InitializeSpawnedProjectileRpc(projectile, speed, mass, damage);
    }

    [ClientRpc]
    void InitializeSpawnedProjectileRpc(GameObject projectile, float speed, float mass, float damage)
    {
        if (!projectile)
        {
            return;
        }
        InitializeSpawnedProjectile(projectile, speed, mass, damage);
    }

    [Server]
    void InitializeSpawnedProjectileServer(GameObject projectile, float speed, float mass, float damage)
    {
        if (!projectile)
        {
            return;
        }
        InitializeSpawnedProjectile(projectile, speed, mass, damage);
    }

    public void InitializeSpawnedProjectile(GameObject projectile, float speed, float mass, float damage)
    {
        Rigidbody2D projectileRigidBody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidBody.mass = mass;
        projectileRigidBody.velocity = projectile.transform.right * speed;
    }

    //Created this function as a workaround for the "default parameterized" CreateObject
    //We are unable to use Quaternion.Identity as default parameter due to it is not a compile time constant.
    public void CreateObject(GameObject prefab, Vector3 position)
    {
        CreateObject(prefab, position, Quaternion.Euler(0, 0, 0));
    }

    public void CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!NetworkServer.active)
        {
            CreateObjectServer(prefab, position, rotation);
        }
        SpawnObject(prefab);
    }

    [Command]
    public void CreateProjectileServer(GameObject prefab, float speed, float mass, float damage, Transform start)
    {
        CreateProjectile(prefab, speed, mass, damage, start);
    }

    [Command]
    public void CreateObjectServer(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        CreateObject(prefab, position, rotation);
    }

    [ServerCallback]
    void SpawnObject(GameObject gameObject)
    {
        if (ConnectionToHost == null)
        {
            Debug.Log("No host yet");
            return;
        }
        NetworkServer.Spawn(gameObject, ConnectionToHost);
    }

    [ContextMenu("Spawn HEG")]
    void SpawnHEG()
    {
        CreateObject(heg, transform.position, Quaternion.identity);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedProjectileFactory : NetworkBehaviour
{
    [SerializeField]
    GameObject _projectilePrefab;
    [SerializeField]
    Rigidbody2D _parentRigidbody;
    [SerializeField]
    Transform _bulletSpawn;

    [SerializeField]
    float _defaultProjectileSpeed = 10;
    [SerializeField]
    float _defaultProjectileMass = 1;
    [SerializeField]
    float _defaultProjectileDamage = 1;
    [SerializeField]
    [SyncVar]
    float _fireRate = 300;
    [SerializeField]
    float _ammoLeft = 30;
    [SerializeField]

    bool _infiniteAmmo = false;

    [SyncVar]
    float _minTimeBetweenShots;
    [SyncVar]
    float _lastShotTime = 0;

    [ContextMenu("Recalc")]
    void CalcMinTimeBetweenShots()
    {
        _minTimeBetweenShots = 1 / _fireRate * 60;
    }

    public void Trigger()
    {
        if (!hasAuthority)
        {
            return;
        }

        var timeDifference = Time.time - _lastShotTime;
        if (timeDifference > _minTimeBetweenShots)
        {
            _lastShotTime = Time.time;
            Shoot();
        }
    }

    void CreateProjectile(float speed, float mass, float damage)
    {
        int index = NetworkManager.singleton.spawnPrefabs.IndexOf(_projectilePrefab);
        if (index != -1)
        {
            Projectile projectile = ObjectPoolManager.Instance.GetObjectPool(_projectilePrefab.name).GetObject().GetComponent<Projectile>();
            projectile.Initialize(mass, speed, damage, _bulletSpawn.transform.position, _bulletSpawn.transform.rotation);
        }
    }

    void Awake()
    {
        CalcMinTimeBetweenShots();
        transform.SetParent(null, true);
    }

    void Shoot()
    {
        ShootCommand();
    }

    [Command]
    void ShootCommand()
    {
        if (_infiniteAmmo || _ammoLeft-- > 0)
        {
            Vector2 handlerVelocity = _parentRigidbody.velocity;
            Vector3 startVelocity = transform.right * _defaultProjectileSpeed + new Vector3(handlerVelocity.x, 0, 0);
            CreateProjectile(_defaultProjectileSpeed, _defaultProjectileMass, _defaultProjectileDamage);
        }
        if (_ammoLeft < 0)
        {
            _ammoLeft = 0;
        }
    }
}

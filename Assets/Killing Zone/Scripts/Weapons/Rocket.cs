using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Rocket : NetworkBehaviour
{

    [SerializeField] private float _speed;
    [SerializeField] private float _lifetime;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _explosionRange;
    [SerializeField] private float _explosionDamage;

    private Rigidbody _rocketRigidbody;
    private float _timer;

    // Use this for initialization
    void Awake()
    {
        _rocketRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _lifetime)
        {
            Explode();
        }
    }

    public void Shoot(Vector3 direction)
    {
        transform.forward = direction;
        _rocketRigidbody.velocity = direction * _speed;
    }

    public void OnTriggerEnter(Collider otherCollider)
    {
        Explode();
    }

    private void Explode()
    {
        if (isServer)
        {
            CmdAddExplosion();

            Destroy(gameObject);
        }
    }

    [Command]
    private void CmdAddExplosion()
    {
        GameObject explosion = Instantiate(_explosionPrefab);
        explosion.transform.position = transform.position;

        NetworkServer.Spawn(explosion);

        explosion.GetComponent<Explosion>().Explode(_explosionRange, _explosionDamage);
    }
}

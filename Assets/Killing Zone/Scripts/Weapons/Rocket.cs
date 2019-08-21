using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _lifeTime;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] float _explosionRange;
    [SerializeField] float _explosionDamage;

    Rigidbody _rocketRigidbody;
    float _timer;

    // Start is called before the first frame update
    void Awake()
    {
        _rocketRigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
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
        GameObject explosion = Instantiate(_explosionPrefab);
        explosion.transform.position = transform.position;
        explosion.GetComponent<Explosion>().Explode(_explosionRange, _explosionDamage);
        Destroy(gameObject);
    }
}

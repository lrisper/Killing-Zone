using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamgeable
{
    [SerializeField] float _health;
    [SerializeField] float _hitSmoothness;

    [SerializeField] protected float _damage;

    protected Rigidbody _enemyRigibody;

    float _targerScale = 1f;


    void Awake()
    {
        _enemyRigibody = transform.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
            Mathf.Lerp(transform.localScale.x, _targerScale, Time.deltaTime * _hitSmoothness),
            Mathf.Lerp(transform.localScale.y, _targerScale, Time.deltaTime * _hitSmoothness),
            Mathf.Lerp(transform.localScale.z, _targerScale, Time.deltaTime * _hitSmoothness));
    }

    public int Damage(float amount)
    {
        if (_health > 0)
        {
            transform.localScale = Vector3.one * 0.9f;
        }
        _health -= amount;


        if (_health <= 0)
        {
            _targerScale = 0;
            Destroy(gameObject, 1f);
        }
        return 0;
    }
}

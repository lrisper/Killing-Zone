using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float _speed;
    Rigidbody _rocketRigidbody;

    // Start is called before the first frame update
    void Awake()
    {
        _rocketRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shoot(Vector3 direction)
    {
        transform.forward = direction;
        _rocketRigidbody.velocity = direction * _speed;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Flying")]
    [SerializeField] float _distanceFromFloor;
    [SerializeField] float _hoverSmoothness;
    [SerializeField] float _bounceAmplitude;
    [SerializeField] float _bounceSpeed;

    [Header("Chasing")]
    [SerializeField] float _chasingRange;
    [SerializeField] float _chasingSpeed;
    [SerializeField] float _chasingSmoothness;

    float _bounceAngle;
    Player _target;

    public void Start()
    {

    }

    public void Update()
    {
        // make enemy fly
        Fly();

        // chase player
        Chase();


    }

    private void Chase()
    {
        Vector3 targetVelocity = Vector3.zero;


        if (_target = null)
        {
            // find a player
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, _chasingRange / 2, Vector3.down);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<Player>() != null)
                {
                    _target = hit.transform.GetComponent<Player>();
                }

            }

            // check if target is too far
            if (_target != null && Vector3.Distance(transform.position, _target.transform.position) > _chasingRange)
            {
                _target = null;
            }

            // chase the target
            if (_target != null)
            {
                Vector3 direction = (_target.transform.position - transform.position).normalized;

                // remove vertical component
                direction = new Vector3(direction.x, 0, direction.z);
                direction.Normalize();

                // calculate taeget velocity
                targetVelocity = direction * _chasingSpeed;
            }

            // make the enemy move
            _enemyRigibody.velocity = new Vector3(
                Mathf.Lerp(_enemyRigibody.velocity.x, targetVelocity.x, Time.deltaTime * _chasingSmoothness),
                Mathf.Lerp(_enemyRigibody.velocity.y, targetVelocity.y, Time.deltaTime * _chasingSmoothness),
                Mathf.Lerp(_enemyRigibody.velocity.z, targetVelocity.z, Time.deltaTime * _chasingSmoothness));

        }
    }

    private void Fly()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // get ground position
            Vector3 targetPos = hit.point;

            // move enemy up
            targetPos = new Vector3(
                targetPos.x,
                targetPos.y + _distanceFromFloor,
                targetPos.z);

            // swing the enemy
            _bounceAngle += Time.deltaTime * _bounceSpeed;
            float offset = Mathf.Cos(_bounceAngle) * _bounceAmplitude;

            targetPos = new Vector3(
               targetPos.x,
               targetPos.y + offset,
               targetPos.z);

            // apply the position
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, targetPos.x, Time.deltaTime * _hoverSmoothness),
                Mathf.Lerp(transform.position.y, targetPos.y, Time.deltaTime * _hoverSmoothness),
                Mathf.Lerp(transform.position.z, targetPos.z, Time.deltaTime * _hoverSmoothness));


        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Flying")]
    [SerializeField] float _distanceFromFloor;
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

            // chase the target
            if (_target != null)
            {
                //
            }

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
            transform.position = targetPos;


        }
    }
}


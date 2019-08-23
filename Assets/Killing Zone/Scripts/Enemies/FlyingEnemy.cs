using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] float _distanceFromFloor;
    [SerializeField] float _bounceAmplitude;
    [SerializeField] float _bounceSpeed;

    float _bounceAngle;

    public void Start()
    {

    }

    public void Update()
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


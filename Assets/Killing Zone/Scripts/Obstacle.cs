using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] int _health;
    [SerializeField] int _cost;

    public int Cost
    {
        get
        {
            return _cost;
        }
    }

    Collider _obstacleCollider;

    // Start is called before the first frame update
    void Awake()
    {
        _obstacleCollider = GetComponentInChildren<Collider>();

        // start with obstacle collider disabled
        _obstacleCollider.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Place()
    {
        // enable collider
        _obstacleCollider.enabled = true;
    }
}

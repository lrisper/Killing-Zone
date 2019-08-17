using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] int _health;
    [SerializeField] int _cost;
    Renderer _obstacleRender;

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

        //working with transparancy
        _obstacleRender = GetComponentInChildren<Renderer>();
        _obstacleRender.material.color = new Color(1f, 1f, 1f, 0.5f);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Place()
    {
        // enable collider
        _obstacleCollider.enabled = true;

        // make obstacle opaque
        _obstacleRender.material.color = Color.white;

    }
}

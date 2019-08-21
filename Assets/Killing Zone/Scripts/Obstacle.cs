using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IDamgeable
{
    [SerializeField] float _health;
    [SerializeField] int _cost;
    [SerializeField] float _hitSmoothness;

    Renderer _obstacleRender;
    int _targetScale = 1;

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
        transform.localScale = new Vector3(
            Mathf.Lerp(transform.localScale.x, _targetScale, _hitSmoothness * Time.deltaTime),
            Mathf.Lerp(transform.localScale.y, _targetScale, _hitSmoothness * Time.deltaTime),
            Mathf.Lerp(transform.localScale.z, _targetScale, _hitSmoothness * Time.deltaTime));
    }

    public void Place()
    {
        // enable collider
        _obstacleCollider.enabled = true;

        // make obstacle opaque
        _obstacleRender.material.color = Color.white;

    }

    public int Damage(float amount)
    {
        transform.localScale = Vector3.one * 0.8f;

        _health -= amount;
        if (_health <= 0)
        {
            _targetScale = 0;
            Destroy(gameObject, 1f);
        }
        return 0;
    }
}

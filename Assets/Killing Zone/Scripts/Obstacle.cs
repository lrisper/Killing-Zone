using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IDamageable
{
    [SerializeField] private float _health;
    [SerializeField] private int _cost;
    [SerializeField] private float _hitSmoothness;

    private Renderer _obstacleRender;
    private int _targetScale = 1;

    public int Cost
    {
        get
        {
            return _cost;
        }
    }

    public Collider _obstacleCollider;

    // Start is called before the first frame update
    public void Awake()
    {
        _obstacleCollider = GetComponentInChildren<Collider>();
        _obstacleRender = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    public void Update()
    {
        transform.localScale = new Vector3(
            Mathf.Lerp(transform.localScale.x, _targetScale, _hitSmoothness * Time.deltaTime),
            Mathf.Lerp(transform.localScale.y, _targetScale, _hitSmoothness * Time.deltaTime),
            Mathf.Lerp(transform.localScale.z, _targetScale, _hitSmoothness * Time.deltaTime));
    }

    public void SetPositioningMode()
    {
        // start with obstacle collider disabled
        _obstacleCollider.enabled = false;

        // make the obstacle opaque 
        _obstacleRender.material.color = new Color(1f, 1f, 1f, 0.5f);
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

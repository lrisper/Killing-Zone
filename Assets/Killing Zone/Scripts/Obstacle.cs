using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Obstacle : NetworkBehaviour, IDamageable
{

    [SerializeField] private float _initialHealth;
    [SerializeField] private int _cost;
    [SerializeField] private float _hitSmoothness;

    private Renderer _obstacleRenderer;
    private int _targetScale = 1;

    private Health _health;

    public int Cost
    {
        get
        {
            return _cost;
        }
    }

    private Collider obstacleCollider;

    // Use this for initialization
    void Awake()
    {
        obstacleCollider = GetComponentInChildren<Collider>();
        _obstacleRenderer = GetComponentInChildren<Renderer>();

        _health = GetComponent<Health>();
        _health.Value = _initialHealth;
        _health.OnHealthChanged += OnHealthChanged;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
            Mathf.Lerp(transform.localScale.x, _targetScale, _hitSmoothness * Time.deltaTime),
            Mathf.Lerp(transform.localScale.y, _targetScale, _hitSmoothness * Time.deltaTime),
            Mathf.Lerp(transform.localScale.z, _targetScale, _hitSmoothness * Time.deltaTime)
        );
    }

    public void SetPositioningMode()
    {
        // Start with the obstacle collider disabled.
        obstacleCollider.enabled = false;

        // Make the obstacle transparent.
        _obstacleRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public void Place()
    {
        // Enable the collider.
        obstacleCollider.enabled = true;

        // Make the obstacle opaque.
        _obstacleRenderer.material.color = Color.white;
    }

    public int Damage(float amount)
    {
        _health.Damage(amount);
        return 0;
    }

    private void OnHealthChanged(float newHealth)
    {
        transform.localScale = Vector3.one * 0.8f;

        if (newHealth < 0.01f)
        {
            _targetScale = 0;

            if (isServer)
            {
                Destroy(gameObject, 1.0f);
            }
        }
    }
}


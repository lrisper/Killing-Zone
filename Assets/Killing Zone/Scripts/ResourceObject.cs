using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceObject : NetworkBehaviour, IDamageable
{

    [SerializeField] private int _resourceAmount;
    [SerializeField] private float _amountOfHits;
    [SerializeField] private float _hitScale;
    [SerializeField] private float _hitSmoothness;

    private float _hits;
    private float _targetScale;
    private Health _health;

    public float HealthValue { get { return _health.Value; } }
    public int ResourceAmount { get { return _resourceAmount; } }

    // Use this for initialization
    void Start()
    {
        _targetScale = 1;

        _health = GetComponent<Health>();
        _health.Value = _amountOfHits;
        _health.OnHealthChanged += OnHealthChanged;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
            Mathf.Lerp(transform.localScale.x, _targetScale, Time.deltaTime * _hitSmoothness),
            Mathf.Lerp(transform.localScale.y, _targetScale, Time.deltaTime * _hitSmoothness),
            Mathf.Lerp(transform.localScale.z, _targetScale, Time.deltaTime * _hitSmoothness)
        );
    }

    public int Damage(float amount)
    {
        _health.Damage(amount);
        if (_health.Value < 0.01f) return _resourceAmount;
        else return 0;
    }

    private void OnHealthChanged(float newHealth)
    {
        transform.localScale = Vector3.one * _hitScale;

        if (newHealth < 0.01f)
        {
            _targetScale = 0;

            if (isServer)
            {
                Destroy(gameObject, 1);
            }
        }
    }
}

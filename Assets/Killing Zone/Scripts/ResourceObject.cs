using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    [SerializeField] int _resourceAmount;
    [SerializeField] int _amoumtOfHits;
    [SerializeField] float _hitScale;
    [SerializeField] float _hitSmoothness;

    int _hits;
    float _targetScale;

    // Start is called before the first frame update
    void Start()
    {
        _targetScale = 1;
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

    public int Collect()
    {
        _hits++;

        transform.localScale = Vector3.one * _hitScale;

        if (_hits == _amoumtOfHits)
        {
            Destroy(gameObject, 1);
            _targetScale = 0;

            return _resourceAmount;
        }
        return 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    [SerializeField] int _resourceAmount;
    [SerializeField] int _amoumtOfHits;

    int _hits;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact()
    {
        _hits++;
        if (_hits == _amoumtOfHits)
        {
            Destroy(gameObject);
        }
    }
}

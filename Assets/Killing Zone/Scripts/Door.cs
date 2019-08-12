using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool _opensInWards;
    [SerializeField] float _openingSpeed;

    bool _isOpen;
    float _targetAngle;

    // Update is called once per frame
    void Update()
    {
        Quaternion smoothRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, _targetAngle, 0), _openingSpeed * Time.deltaTime);
        transform.localRotation = smoothRotation;
    }

    public void Interact()
    {
        _isOpen = !_isOpen;
        if (_isOpen)
        {
            if (_opensInWards)
            {
                _targetAngle = -90f;
            }
            else
            {
                _targetAngle = 90f;
            }
        }
        else
        {
            _targetAngle = 0;
        }
    }
}

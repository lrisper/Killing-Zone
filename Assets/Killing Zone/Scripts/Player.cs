using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] GameObject _focalPoint;
    [SerializeField] float _focalDistance;
    [SerializeField] KeyCode _changefocalSideKey;

    bool _isFocalPointOnLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_changefocalSideKey))
        {
            _isFocalPointOnLeft = !_isFocalPointOnLeft;
        }

        float targetX = _focalDistance * (_isFocalPointOnLeft ? -1 : 1);
        _focalPoint.transform.localPosition = new Vector3(targetX, _focalPoint.transform.localPosition.y, _focalPoint.transform.localPosition.z);
    }
}

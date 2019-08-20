using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [Header("Positioning")]
    [SerializeField] GameObject _target;
    [SerializeField] GameObject _rotationAnchorObject;
    [SerializeField] Vector3 _translationOffset;
    [SerializeField] Vector3 _followOffset;
    [SerializeField] float _maxViewingAngle;
    [SerializeField] float _minViewingAngle;
    [SerializeField] float _rotationSensitivity;

    [Header("Zooming")]
    [SerializeField] float _zoomOutFOV;
    [SerializeField] float _zoomInFOV;


    float _verticalRotationAngle;

    public Vector3 FollowOffset { get { return _followOffset; } }
    public bool IsZoomedIn { get { return Mathf.RoundToInt(GetComponent<Camera>().fieldOfView) == Mathf.RoundToInt(_zoomInFOV); } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Camra look at target
        float yAngle = _target.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, yAngle, 0);
        transform.position = _target.transform.position - (rotation * _followOffset);
        transform.LookAt(_target.transform.position + _translationOffset);

        //Camera look up or down
        _verticalRotationAngle += Input.GetAxis("Mouse Y");

        // Camera lock on up and down view
        _verticalRotationAngle = Mathf.Clamp(_verticalRotationAngle + Input.GetAxis("Mouse Y") * _rotationSensitivity, _minViewingAngle, _maxViewingAngle);

        transform.RotateAround(_rotationAnchorObject.transform.position, _rotationAnchorObject.transform.right, -_verticalRotationAngle);
    }

    public void ZoomIn()
    {
        GetComponent<Camera>().fieldOfView = _zoomInFOV;

    }

    public void ZoomOut()
    {
        GetComponent<Camera>().fieldOfView = _zoomOutFOV;


    }

    public void TrigerZoom()
    {
        if (IsZoomedIn)
        {
            ZoomOut();
        }
        else
        {
            ZoomIn();
        }
    }

}

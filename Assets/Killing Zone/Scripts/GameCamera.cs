using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [Header("Positioning")]
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _rotationAnchorObject;
    [SerializeField] private Vector3 _translationOffset;
    [SerializeField] private Vector3 _followOffset;
    [SerializeField] private float _maxViewingAngle;
    [SerializeField] private float _minViewingAngle;
    [SerializeField] private float _rotationSensitivity;
    [SerializeField] private GameObject _obstaclePlacementContainer;

    [Header("Zooming")]
    [SerializeField] private float _zoomOutFOV;
    [SerializeField] private float _zoomInFOV;

    private float _verticalRotationAngle;

    public Vector3 FollowOffset { get { return _followOffset; } }
    public bool IsZoomedIn { get { return Mathf.RoundToInt(GetComponent<Camera>().fieldOfView) == Mathf.RoundToInt(_zoomInFOV); } }
    public GameObject ObstaclePlacementContainer { get { return _obstaclePlacementContainer; } }
    public GameObject Target { set { _target = value; } }
    public GameObject RotationAnchorObject { set { _rotationAnchorObject = value; } }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            // Make the camera look at the target.
            float yAngle = _target.transform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, yAngle, 0);

            transform.position = _target.transform.position - (rotation * _followOffset);
            transform.LookAt(_target.transform.position + _translationOffset);

            // Make the camera look up or down.
            _verticalRotationAngle = Mathf.Clamp(_verticalRotationAngle + Input.GetAxis("Mouse Y") * _rotationSensitivity, _minViewingAngle, _maxViewingAngle);

            transform.RotateAround(_rotationAnchorObject.transform.position, _rotationAnchorObject.transform.right, -_verticalRotationAngle);
        }
    }

    public void ZoomIn()
    {
        GetComponent<Camera>().fieldOfView = _zoomInFOV;
    }

    public void ZoomOut()
    {
        GetComponent<Camera>().fieldOfView = _zoomOutFOV;
    }

    public void TriggerZoom()
    {
        if (IsZoomedIn) ZoomOut();
        else ZoomIn();
    }
}


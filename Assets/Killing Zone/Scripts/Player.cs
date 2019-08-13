using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] GameObject _focalPoint;
    [SerializeField] float _focalDistance;
    [SerializeField] KeyCode _changefocalSideKey;
    [SerializeField] float _focalSmoothness = 7.5f;

    [Header("Interaction")]
    [SerializeField] GameCamera _gameCamera;
    [SerializeField] KeyCode _interactionKey;
    [SerializeField] float _interactionDistance;

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
        float smoothX = Mathf.Lerp(_focalPoint.transform.localPosition.x, targetX, _focalSmoothness * Time.deltaTime);
        _focalPoint.transform.localPosition = new Vector3(smoothX, _focalPoint.transform.localPosition.y, _focalPoint.transform.localPosition.z);

        // interaction logic
#if UNITY_EDITOR
        //Draw interaction line
        Debug.DrawLine(_gameCamera.transform.position, _gameCamera.transform.position + _gameCamera.transform.forward * _interactionDistance, Color.green);
#endif
        if (Input.GetKeyDown(_interactionKey))
        {
            RaycastHit hit;
            if (Physics.Raycast(_gameCamera.transform.position, _gameCamera.transform.forward, out hit, _interactionDistance))
            {
                if (hit.transform.GetComponent<Door>())
                {
                    hit.transform.GetComponent<Door>().Interact();
                }
            }
        }

    }
}

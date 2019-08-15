using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum playerTool
    {
        pickaxe,
        Grenade,
        None
    }

    [Header("Focal Point Variables")]
    [SerializeField] GameObject _focalPoint;
    [SerializeField] float _focalDistance;
    [SerializeField] KeyCode _changefocalSideKey;
    [SerializeField] float _focalSmoothness = 7.5f;

    [Header("Interaction")]
    [SerializeField] GameCamera _gameCamera;
    [SerializeField] KeyCode _interactionKey;
    [SerializeField] float _interactionDistance;

    [Header("Interface")]
    [SerializeField] HUDController _hud;

    [Header("Game Play")]
    [SerializeField] KeyCode _toolSwitchKey;
    [SerializeField] playerTool _tool;


    bool _isFocalPointOnLeft = true;
    int _resources = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _hud.Resources = _resources;
        _hud.Tool = 0;
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
        if (Input.GetKeyDown(_toolSwitchKey))
        {
            int currentToolIndex = (int)_tool;
            currentToolIndex++;

            if (currentToolIndex == System.Enum.GetNames(typeof(playerTool)).Length)
            {
                currentToolIndex = 0;
            }

            _tool = (playerTool)currentToolIndex;
            _hud.Tool = _tool;
        }

    }
}

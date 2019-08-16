using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerTool
    {
        pickaxe,
        ObstacleVertical,
        ObstacleRamp,
        ObstacleHorizontal,
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
    [SerializeField] PlayerTool _tool;
    [SerializeField] float _resourceCollectionCooldown;

    [Header("Obstacles")]
    [SerializeField] GameObject _obstaclePlacementContainer;
    [SerializeField] GameObject[] _obstaclePrefabs;


    bool _isFocalPointOnLeft = true;
    int _resources;
    float _resourceCollectionCooldownTimer = 0;
    GameObject _currentObstacle;

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
        // update timers
        _resourceCollectionCooldown -= Time.deltaTime;

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

        // tool switch logic
        if (Input.GetKeyDown(_toolSwitchKey))
        {
            SwitchTool();

        }

        // Preserving the Obstacle Horizontal Rotation
        if (_currentObstacle != null)
        {
            _currentObstacle.transform.eulerAngles = new Vector3(
                0,
                transform.eulerAngles.y,
                transform.eulerAngles.z
                );
        }

        // user logic
        if (Input.GetAxis("Fire1") > 0)
        {
            UseTool();
        }
    }

    private void UseTool()
    {
        if (_tool == PlayerTool.pickaxe)
        {
            RaycastHit hit;
            if (Physics.Raycast(_gameCamera.transform.position, _gameCamera.transform.forward, out hit, _interactionDistance))
            {
                if (_resourceCollectionCooldown <= 0 && hit.transform.GetComponent<ResourceObject>() != null)
                {
                    _resourceCollectionCooldownTimer = _resourceCollectionCooldown;

                    ResourceObject resourceObject = hit.transform.GetComponent<ResourceObject>();
                    Debug.Log("hit the object");
                    int collectedResources = resourceObject.Collect();
                    _resources += collectedResources;
                    _hud.Resources = _resources;
                }
            }
        }
    }

    private void SwitchTool()
    {

        // cycle between tools
        int currentToolIndex = (int)_tool;
        currentToolIndex++;

        if (currentToolIndex == System.Enum.GetNames(typeof(PlayerTool)).Length)
        {
            currentToolIndex = 0;
        }

        // get new tool
        _tool = (PlayerTool)currentToolIndex;
        _hud.Tool = _tool;

        int obstacleToAddIndex = -1;
        // check for obstacle placement logic
        if (_tool == PlayerTool.ObstacleVertical)
        {
            obstacleToAddIndex = 0;
            // show obstacle in placement mode
            Debug.Log("Choose OV");

        }
        else if (_tool == PlayerTool.ObstacleRamp)
        {
            obstacleToAddIndex = 1;
            //
            Debug.Log("Choose OR");
        }
        else if (_tool == PlayerTool.ObstacleHorizontal)
        {
            obstacleToAddIndex = 2;
            //
            Debug.Log("Choose OH");
        }

        if (_currentObstacle != null)
        {
            Destroy(_currentObstacle);
        }

        if (obstacleToAddIndex >= 0)
        {
            _currentObstacle = Instantiate(_obstaclePrefabs[obstacleToAddIndex]);
            _currentObstacle.transform.SetParent(_obstaclePlacementContainer.transform);

            _currentObstacle.transform.localPosition = Vector3.zero;
            _currentObstacle.transform.localRotation = Quaternion.identity;
        }

    }
}

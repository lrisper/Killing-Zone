using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] int _initialResourceCount;
    [SerializeField] float _resourceCollectionCooldown;

    [Header("Obstacles")]
    [SerializeField] GameObject _obstaclePlacementContainer;
    [SerializeField] GameObject _obstacleContainer;
    [SerializeField] GameObject[] _obstaclePrefabs;

    [Header("Weapons")]
    [SerializeField] GameObject _shootOrigin;
    [SerializeField] GameObject _rocketPrefab;

    [Header("Debug")]
    [SerializeField] GameObject _debugPositionPrefab;

    bool _isFocalPointOnLeft = true;
    int _resources;
    float _resourceCollectionCooldownTimer = 0;
    GameObject _currentObstacle;
    bool _obstaclePlacementLock;
    bool _isUsingTools = true;

    List<Weapon> _weapons;
    Weapon _weapon;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _resources = _initialResourceCount;

        _hud.Resources = _resources;
        _hud.Tool = 0;
        _hud.UpdateWeapon(null);

        _weapons = new List<Weapon>();

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

        // select weapons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(3);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchWeapon(4);
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

        // Tool usage logic(continuous)
        if (Input.GetAxis("Fire1") > 0.1f)
        {
            UseToolContinuous();
        }

        // Tool usage logic(Trigger)
        if (Input.GetAxis("Fire1") > 0.1f && !_obstaclePlacementLock)
        {
            _obstaclePlacementLock = true;
            UseToolTrigger();
        }

        if (Input.GetAxis("Fire1") < 0.1f)
        {
            _obstaclePlacementLock = false;
        }

        UpdateWeapon();
    }

    private void UseToolTrigger()
    {
        if (_currentObstacle != null && _resources >= _currentObstacle.GetComponent<Obstacle>().Cost)
        {
            int cost = _currentObstacle.GetComponent<Obstacle>().Cost;
            _resources -= cost;

            _hud.Resources = _resources;
            _hud.UpdateResourcesRequirement(cost, _resources);

            GameObject newObstacle = Instantiate(_currentObstacle);
            newObstacle.transform.SetParent(_obstacleContainer.transform);
            newObstacle.transform.position = _currentObstacle.transform.position;
            newObstacle.transform.rotation = _currentObstacle.transform.rotation;
            newObstacle.GetComponent<Obstacle>().Place();
        }
    }

    private void UseToolContinuous()
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

    private void SwitchWeapon(int index)
    {
        if (index < _weapons.Count)
        {
            _isUsingTools = false;

            _weapon = _weapons[index];
            _hud.UpdateWeapon(_weapon);

            _tool = PlayerTool.None;
            _hud.Tool = _tool;

            if (_currentObstacle != null)
            {
                Destroy(_currentObstacle);
            }

            // zoom out
            if (!(_weapon is Sniper))
            {
                _gameCamera.ZoomOut();
                _hud.sniperAimVisibilty = false;
            }
        }


    }

    private void SwitchTool()
    {
        _isUsingTools = true;

        _weapon = null;
        _hud.UpdateWeapon(_weapon);

        // zoom the camera out
        _gameCamera.ZoomOut();
        _hud.sniperAimVisibilty = false;


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

            _hud.UpdateResourcesRequirement(_currentObstacle.GetComponent<Obstacle>().Cost, _resources);
        }

    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<ItemBox>() != null)
        {
            ItemBox itemBox = otherCollider.gameObject.GetComponent<ItemBox>();

            GiveItem(itemBox.Type, itemBox.Amount);

            Destroy(otherCollider.gameObject);
        }
    }

    private void GiveItem(ItemBox.ItemType type, int amount)
    {

        // create a weapon reference
        Weapon currentWeapon = null;

        // check if we already have weapon
        for (int i = 0; i < _weapons.Count; i++)
        {
            if (type == ItemBox.ItemType.Pistol && _weapons[i] is Pistol)
            {
                currentWeapon = _weapons[i];
            }
            else if (type == ItemBox.ItemType.MachineGun && _weapons[i] is MachineGun)
            {
                currentWeapon = _weapons[i];
            }
            else if (type == ItemBox.ItemType.Shotgun && _weapons[i] is Shotgun)
            {
                currentWeapon = _weapons[i];
            }
            else if (type == ItemBox.ItemType.Sniper && _weapons[i] is Sniper)
            {
                currentWeapon = _weapons[i];
            }
            else if (type == ItemBox.ItemType.RocketLauncher && _weapons[i] is RocketLauncher)
            {
                currentWeapon = _weapons[i];
            }
        }

        // create weapon if we don't have one and add to list
        if (currentWeapon == null)
        {
            if (type == ItemBox.ItemType.Pistol)
            {
                currentWeapon = new Pistol();
            }
            else if (type == ItemBox.ItemType.MachineGun)
            {
                currentWeapon = new MachineGun();
            }
            else if (type == ItemBox.ItemType.Shotgun)
            {
                currentWeapon = new Shotgun();
            }
            else if (type == ItemBox.ItemType.Sniper)
            {
                currentWeapon = new Sniper();
            }
            else if (type == ItemBox.ItemType.RocketLauncher)
            {
                currentWeapon = new RocketLauncher();
            }


            _weapons.Add(currentWeapon);
        }


        currentWeapon.AddAmmunition(amount);
        currentWeapon.LoadClip();

        if (currentWeapon == _weapon)
        {
            _hud.UpdateWeapon(_weapon);
        }

        Debug.Log(currentWeapon.ClipAmmunition);
        Debug.Log(currentWeapon.TotalAmmunition);

    }

    private void UpdateWeapon()
    {
        if (_weapon != null)
        {
            // press R key to reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                _weapon.Reload();
            }

            float timeElapsed = Time.deltaTime;
            bool isPressingTrigger = Input.GetAxis("Fire1") > 0.1f;

            bool hasShot = _weapon.Update(timeElapsed, isPressingTrigger);
            if (hasShot)
            {
                _hud.UpdateWeapon(_weapon);
            }

            if (hasShot)
            {
                Shoot();
            }

            // zoom logic
            if (_weapon is Sniper)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    _gameCamera.TrigerZoom();
                    _hud.sniperAimVisibilty = _gameCamera.IsZoomedIn;
                }
            }
        }
    }

    private void Shoot()
    {
        int amountOfBullets = 1;
        if (_weapon is Shotgun)
        {
            amountOfBullets = ((Shotgun)_weapon).AmountOfBullets;
        }
        for (int i = 0; i < amountOfBullets; i++)
        {
            //Debug.Log("Shoot");
            float distanceFromCamera = Vector3.Distance(_gameCamera.transform.position, transform.position);

            RaycastHit targetHit;
            if (Physics.Raycast(_gameCamera.transform.position + (_gameCamera.transform.forward * distanceFromCamera), _gameCamera.transform.forward, out targetHit))
            {
                Vector3 hitPosition = targetHit.point;

                Vector3 shootDirection = (hitPosition - _shootOrigin.transform.position).normalized;
                shootDirection = new Vector3(
                   shootDirection.x + Random.Range(-_weapon.AimVariation, _weapon.AimVariation),
                   shootDirection.y + Random.Range(-_weapon.AimVariation, _weapon.AimVariation),
                   shootDirection.z + Random.Range(-_weapon.AimVariation, _weapon.AimVariation));

                shootDirection.Normalize();

                if (!(_weapon is RocketLauncher))
                {
                    RaycastHit shootHit;
                    if (Physics.Raycast(_shootOrigin.transform.position, shootDirection, out shootHit))
                    {
                        GameObject debugPositionInstance = Instantiate(_debugPositionPrefab);
                        _debugPositionPrefab.transform.position = shootHit.point;

                        Destroy(debugPositionInstance, .5f);

                        GameObject target = shootHit.transform.gameObject;

                        if (target.tag == "obstacleShape")
                        {
                            target.transform.parent.gameObject.GetComponent<Obstacle>().Hit();
                        }

                        //Debug.Log(target.name);

#if UNITY_EDITOR
                        //Draw line to show shooting ray
                        Debug.DrawLine(_shootOrigin.transform.position, _shootOrigin.transform.position + shootDirection * 100, Color.red);
#endif
                    }
                }
                else
                {

                }

            }

        }

    }
}


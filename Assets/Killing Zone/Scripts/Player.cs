using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour, IDamageable
{
    public enum PlayerTool
    {
        Pickaxe,
        ObstacleVertical,
        ObstacleRamp,
        ObstacleHorizontal,
        None
    }

    [Header("Focal Point variables")]
    [SerializeField] private GameObject _focalPoint;
    [SerializeField] private GameObject _rotationPoint;
    [SerializeField] private float _focalDistance;
    [SerializeField] private float _focalSmoothness;
    [SerializeField] private KeyCode _changeFocalSideKey;

    [Header("Interaction")]
    [SerializeField] private KeyCode _interactionKey;
    [SerializeField] private float _interactionDistance;

    [Header("Gameplay")]
    [SerializeField] private KeyCode _toolSwitchKey;
    [SerializeField] private PlayerTool _tool;
    [SerializeField] private int _initialResourceCount;
    [SerializeField] private float _resourceCollectionCooldown;

    [Header("Obstacles")]
    [SerializeField] private GameObject[] _obstaclePrefabs;

    [Header("Weapons")]
    [SerializeField] private GameObject _shootOrigin;
    [SerializeField] private GameObject _rocketPrefab;

    [Header("Debug")]
    [SerializeField] private GameObject _debugPositionPrefab;

    private bool _isFocalPointOnLeft = true;
    private int _resources = 0;
    private float _resourceCollectionCooldownTimer = 0;
    private GameObject _currentObstacle;
    private bool _obstaclePlacementLock;

    private List<Weapon> _weapons;

    private Weapon weapon;

    private HUDController _hud;
    private GameCamera _gameCamera;
    private GameObject _obstaclePlacementContainer;
    private GameObject _obstacleContainer;
    private int _obstacleToAddIndex;
    private Health _health;

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Initialize values
        _resources = _initialResourceCount;
        _weapons = new List<Weapon>();
        _health = GetComponent<Health>();
        _health.OnHealthChanged += OnHealthChanged;

        if (isLocalPlayer)
        {
            // Game camera
            _gameCamera = FindObjectOfType<GameCamera>();
            _obstaclePlacementContainer = _gameCamera.ObstaclePlacementContainer;
            _gameCamera.Target = _focalPoint;
            _gameCamera.RotationAnchorObject = _rotationPoint;

            // HUD elements
            _hud = FindObjectOfType<HUDController>();
            _hud.ShowScreen("regular");
            _hud.Health = _health.Value;
            _hud.Resources = _resources;
            _hud.Tool = _tool;
            _hud.UpdateWeapon(null);
        }

        // Obstacle container
        _obstacleContainer = GameObject.Find("ObstacleContainer");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        // Update timers.
        _resourceCollectionCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(_changeFocalSideKey))
        {
            _isFocalPointOnLeft = !_isFocalPointOnLeft;
        }

        float targetX = _focalDistance * (_isFocalPointOnLeft ? -1 : 1);
        float smoothX = Mathf.Lerp(_focalPoint.transform.localPosition.x, targetX, _focalSmoothness * Time.deltaTime);
        _focalPoint.transform.localPosition = new Vector3(smoothX, _focalPoint.transform.localPosition.y, _focalPoint.transform.localPosition.z);

        // Interaction logic.
#if UNITY_EDITOR
        // Draw interaction line.
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

        // Select weapons.
        if (Input.GetKeyDown("1"))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            SwitchWeapon(2);
        }
        else if (Input.GetKeyDown("4"))
        {
            SwitchWeapon(3);
        }
        else if (Input.GetKeyDown("5"))
        {
            SwitchWeapon(4);
        }

        // Tool switch logic.
        if (Input.GetKeyDown(_toolSwitchKey))
        {
            SwitchTool();
        }

        // Preserving the obstacles' horizontal rotation.
        if (_currentObstacle != null)
        {
            _currentObstacle.transform.eulerAngles = new Vector3(
                0,
                _currentObstacle.transform.eulerAngles.y,
                _currentObstacle.transform.eulerAngles.z
            );
        }

        // Tool usage logic (continuous).
        if (Input.GetAxis("Fire1") > 0.1f)
        {
            UseToolContinuous();
        }

        // Tool usage logic (trigger).
        if (Input.GetAxis("Fire1") > 0.1f)
        {
            if (!_obstaclePlacementLock)
            {
                _obstaclePlacementLock = true;
                UseToolTrigger();
            }
        }
        else
        {
            _obstaclePlacementLock = false;
        }

        UpdateWeapon();
    }

    private void SwitchWeapon(int index)
    {
        if (index < _weapons.Count)
        {
            weapon = _weapons[index];
            _hud.UpdateWeapon(weapon);

            _tool = PlayerTool.None;
            _hud.Tool = _tool;

            if (_currentObstacle != null) Destroy(_currentObstacle);

            // Zoom out.
            if (!(weapon is Sniper))
            {
                _gameCamera.ZoomOut();
                _hud.SniperAimVisibility = false;
            }
        }
    }

    private void SwitchTool()
    {
        weapon = null;
        _hud.UpdateWeapon(weapon);

        // Zoom the camera out.
        _gameCamera.ZoomOut();
        _hud.SniperAimVisibility = false;

        // Cycle between the avaiable tools.
        int currentToolIndex = (int)_tool;
        currentToolIndex++;

        if (currentToolIndex == System.Enum.GetNames(typeof(PlayerTool)).Length)
        {
            currentToolIndex = 0;
        }

        // Get the new tool.
        _tool = (PlayerTool)currentToolIndex;
        _hud.Tool = _tool;

        // Check for obstacle placement logic.
        _obstacleToAddIndex = -1;
        if (_tool == PlayerTool.ObstacleVertical)
        {
            _obstacleToAddIndex = 0;
        }
        else if (_tool == PlayerTool.ObstacleRamp)
        {
            _obstacleToAddIndex = 1;
        }
        else if (_tool == PlayerTool.ObstacleHorizontal)
        {
            _obstacleToAddIndex = 2;
        }

        if (_currentObstacle != null) Destroy(_currentObstacle);
        if (_obstacleToAddIndex >= 0)
        {
            _currentObstacle = Instantiate(_obstaclePrefabs[_obstacleToAddIndex]);
            _currentObstacle.transform.SetParent(_obstaclePlacementContainer.transform);

            _currentObstacle.transform.localPosition = Vector3.zero;
            _currentObstacle.transform.localRotation = Quaternion.identity;

            _currentObstacle.GetComponent<Obstacle>().SetPositioningMode();

            _hud.UpdateResourcesRequirement(_currentObstacle.GetComponent<Obstacle>().Cost, _resources);
        }
    }

    private void UseToolContinuous()
    {
        if (_tool == PlayerTool.Pickaxe)
        {
            RaycastHit hit;
            if (Physics.Raycast(_gameCamera.transform.position, _gameCamera.transform.forward, out hit, _interactionDistance))
            {
                if (_resourceCollectionCooldownTimer <= 0 && hit.transform.GetComponent<ResourceObject>() != null)
                {
                    _resourceCollectionCooldownTimer = _resourceCollectionCooldown;

                    ResourceObject resourceObject = hit.transform.GetComponent<ResourceObject>();

                    int collectedResources = 0;
                    float resourceHealth = resourceObject.HealthValue;

                    if (resourceHealth - 1 < 0.01f)
                    {
                        collectedResources = resourceObject.ResourceAmount;
                    }

                    CmdDamage(hit.transform.gameObject, 1);

                    _resources += collectedResources;
                    _hud.Resources = _resources;
                }
            }
        }
    }

    private void UseToolTrigger()
    {
        if (_currentObstacle != null && _resources >= _currentObstacle.GetComponent<Obstacle>().Cost)
        {
            int cost = _currentObstacle.GetComponent<Obstacle>().Cost;
            _resources -= cost;

            _hud.Resources = _resources;
            _hud.UpdateResourcesRequirement(cost, _resources);

            CmdPlaceObstacle(_obstacleToAddIndex, _currentObstacle.transform.position, _currentObstacle.transform.rotation);
        }
    }

    [Command]
    void CmdPlaceObstacle(int index, Vector3 position, Quaternion rotation)
    {
        GameObject newObstacle = Instantiate(_obstaclePrefabs[index]);
        newObstacle.transform.SetParent(_obstacleContainer.transform);
        newObstacle.transform.position = position;
        newObstacle.transform.rotation = rotation;
        newObstacle.GetComponent<Obstacle>().Place();

        NetworkServer.Spawn(newObstacle);
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (!isLocalPlayer) return;

        if (otherCollider.gameObject.GetComponent<ItemBox>() != null)
        {
            ItemBox itemBox = otherCollider.gameObject.GetComponent<ItemBox>();

            GiveItem(itemBox.Type, itemBox.Amount);

            CmdCollectBox(otherCollider.gameObject);
        }
    }

    [Command]
    void CmdCollectBox(GameObject box)
    {
        Destroy(box);
    }

    private void GiveItem(ItemBox.ItemType type, int amount)
    {
        // Create a weapon reference.
        Weapon currentWeapon = null;

        // Check if we already have an instance of this weapon.
        for (int i = 0; i < _weapons.Count; i++)
        {
            if (type == ItemBox.ItemType.Pistol && _weapons[i] is Pistol) currentWeapon = _weapons[i];
            else if (type == ItemBox.ItemType.MachineGun && _weapons[i] is MachineGun) currentWeapon = _weapons[i];
            else if (type == ItemBox.ItemType.Shotgun && _weapons[i] is Shotgun) currentWeapon = _weapons[i];
            else if (type == ItemBox.ItemType.Sniper && _weapons[i] is Sniper) currentWeapon = _weapons[i];
            else if (type == ItemBox.ItemType.RocketLauncher && _weapons[i] is RocketLauncher) currentWeapon = _weapons[i];
        }

        // If we don't have a weapon of this type, create one, and add it to the weapons list.
        if (currentWeapon == null)
        {
            if (type == ItemBox.ItemType.Pistol) currentWeapon = new Pistol();
            else if (type == ItemBox.ItemType.MachineGun) currentWeapon = new MachineGun();
            else if (type == ItemBox.ItemType.Shotgun) currentWeapon = new Shotgun();
            else if (type == ItemBox.ItemType.Sniper) currentWeapon = new Sniper();
            else if (type == ItemBox.ItemType.RocketLauncher) currentWeapon = new RocketLauncher();
            _weapons.Add(currentWeapon);
        }

        currentWeapon.AddAmmunition(amount);
        currentWeapon.LoadClip();

        if (currentWeapon == weapon)
        {
            _hud.UpdateWeapon(weapon);
        }
    }

    private void UpdateWeapon()
    {
        if (weapon != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                weapon.Reload();
            }

            float timeElapsed = Time.deltaTime;
            bool isPressingTrigger = Input.GetAxis("Fire1") > 0.1f;

            bool hasShot = weapon.Update(timeElapsed, isPressingTrigger);
            _hud.UpdateWeapon(weapon);
            if (hasShot)
            {
                Shoot();
            }

            // Zoom logic.
            if (weapon is Sniper)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    _gameCamera.TriggerZoom();
                    _hud.SniperAimVisibility = _gameCamera.IsZoomedIn;
                }
            }
        }
    }

    private void Shoot()
    {
        int amountOfBullets = 1;
        if (weapon is Shotgun)
        {
            amountOfBullets = ((Shotgun)weapon).AmountOfBullets;
        }

        for (int i = 0; i < amountOfBullets; i++)
        {
            float distanceFromCamera = Vector3.Distance(_gameCamera.transform.position, transform.position);
            RaycastHit targetHit;
            if (Physics.Raycast(_gameCamera.transform.position + (_gameCamera.transform.forward * distanceFromCamera), _gameCamera.transform.forward, out targetHit))
            {
                Vector3 hitPosition = targetHit.point;

                Vector3 shootDirection = (hitPosition - _shootOrigin.transform.position).normalized;
                shootDirection = new Vector3(
                    shootDirection.x + Random.Range(-weapon.AimVariation, weapon.AimVariation),
                    shootDirection.y + Random.Range(-weapon.AimVariation, weapon.AimVariation),
                    shootDirection.z + Random.Range(-weapon.AimVariation, weapon.AimVariation)
                );
                shootDirection.Normalize();

                if (!(weapon is RocketLauncher))
                {
                    RaycastHit shootHit;
                    if (Physics.Raycast(_shootOrigin.transform.position, shootDirection, out shootHit))
                    {
                        GameObject debugPositionInstance = Instantiate(_debugPositionPrefab);
                        debugPositionInstance.transform.position = shootHit.point;
                        Destroy(debugPositionInstance, 0.5f);

                        if (shootHit.transform.GetComponent<IDamageable>() != null)
                        {
                            CmdDamage(shootHit.transform.gameObject, weapon.Damage);
                        }
                        else if (shootHit.transform.GetComponentInParent<IDamageable>() != null)
                        {
                            CmdDamage(shootHit.transform.parent.gameObject, weapon.Damage);
                        }

#if UNITY_EDITOR
                        // Draw a line to show the shooting ray.
                        Debug.DrawLine(_shootOrigin.transform.position, _shootOrigin.transform.position + shootDirection * 100, Color.red);
#endif
                    }
                }
                else
                {
                    CmdSpawnRocket(shootDirection);
                }
            }
        }
    }

    [Command]
    private void CmdSpawnRocket(Vector3 shootDirection)
    {
        GameObject rocket = Instantiate(_rocketPrefab);
        rocket.transform.position = _shootOrigin.transform.position + shootDirection;
        rocket.GetComponent<Rocket>().Shoot(shootDirection);

        NetworkServer.Spawn(rocket);
    }

    [Command]
    private void CmdDamage(GameObject target, float damage)
    {
        if (target != null) target.GetComponent<IDamageable>().Damage(damage);
    }

    public int Damage(float amount)
    {
        GetComponent<Health>().Damage(amount);
        return 0;
    }

    private void OnHealthChanged(float newHealth)
    {
        if (!isLocalPlayer) return;

        _hud.Health = newHealth;

        if (newHealth < 0.01f)
        {
            _hud.ShowScreen("gameOver");
            CmdDestroy();
        }
    }

    [Command]
    void CmdDestroy()
    {
        Destroy(gameObject);
    }
}

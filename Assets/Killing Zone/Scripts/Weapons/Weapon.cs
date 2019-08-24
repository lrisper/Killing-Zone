using System.Collections;
using System.Collections.Generic;

public abstract class Weapon
{
    // Ammunition fields.
    private int _clipAmmunition = 0;
    private int _totalAmmunition = 0;

    // Weapon settings (customizable on weapon classes).
    protected int _clipSize = 0;
    protected int _maxAmmunition = 0;
    protected float _reloadDuration = 0.0f;
    protected float _cooldownDuration = 0.0f;
    protected bool _isAutomatic = false;
    protected string _weaponName = "";
    protected float _aimVariation = 0.0f;
    protected float _damage = 0.0f;

    // Private fields.
    private float _reloadTimer = -1.0f;
    private float _cooldownTimer = 0.0f;
    private bool _pressedTrigger = false;

    // Properties
    public int ClipAmmunition { get { return _clipAmmunition; } set { _clipAmmunition = value; } }
    public int TotalAmmunition { get { return _totalAmmunition; } set { _totalAmmunition = value; } }

    public int ClipSize { get { return _clipSize; } }
    public int MaxAmmunition { get { return _maxAmmunition; } }
    public float ReloadDuration { get { return _reloadDuration; } }
    public float CooldownDuration { get { return _cooldownDuration; } }
    public bool IsAutomatic { get { return _isAutomatic; } }
    public string Name { get { return _weaponName; } }
    public float AimVariation { get { return _aimVariation; } }
    public float Damage { get { return _damage; } }

    public float ReloadTimer { get { return _reloadTimer; } }

    public void AddAmmunition(int amount)
    {
        _totalAmmunition = System.Math.Min(_totalAmmunition + amount, _maxAmmunition);
    }

    public void LoadClip()
    {
        int maximumAmmunitionToLoad = _clipSize - _clipAmmunition;
        int ammunitionToLoad = System.Math.Min(maximumAmmunitionToLoad, _totalAmmunition);

        _clipAmmunition += ammunitionToLoad;
        _totalAmmunition -= ammunitionToLoad;
    }

    public bool Update(float deltaTime, bool isPressingTrigger)
    {
        bool hasShot = false;

        // Cooldown logic.
        _cooldownTimer -= deltaTime;
        if (_cooldownTimer <= 0)
        {
            bool canShoot = false;
            if (_isAutomatic) canShoot = isPressingTrigger;
            else if (!_pressedTrigger && isPressingTrigger) canShoot = true;

            if (canShoot && _reloadTimer <= 0.0f)
            {
                _cooldownTimer = _cooldownDuration;

                // Only shoot if there are any available bullets.
                if (_clipAmmunition > 0)
                {
                    _clipAmmunition--;
                    hasShot = true;
                }

                if (_clipAmmunition == 0)
                {
                    // Automatically reload the weapon.
                    Reload();
                }
            }

            _pressedTrigger = isPressingTrigger;
        }

        // Reload logic.
        if (_reloadTimer > 0.0f)
        {
            _reloadTimer -= deltaTime;
            if (_reloadTimer <= 0.0f)
            {
                LoadClip();
            }
        }

        return hasShot;
    }

    public void Reload()
    {
        // Only reload if the weapon is not currently reloading,
        // the clip is not full and we have more bullets left.

        if (_reloadTimer <= 0.0f && _clipAmmunition < _clipSize && _totalAmmunition > 0)
        {
            _reloadTimer = _reloadDuration;
        }
    }
}

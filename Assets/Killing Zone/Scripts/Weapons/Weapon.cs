using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Weapon
{
    // Ammunition fields
    int _clipAmmunition;
    int _totalAmmunition;

    // weapon settings(customizable on each weapon)
    protected int _clipSize;
    protected int _maxAmmunition;
    protected float _reloadDuration;
    protected float _cooldownDuration;
    protected bool _isAutomatic;
    protected string _weaponName = "";

    // private fields
    float _reloadTimer = -1f;
    float _coolDownTimer;
    bool _pressedTrigger;

    // properties
    public int ClipAmmunition { get { return _clipAmmunition; } set { _clipAmmunition = value; } }
    public int TotalAmmunition { get { return _totalAmmunition; } set { _totalAmmunition = value; } }

    public int ClipSize { get { return _clipSize; } }
    public int MaxAmmunition { get { return _maxAmmunition; } }
    public float ReloadDuration { get { return _reloadDuration; } }
    public float CooldownDuration { get { return _cooldownDuration; } }
    public bool IsAutomatic { get { return _isAutomatic; } }
    public string WeaponName { get { return _weaponName; } }
    public float ReloadTimer { get { return _reloadTimer; } }

    public void LoadClip()
    {
        int maxAmmunitionToLoad = _clipSize - _clipAmmunition;
        int ammunitiontoLoad = System.Math.Min(maxAmmunitionToLoad, _totalAmmunition);

        _clipAmmunition += ammunitiontoLoad;
        _totalAmmunition -= ammunitiontoLoad;
    }

    public void AddAmmunition(int amount)
    {
        TotalAmmunition = System.Math.Min(_totalAmmunition + amount, _maxAmmunition);
    }

    public bool Update(float deltaTime, bool isPressingTrigger)
    {
        bool hasShot = false;

        // cooldown logic
        _coolDownTimer -= deltaTime;
        if (_coolDownTimer <= 0)
        {
            bool canShoot = false;
            if (_isAutomatic)
            {
                canShoot = isPressingTrigger;
            }
            else if (!_pressedTrigger && isPressingTrigger)
            {
                canShoot = true;
            }

            if (canShoot && _reloadTimer <= 0)
            {
                _coolDownTimer = _cooldownDuration;

                // only shoot if there are any available bullets
                if (_clipAmmunition > 0)
                {
                    _clipAmmunition--;
                    hasShot = true;
                }

                if (_clipAmmunition == 0)
                {
                    // automatically reload weapon 
                    Reload();
                }
            }

            _pressedTrigger = isPressingTrigger;
        }

        // reload logic
        if (_reloadTimer > 0)
        {
            _reloadTimer -= deltaTime;
            if (_reloadTimer <= 0)
            {
                LoadClip();
            }
        }

        return hasShot;
    }

    public void Reload()
    {
        // only reload weapon if weapon is not currently reloading
        // and the clip is not full
        if (_reloadTimer <= 0 && _clipAmmunition < _clipSize)
        {
            _reloadTimer = _reloadDuration;
        }

    }
}

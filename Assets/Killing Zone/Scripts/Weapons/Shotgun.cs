using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    int _amountOfBullets = 5;
    public int AmountOfBullets { get { return _amountOfBullets; } }

    public Shotgun()
    {
        _clipSize = 4;
        _maxAmmunition = 12;
        _reloadDuration = 3f;
        _cooldownDuration = 1f;
        _isAutomatic = false;
        _weaponName = "Shotgun";
        _aimVariation = .08f;
        _damage = 4f;
    }
}

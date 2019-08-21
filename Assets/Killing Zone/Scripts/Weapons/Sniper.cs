using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Weapon
{
    public Sniper()
    {
        _clipSize = 1;
        _maxAmmunition = 10;
        _reloadDuration = 2f;
        _cooldownDuration = 1f;
        _isAutomatic = false;
        _weaponName = "Sinper";
        _aimVariation = 0.0f;
        _damage = 80f;
    }
}

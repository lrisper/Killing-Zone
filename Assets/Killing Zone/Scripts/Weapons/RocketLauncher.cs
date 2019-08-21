using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Weapon
{
    public RocketLauncher()
    {
        _clipSize = 1;
        _maxAmmunition = 4;
        _reloadDuration = 3f;
        _cooldownDuration = .5f;
        _isAutomatic = false;
        _weaponName = "RPG";
        _aimVariation = 0.01f;
    }
}

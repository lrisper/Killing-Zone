using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
{
    public MachineGun()
    {
        _clipSize = 30;
        _maxAmmunition = 120;
        _reloadDuration = 2f;
        _cooldownDuration = .08f;
        _isAutomatic = true;
        _weaponName = "Machine Gun";
        _aimVariation = .04f;
        _damage = 2f;

    }
}


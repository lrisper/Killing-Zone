using System.Collections;
using System.Collections.Generic;

public class Pistol : Weapon
{
    public Pistol()
    {
        _clipSize = 16;
        _maxAmmunition = 64;
        _reloadDuration = 1f;
        _cooldownDuration = .10f;
        _isAutomatic = false;
        _weaponName = "Pistol";
        _aimVariation = .01f;
        _damage = 5f;

    }
}

using System.Collections;
using System.Collections.Generic;

public class Pistol : Weapon
{
    public Pistol()
    {
        _clipSize = 16;
        _maxAmmunition = 64;
        _reloadDuration = 2f;
        _cooldownDuration = .25f;
        _isAutomatic = false;
        _weaponName = "Pistol";

    }
}

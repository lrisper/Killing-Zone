using System.Collections;
using System.Collections.Generic;

public class Pistol : Weapon
{
    public Pistol()
    {
        _clipSize = 16;
        _maxAmmunition = 64;
        _reloadTime = 2f;
        _cooldownTime = .25f;
        _isAutomatic = false;
        _weaponName = "Pistol";

    }
}

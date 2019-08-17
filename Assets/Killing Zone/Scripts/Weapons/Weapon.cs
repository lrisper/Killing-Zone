using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{

    int _clipAmmunition;
    int _totalAmmunition;

    int _clipSize;
    int _maxAmmunition;
    float _reloadTime;
    float _cooldownTime;
    bool _isAutomatic;



    public int ClipAmmunition { get { return _clipAmmunition; } set { _clipAmmunition = value; } }
    public int TotalAmmunition { get { return _totalAmmunition; } set { _totalAmmunition = value; } }

    public int ClipSize { get { return _clipSize; } }
    public int MaxAmmunition { get { return _maxAmmunition; } }
    public float ReloadTime { get { return _reloadTime; } }
    public float CooldownTime { get { return _cooldownTime; } }
    public bool IsAutomatic { get { return _isAutomatic; } }


}

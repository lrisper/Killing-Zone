using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon
{

    int _clipAmmunition;
    int _totalAmmunition;

    protected int _clipSize;
    protected int _maxAmmunition;
    protected float _reloadTime;
    protected float _cooldownTime;
    protected bool _isAutomatic;



    public int ClipAmmunition { get { return _clipAmmunition; } set { _clipAmmunition = value; } }
    public int TotalAmmunition { get { return _totalAmmunition; } set { _totalAmmunition = value; } }

    public int ClipSize { get { return _clipSize; } }
    public int MaxAmmunition { get { return _maxAmmunition; } }
    public float ReloadTime { get { return _reloadTime; } }
    public float CooldownTime { get { return _cooldownTime; } }
    public bool IsAutomatic { get { return _isAutomatic; } }


}

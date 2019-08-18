﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Interface Elements")]
    [SerializeField] Text _resourcesText;
    [SerializeField] Text _resourcesRequirementText;
    [SerializeField] Text _weaponNameText;
    [SerializeField] Text _weaponAmmunitionText;

    [Header("Tool Selctor")]
    [SerializeField] GameObject _toolFocus;
    [SerializeField] GameObject _toolContainer;
    [SerializeField] float _focusSmooveness;

    float targetFocusX;

    public int Resources
    {
        set { _resourcesText.text = "Resources: " + value; }
    }

    public Player.PlayerTool Tool
    {
        set
        {
            targetFocusX = _toolContainer.transform.GetChild((int)value).transform.position.x;
            if (value != Player.PlayerTool.ObstacleHorizontal &&
                value != Player.PlayerTool.ObstacleRamp &&
                value != Player.PlayerTool.ObstacleVertical)
            {
                _resourcesRequirementText.enabled = false;
            }
            else
            {
                _resourcesRequirementText.enabled = true;
            }
        }
    }

    private void Start()
    {
        targetFocusX = _toolContainer.transform.GetChild(0).transform.position.x;
        _toolFocus.transform.position = new Vector3(targetFocusX, _toolFocus.transform.position.y);
    }

    private void Update()
    {
        _toolFocus.transform.position = new Vector3(
            Mathf.Lerp(_toolFocus.transform.position.x, targetFocusX, Time.deltaTime * _focusSmooveness),
             _toolFocus.transform.position.y);
    }

    public void UpdateResourcesRequirement(int cost, int balance)
    {
        _resourcesRequirementText.text = "Requires: " + cost;
        if (balance < cost)
        {
            _resourcesRequirementText.color = Color.red;
        }
        else
        {
            _resourcesRequirementText.color = Color.white;
        }
    }

    public void UpdateWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            _weaponNameText.enabled = false;
            _weaponAmmunitionText.enabled = false;
        }
        else
        {
            _weaponNameText.enabled = true;
            _weaponAmmunitionText.enabled = true;

            _weaponNameText.text = weapon.WeaponName;
            _weaponAmmunitionText.text = weapon.ClipAmmunition + " / " + weapon.TotalAmmunition;
        }
    }

}

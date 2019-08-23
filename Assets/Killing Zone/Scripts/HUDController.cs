using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] GameObject _regularScreen;
    [SerializeField] GameObject _gameOverScreen;

    [Header("Interface Elements")]
    [SerializeField] Text _healthText;
    [SerializeField] Text _resourcesText;
    [SerializeField] Text _resourcesRequirementText;
    [SerializeField] Text _weaponNameText;
    [SerializeField] Text _weaponAmmunitionText;
    [SerializeField] RectTransform _weaponReloadBar;
    [SerializeField] GameObject _sniperAim;

    [Header("Tool Selctor")]
    [SerializeField] GameObject _toolFocus;
    [SerializeField] GameObject _toolContainer;
    [SerializeField] float _focusSmooveness;

    float targetFocusX;

    public float Health
    {
        set { _healthText.text = "Health: " + Mathf.CeilToInt(value); }
    }

    public int Resources
    {
        set { _resourcesText.text = "Resources: " + value; }
    }

    public Player.PlayerTool Tool
    {
        set
        {
            if (value != Player.PlayerTool.None)
            {
                _toolFocus.SetActive(true);
                targetFocusX = _toolContainer.transform.GetChild((int)value).transform.position.x;
            }
            else
            {
                _toolFocus.SetActive(false);
            }

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

    public bool sniperAimVisibilty { set { _sniperAim.SetActive(value); } }

    private void Start()
    {
        ShowScreen("");

        targetFocusX = _toolContainer.transform.GetChild(0).transform.position.x;
        _toolFocus.transform.position = new Vector3(targetFocusX, _toolFocus.transform.position.y);

        // hide sniper aim
        _sniperAim.SetActive(false);
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
            _weaponReloadBar.localScale = new Vector3(0, 1, 1);
        }
        else
        {
            _weaponNameText.enabled = true;
            _weaponAmmunitionText.enabled = true;

            _weaponNameText.text = weapon.WeaponName;
            _weaponAmmunitionText.text = weapon.ClipAmmunition + " / " + weapon.TotalAmmunition;

            if (weapon.ReloadTimer > 0)
            {
                _weaponReloadBar.localScale = new Vector3(weapon.ReloadTimer / weapon.ReloadDuration, 1, 1);
            }
            else
            {
                _weaponReloadBar.localScale = new Vector3(0, 1, 1);
            }
        }
    }

    public void ShowScreen(string screenName)
    {
        _regularScreen.SetActive(screenName == "regular");
        _gameOverScreen.SetActive(screenName == "gameOver");
    }

}

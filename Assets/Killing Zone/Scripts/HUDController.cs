using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Interface Elements")]
    [SerializeField] Text _resourcesText;

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

}

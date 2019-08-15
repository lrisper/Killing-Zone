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
    [SerializeField] GameObject[] _tools;

    public int Resources
    {
        set { _resourcesText.text = "Resources: " + value; }
    }

    public Player.playerTool Tool
    {
        set
        {
            _toolFocus.transform.position = new Vector3(
           _tools[(int)value].transform.position.x,
           _toolFocus.transform.position.y
           );
        }
    }

    public void Start()
    {

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Text _resourcesText;

    public int Resources
    {
        set { _resourcesText.text = "Resources: " + value; }
    }

}

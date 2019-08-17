using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] string _itemName;
    [SerializeField] int _itemAmount;

    [Header("Visuals")]
    [SerializeField] float _rotationAngle;
    [SerializeField] float _verticalRange;
    [SerializeField] float _verticalSpeed;

    GameObject _floatingObject;
    float _verticalAngle;

    // Start is called before the first frame update
    void Start()
    {
        _floatingObject = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        _verticalAngle += _verticalSpeed * Time.deltaTime;

        _floatingObject.transform.Rotate(0, _rotationAngle * Time.deltaTime, 0);
        _floatingObject.transform.localPosition = new Vector3(
            0,
            Mathf.Cos(_verticalAngle) * _verticalRange,
            0);
    }
}

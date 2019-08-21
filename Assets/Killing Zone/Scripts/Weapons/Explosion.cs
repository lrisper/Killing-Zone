﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public void Explode(float range, float damage)
    {
        transform.GetChild(0).localScale = Vector3.one * range * 2;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, range, transform.up);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.name);
        }
        Destroy(gameObject, .5f);
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFloat : MonoBehaviour
{
    public float speed;
    private Light mylight;

    void Start()
    {
        mylight = GetComponentInChildren<Light>();
    }

    void Update()
    {
        mylight.intensity = 0.6f + Mathf.Sin(Time.time * speed) * 0.3f;
        transform.position = new Vector3(transform.position.x, -1 + Mathf.Sin(Time.time * speed) * 0.3f, transform.position.z);
    }
}

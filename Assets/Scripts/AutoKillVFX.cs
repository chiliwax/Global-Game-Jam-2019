﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKillVFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration + GetComponent<ParticleSystem>().main.startLifetime.constantMax);
    }

}

using System;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!_particleSystem.isEmitting)
        {
            Destroy(gameObject);
        }
    }
}

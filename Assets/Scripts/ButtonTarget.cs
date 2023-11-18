using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTarget : MonoBehaviour
{
    private Transform _player;

    private void Awake()
    {
        _player = FindObjectOfType<Camera>().transform;
    }

    private void Update()
    {
        transform.LookAt(_player.transform.position, Vector3.up);
        transform.Rotate(0f, 180f, 0f);
    }
}
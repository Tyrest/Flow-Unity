using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private static HUD _instance;

    public static HUD Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<HUD>();
            }

            return _instance;
        }
    }

    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        scoreText.text = "0";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score < 0 ? "" : $"{score}";
    }
}
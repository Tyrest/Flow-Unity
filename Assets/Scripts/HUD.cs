using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "0";
    }
    
    public void UpdateScore(int score)
    {
        scoreText.text = string.Format("{0}", score);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsManager : MonoBehaviour
{
    private static ResultsManager _instance;

    public static ResultsManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<ResultsManager>();
            }

            return _instance;
        }
    }
    
    public GameObject resultsPanel;
    public GameObject resultsText;
    
    private void Awake()
    {
        _instance = this;
    }
    
    public void ShowResults(int score)
    {
        resultsPanel.SetActive(true);
        resultsText.GetComponent<TextMeshPro>().text = "Score: " + score;
    }
    
    public void HideResults()
    {
        resultsPanel.SetActive(false);
    }
}
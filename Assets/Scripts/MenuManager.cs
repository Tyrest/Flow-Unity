using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;

    public static MenuManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MenuManager>();
            }

            return _instance;
        }
    }

    public GameObject buttonPrefab;
    public GameObject quitButton;
    public GameObject optionsButton;
    public GameObject startButton;
    private List<GameObject> _songButtons;
    private int _currentSongIndex;

    private void Awake()
    {
        _instance = this;
        _songButtons = new List<GameObject>();
        _currentSongIndex = 0;
    }

    private void Start()
    {
    }

    private void ToggleButtons(bool toggle)
    {
        quitButton.gameObject.SetActive(toggle);
        optionsButton.gameObject.SetActive(toggle);
        startButton.gameObject.SetActive(toggle);
    }

    public void SongSelect()
    {
        ToggleButtons(false);
        SongManager.Instance.GetSongs().ForEach(song =>
        {
            var button = Instantiate(buttonPrefab, transform);
            button.transform.localPosition += Vector3.down * ((_songButtons.Count) * 3);
            button.GetComponentInChildren<TMPro.TextMeshPro>().text = song.Title;
            button.gameObject.SetActive(true);
            _songButtons.Add(button);
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
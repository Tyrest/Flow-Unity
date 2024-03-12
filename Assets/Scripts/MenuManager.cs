using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

enum MenuState
{
    Play,
    Paused,
    Title,
    SongSelect,
    Options
}

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

    private float SongButtonOffset
    {
        get => _songButtonOffset;
        set => _songButtonOffset = Mathf.Clamp(value, -0.5f, _songButtons.Count - 0.5f);
    }

    public float scrollSpeed = 10f;
    public GameObject buttonPrefab;
    public GameObject titleButtons;
    public GameObject songSelectButtons;
    public GameObject optionsButtons;
    public GameObject pauseButtons;
    private List<GameObject> _songButtons;
    private float _songButtonOffset;
    private MenuState _menuState;

    private void Awake()
    {
        _instance = this;
        _songButtons = new List<GameObject>();
    }
    
    private void Start()
    {
        ToggleMenuState(MenuState.Title);
    }

    private void ToggleMenuState(MenuState state)
    {
        _menuState = state;
        titleButtons.SetActive(state == MenuState.Title);
        songSelectButtons.SetActive(state == MenuState.SongSelect);
        optionsButtons.SetActive(state == MenuState.Options);
        pauseButtons.SetActive(state == MenuState.Paused);
    }

    private UnityEngine.Events.UnityAction CreateStartSongAction(int count)
    {
        return () => { StartSong(count); };
    }


    public void SongSelect()
    {
        ToggleMenuState(MenuState.SongSelect);
        SongManager.Instance.GetSongs().ForEach(song =>
        {
            var button = Instantiate(buttonPrefab, transform);
            button.transform.localPosition += Vector3.down * (_songButtons.Count * 3);
            button.GetComponentInChildren<TMPro.TextMeshPro>().text = song.Title;
            button.gameObject.SetActive(true);
            button.GetComponent<SongTarget>().AddInteraction(CreateStartSongAction(_songButtons.Count));
            button.transform.parent = songSelectButtons.transform;
            _songButtons.Add(button);
        });
        _songButtonOffset = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void HandleSongButtons()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SongButtonOffset += -1f;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SongButtonOffset += 1f;
        }

        SongButtonOffset += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        // Lerp SongButtonOffset to nearest integer
        SongButtonOffset = Mathf.Lerp(SongButtonOffset, Mathf.Round(SongButtonOffset), Time.deltaTime * 10f);

        var targetY = SongButtonOffset * 3;
        for (var i = 0; i < _songButtons.Count; i++)
        {
            // Animate song buttons to target position
            var button = _songButtons[i];
            button.transform.localPosition = Vector3.Lerp(button.transform.localPosition,
                new Vector3(0, targetY - i * 3, 0), Time.deltaTime * 10f);

            // Turn on outline if button is selected
            button.TryGetComponent<SongTarget>(out var songTarget);
            songTarget.ToggleOutline(i == Mathf.RoundToInt(SongButtonOffset));
        }
    }

    private void StartSong(int index)
    {
        ToggleMenuState(MenuState.Play);
        SongManager.Instance.currentSongIndex = index;
        SongManager.Instance.currentDifficulty = "intermediate";
        GameManager.Instance.StartSong();
        
        // DontDestroyOnLoad(SongManager.Instance.gameObject);
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Main");

        // Destroy(MenuManager.Instance.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_menuState == MenuState.Title)
            {
                QuitGame();
            }
            else if (_menuState == MenuState.SongSelect)
            {
                ToggleMenuState(MenuState.Title);
            }
            else if (_menuState == MenuState.Play)
            {
                GameManager.Instance.Pause();
                ToggleMenuState(MenuState.Paused);
            }
            else if (_menuState == MenuState.Paused)
            {
                GameManager.Instance.Resume();
                ToggleMenuState(MenuState.Play);
            }
        }

        HandleSongButtons();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartSong(Mathf.RoundToInt(SongButtonOffset));
        }
    }
}
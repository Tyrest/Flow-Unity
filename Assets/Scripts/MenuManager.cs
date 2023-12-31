using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private float SongButtonOffset
    {
        get => _songButtonOffset;
        set => _songButtonOffset = Mathf.Clamp(value, -0.5f, _songButtons.Count - 0.5f);
    }

    public float scrollSpeed = 10f;
    public GameObject buttonPrefab;
    public GameObject quitButton;
    public GameObject optionsButton;
    public GameObject startButton;
    private List<GameObject> _songButtons;
    private float _songButtonOffset;

    private void Awake()
    {
        _instance = this;
        _songButtons = new List<GameObject>();
    }

    private void ToggleButtons(bool toggle)
    {
        quitButton.gameObject.SetActive(toggle);
        optionsButton.gameObject.SetActive(toggle);
        startButton.gameObject.SetActive(toggle);
    }

    private static UnityEngine.Events.UnityAction CreateStartSongAction(int count)
    {
        return () => { StartSong(count); };
    }


    public void SongSelect()
    {
        ToggleButtons(false);
        SongManager.Instance.GetSongs().ForEach(song =>
        {
            var button = Instantiate(buttonPrefab, transform);
            button.transform.localPosition += Vector3.down * (_songButtons.Count * 3);
            button.GetComponentInChildren<TMPro.TextMeshPro>().text = song.Title;
            button.gameObject.SetActive(true);
            button.GetComponent<SongTarget>().AddInteraction(CreateStartSongAction(_songButtons.Count));
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

    private static void StartSong(int index)
    {
        DontDestroyOnLoad(SongManager.Instance.gameObject);
        SongManager.Instance.currentSongIndex = index;
        SongManager.Instance.currentDifficulty = "intermediate";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");

        // Destroy(MenuManager.Instance.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleButtons(true);
            _songButtons.ForEach(Destroy);
            _songButtons.Clear();
        }

        HandleSongButtons();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartSong(Mathf.RoundToInt(SongButtonOffset));
        }
    }
}
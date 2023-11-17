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

    public float SongButtonOffset
    {
        get => _songButtonOffset;
        set => _songButtonOffset = Mathf.Clamp(value, -0.5f, _songButtons.Count - 0.5f);
    }

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

        SongButtonOffset += Input.GetAxis("Mouse ScrollWheel");
        
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
            SongManager.Instance.currentSongIndex = _songButtons.Count - 1 - (int)_songButtonOffset;
            SongManager.Instance.currentSong = SongManager.Instance.GetSongs()[SongManager.Instance.currentSongIndex];
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }
}
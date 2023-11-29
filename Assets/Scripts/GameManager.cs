using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public enum TargetScore
{
    Perfect,
    Great,
    Good,
    Miss,
    Empty
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    private const float MaxDistance = 100f;

    private readonly Dictionary<TargetScore, int> _scoreMapping = new Dictionary<TargetScore, int>
    {
        { TargetScore.Perfect, 300 },
        { TargetScore.Great, 200 },
        { TargetScore.Good, 100 },
        { TargetScore.Miss, 0 },
        { TargetScore.Empty, 0 }
    };

    public AudioSource audioSource;

    public HUD hud;
    public GameObject targetPrefab;
    public bool spawnRandom;
    public float minDistance = 10f;
    public float maxDistance = 20f;
    public float minYSpawnAngle = -30f;
    public float maxYSpawnAngle = 30f;
    public float minXSpawnAngle = -90f;
    public float maxXSpawnAngle = 90f;
    public float spawnPeriod = 1.0f;
    public float graceTime = 0.0f;

    private Camera _mainCamera;
    private float _spawnTimer;
    private float _songTimer;
    private BeatMap _beatMap;
    private int _beatIndex;
    private float _songTime;

    private int _score;

    private void Awake()
    {
        _instance = this;
        _songTimer = -graceTime;
    }

    private async void Start()
    {
        _mainCamera = Camera.main;
        hud.UpdateScore(_score);
        var song = SongManager.Instance.GetSong();
        if (song == null)
        {
            spawnRandom = true;
        }
        else
        {
            spawnRandom = false;
            _beatMap = SongManager.Instance.GetBeatMap();
            audioSource.clip = await song.GetAudio();
            _songTime = song.Offset;
        }
    }

    private void SpawnTarget(float x, float y, float distance)
    {
        var target = Instantiate(targetPrefab, this.transform);

        var t = transform;
        var position = t.position + t.forward * distance;
        position = Quaternion.AngleAxis(y, t.right) * position;
        position = Quaternion.AngleAxis(x, t.up) * position;
        target.transform.position = position;
    }

    private void SpawnRandom()
    {
        var randomSpawnX = Random.Range(minXSpawnAngle, maxXSpawnAngle);
        var randomSpawnY = Random.Range(minYSpawnAngle, maxYSpawnAngle);
        var randomDistance = Random.Range(minDistance, maxDistance);
        SpawnTarget(randomSpawnX, randomSpawnY, randomDistance);
    }

    private void CheckClick()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, MaxDistance, LayerMask.GetMask("Target")) ||
            !hit.collider.gameObject.transform.parent.TryGetComponent(out Target target))
        {
            return;
        }

        _score += _scoreMapping[target.Hit()];
        hud.UpdateScore(_score);
    }

    private void Update()
    {
        if (spawnRandom) HandleRandomSpawn();
        else HandleSongSpawn();

        CheckClick();
    }

    private void HandleRandomSpawn()
    {
        _spawnTimer += Time.deltaTime;
        if (!(_spawnTimer > spawnPeriod)) return;
        SpawnRandom();
        _spawnTimer = 0f;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"X: {_songTimer}");
    }

    private void HandleSongSpawn()
    {
        _songTimer += Time.deltaTime / 2;

        if (!audioSource.isPlaying && _songTimer >= 0)
        {
            audioSource.time = _songTime;
            audioSource.Play();
        }
        
        while (_beatIndex < _beatMap.Beats.Count &&
               _songTimer >= _beatMap.Beats[_beatIndex].beatTime - _beatMap.approachPeriod)
        {
            var beat = _beatMap.Beats[_beatIndex];
            SpawnRandom();
            // SpawnTarget(beat.x, beat.y, beat.distance);
            _beatIndex += 1;
        }

        _songTimer += Time.deltaTime / 2;
    }
}
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
    private float _songTime;
    private BeatMap _beatMap;
    private int _beatIndex;
    private bool _playing;

    private int _score;

    private void Awake()
    {
        _instance = this;
        _songTime = -graceTime;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        hud.UpdateScore(-1);
        _playing = false;
    }
    
    public async void StartSong()
    {
        _songTime = -graceTime;
        _beatIndex = 0;
        _playing = true;
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
            audioSource.time = song.Offset;
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
        if (!_playing)
        {
            return;
        }
        if (spawnRandom) HandleRandomSpawn();
        else HandleSongSpawn();

        CheckClick();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _playing = false;
            audioSource.Stop();
            hud.UpdateScore(-1);
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void HandleRandomSpawn()
    {
        _spawnTimer += Time.deltaTime;
        if (!(_spawnTimer > spawnPeriod)) return;
        SpawnRandom();
        _spawnTimer = 0f;
    }

    private void HandleSongSpawn()
    {
        _songTime += Time.deltaTime / 2;
        // Debug.Log(_songTime);

        if (!audioSource.isPlaying && _songTime >= 0)
        {
            audioSource.Play();
        }

        while (_beatIndex < _beatMap.Beats.Count &&
               _songTime >= _beatMap.Beats[_beatIndex].beatTime - _beatMap.approachPeriod)
        {
            var beat = _beatMap.Beats[_beatIndex];
            // SpawnRandom();
            SpawnTarget(beat.x, beat.y, beat.distance);
            _beatIndex += 1;
        }

        _songTime += Time.deltaTime / 2;
    }
}
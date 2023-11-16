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
            if (_instance == null)
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
    public VisualEffect explosion;

    public HUD hud;
    public GameObject targetPrefab;
    public bool spawnRandom = true;
    public float minDistance = 10f;
    public float maxDistance = 20f;
    public float minYSpawnAngle = -30f;
    public float maxYSpawnAngle = 30f;
    public float minXSpawnAngle = -90f;
    public float maxXSpawnAngle = 90f;
    public float spawnPeriod = 1.0f;

    private Camera _mainCamera;
    private float _spawnTimer;

    private int _score;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        hud.UpdateScore(_score);
    }

    private void SpawnRandom()
    {
        var randomDistance = Random.Range(minDistance, maxDistance);
        var randomSpawnY = Random.Range(minYSpawnAngle, maxYSpawnAngle);
        var randomSpawnX = Random.Range(minXSpawnAngle, maxXSpawnAngle);
        var target = Instantiate(targetPrefab, this.transform);

        var t = transform;
        var position = t.position + t.forward * randomDistance;
        position = Quaternion.AngleAxis(randomSpawnY, t.right) * position;
        position = Quaternion.AngleAxis(randomSpawnX, t.up) * position;
        target.transform.position = position;
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
        if (spawnRandom)
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer > spawnPeriod)
            {
                SpawnRandom();
                _spawnTimer = 0f;
            }
        }

        CheckClick();
    }
}
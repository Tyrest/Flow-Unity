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
    Miss
}

public class GameManager : MonoBehaviour
{
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
    public int perfectScore = 300;
    public int greatScore = 200;
    public int goodScore = 100;
    public int missScore = 0;
    
    private float _spawnTimer = 0f;

    private int _score = 0;
    // private HashSet<GameObject> _objects = new HashSet<GameObject>();

    void SpawnRandom()
    {
        float randomDistance = Random.Range(minDistance, maxDistance);
        float randomSpawnY = Random.Range(minYSpawnAngle, maxYSpawnAngle);
        float randomSpawnX = Random.Range(minXSpawnAngle, maxXSpawnAngle);
        GameObject target = Instantiate(targetPrefab);
        target.transform.parent = this.transform;
        Vector3 position = transform.position + transform.forward * randomDistance;
        // rotate the position vector by the random angles
        position = Quaternion.AngleAxis(randomSpawnY, transform.right) * position;
        position = Quaternion.AngleAxis(randomSpawnX, transform.up) * position;
        target.transform.position = position;
        // _objects.Add(cube);
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.GetComponentInParent<Target>() != null)
            {
                TargetScore score = hit.collider.gameObject.GetComponentInParent<Target>().Hit();
                switch (score)
                {
                    case TargetScore.Perfect:
                        _score += perfectScore;
                        break;
                    case TargetScore.Great:
                        _score += greatScore;
                        break;
                    case TargetScore.Good:
                        _score += goodScore;
                        break;
                    case TargetScore.Miss:
                        _score += missScore;
                        break;
                }
                Debug.Log("Accuracy: " + score + "\nScore: " + _score);
                hud.UpdateScore(_score);
            }

        }
    }


    // Update is called once per frame
    void Update()
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

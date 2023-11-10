using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public bool spawnRandom = true;
    public float minDistance = 10f;
    public float maxDistance = 20f;
    public float minYSpawnAngle = -30f;
    public float maxYSpawnAngle = 30f;
    public float minXSpawnAngle = -90f;
    public float maxXSpawnAngle = 90f;
    public float spawnPeriod = 1.0f;
    
    private float _spawnTimer = 0f;
    private HashSet<GameObject> _objects = new HashSet<GameObject>();

    void SpawnRandom()
    {
        float randomDistance = Random.Range(minDistance, maxDistance);
        float randomSpawnY = Random.Range(minYSpawnAngle, maxYSpawnAngle);
        float randomSpawnX = Random.Range(minXSpawnAngle, maxXSpawnAngle);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cube.transform.parent = this.transform;
        Vector3 position = transform.position + transform.forward * randomDistance;
        // rotate the position vector by the random angles
        position = Quaternion.AngleAxis(randomSpawnY, transform.right) * position;
        position = Quaternion.AngleAxis(randomSpawnX, transform.up) * position;
        cube.transform.position = position;
        cube.AddComponent<Rigidbody>();
        cube.GetComponent<Rigidbody>().useGravity = false;
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.AddComponent<VisualEffect>();
        _objects.Add(cube);
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && _objects.Contains(hit.collider.gameObject))
            {
                if (hit.collider.gameObject.GetComponent<Renderer>().material.color == Color.red)
                {
                    hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
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

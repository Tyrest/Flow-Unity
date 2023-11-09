using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public bool spawn_random = true;
    float min_distance = 10f;
    float max_distance = 20f;
    float min_y_spawn_angle = -30f;
    float max_y_spawn_angle = 30f;
    float min_x_spawn_angle = -30f;
    float max_x_spawn_angle = 30f;
    float spawn_timer = 0f;

    protected List<GameObject> objects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SpawnRandom()
    {
        float random_distance = Random.Range(min_distance, max_distance);
        float random_spawn_y = Random.Range(min_y_spawn_angle, max_y_spawn_angle);
        float random_spawn_x = Random.Range(min_x_spawn_angle, max_x_spawn_angle);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = this.transform;
        Vector3 position = transform.position + transform.forward * random_distance;
        position.y += Mathf.Tan(random_spawn_y * Mathf.Deg2Rad) * random_distance;
        position.x += Mathf.Tan(random_spawn_x * Mathf.Deg2Rad) * random_distance;
        cube.transform.position = position;
        cube.AddComponent<Rigidbody>();
        cube.GetComponent<Rigidbody>().useGravity = false;
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.AddComponent<VisualEffect>();
        objects.Add(cube);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn_random)
        {
            spawn_timer += Time.deltaTime;
            if (spawn_timer > 0.1f)
            {
                SpawnRandom();
                spawn_timer = 0f;
            }
        }
    }
}

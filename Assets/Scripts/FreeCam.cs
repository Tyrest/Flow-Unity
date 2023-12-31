using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float sensitivity = 1f;
    private float _rotationX;
    private float _rotationY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Move()
    {
        var t = transform;
        if (Input.GetKey(KeyCode.W))
        {
            t.position += t.forward * (Time.deltaTime * 10f);
        }

        if (Input.GetKey(KeyCode.S))
        {
            t.position -= t.forward * (Time.deltaTime * 10f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            t.position -= t.right * (Time.deltaTime * 10f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            t.position += t.right * (Time.deltaTime * 10f);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            t.position += t.up * (Time.deltaTime * 10f);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            t.position -= t.up * (Time.deltaTime * 10f);
        }

        transform.position = t.position;
    }

    private void Rotate()
    {
        _rotationX += Input.GetAxis("Mouse X") * sensitivity;
        _rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);

        var t = Quaternion.AngleAxis(_rotationX, Vector3.up);
        t *= Quaternion.AngleAxis(_rotationY, Vector3.left);
        transform.localRotation = t;
    }

    // private void OnGUI()
    // {
    //     GUI.Label(new Rect(10, 10, 100, 20), $"X: {_rotationX}");
    //     GUI.Label(new Rect(10, 30, 100, 20), $"Y: {_rotationY}");
    // }

    private void Update()
    {
        // Move();
        Rotate();
    }
}
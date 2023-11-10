using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Range(0f, 2.0f)]
    public float scaleFactor = 0.5f; // Adjust this variable to control the scaling factor
    public float scalePeriod = 3.0f; // Adjust this variable to control the time from 0 to 1

    private GameObject _childObject;
    private Vector3 _originalScale;
    private float _time;

    void Start()
    {
        // Create a child object with a MeshFilter
        CreateChildObject();
        _time = 0.0f;
    }

    void CreateChildObject()
    {
        // Create a child GameObject
        _childObject = new GameObject("TimingObject");
        _childObject.transform.parent = transform; // Make it a child of the current GameObject
        _childObject.transform.localPosition = Vector3.zero;
        
        MeshFilter meshFilter = _childObject.AddComponent<MeshFilter>();
        meshFilter.mesh = GetComponent<MeshFilter>().sharedMesh;
        
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        Color color = transform.GetComponent<MeshRenderer>().sharedMaterial.color;
        color.a = 1.0f;
        mpb.SetColor("_Color", color);
        
        MeshRenderer meshRenderer = _childObject.AddComponent<MeshRenderer>();
        meshRenderer.material = GetComponent<MeshRenderer>().sharedMaterial;
        meshRenderer.SetPropertyBlock(mpb);
        
        _originalScale = _childObject.transform.localScale;
    }

    void Update()
    {
        Vector3 scaleValue = _originalScale * (scaleFactor * _time / scalePeriod);
        _time += Time.deltaTime;

        // Apply the new scale to the child object
        _childObject.transform.localScale = scaleValue;
    }
}
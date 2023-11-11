using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject target;
    public GameObject scoreText;
    public float scaleStart = 3.0f; // Adjust this variable to control the scaling factor
    public float scalePeriod = 0.5f; // Adjust this variable to control the time from 0 to 1
    public float perfectThreshold = 0.1f;
    public float greatThreshold = 0.2f;
    public float goodThreshold = 0.3f; // Also determines fade out time
    public Color outlineColor = Color.white;

    private GameObject _childObject;
    private Vector3 _originalScale;
    private Color _startingOutlineColor;
    private float _time = 0f;
    private bool _hit = false;
    private TargetScore _score = TargetScore.Miss;
    private bool _pastPeak = false;

    void Start()
    {
        _startingOutlineColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.0f);
        // Create a child object with a MeshFilter
        CreateChildObject();
        _time = 0.0f;
    }

    void CreateChildObject()
    {
        // Create a child GameObject
        _childObject = new GameObject("TimingObject");
        _childObject.transform.parent = target.transform; // Make it a child of the current GameObject
        _childObject.transform.localPosition = Vector3.zero;
        
        MeshFilter meshFilter = _childObject.AddComponent<MeshFilter>();
        meshFilter.mesh = target.GetComponent<MeshFilter>().sharedMesh;
        
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        Color color = target.GetComponent<MeshRenderer>().sharedMaterial.color;
        color.a = 0.0f;
        mpb.SetColor("_Color", color);
        
        MeshRenderer meshRenderer = _childObject.AddComponent<MeshRenderer>();
        meshRenderer.material = target.GetComponent<MeshRenderer>().sharedMaterial;
        meshRenderer.SetPropertyBlock(mpb);
        
        var outline = _childObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = _startingOutlineColor;
        outline.OutlineWidth = 5f;
        
        _originalScale = _childObject.transform.localScale;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
    
    void TurnOffRenderer()
    {
        Renderer rendererComponent = target.GetComponent<MeshRenderer>();
        rendererComponent.enabled = false;
        Outline outlineComponent = _childObject.GetComponent<Outline>();
        if (outlineComponent != null)
        {
            outlineComponent.enabled = false;
        }

        MeshRenderer meshRendererComponent = _childObject.GetComponent<MeshRenderer>();
        if (meshRendererComponent != null)
        {
            meshRendererComponent.enabled = false;
        }
    }
    
    IEnumerator FadeOutCoroutine(float fadeDuration)
    {
        Renderer rendererComponent = target.GetComponent<MeshRenderer>();
        float startAlpha = rendererComponent.material.color.a;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            Color newColor = rendererComponent.material.color;
            newColor.a = newAlpha;
            rendererComponent.material.color = newColor;
            elapsedTime += Time.deltaTime;
            
            _childObject.GetComponent<Outline>().OutlineColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, newAlpha);

            yield return null;
        }
        
        TurnOffRenderer();
        
        if (!_hit)
        {
            StartCoroutine(DisplayScoreCoroutine());
        }
    }
    
    IEnumerator DisplayScoreCoroutine()
    {
        GameObject text = Instantiate(scoreText, transform, true);
        TextMeshPro textTMP = text.GetComponent<TextMeshPro>();
        textTMP.text = _score.ToString();
        text.transform.localPosition = Vector3.zero;
        text.transform.LookAt(Vector3.zero, Vector3.up);
        text.transform.Rotate(0f, 180f, 0f);
        
        Color textColor = textTMP.color;
        Color clearColor = new Color(textColor.r, textColor.g, textColor.b, 0f);
        
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            text.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (float) Math.Exp(-Math.Pow(2 * (elapsedTime - 0.25f), 2)));
            textTMP.color = Color.Lerp(clearColor, textColor, (float) Math.Exp(-Math.Pow(4 * (elapsedTime - 0.25f), 4)));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }
        
        Destroy(gameObject);
    }

    public TargetScore Hit()
    {
        if (_hit || _time > goodThreshold + scalePeriod)
        {
            return TargetScore.Empty;
        }
        _hit = true;
        switch (Mathf.Abs(_time - scalePeriod))
        {
            case float n when n < perfectThreshold:
                _score = TargetScore.Perfect;
                break;
            case float n when n < greatThreshold:
                _score = TargetScore.Great;
                break;
            case float n when n < goodThreshold:
                _score = TargetScore.Good;
                break;
            default:
                _score = TargetScore.Miss;
                break;
        }
        TurnOffRenderer();
        StartCoroutine(DisplayScoreCoroutine());
        return _score;
    }

    void Update()
    {
        if (!_hit)
        {
            if (_time < scalePeriod)
            {
                Vector3 scaleValue = Vector3.Lerp(Vector3.one * scaleStart, Vector3.one, _time / scalePeriod);
                _childObject.transform.localScale = scaleValue;
                Color outlineColorValue = Color.Lerp(_startingOutlineColor, outlineColor, _time * 2.0f / scalePeriod);
                _childObject.GetComponent<Outline>().OutlineColor = outlineColorValue;
            }
            else if (!_pastPeak)
            {
                _pastPeak = true;
                StartCoroutine(FadeOutCoroutine(goodThreshold));
            }
        }
        _time += Time.deltaTime;
    }
}
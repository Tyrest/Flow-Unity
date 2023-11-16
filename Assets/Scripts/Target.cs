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
    private Color _startingOutlineColor;
    private float _time;
    private bool _hit;
    private TargetScore _score = TargetScore.Miss;
    private bool _pastPeak;
    
    private MeshRenderer _targetMeshRenderer;
    private MeshRenderer _childMeshRenderer;
    private Outline _childOutline;
    
    private void Awake()
    {
        _targetMeshRenderer = target.GetComponent<MeshRenderer>();
        _startingOutlineColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.0f);
        // Create a child object with a MeshFilter
        CreateChildObject();
        
        _time = 0.0f;
    }

    private void CreateChildObject()
    {
        // Create a child GameObject
        _childObject = new GameObject("TimingObject")
            { transform = { parent = target.transform, localPosition = Vector3.zero } };

        var meshFilter = _childObject.AddComponent<MeshFilter>();
        meshFilter.mesh = target.GetComponent<MeshFilter>().sharedMesh;

        var mpb = new MaterialPropertyBlock();
        var color = target.GetComponent<MeshRenderer>().sharedMaterial.color;
        color.a = 0.0f;
        mpb.SetColor(Shader.PropertyToID("_Color"), color);

        var meshRenderer = _childObject.AddComponent<MeshRenderer>();
        meshRenderer.material = target.GetComponent<MeshRenderer>().sharedMaterial;
        meshRenderer.SetPropertyBlock(mpb);

        var outline = _childObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = _startingOutlineColor;
        outline.OutlineWidth = 5f;
        
        _childMeshRenderer = _childObject.GetComponent<MeshRenderer>();
        _childOutline = _childObject.GetComponent<Outline>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }

    private void TurnOffRenderer()
    {
        if (_targetMeshRenderer)
        {
            _targetMeshRenderer.enabled = false;
        }

        if (_childMeshRenderer)
        {
            _childMeshRenderer.enabled = false;
        }
    }

    private IEnumerator FadeOutCoroutine(float fadeDuration)
    {
        var startAlpha = _targetMeshRenderer.material.color.a;
        var elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            var newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            var m = _targetMeshRenderer.material;
            m.color = new Color(m.color.r, m.color.g, m.color.b, newAlpha);

            elapsedTime += Time.deltaTime;

            _childOutline.OutlineColor =
                new Color(outlineColor.r, outlineColor.g, outlineColor.b, newAlpha);

            yield return null;
        }

        TurnOffRenderer();

        if (!_hit)
        {
            StartCoroutine(DisplayScoreCoroutine());
        }
    }

    private IEnumerator DisplayScoreCoroutine()
    {
        var text = Instantiate(scoreText, transform, true);
        var textTMP = text.GetComponent<TextMeshPro>();
        textTMP.text = _score.ToString();
        text.transform.localPosition = Vector3.zero;
        text.transform.LookAt(Vector3.zero, Vector3.up);
        text.transform.Rotate(0f, 180f, 0f);

        var textColor = textTMP.color;
        var clearColor = new Color(textColor.r, textColor.g, textColor.b, 0f);

        var elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            text.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one,
                (float)Math.Exp(-Math.Pow(2 * (elapsedTime - 0.25f), 2)));
            textTMP.color = Color.Lerp(clearColor, textColor, (float)Math.Exp(-Math.Pow(4 * (elapsedTime - 0.25f), 4)));
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
            case var n when n < perfectThreshold:
                _score = TargetScore.Perfect;
                break;
            case var n when n < greatThreshold:
                _score = TargetScore.Great;
                break;
            case var n when n < goodThreshold:
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
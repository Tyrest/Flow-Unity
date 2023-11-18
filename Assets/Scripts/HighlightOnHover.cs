using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is to be put on the Camera object
// It will raycast from the center of the screen and if it hits something, check if it has the outline property
// If it does, it will enable the outline
// When it stops hitting something, it will disable the outline
// This is to be used for the button targets
public class HighlightOnHover : MonoBehaviour
{
    private Camera _mainCamera;
    private Outline _currentOutline;

    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        var ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (!Physics.Raycast(ray, out var hit) ||
            !hit.collider.TryGetComponent<Outline>(out var outline))
        {
            if (!_currentOutline) return;
            _currentOutline.enabled = false;
            _currentOutline = null;
        }
        else if (_currentOutline != outline)
        {
            if (_currentOutline)
            {
                _currentOutline.enabled = false;
            }

            _currentOutline = outline;
            _currentOutline.enabled = true;
        }
    }
}
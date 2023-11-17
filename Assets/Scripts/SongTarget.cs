using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongTarget : MonoBehaviour
{
    [SerializeField] private Outline outline;

    public void ToggleOutline(bool toggle)
    {
        outline.enabled = toggle;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongTarget : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private Interact interact;

    public void ToggleOutline(bool toggle)
    {
        outline.enabled = toggle;
    }

    public void AddInteraction(UnityEngine.Events.UnityAction action)
    {
        interact.interactEvent.AddListener(action);
    }
}
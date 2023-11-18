using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))] //A collider is needed to receive clicks
public class Interact : MonoBehaviour
{
    public UnityEvent interactEvent;

    private void OnMouseDown()
    {
        interactEvent.Invoke();
    }
}
using System;
using System.Collections.Generic;
using Project.Interfaces;
using UnityEngine;

public class Interaction : CoreComponent
{
    private readonly List<IInteractable> interactables = new List<IInteractable>();

    public event Action<IInteractable> OnInteract;
    
    private IInteractable currentInteractable;
    private float distanceToCurrentInteractable;

    public void TriggerInteraction(bool value)
    {        
        if (currentInteractable == null || !value) return;
        OnInteract?.Invoke(currentInteractable);
    }
    
    private void Update()
    {
        if (currentInteractable != null)
        {
            distanceToCurrentInteractable = Vector3.Distance(transform.position, currentInteractable.GetPosition());
        }

        foreach (IInteractable interactable in interactables)
        {
            if(currentInteractable == interactable) continue;
            
            if (currentInteractable == null)
            {
                currentInteractable = interactable;
                distanceToCurrentInteractable = Vector3.Distance(transform.position, currentInteractable.GetPosition());
                currentInteractable.EnableInteraction();
                continue;
            }

            if (Vector3.Distance(transform.position, interactable.GetPosition()) < distanceToCurrentInteractable)
            {
                currentInteractable.DisableInteraction();
                currentInteractable = interactable;
                distanceToCurrentInteractable = Vector3.Distance(transform.position, currentInteractable.GetPosition());
                currentInteractable.EnableInteraction();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out IInteractable interactable))
        {
            interactables.Add(interactable);            
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.TryGetComponent(out IInteractable interactable)) return;
        
        if (interactable == currentInteractable)
        {
            interactable.DisableInteraction();
            currentInteractable = null;
        }
        interactables.Remove(interactable);
    }

    private void OnDrawGizmos()
    {
        foreach (IInteractable interactable in interactables)
        {
            Gizmos.color = interactable == currentInteractable ? Color.red : Color.white;

            Gizmos.DrawLine(transform.position, interactable.GetPosition());
        }
    }
}
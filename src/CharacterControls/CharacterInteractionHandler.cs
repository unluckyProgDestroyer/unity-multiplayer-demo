using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInteractionHandler : MonoBehaviour
{
    [SerializeField]
    CharacterWeaponHandler _weaponHandler;
    [SerializeField]
    CharacterThrowableHandler _throwableHandler;
    List<Interactable> _interactablesInRange = new List<Interactable>();
    bool _interactInput = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Interactable")) return;

        Interactable interactable = collision.GetComponent<Interactable>();
        _interactablesInRange.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Interactable")) return;

        Interactable interactable = collision.GetComponent<Interactable>();
        _interactablesInRange.Remove(interactable);
    }

    float CalculateInteractableDistance2D(Interactable item)
    {
        Vector2 objectPosition2d = new Vector2(transform.position.x, transform.position.y);
        Vector2 itemPosition2d = new Vector2(item.transform.position.x, item.transform.position.y);
        return (objectPosition2d-itemPosition2d).magnitude;
    }

    int FindIndexOfClosestInteractable()
    {
        float min = float.MaxValue;
        int minimumIndex = -1;
        for (int i = 0; i < _interactablesInRange.Count; i++)
        {
            float distance = CalculateInteractableDistance2D(_interactablesInRange[i]);
            if (distance < min)
            {
                min = distance;
                minimumIndex = i;
            }
        }
        return minimumIndex;
    }

    void Interact()
    {
        if (_interactablesInRange.Count == 0) return;

        int indexOfClosest = FindIndexOfClosestInteractable();
        
        if (indexOfClosest == -1) return;
        
        _interactablesInRange[indexOfClosest].Interact(this);
    }

    private void Update()
    { 
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        _weaponHandler.AddWeapon(weapon);
    }

    public void PickUpThrowable(Throwable throwable)
    {
        _throwableHandler.AddThrowable(throwable);
    }
}

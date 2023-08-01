using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCursor : NetworkBehaviour
{
    [SerializeField]
    bool onlyFlip = true;

    [SyncVar(hook = nameof(rotationChanged))]
    Quaternion _rotation;

    // Update is called once per frame
    void Update()
    {
        DoLookAtCursor();
    }

    void DoLookAtCursor()
    {
        if (!isLocalPlayer) return;
        
        if (!hasAuthority)
        {
            return;
        }

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotation = 0;
        if (!onlyFlip)
        {
            rotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        if (difference.x > 0)
        {
            SetRotation(Quaternion.Euler(0f, 0f, rotation));
        }
        else
        {
            if(!onlyFlip) rotation = 180 - rotation;
            SetRotation(Quaternion.Euler(0f, 180f, rotation));
        }

    }

    [Command]
    public void SetRotation(Quaternion rotation)
    {
        _rotation = rotation;
        transform.rotation = rotation;
    }

    private void rotationChanged(Quaternion oldRotation, Quaternion newRotation)
    {
        transform.rotation = newRotation;
    }

}

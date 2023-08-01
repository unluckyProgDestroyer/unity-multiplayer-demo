using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    GameObject characterMovementHandler;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag =="Ground")
        {
            characterMovementHandler.SendMessage("changeGrounded",true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            characterMovementHandler.SendMessage("changeGrounded", false);
        }
    }
}

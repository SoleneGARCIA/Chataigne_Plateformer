using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trigger : MonoBehaviour
{
    private bool isTriggered = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Joueur"))
        {
            Debug.Log("je rentre dans le trigger");
        }
    }
    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Joueur"))
        {
            Debug.Log("je sors du trigger");
        }
    }
}

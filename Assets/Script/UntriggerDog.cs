using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UntriggerDog : MonoBehaviour
{
    private bool isTriggered = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Joueur"))
        {
            if (!isTriggered){
                isTriggered = true;
                GameObject hades = GameObject.Find("HADES");
                Ennemy e = hades.GetComponent<Ennemy>();
                e.setAggro(false);
                GameObject follow = GameObject.Find("Camera Run");
                CameraRun cr = follow.GetComponent<CameraRun>();
                cr.toggleFollowPlayer(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Joueur"))
        {
            if (isTriggered){
                isTriggered = false;
            }
        }
    }
}

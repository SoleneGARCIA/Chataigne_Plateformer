using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerDog : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Joueur"))
        {
            GameObject hades = GameObject.Find("HADES");
            Ennemy e = hades.GetComponent<Ennemy>();
            e.setAggro();
        }
    }
}

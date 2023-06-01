using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damager : MonoBehaviour
{
    [SerializeField] public int damageValue;
    bool isHurt = false;
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Joueur"))
        {
            if (isHurt == false)
            {
                isHurt = true;
                Player player = collision.gameObject.GetComponent<Player>();
                player.hurt(damageValue);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Joueur"))
        {
            isHurt = false;
        }
    }


}

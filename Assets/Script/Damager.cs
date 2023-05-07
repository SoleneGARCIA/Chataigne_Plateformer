using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damager : MonoBehaviour
{
    [SerializeField] public int damageValue;
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Joueur"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            Debug.Log(player.health);
            player.hurt(damageValue);
        }
    }


}

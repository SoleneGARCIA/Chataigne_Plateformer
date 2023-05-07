using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{

    [SerializeField] public Vector2 spawnCoords;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        spawnCoords = rb.position;
    }

 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Joueur"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.setSpawnPoint(spawnCoords);
        }
    }


}

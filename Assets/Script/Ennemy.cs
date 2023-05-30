using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animController;
    [SerializeField] private bool isAggro = false;
    [SerializeField] private float aggroDistance = 30f;
    [SerializeField] private float speed = 20f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
    }

    void Update()
    {
        Player player = GameObject.Find("Player").GetComponent<Player>();
        Vector2 playerPosition = player.getCurrentCoords();
        Vector2 currentPosition = rb.transform.position;
        Vector2 direction = playerPosition - currentPosition;
        direction = direction.normalized;
        if (isAggro){
            direction *= speed;
            rb.velocity += new Vector2(direction.x, 0);
        }
    }

    public void setAggro(){
        isAggro = true;
    }

}
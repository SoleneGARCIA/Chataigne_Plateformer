using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    public int heartNumber;
    private GameObject player;
    private Player p;
    private Image image;

    void Start(){
        player = GameObject.Find("Player");
        p = player.GetComponent<Player>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (p.health >= heartNumber){
            image.enabled = true;
        }
        else{
            image.enabled = false;
        }
    }
}
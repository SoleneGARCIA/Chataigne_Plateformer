using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeCountUI : MonoBehaviour
{
    public Image[] lives;
    public int livesRemaining;

    public void LoseLife()
    {
        // Diminue la valeur de "livesRemaining"
        livesRemaining--;
        //Cache une des vies (images)
        lives[livesRemaining].enabled = false;

        //Si on n'a plus de vie, on perd 
        if (livesRemaining == 0)
        {
            Debug.Log("YOU LOST");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            LoseLife();
    }

}

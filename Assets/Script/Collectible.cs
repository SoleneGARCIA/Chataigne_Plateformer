using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Collectible : MonoBehaviour
{
    public float boostDuration = 5f;
    private bool isBoostActive = false;

    public void ActivateBoost()
    {
        if (!isBoostActive)
        {
            isBoostActive = true;
            Debug.Log("Boost activé pendant " + boostDuration + " secondes.");

            StartCoroutine(BoostCoroutine());
        }
        else
        {
            Debug.Log("Le boost est déjà actif.");
        }
    }

    private IEnumerator BoostCoroutine()
    {
        yield return new WaitForSeconds(boostDuration);

        isBoostActive = false;
        Debug.Log("Boost terminé.");
    }
}

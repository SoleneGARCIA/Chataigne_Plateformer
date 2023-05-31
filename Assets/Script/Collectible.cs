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
            Debug.Log("Boost activ� pendant " + boostDuration + " secondes.");

            StartCoroutine(BoostCoroutine());
        }
        else
        {
            Debug.Log("Le boost est d�j� actif.");
        }
    }

    private IEnumerator BoostCoroutine()
    {
        yield return new WaitForSeconds(boostDuration);

        isBoostActive = false;
        Debug.Log("Boost termin�.");
    }
}

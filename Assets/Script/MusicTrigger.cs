using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicTrigger : MonoBehaviour
{

    private AudioSource audioSource;
    private bool isPlaying = false;
    [SerializeField] private AudioClip currentMusic;

    private void Start(){
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1;
        Debug.Log("AudioSource : " + audioSource);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Joueur") && !isPlaying){
            Debug.Log("Lancement de la musique");
            isPlaying = true;
            audioSource.clip = currentMusic;
            audioSource.Play();
        }
    }

    private void OnTriggerStay2D(Collider2D collision){
        Debug.Log("est dans la zone de musique");
        if(collision.gameObject.CompareTag("Joueur") && !isPlaying){
            Debug.Log("Lancement de la musique");
            isPlaying = true;
            audioSource.clip = currentMusic;
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Joueur") && isPlaying){
            Debug.Log("Arret de la musique");
            isPlaying = false;
            audioSource.Stop();
        }
    }


    private IEnumerator FadeOut(){
        while(audioSource.volume > 0){
            audioSource.volume -= 1 * Time.deltaTime;
            yield return null;
        }
        audioSource.Stop();
    }

    private IEnumerator FadeIn(){
        float volume = audioSource.volume;
        while(audioSource.volume < 1){
            audioSource.volume += 1 * Time.deltaTime;
            yield return null;
        }
    }
}

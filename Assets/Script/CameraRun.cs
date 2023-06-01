using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraRun : MonoBehaviour
{
    [SerializeField] private bool followPlayer = true;
    private GameObject player;
    private Player p;
    private GameObject camera;
    private Camera cam;
    private CinemachineVirtualCamera cvc;
    
    void Start(){
        player = GameObject.Find("Player");
        camera = GameObject.Find("CM vcam1");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        p = player.GetComponent<Player>();
        cvc = camera.GetComponent<CinemachineVirtualCamera>();
    }

    void Update(){
        if (player.transform.position.x < camera.transform.position.x - 10 - cam.orthographicSize/2 ||
            player.transform.position.x > camera.transform.position.x + 10 + cam.orthographicSize/2){
            p.hurt(999);
            if(!followPlayer){
                toggleFollowPlayer();
            }
            GameObject.Find("HADES").GetComponent<Ennemy>().reset();
        }
    }

    public void toggleFollowPlayer(){
        followPlayer = !followPlayer;
        if(followPlayer){
            cvc.Follow = player.transform;
            cvc.LookAt = player.transform;
        }
        else{
            gameObject.transform.position = player.transform.position;
            cvc.Follow = gameObject.transform;
            cvc.LookAt = gameObject.transform;
        }
    }
}
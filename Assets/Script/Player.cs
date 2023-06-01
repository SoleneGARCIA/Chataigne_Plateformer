using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
DONE
- Collectibles (ca donnerai un boost au chat pendant la course poursuite, maybe desactiver le dash pour pas pouvoir le spam)
-Faire defiler les nuages, paralaxe
- S'accroupir done ------------> (Il peut crouch et courir en même temps) (PEUT-ÊTRE LE GARDER)
- Courir done ----------------->(Sunny a un saut amplifié quand tu sautes en pentes ou en prenant de l'elant avec la course)rajouter une velocité 
de force moins forte pour pas qu'il y ai l'ajout de force sur les pentes
- CROUCH BUG : exemple : si je marche mais que je veux crouch l'animation a une latence mais la lenteur du crouch est la (si j'appuie deux touches en mm temps)
- Que la barre de vie soit de -1 en -1 et non pas de -2 en -2 
-Je voudrais que pendant la course poursuite, le chat ne puisse pas retourner en arrière, la camera avancerai, soit il meurt parce que le chien l'a touché, soit parce 
qu'il est trop loin dans la map, il aurait des boosts pour gagner du terrain sur le chien 

DOING

Diminuer le player quand il crouch
Mettre une fin a la course poursuite
Faire mes UI start, crédit
Mettre le crouch sur la fleche du bas et le jump sur la fleche du haut
importer mes anims


BUG FIXES

- Je veux qu'on ai du mal a sauter dans le Mud, que les deplacements soit embourbés


- Supprimer l'accélération sur le box collider des ladders
    faut separer les deux modificateur de vitesse dans le calcul de velocity, un pour le vertical, l'autre pour l'horizontal exemple : courrir = horizontal speed boost
    ca permet de mieux controller le climb

UPDATE


*/


public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animController;
    private Vector2 ref_velocity = Vector2.zero;
    private float originalGravity = 3f;
    private float walkingSpeed = 400f;
    private float runningSpeed = 800f;
    private float crouchingSpeed = 200f;
    private float climbingSpeed = 1200f;
    private float horizontalValue;
    private float verticalValue;

    [SerializeField] float bonusSpeed = 1.2f;    
    [SerializeField]public float boostDuration = 5f;
    [SerializeField]private bool isBoostActive = false;

    //SerializeField sert à afficher les paramètres dans le player
    //Le dash limit sert à mettre une Limite de Dash, je peux le changer directement dans le "Inspector" du player.
    [SerializeField] public int health = 9;
    private int dashLimit = 1;
    private int currentDash;
    [SerializeField] private float jumpForce = 10f;
    private Vector2 spawnPoint = new Vector2(-28,0); //TODO set first spawn point
    private bool isSlowed = false;
    private bool canCrouch = false;
    private bool grounded = false;
    private bool canClimb = true;
    private bool canJump = true;
    private bool canRun = true;
    private bool canDash = true;
    private bool isCrouching = false;
    private bool isClimbing = false;
    private bool isJumping = false;
    private bool isRunning = false;
    private bool isDashing = false;
    //C'est la force du dash en vertical et horizontal. Sunny se tourne automatiquement vers la droite, à modifier. 
    //Sunny dash vers le haut uniquement si Sunny touche le sol (appuie sur shift flèche du haut)
    //Si Sunny dash vers le haut en sauter, ca arrête sa force et il tombe
    private float horizontalDashingPower = 24f;
    private float verticalDashingPower = 14f;
    private float dashingTime = 0.001f;
    //Le chiffre inscrit correspond au temps avant que Sunny puisse re-sauter. 
    private float dashingCooldown = 1f;

    //////////////////////////////////////////////////////////////
    //                                                          //
    //                                                          //
    //                Fonction d'initialisation                 //
    //                                                          //
    //                                                          //
    //////////////////////////////////////////////////////////////


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        setSpawnPoint(rb.position);
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        currentDash = dashLimit;
    }


    //////////////////////////////////////////////////////////////
    //                                                          //
    //                                                          //
    //                    Fonctions d'updates                   //
    //                                                          //
    //                                                          //
    //////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        //Gère l'orientation du sprite en horizontal
        horizontalValue = Input.GetAxis("Horizontal");

        if (horizontalValue != 0){
            animController.SetBool("Walk", true);
        } 
        else {
            animController.SetBool("Walk", false);
        }

        if (horizontalValue < 0) sr.flipX = true;
        else if (horizontalValue > 0) sr.flipX = false;
        //animController.SetBool("Running", horizontalValue != 0);
        animController.SetFloat("speed", Mathf.Abs(horizontalValue));
        verticalValue = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            //TODO EXPLAIN
            if (isClimbing)
            {
                isJumping = true;
            }
            else
            {
                isJumping = true;
                animController.SetBool("Jumping", true);
            }
        }

        HanddleRun();

        HanddleCrouch();

        animController.SetFloat("speed", Mathf.Abs(horizontalValue));

        //Rémi a dit qu'il était pas fan du Coroutine, donc si vous arrivez à modifier c'est tant mieux
        /* if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && currentDash > 0)
        {
            StartCoroutine(Dash());
        } */
    }
    
    void FixedUpdate()
    {
        /*
        if(isJumping == false)
        {
            .SetBool("Jumping",false);
        }
        */


        if (isDashing)
        {
            return;
        }
        if (isJumping)
        {
            grounded = false;
            isJumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            //rb.velocity += new Vector2(0,jumpForce);
            //rb.velocity = rb.velocity.normalized;
            canJump = false;
        }

        // Si tu cours pas, est ce que tu climb ? Si tu climb pas, est-ce que 
        // tu crouch? Si tu cours pas, tu climb, tu crouch pas, c'est que tu marches
        float speedModifer;
        
        // Modificateurs de vitesse en fonction de l'action du joueur
        if (isRunning)
        {
            speedModifer = runningSpeed;
        }
        else if (isClimbing)
        {
            speedModifer = climbingSpeed;
        }
        else if (isCrouching)
        {
            speedModifer = crouchingSpeed;
        }
        else
        {
            speedModifer = walkingSpeed;
        }
        
        // Modificateurs de vitesse en fonction du status du joueur

        if (isSlowed){
            speedModifer = speedModifer / 2.0f;
        }

        if (isBoostActive){
            speedModifer = speedModifer * bonusSpeed;
        }

        Vector2 horizontalVelocity  = new Vector2(horizontalValue * speedModifer * Time.fixedDeltaTime, rb.velocity.y);
        Vector2 verticalVelocity = new Vector2(rb.velocity.x, verticalValue * speedModifer * Time.fixedDeltaTime);
        Vector2 targetVelocity = new Vector2();

        if (isClimbing)
        {
            targetVelocity.y = verticalVelocity.y;
        }

        targetVelocity += horizontalVelocity;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref ref_velocity, 0.05f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladders"))
        {
            isClimbing = false;
            grounded = true;
            rb.gravityScale = originalGravity;
            rb.velocity = new Vector2();
            animController.SetBool("Climbing", false);        
        }
        if(collision.gameObject.CompareTag("Slowdown")){
            isSlowed = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladders"))
        {
            rb.velocity = new Vector2(0,0); 
            rb.gravityScale = 0f;       
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        animController.SetBool("Jumping", false);
        
        if(collision.gameObject.CompareTag("Ladders"))
        {
            if (!isClimbing){ 
                isClimbing = true;
                grounded = false;
                //rb.gravityScale = 0f;
                rb.velocity = new Vector2();
                animController.SetBool("Climbing", true);
                Debug.Log("Escalader");
            }
        }
        
        if(collision.gameObject.CompareTag("Slowdown")){
            isSlowed = true;
        }
        if(collision.gameObject.CompareTag("Collectible")){
            collision.gameObject.SetActive(false);
            ActivateBoost();
        }
        currentDash = dashLimit;
        grounded = true;
        canJump = true;
    }


    //////////////////////////////////////////////////////////////
    //                                                          //
    //                                                          //
    //                 Fonctions de mouvements                  //
    //                                                          //
    //                                                          //
    //////////////////////////////////////////////////////////////

    private void HanddleRun(){
        if (Input.GetKeyDown(KeyCode.LeftControl) && canRun)
        {
            isRunning = true;
            animController.SetBool("Run", true);

        }
        if (Input.GetKeyUp(KeyCode.LeftControl) && canRun)
        {
            isRunning = false;
            animController.SetBool("Run", false);
        }
    }

    private void HanddleCrouch(){
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isCrouching = true;
            animController.SetBool("Crouching", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isCrouching = false;
            animController.SetBool("Crouching", false);
        }
    }

    private IEnumerator Dash()
    {
        currentDash--;
        //Pour dash vers le haut, clic, flèche haut puis left 
        //Ces lignes permettent de dash dans toute les directions
        Vector2 DashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        DashDirection = DashDirection.normalized;
        DashDirection = new Vector2(DashDirection.x * horizontalDashingPower, DashDirection.y * verticalDashingPower);
        if (!isRunning)
        {
            rb.velocity += DashDirection;
        }
        //Je crois que "return" est utilisé pour relancer le code dès que Sunny a toucher le sol
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void respawn(){
        rb.transform.position = spawnPoint;
    }

    public Vector2 getCurrentCoords(){
        return rb.transform.position;
    }

    public void setSpawnPoint(Vector2 coord){
        spawnPoint = coord;
    }

    public void heal(int value){
        health += value;
    }

    public void hurt(int value){
        health -= value;
        if (health <= 0){
            health = 9;
            respawn();
        }
    }

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
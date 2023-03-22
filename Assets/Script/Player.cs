using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 
DONE
- Dash done
- Courir done ----------------->(Sunny a un saut amplifi� quand tu sautes en pentes ou en prenant de l'elant avec la course)
- S'accroupir done ------------>(Il peut crouch et courir en m�me temps)
- Climb done ------------------>

DOING
- Une zone ou tu es ralenti
- Un objet immobile qui te ferait -1 de d�g�t si tu le touche (exemple, une barri�re ) 
- Un objet qui te ferait -1 de d�g�t si il te tombe dessus. (Ex: une caisse tombe du ciel quand le chat passe en des

TODO
- Une barre de vie constituer de 9 coeurs
- Respawn & Checkpoint
- MAYBE mettre une stamina uniquement sur le Climb
- Collectibles
- Se faire poursuivre par le chien, si il nous touche, on dead
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
    //Lui mettre son animation, idem pour la course, le dash & le climb
    private float crouchingSpeed = 200f;
    private float climbingSpeed = 300f;
    private float horizontalValue;
    private float verticalValue;

    //SerializeField sert � afficher les param�tres dans le player
    //Le dash limit sert � mettre une Limite de Dash, je peux le changer directement dans le "Inspector" du player.
    [SerializeField] private int dashLimit = 1;
    [SerializeField] private int currentDash;
    [SerializeField] private float jumpForce = 10f;
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
    //C'est la force du dash en vertical et horizontal. Sunny se tourne automatiquement vers la droite, � modifier. 
    //Sunny dash vers le haut uniquement si Sunny touche le sol (appuie sur shift fl�che du haut)
    //Si Sunny dash vers le haut en sauter, ca arr�te sa force et il tombe
    [SerializeField] private float horizontalDashingPower = 24f;
    [SerializeField] private float verticalDashingPower = 14f;
    [SerializeField] private float dashingTime = 0.001f;
    //Le chiffre inscrit correspond au temps avant que Sunny puisse re-sauter. 
    [SerializeField] private float dashingCooldown = 1f;

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
        //G�re l'orientation du sprite en horizontal
        horizontalValue = Input.GetAxis("Horizontal");
        if (horizontalValue < 0) sr.flipX = true;
        else if (horizontalValue > 0) sr.flipX = false;
        //animController.SetBool("Running", horizontalValue != 0);
        animController.SetFloat("speed", Mathf.Abs(horizontalValue));
        verticalValue = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
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

        //R�mi a dit qu'il �tait pas fan du Coroutine, donc si vous arrivez � modifier c'est tant mieux
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && currentDash > 0)
        {
            StartCoroutine(Dash());
        }
    }
    
    void FixedUpdate()
    {

        if (isDashing)
        {
            return;
        }
        if (isJumping)
        {
            grounded = false;
            isJumping = false;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            canJump = false;
        }

        // Si tu cours pas, est ce que tu climb ? Si tu climb pas, est-ce que 
        // tu crouch? Si tu cours pas, tu climb, tu crouch pas, c'est que tu marches
        float speedModifer;
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
        Vector2 horizontalVelocity  = new Vector2(horizontalValue * speedModifer * Time.fixedDeltaTime, rb.velocity.y);
        Vector2 verticalVelocity = new Vector2(rb.velocity.x, verticalValue * speedModifer * Time.fixedDeltaTime);
        Vector2 targetVelocity = new Vector2();
        if (isClimbing){
            targetVelocity += verticalVelocity;
        }
        targetVelocity += horizontalVelocity;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref ref_velocity, 0.05f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Ladders"))
        {
            isClimbing = true;
            grounded = false;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2();
            animController.SetBool("Climbing", true);
            Debug.Log("Escalader");
            /*if (isJumping)
            {
                animController.SetBool("Climbing", false);
            }*/
        }/*else
        {
            Debug.Log("Pas esclade");
            //isClimbing = false;
            //animController.SetBool("Climbing", false);
        }*/
        
        currentDash = dashLimit;
        grounded = true;
        canJump = true;
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animController.SetBool("Jumping", false);
    }


    //////////////////////////////////////////////////////////////
    //                                                          //
    //                                                          //
    //                 Fonctions de mouvements                  //
    //                                                          //
    //                                                          //
    //////////////////////////////////////////////////////////////

    void HanddleRun(){
        if (Input.GetKeyDown(KeyCode.LeftControl) && canRun)
        {
            isRunning = true;
            animController.SetBool("Running", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) && canRun)
        {
            isRunning = false;
            animController.SetBool("Running", false);
        }
    }

    void HanddleCrouch(){
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
        //Pour dash vers le haut, clic, fl�che haut puis left 
        //Ces lignes permettent de dash dans toute les directions
        Vector2 DashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        DashDirection = DashDirection.normalized;
        DashDirection = new Vector2(DashDirection.x * horizontalDashingPower, DashDirection.y * verticalDashingPower);
        if (!isRunning)
        {
            rb.velocity += DashDirection;
        }


        //Je crois que "return" est utilis� pour relancer le code d�s que Sunny a toucher le sol
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
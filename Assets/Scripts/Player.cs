using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator = null;
    public GameObject iddleFX = null;
    public GameObject walkFX = null;
    public GameObject runFX = null;

    ///// FX GENERATION
    public float FxGenerationTime = 1.0f;
    private float FxTimeLeft;
    /////

    public float speed = 5.0f;
    public float RunSpeed = 10.0f;

    ///// STAMINA SPRINT VARIABLE
    private float Stamina;
    public float MaxStamina = 50.0f;

    
    private Rigidbody2D rb;

    private Vector2 moveVelocity;
    private CircleCollider2D circleDetection;

    private enum Estate {IDDLE,WALK,RUN};

    private Estate state = Estate.IDDLE;

    private AudioSource stepSound;

    public float lookRange;
    public float lookSphereCastRadius;
    public float searchingTurnSpeed;
    public float searchDuration;
    [HideInInspector] public GameObject seenBy;

    // Start is called before the first frame update
    void Start()
    {
        stepSound = GetComponent<AudioSource>();
        Stamina = MaxStamina;
        rb = GetComponent<Rigidbody2D>();
        circleDetection = GetComponent<CircleCollider2D>();
        FxTimeLeft = FxGenerationTime;
    }

    // Update is called once per frame
    void Update()
    {
        float isRunning = Input.GetAxis("Run");
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        ///ANIMATOR
        if (moveInput.x > 0 )
            animator.SetInteger("Way", 1);
        else if (moveInput.x < 0 )
            animator.SetInteger("Way", 3);
        else if (moveInput.x == 0 && moveInput.y > 0)
            animator.SetInteger("Way", 4);
        else if (moveInput.x == 0 && moveInput.y < 0)
            animator.SetInteger("Way", 2);
        else
            animator.SetInteger("Way", 0);
        ///


        ///FX
        FxTimeLeft = Mathf.Clamp(FxTimeLeft + Time.deltaTime, 0, FxGenerationTime);
        if (FxTimeLeft == FxGenerationTime) {
            if (FxUpdate() != Estate.IDDLE)
            FxTimeLeft = 0;
        }
        ////
        
        //RUNNING
        if (isRunning > 0 && Stamina > 0) {
            state = Estate.RUN;
            if (Stamina == 49)
                FxUpdate();
            moveVelocity = moveInput.normalized * RunSpeed;
            Stamina -= 1;
            if (Stamina == 0) {
                Stamina = -30;
            }
        }
        //WALKING
        else if (moveInput.x != 0 || moveInput.y != 0) { 
            state = Estate.WALK;
            moveVelocity = moveInput.normalized * speed;
            if (Stamina < MaxStamina && Stamina > 0 && isRunning == 0) {
                Stamina += 1;
            }
        }
        //IDLE
        else {
            state = Estate.IDDLE;
            moveVelocity = moveInput.normalized * speed;
            if (Stamina < MaxStamina && isRunning == 0) {
                Stamina += 1;
            }
        }
    }

    Estate FxUpdate()
    {
            switch (state){
                case (Estate.IDDLE):
                    if (iddleFX)
                    Instantiate(iddleFX, transform.position, Quaternion.identity, gameObject.transform);
                    circleDetection.radius = 0.53f;
                break;
                case (Estate.WALK):
                    if (walkFX)
                    walkFX.transform.localScale = new Vector3(0.5f,0.5f,1);
                    Instantiate(walkFX, transform.position, Quaternion.identity, gameObject.transform);
                    circleDetection.radius = 0.53f * 5;
                    stepSound.Play();
                break;
                case (Estate.RUN):
                    if (runFX)
                    runFX.transform.localScale = new Vector3(1,1,1);
                    Instantiate(runFX, transform.position, Quaternion.identity, gameObject.transform);
                    circleDetection.radius = 0.53f * 10;
                    stepSound.Play();
                break;
                default:
                break;
            };
            return state;
    }
    void FixedUpdate() {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }
    public void Seen(GameObject maid)
    {
        seenBy = maid;
    }
}

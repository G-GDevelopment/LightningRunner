using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    // Variables
    private Collision_Player coll;
    [HideInInspector]
    public Rigidbody2D rb;
    [SerializeField]
    private Camera _mainCam;
   // private AnimationScript anim;


    [Space]
    [Header("Basic Stats")]
    public float mov_Speed = 10;
    public float jumpForce = 10;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    [Space]
    [Header("Ability Stats")]
    public float dashSpeed = 30;
    [SerializeField]
    private float _holdDownButtonTimerForDash;
    [SerializeField]
    private float _maxHoldDownTime = 2f;
    [SerializeField]
    private float _dashMaxForceTranslation = 5f;
    private float _dashMaxForce;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallSlide;
    public bool wallGrab;
    public bool wallJumped;
    public bool isDashing;
    public bool DieIfDashInWall;
    [SerializeField]
    private bool startMoving;
    [Space]
    [Header("Ledge check")]
    private bool canClimbLedge = false;
    private bool ledgeDetected;

    private Vector2 _ledgePosBottom;
    private Vector2 _ledgePos1;
    private Vector2 _ledgePos2;
    private Vector2 workspace;

    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;

    //NEW SHIT
    private Vector2 detectedPos;
    private Vector2 cornerPos;
    private Vector2 startPos;
    private Vector2 stopPos;
    [Header("Ledge Climb State")]
    public Vector2 startOffset;
    public Vector2 stopOffset;




    [Space]
    public bool facingRight;
    public bool groundTouch;
    public bool hasDashed;
    [Space]
    [Header("Player Accesiblility")]
    public float hangTime = 1.5f;
    [SerializeField]
    private float hangCounter;
    [SerializeField]
    private float jumpTimer;
    public float jumpDelay = 0.25f;
    public bool isjumping;
    public bool jumpNow;

    [Space]
    [Header("Particles")]
    public ParticleSystem dashParticle;
    public ParticleSystem dustParticle;

    public GameObject ghostPlayer;
    public BoxCollider2D boxCollider;
    public int side = 1; //Uses turn the player 180 degrees

    void Start()
    {

        coll = GetComponent<Collision_Player>();
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponentInChildren<AnimationScript>();

    }

    private void FixedUpdate()
    {
        DetermineCornerPosition();
        //Change the DetectedPos to Players current position
        if (coll.isTouchingWall && !coll.isTouchingRightLedge || !coll.isTouchingLeftLedge)
        {
            SetDetectedPosition(transform.position);
        }

        if (jumpTimer > Time.time) //jump buffer
        {
            if (coll.onGround && !coll.onWall)
                Jump(Vector2.up, false);

            if (coll.onWall)
            {

                WallJump();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(x, y);

        KillPlayer();

        Walk(dir);
        //anim.SetHorizontalMovement((Mathf.Abs(x)), y, rb.velocity.y);

        

        if (coll.onWall && !coll.isTouchingRightLedge)
        CheckLedgeClimb();
        if(canClimbLedge && coll.onGround)
        FinishLedgeClimb();

        if (coll.onWall && Input.GetButton("L2") && canMove)
        {
            if (side != coll.wallSide)
            {
                //anim.Flip(side * -1);

            }
            wallGrab = true;
            wallSlide = false;

        }

        if (Input.GetButtonUp("L2") || !coll.onWall || !canMove)
        {
            wallSlide = false;
            wallGrab = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<Better_Jump_Mechanics>().enabled = true;
        }


        if (wallGrab)
        {
            rb.gravityScale = 0;

            if (x > .1f || x < -.1f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            //float speedModifier = y > 0 ? .5f : 1;
            //rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier)); Climp up and down function
        }
        else
        {
            rb.gravityScale = 3f;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        if (Input.GetButtonDown("Jump")) //Jump Function
        {
           // anim.SetTrigger("jump");
            jumpTimer = Time.time + jumpDelay;
        }
        #region Dash
        if (Input.GetButtonDown("Square") && !hasDashed)
        {
            
            if (xRaw != 0 || yRaw != 0)
            {
                Dash(xRaw, yRaw);
            }
        }

        //SuperDash
        if (Input.GetButtonDown("Square") && coll.onGround)
        {
            if (coll.onGround && !startMoving)
            {
                _dashMaxForce = _dashMaxForceTranslation;
                _holdDownButtonTimerForDash = Time.time;
            }

        }
        if (startMoving)
        {
            _holdDownButtonTimerForDash = 0f;
            _dashMaxForce = 0f;
            ghostPlayer.SetActive(false);
        }
        if (Input.GetButton("Square") && !hasDashed && coll.onGround)
        {
            float holdDownTime = Time.time - _holdDownButtonTimerForDash;
            SuperDashBuildUp(CalculateHoldDownForce(holdDownTime));
        }
        if (Input.GetButtonUp("Square")) 
        {
            if (!startMoving && coll.onGround)
            {
                float holdDownTime = Time.time - _holdDownButtonTimerForDash;
                StartCoroutine(SuperDash(0.3f));

                SuperDashLaunch(CalculateHoldDownForce(holdDownTime));
                DieIfDashInWall = true;

            }
        }

        #endregion


        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
            isjumping = false;
        }

        if (!coll.onGround && groundTouch)
        {

            groundTouch = false;
        }


        if (dir.x < 0)
        {
            if (groundTouch == true && !wallGrab)
               // dustParticle.Play();

            if (wallGrab != true)
            {
                side = -1;
                    //anim.Flip(side);
                    startMoving = true;

            }

        }

        if (dir.x > 0)
        {
            if (groundTouch == true && !wallGrab)
                // dustParticle.Play();

                if (wallGrab != true)
                {
                    side = 1;
                    // anim.Flip(side);
                    startMoving = true;
                }
        }

        if (dir.x == 0)
        {
            if (coll.onGround && !wallGrab)
            {
                startMoving = false;
            }
        }
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

    }
    public void SetDetectedPosition(Vector2 pos) => detectedPos = pos;
    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(coll.wallCheck.position, Vector2.right * side, coll.wallCheckDistance, coll.groundLayer);
        float xDist = xHit.distance;
        workspace.Set((xDist * side) * side, 0.015f);
        RaycastHit2D yHit = Physics2D.Raycast(coll.ledgeCheck.position + (Vector3)(workspace), Vector2.down, coll.ledgeCheck.position.y - coll.wallCheck.position.y + 0.015f, coll.groundLayer);
        float yDist = yHit.distance;

        //Upper Corner Position of ledge
        workspace.Set(coll.wallCheck.position.x + (xDist * side), coll.ledgeCheck.position.y - yDist);
        return workspace;
    }
    private void CheckLedgeClimb()
    {

        if (coll.isTouchingWall && !coll.isTouchingRightLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            //Freeze player in the detectedPos
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            transform.position = detectedPos;
            cornerPos = DetermineCornerPosition();
        }

        if(ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;
            startPos.Set(cornerPos.x - (side * startOffset.x), cornerPos.y - startOffset.y);
            stopPos.Set(cornerPos.x + (side * stopOffset.x), cornerPos.y + stopOffset.y);
        }

        
        canMove = false;
        transform.position = startPos;
        canClimbLedge = true;

        /*
         if(coll.isTouchingWall && !coll.onLedge && !ledgeDetected)
         {
             ledgeDetected = true;
             //_ledgePos1 = coll.wallCheck.position;
             _ledgePos1 = playerBodyBaby.position;
         }

         if(ledgeDetected && !canClimbLedge)
         {
             canClimbLedge = true;

             if (coll.isTouchingRightLedge)
             {
                 _ledgePos1 = new Vector2(Mathf.Floor(_ledgePosBottom.x + coll.wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(_ledgePosBottom.y) + ledgeClimbYOffset1);
                 _ledgePos2 = new Vector2(Mathf.Floor(_ledgePosBottom.x + coll.wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(_ledgePosBottom.y) + ledgeClimbYOffset2);

             }

            // canMove = false;
             //canFlip = false;

             //anim.SetBool("canClimbLedge", canClimbLedge);
         }

         if (canClimbLedge)
         {
             transform.position = _ledgePos1;
         } */
    }

    public void FinishLedgeClimb()
    {
       // transform.position = stopPos;
       /* if (canClimbLedge)
        {
            canClimbLedge = false;
            transform.position = _ledgePos2;
            canMove = true;
            //canFlip = true;
            ledgeDetected = false;
            //anim.SetBool("canClimbLedge", canClimbLedge);
        }*/

    }
    private void Dash(float x, float y)
    {
        /*
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position)); */


        hasDashed = true;

        //anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }
    private float CalculateHoldDownForce(float holdTime)
    {
        float holdTimeNormalized = Mathf.Clamp01(holdTime / _maxHoldDownTime);
        float force = holdTimeNormalized * _dashMaxForce;
        Debug.Log(force);
        return force;
    }
    private void SuperDashLaunch(float force)
    {
        if(side == 1)
        {
            //Positive vector
            transform.position = new Vector2(transform.position.x + force, transform.position.y);
            //StartCourotine
        }
        else if (side == -1)
        {
            transform.position = new Vector2(transform.position.x - force, transform.position.y);
        }
        
    }
    private void SuperDashBuildUp(float force)
    {
        //Play Build particles
        if (!startMoving && coll.onGround)
        {
            
            _mainCam.orthographicSize = Mathf.Lerp(5, 7, Mathf.SmoothStep(0.0f, 1.0f, force/3));

            if (side == 1)
            {
                ghostPlayer.SetActive(true);
                ghostPlayer.transform.position = new Vector2(transform.position.x + force, transform.position.y);
                
            }
            else if (side == -1)
            {
                ghostPlayer.SetActive(true);
                ghostPlayer.transform.position = new Vector2(transform.position.x - force, transform.position.y);
            }
        }
    }
    IEnumerator SuperDash(float time)
    {
        _mainCam.orthographicSize = 5f;
        ghostPlayer.SetActive(false);
        ghostPlayer.transform.position = transform.position;
        //Play Particles
        rb.gravityScale = 0f;
        GetComponent<Better_Jump_Mechanics>().enabled = false;
        isDashing = true;
        StartCoroutine(DisableMovement(time));
        yield return new WaitForSeconds(time);
        rb.gravityScale = 3;
        rb.gravityScale = 0f;
        GetComponent<Better_Jump_Mechanics>().enabled = true;
        isDashing = false;
        DieIfDashInWall = false;

    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());

      //  dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<Better_Jump_Mechanics>().enabled = false;

        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

       // dashParticle.Stop();
        rb.gravityScale = 3;

        GetComponent<Better_Jump_Mechanics>().enabled = true;

        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
        {
            hasDashed = false;
        }

    }



    private void Walk(Vector2 dir)
    {
        if (!canMove)
        {
            return;
        }
        if (wallGrab)
        {
            return;
        }
        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * mov_Speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(dir.x * mov_Speed, rb.velocity.y), wallJumpLerp * Time.deltaTime);
        }

    }

    private void Jump(Vector2 dir, bool wall)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
        jumpTimer = 0f;
        hangCounter = 0;
        isjumping = true;
        jumpNow = false;
    }

    private void WallJump()
    {
        StartCoroutine(DisableMovement(0.2f));
        StopCoroutine(DisableMovement(0));
        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);


        wallJumped = true;

    }

    private void WallSlide()
    {

        if (!canMove)
        {
            return;
        }

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall) && !canClimbLedge)
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;
        rb.velocity = new Vector2(push, -slideSpeed);
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private void KillPlayer()
    {
        if (DieIfDashInWall && coll.onWall)
        {
            //Destroy(gameObject, .5f);
            Debug.Log(gameObject.name + " is now dead");
        }
    }
}

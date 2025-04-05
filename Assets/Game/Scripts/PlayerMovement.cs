using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public enum PlayerMovementType
{
    Default, Flying
}

public class PlayerMovement : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Running Settings")]
            public PlayerMovementType curPlayerMovementType;
            public float moveSpeed = 5f;
            public float originalMoveSpeed;
            public float jumpForce = 10f;
            public float downwardJumpForce = 10f;
            public float minGravitation;
            public float maxGravitation;
            [ShowOnly] public bool isDownwardJumping;
            [ShowOnly] public bool isDucking;
            [ShowOnly] public bool isFalling;
            [ShowOnly]public Vector3 lastPosition;
            [ShowOnly] public Rigidbody2D rb;
            [ShowOnly] public SpriteRenderer sr;
            [ShowOnly] public bool isInDialogue;
            [ShowOnly] public PlayerInteraction interactionScript;

        [Space(20)] [Header("JUMP SETTINGS")]

            [Header("Basic Variables")]
            public int maxJumps = 2;
            [ShowOnly] public int jumpCount;

            [Header("Coyote Time Settings")]
            public float coyoteTime = 0.15f;
            [ShowOnly] public float coyoteTimeCounter; 

            [Header("Jump Buffer Settings")]
            public float jumpBufferTime = 0.15f;
            [ShowOnly] public float jumpBufferCounter; 

            [Header("Ground Check Settings")]
            public LayerMask groundLayer;
            public float groundCheckDistance = 0.2f;
            public bool isGrounded;
            
        [Space(20)] [Header("ANIMATION")]

            [Header("Animation Names")]   
            public AnimationSystem animationScript;
            const string IDLE_ANIMATION = "Player_Idle";
            const string RUNNING_ANIMATION = "Player_Running";
            const string FALLING_ANIMATION = "Player_Falling";
            const string JUMPING_ANIMATION = "Player_Jumping";
            const string DUCKING_ANIMATION = "Player_Ducking";

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = minGravitation;
            
            sr = GetComponent<SpriteRenderer>();

            interactionScript = GetComponent<PlayerInteraction>();

            originalMoveSpeed = moveSpeed;
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            // Perform initial setup that occurs when the game starts.
            // Example: Initialize game state, start coroutines, load resources, etc.
            
            // Example of starting a coroutine.
            // StartCoroutine(ExampleCoroutine());
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if(isInDialogue) return;
            
            IsOnGround();
            IsOnFlip();
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {
            if(isInDialogue)
            {
                DefaultState();
                return;
            }

            if(curPlayerMovementType == PlayerMovementType.Default)
            {
                Ducking();
                Running();
                Jumping();
                Falling();
                GravitationPull();
            }
            else
            {
                FlyingMovement();
                Ducking();
            }

            lastPosition = transform.position;
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// The state of idle talking.
        /// </summary>
        void DefaultState()
        {
            rb.linearVelocity = new Vector2(0f, 0f);
            rb.linearVelocity += Vector2.down * downwardJumpForce;
            animationScript.ChangeAnimationState(IDLE_ANIMATION);  
        }

        /// <summary>
        /// Ducking/crouching system.
        /// </summary>
        void Ducking()
        {
            if((Input.GetKeyDown(KeyCode.S)) && (isGrounded))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                transform.position += new Vector3(0f, -0.8f, 0f);
            }

            if(Input.GetKey(KeyCode.S))
            {
                isDucking = true;
                animationScript.ChangeAnimationState(DUCKING_ANIMATION);  
                
                interactionScript.ChangeCameraOffset(interactionScript.cameraOffset.x, 1f);    
            }
            else
            {
                isDucking = false;
                isDownwardJumping = false;
            }
        }

        /// <summary>
        /// Moving the player.
        /// </summary>
        void Running()
        {
            float moveInput = Input.GetAxis("Horizontal");
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            // Handling animation
            if((isGrounded) && (!isDucking))
            {
                if(rb.linearVelocity.x != 0f)
                {
                    animationScript.ChangeAnimationState(RUNNING_ANIMATION);
                }
                else
                {
                    animationScript.ChangeAnimationState(IDLE_ANIMATION);
                }
            }
        }

        /// <summary>
        /// Jumping system.
        /// </summary>
        void Jumping()
        {
            // Buffer jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }   
            
            // Jumping
            if (jumpBufferCounter > 0f && (coyoteTimeCounter > 0f || jumpCount < maxJumps - 1))
            {
                jumpCount++;

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                //rb.linearVelocity += Vector2.up * jumpForce;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }

            // Downward jumping
            if((!isGrounded) && (isDucking))
            {
                isDownwardJumping = true;
                DownwardJumping();
            }

            // Handling animation
            if((!isGrounded) && (!isFalling))
            {
                if(!isDownwardJumping)
                {
                    animationScript.ChangeAnimationState(JUMPING_ANIMATION);
                }
                else
                {
                    animationScript.ChangeAnimationState(DUCKING_ANIMATION);
                }
            }
        }

        /// <summary>
        /// Falling logic.
        /// </summary>
        void Falling()
        {
            if((transform.position.y < lastPosition.y) && (!isGrounded) && (!isDownwardJumping))
            {
                isFalling = true;
                animationScript.ChangeAnimationState(FALLING_ANIMATION);
            }
            else
            {
                isFalling = false;
            }
        }

        /// <summary>
        /// Downward jumping system.
        /// </summary>
        void DownwardJumping()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.linearVelocity += Vector2.down * downwardJumpForce;
        }

        /// <summary>
        /// Dragging the player down by gravity.
        /// </summary>
        void GravitationPull()
        {
            if(transform.position.y < lastPosition.y)
            {
                rb.gravityScale += 0.1f;
                                
                if(rb.gravityScale > maxGravitation) rb.gravityScale = maxGravitation;

                moveSpeed = originalMoveSpeed;
            }
            else if(transform.position.y > lastPosition.y)
            {                
                interactionScript.ChangeCameraOffset(interactionScript.cameraOffset.x, 4f);
            }
            else
            {
                rb.gravityScale = minGravitation;

                moveSpeed = originalMoveSpeed;

                if(!isDucking) interactionScript.ChangeCameraOffset(interactionScript.cameraOffset.x, 3f);
            }
        }

        /// <summary>
        /// Flying-based movement.
        /// </summary>
        void FlyingMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * moveSpeed);
            
            // Handling animation
            if(!isDucking)
            {
                if (isGrounded)
                {                   
                    if(rb.linearVelocity.x != 0f)
                    {
                        animationScript.ChangeAnimationState(RUNNING_ANIMATION);
                    }
                    else
                    {
                        animationScript.ChangeAnimationState(IDLE_ANIMATION);
                    }
                }
                else if(transform.position.y <= lastPosition.y)
                {
                    animationScript.ChangeAnimationState(FALLING_ANIMATION);
                }
                else if(transform.position.y > lastPosition.y)
                {
                    animationScript.ChangeAnimationState(JUMPING_ANIMATION);
                }
            }
        }

        /// <summary>
        /// Checking if the player touches ground.
        /// </summary>
        void IsOnGround()
        {
            isGrounded = (Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer)) ||
                        (Physics2D.Raycast(transform.position + new Vector3(-0.4f, 0f, 0f), Vector2.down + new Vector2(-0.4f, 0f), groundCheckDistance, groundLayer)) ||
                        (Physics2D.Raycast(transform.position + new Vector3(0.4f, 0f, 0f), Vector2.down + new Vector2(0.4f, 0f), groundCheckDistance, groundLayer)) ;
            
            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                jumpCount = 0;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Flipping the sprite according to direction.
        /// </summary>
        void IsOnFlip()
        {
            float moveInput = Input.GetAxis("Horizontal");

            if (moveInput > 0f)
            {
                sr.flipX = false;
                interactionScript.ChangeCameraOffset(2f, interactionScript.cameraOffset.y);
            }
            else if (moveInput < 0f)
            {
                sr.flipX = true;
                interactionScript.ChangeCameraOffset(-2f, interactionScript.cameraOffset.y);
            }
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="obj">The Collision2D data associated with this collision.</param>
        void OnCollisionEnter2D(Collision2D obj) 
        {
            if((obj.gameObject.CompareTag("Cell")) && (isDownwardJumping))
            {
                PlatformerCell cellScriptMid = obj.gameObject.GetComponent<PlatformerCell>();

                CellularAutomataAlgorithm CAScript = FindObjectOfType<CellularAutomataAlgorithm>();
                GameObject cellLeft = CAScript.GetCell(cellScriptMid.posX - 1, cellScriptMid.posY);
                GameObject cellRight = CAScript.GetCell(cellScriptMid.posX + 1, cellScriptMid.posY);

                if(cellLeft != null)
                {
                    PlatformerCell cellScriptLeft = cellLeft.GetComponent<PlatformerCell>();
                    cellScriptLeft.DestroyCell();
                }

                if(cellRight != null)
                {
                    PlatformerCell cellScriptRight = cellRight.GetComponent<PlatformerCell>();
                    cellScriptRight.DestroyCell();
                }

                cellScriptMid.DestroyCell();
                
                isDownwardJumping = false;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                transform.position += new Vector3(0f, -1f, 0f);
            }
        }
        
        /// <summary>
        /// Visualization of environment.
        /// </summary>
        void OnDrawGizmos()
        {
            // Ground-check visualization
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
            Gizmos.DrawLine(transform.position + new Vector3(-0.4f, 0f, 0f), transform.position + new Vector3(-0.4f, 0f, 0f) + Vector3.down * groundCheckDistance);
            Gizmos.DrawLine(transform.position + new Vector3(0.4f, 0f, 0f), transform.position + new Vector3(0.4f, 0f, 0f) + Vector3.down * groundCheckDistance);
        }

    #endregion

}
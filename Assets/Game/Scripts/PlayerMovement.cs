using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Running Settings")]
            public float moveSpeed = 5f;
            public float originalMoveSpeed;
            public float jumpForce = 10f;
            public float downwardJumpForce = 10f;
            public bool isDownwardJumping;
            public bool isDucking;
            public bool isFalling;
            public float minGravitation;
            public float maxGravitation;
            private Vector3 lastPosition;
            private Rigidbody2D rb;
            private SpriteRenderer sr;

            [Header("Jump Settings")]
            public int maxJumps = 2;
            private int jumpCount;

            [Header("Coyote Time Settings")]
            public float coyoteTime = 0.15f;
            private float coyoteTimeCounter; 

            [Header("Jump Buffer Settings")]
            public float jumpBufferTime = 0.15f;
            private float jumpBufferCounter; 

            [Header("Ground Check Settings")]
            public LayerMask groundLayer;
            public float groundCheckDistance = 0.2f;
            public bool isGrounded;
            
        [Space(20)] [Header("VARIABLES")]

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
            IsOnGround();
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {
            Ducking();
            Running();
            Jumping();
            Falling();
            GravitationPull();

            lastPosition = transform.position;
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Checking if the player touches ground.
        /// </summary>
        void IsOnGround()
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
            
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

            IsOnFlip();
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
                rb.linearVelocity += Vector2.up * jumpForce;
                
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
            // Reseting speed in max high
            if ((rb.linearVelocity.y > 0f) && (rb.linearVelocity.y < 0.5f))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.gravityScale = 0.5f;
                moveSpeed = originalMoveSpeed * 1.3f;
            }
            else if(transform.position.y < lastPosition.y)
            {
                rb.gravityScale += 0.1f;
                                
                if(rb.gravityScale > maxGravitation) rb.gravityScale = maxGravitation;

                moveSpeed = originalMoveSpeed;
            }
            else
            {
                rb.gravityScale = minGravitation;

                moveSpeed = originalMoveSpeed;
            }
        }

        /// <summary>
        /// Flipping the sprite according to direction.
        /// </summary>
        void IsOnFlip()
        {
            if (rb.linearVelocity.x > 0)
            {
                sr.flipX = false;
            }
            else if (rb.linearVelocity.x < 0)
            {
                sr.flipX = true;
            }
        }

        void OnCollisionEnter2D(Collision2D obj) 
        {
            if((obj.gameObject.CompareTag("Cell")) && (isDownwardJumping))
            {
                PlatformerCell cellScriptMid = obj.gameObject.GetComponent<PlatformerCell>();

                GameObject cellLeft = CellularAutomataAlgorithm.instance.GetCell(cellScriptMid.posX - 1, cellScriptMid.posY);
                GameObject cellRight = CellularAutomataAlgorithm.instance.GetCell(cellScriptMid.posX + 1, cellScriptMid.posY);

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
        /// An example coroutine that waits for 2 seconds.
        /// </summary>
        IEnumerator ExampleCoroutine()
        {
            // Wait for 2 seconds before executing further code.
            yield return new WaitForSeconds(2f);

            Debug.Log("Action after 2 seconds.");
        }
        
        /// <summary>
        /// Visualization of environment.
        /// </summary>
        void OnDrawGizmos()
        {
            // Ground-check visualization
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        }

    #endregion

}
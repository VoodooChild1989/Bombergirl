using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.EventSystems;
using TMPro;

public enum PlayerMovementType
{
    Default, Flying
}

public class PlayerMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Ground Collision")]
            public Vector2 point;
            public Vector2 size;
            public float resetCollisionIn = 0.2f;
            public LayerMask groundLayer;
            [ShowOnly] public bool isGrounded;
            [ShowOnly] public Collider2D curOneWayPlatform;
            
            [Header("Input")]
            [ShowOnly] public float moveInput;

            [Header("Running Settings")]
            public PlayerMovementType curPlayerMovementType;
            public float moveSpeed = 5f;
            [ShowOnly] public float originalMoveSpeed;
            public float jumpForce = 10f;
            public float groundPoundForce = 10f;
            public float minGravitation;
            public float maxGravitation;
            public GameObject UIMobile;
            public GameObject UICore;
            [ShowOnly] public bool flightUnlocked;
            [ShowOnly] public bool canGroundPound;
            [ShowOnly] public bool isDucking;
            [ShowOnly] public bool isFalling;
            [ShowOnly] public Vector3 lastPosition;
            [ShowOnly] public Rigidbody2D rb;
            [ShowOnly] public SpriteRenderer sr;
            [ShowOnly] public bool isInDialogue;
            [ShowOnly] public PlayerInteraction interactionScript;

            [Header("Switching Modes")]
            public Image switchModesButtonImage;
            public Sprite jumpingIcon;
            public Sprite flyingIcon;
            [ShowOnly] public bool isFlying;

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
            
        [Space(20)] [Header("ANIMATION")]

            [Header("Animation Names")]   
            public AnimationSystem animationScript;
            const string IDLE_ANIMATION = "Player_Idle";
            const string RUNNING_ANIMATION = "Player_Running";
            const string FALLING_ANIMATION = "Player_Falling";
            const string JUMPING_ANIMATION = "Player_Jumping";
            const string DUCKING_ANIMATION = "Player_Ducking";

        [Header("Mobile Controls")]
            public Joystick joystick;
            public bool mobileJump;
            public bool mobileDucking;
            public void MobileJump() => mobileJump = true;
            public void MobileStartDucking()
            {
                if (isGrounded)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                    transform.position += new Vector3(0f, -0.3f, 0f);
                }

                mobileDucking = true;
            }
            public void MobileStopDucking() => mobileDucking = false;
            [SerializeField] private Canvas myCanvas;
            [SerializeField] private Camera myCamera;

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
            myCanvas.worldCamera = GameObject.Find("Main Camera (Camera)").GetComponent<Camera>();
            UICore = GameObject.Find("UI_Core");

            #if UNITY_STANDALONE
                transform.Find("Canvas").gameObject.SetActive(false);
            #endif

            SettingLevels();
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if (isInDialogue) return;

            HandleInput();
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {
            if ((isInDialogue) || (GameManager.instance.isInCutscene))
            {
                DefaultState();
                return;
            }

            UIMobile.SetActive(true);
            UICore.SetActive(true);

            if (curPlayerMovementType == PlayerMovementType.Default)
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
        /// Managing inputs.
        /// </summary>
        void HandleInput()
        {
            // Move
            moveInput = 0f;
            moveInput += Input.GetAxis("Horizontal");
            moveInput += joystick.Horizontal;

            // Jump
            if (mobileJump || Input.GetKeyDown(KeyCode.Space))
            {
                jumpBufferCounter = jumpBufferTime;

                if (isDucking && curOneWayPlatform != null) StartCoroutine(DropThroughPlatform());
            }

            // Switching movement modes
            if (flightUnlocked && Input.GetKeyDown(KeyCode.R)) SwitchMovementModes();
            
            IsOnGround();
            IsOnFlip();
        }

        /// <summary>
        /// The state of idle talking.
        /// </summary>
        void DefaultState()
        {
            rb.linearVelocity = new Vector2(0f, 0f);
            rb.linearVelocity += Vector2.down * groundPoundForce;
            animationScript.ChangeAnimationState(IDLE_ANIMATION); 
            joystick.OnPointerUp(null);
            UIMobile.SetActive(false);
            UICore.SetActive(false);
        }

        /// <summary>
        /// Ducking/crouching system.
        /// </summary>
        void Ducking()
        {
            if ((Input.GetKeyDown(KeyCode.S)) && (isGrounded))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                transform.position += new Vector3(0f, -0.3f, 0f);
            }

            if (Input.GetKey(KeyCode.S) || mobileDucking)
            {
                isDucking = true;
                animationScript.ChangeAnimationState(DUCKING_ANIMATION);  
                
                interactionScript.ChangeCameraOffset(interactionScript.cameraOffset.x, 1f);    
            }
            else
            {
                isDucking = false;
                canGroundPound = false;
            }
        }

        /// <summary>
        /// Moving the player.
        /// </summary>
        void Running()
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            // Handling animation
            if ((isGrounded) && (!isDucking))
            {
                if (rb.linearVelocity.x != 0f)
                {
                    animationScript.ChangeAnimationState(RUNNING_ANIMATION);
                }
                else
                {
                    if (!GetComponent<PlayerShooting>().isInShooting)
                        animationScript.ChangeAnimationState(IDLE_ANIMATION);
                }
            }
        }

        /// <summary>
        /// Jumping system.
        /// </summary>
        void Jumping()
        {
            if (!isDucking && (jumpBufferCounter > 0f && (coyoteTimeCounter > 0f || jumpCount < maxJumps)))
            {
                mobileJump = false;

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animationScript.ChangeAnimationState(JUMPING_ANIMATION);

                jumpBufferCounter = 0f;

                if (!isGrounded)
                {
                    jumpCount++;
                }
            }
            else
            {
                jumpBufferCounter -= Time.fixedDeltaTime;
            }

            // Ground pound
            if ((!isGrounded) && (isDucking))
            {
                canGroundPound = true;
                GroundPounding();
            }

            // Handling animation
            if ((!isGrounded) && (!isFalling))
            {
                if (!canGroundPound)
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
        /// Going down from a platform.
        /// </summary>
        IEnumerator DropThroughPlatform()
        {
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D platformCollider = curOneWayPlatform;

            Physics2D.IgnoreCollision(playerCollider, platformCollider);

            yield return new WaitForSeconds(resetCollisionIn);

            Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        }

        /// <summary>
        /// Falling logic.
        /// </summary>
        void Falling()
        {
            if ((transform.position.y < lastPosition.y) && (!isGrounded) && (!canGroundPound))
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
        void GroundPounding()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.linearVelocity += Vector2.down * groundPoundForce;
        }

        /// <summary>
        /// Dragging the player down by gravity.
        /// </summary>
        void GravitationPull()
        {
            if (transform.position.y < lastPosition.y)
            {
                rb.gravityScale += 0.1f;
                                
                if (rb.gravityScale > maxGravitation) rb.gravityScale = maxGravitation;

                moveSpeed = originalMoveSpeed;
            }
            else if (transform.position.y > lastPosition.y)
            {                
                interactionScript.ChangeCameraOffset(interactionScript.cameraOffset.x, 4f);
            }
            else
            {
                rb.gravityScale = minGravitation;

                moveSpeed = originalMoveSpeed;

                if (!isDucking) interactionScript.ChangeCameraOffset(interactionScript.cameraOffset.x, 3f);
            }
        }

        /// <summary>
        /// Flying-based movement.
        /// </summary>
        void FlyingMovement()
        {
            float horInput = 0f;
            float vertInput = 0f;

            // PC input
            horInput += Input.GetAxis("Horizontal");
            vertInput += Input.GetAxis("Vertical");

            // Mobile input
            horInput += joystick.Horizontal;
            vertInput += joystick.Vertical;

            rb.linearVelocity = new Vector2(horInput * moveSpeed, vertInput * moveSpeed);
            
            // Handling animation
            if (!isDucking)
            {
                if (GetComponent<PlayerShooting>().isInShooting) return;
                
                if (isGrounded)
                {                   
                    if (rb.linearVelocity.x != 0f)
                    {
                        animationScript.ChangeAnimationState(RUNNING_ANIMATION);
                    }
                    else
                    {
                        animationScript.ChangeAnimationState(IDLE_ANIMATION);
                    }
                }
                else if (transform.position.y <= lastPosition.y)
                {
                    animationScript.ChangeAnimationState(FALLING_ANIMATION);
                }
                else if (transform.position.y > lastPosition.y)
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
            isGrounded = Physics2D.OverlapBox((Vector2)transform.position + point, size, 0f, groundLayer);

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
        /// Flipping the sprite according to input.
        /// </summary>
        void IsOnFlip()
        {
            float moveInput = 0f;

            // PC input
            moveInput += Input.GetAxis("Horizontal");

            // Mobile input
            moveInput += joystick.Horizontal;

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
            if (obj.gameObject.GetComponent<OneWayPlatform>() != null) curOneWayPlatform = obj.gameObject.GetComponent<Collider2D>();

            if ((obj.gameObject.CompareTag("Cell")) && (canGroundPound))
            {
                canGroundPound = false;

                PlatformerCell cellScriptMid = obj.gameObject.GetComponent<PlatformerCell>();

                CellularAutomataAlgorithm CAScript = FindObjectOfType<CellularAutomataAlgorithm>();
                GameObject cellLeft = CAScript.GetCell(cellScriptMid.posX - 1, cellScriptMid.posY);
                GameObject cellRight = CAScript.GetCell(cellScriptMid.posX + 1, cellScriptMid.posY);

                if (cellLeft != null)
                {
                    PlatformerCell cellScriptLeft = cellLeft.GetComponent<PlatformerCell>();
                    cellScriptLeft.DestroyCell();
                }

                if (cellRight != null)
                {
                    PlatformerCell cellScriptRight = cellRight.GetComponent<PlatformerCell>();
                    cellScriptRight.DestroyCell();
                }

                cellScriptMid.DestroyCell();
                
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                transform.position += new Vector3(0f, -1f, 0f);
            }
        }
        
        /// <summary>
        /// Sent when a collider on another object stops touching this
        /// object's collider (2D physics only).
        /// </summary>
        /// <param name="obj">The Collision2D data associated with this collision.</param>
        void OnCollisionExit2D(Collision2D obj)
        {
            if (obj.gameObject.GetComponent<OneWayPlatform>() != null && curOneWayPlatform != null) 
            {            
                curOneWayPlatform = null;            
            }
        }

        public void SwitchMovementModes()
        {   
            isFlying = !isFlying;
            PlayerPrefs.SetInt("IsFlying", isFlying ? 1 : 0);

            if(isFlying) 
            {
                curPlayerMovementType = PlayerMovementType.Flying;
                switchModesButtonImage.sprite = flyingIcon;
            }
            else if(!isFlying) 
            {
                curPlayerMovementType = PlayerMovementType.Default;
                switchModesButtonImage.sprite = jumpingIcon;
            }
        }

        /// <summary>
        /// Visualization of environment.
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawCube(transform.position + (Vector3)point, (Vector3)size);
        }

    #endregion

    #region UPGRARES

        public void SettingLevels()
        {
            originalMoveSpeed = PlayerPrefs.GetFloat("PlayerSpeed", 7f);
            maxJumps = PlayerPrefs.GetInt("PlayerJump", 2);
            jumpForce = PlayerPrefs.GetFloat("PlayerJumpForce", 10f);

            if(PlayerPrefs.HasKey("PlayerMovementType")) 
            {
                flightUnlocked = true;
                switchModesButtonImage.gameObject.SetActive(true);
                
                isFlying = PlayerPrefs.GetInt("IsFlying", 0) == 1;
                if(isFlying)
                {    
                    curPlayerMovementType = PlayerMovementType.Flying;
                    switchModesButtonImage.sprite = flyingIcon;
                }

                // curPlayerMovementType = PlayerMovementType.Flying;
            }
            else 
            {
                flightUnlocked = false;
                switchModesButtonImage.gameObject.SetActive(false);
                // curPlayerMovementType = PlayerMovementType.Default;
            }
        }
        
        public void AddSpeed()
        {
            originalMoveSpeed++;
        
            PlayerPrefs.SetFloat("PlayerSpeed", originalMoveSpeed);
        }

        public void AddJump()
        {
            maxJumps++;
        
            PlayerPrefs.SetInt("PlayerJump", maxJumps);
        }
        
        public void AddJumpForce()
        {
            jumpForce++;
        
            PlayerPrefs.SetFloat("PlayerJumpForce", jumpForce);
        }

        public void ApplyFlight()
        {
            flightUnlocked = true;
            switchModesButtonImage.gameObject.SetActive(true);
            curPlayerMovementType = PlayerMovementType.Flying;
            isFlying = true;
            
            PlayerPrefs.SetInt("PlayerMovementType", true ? 1 : 0);
        }

    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        MobileStartDucking();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MobileStopDucking();
    }

    public void DisableGUI()
    { 
        joystick.OnPointerUp(null);
        UIMobile.SetActive(false);   
    }

}
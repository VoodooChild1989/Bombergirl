using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region ENUMS

    public enum DirectionType
    {
        Straight, Wave, AutoNavigation, Spontaneous
    }
        
    public enum WaveDirectionType
    {
        Sin, Cos
    }

    public enum ScaleType
    {
        Default, Gradual, Spontaneous
    }

    public enum ProjectileOwner
    {
        Player, Enemy
    }

#endregion

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes = "If projectile is basic, then just use EnemyProjectile or PlayerProjectile. Otherwise, create a separate script that derives from one of them.";

        [Space(10)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public string name = "Projectile_Name";
            public int damageAmount = 1;
            public float lifetime = 3f;
            [ShowOnly] public Rigidbody2D rb;
            [ShowOnly] public Collider2D col;

        [Space(10)] [Header("ANIMATION")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;
            public float frameDuration = 0.1f;
            public GameObject destructionVFX;
            [ShowOnly] public SpriteRenderer sr;
            [ShowOnly] public Camera camera;
            [ShowOnly] public bool onScreen;
            Coroutine animationCoroutine;

        [Space(10)] [Header("MOVEMENT")]
            
            [Header("Basic Variables")]
            public DirectionType curDirType;
            public ScaleType curScaleType;
            public Vector3 movementDir = new Vector3(1f, 0f, 0f);
            public float movementSpeed = 1f;

            [Header("Wave-Based Movement")]
            public WaveDirectionType currentWaveType = WaveDirectionType.Sin;
            public float frequency = 20.0f;
            public float amplitude = 0.3f;
            private float timeCounter = 0f;
            public bool applyAmplitudeManipulation = false;
            public bool applySpeedManipulation = false;

            [Header("Scale Manipulation")]
            public float scaleDuration = 3f;
            public float startScale = 2f;
            public float finalScale = 5f;   

    #endregion

    #region INITIALIZATION

        protected virtual void Awake()
        {
            Initialization();
            SetState();
            Destroy(gameObject, lifetime);

            if(idleSprites.Length > 0) StartCoroutine(ProjectileAnimation());
        }

        protected virtual void FixedUpdate()
        {
            // OnScreen();
        }

        public void Initialization()
        {
            sr = GetComponent<SpriteRenderer>();
            sr.sortingLayerName = "Player";
            sr.sortingOrder = 10;

            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            camera = FindObjectOfType<Camera>();
            col = GetComponent<Collider2D>();
        }

        void OnScreen()
        {
            Vector3 screenPoint = camera.WorldToViewportPoint(transform.position);
            onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            SetPhysicsActive(onScreen);
        }

        public void SetPhysicsActive(bool isActive)
        {
            if (isActive)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                col.enabled = true;
                sr.enabled = true;
                // PlayAnimation(currentAnimSet);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                // rb.bodyType = RigidbodyType2D.Kinematic;
                // col.enabled = false;
                sr.enabled = false;
                // StopAnimation();
            }
        }

    #endregion

    #region EXTRA
    
        public abstract void OnCollisionEnter2D(Collision2D other);

        public void Destruction()
        {
            Instantiate(destructionVFX, gameObject.transform.position, Quaternion.identity);
            // ProjectilePooling.instance.ReturnProjectile(gameObject);
            Destroy(gameObject);
        }

        IEnumerator ProjectileAnimation()
        {
            while(true)
            {
                foreach(Sprite sprite in idleSprites)
                {
                    sr.sprite = sprite;

                    yield return new WaitForSeconds(frameDuration);
                }
            }
        }

    #endregion

    #region MOVEMENT

        void SetState()
        {
            if(applySpeedManipulation)
            {
                StartCoroutine(SpeedManipulation());
            } 

            if(curDirType == DirectionType.Wave)
            {
                timeCounter = 0f;
                movementDir = Vector3.zero;

                if((currentWaveType == WaveDirectionType.Sin) || (currentWaveType == WaveDirectionType.Cos)) 
                {
                    if(applyAmplitudeManipulation)
                    {
                        StartCoroutine(AmplitudeManipulation());
                    }
                }
            }

            if(curScaleType == ScaleType.Gradual)
            {
                StartCoroutine(GradualScaleManipulation());
            }
        }

        public void SetDirection()
        {
            switch(curDirType)
            {
                case DirectionType.Straight:  
                    // Default movement
                    break;
                case DirectionType.Wave:  
                    SetDirectionWave();
                    break;
            }

            Movement();
        }

        /// <summary>
        /// Projectile's basic movement.
        /// </summary>
        void Movement()
        {
            Vector3 movement = movementDir;
            transform.Translate(movement * movementSpeed * Time.fixedDeltaTime); 
        }
        
        /// <summary>
        /// Wave-based movement system
        /// </summary>
        void SetDirectionWave()
        {
            float x = 0f;
            float y = 0f;
            timeCounter += Time.fixedDeltaTime;

            if(currentWaveType == WaveDirectionType.Sin)
            {
                float phaseShift = -Mathf.PI / 2;
                x = Mathf.Sin(timeCounter * frequency + phaseShift) * amplitude;
                y = movementSpeed * Time.fixedDeltaTime;
            }
            else if(currentWaveType == WaveDirectionType.Cos)
            {
                x = Mathf.Sin(timeCounter * frequency) * amplitude;
                y = movementSpeed * Time.fixedDeltaTime;
            }

            Vector3 movement = new Vector3(x, y, 0f);
            transform.Translate(movement); 
        }

        /// <summary>
        /// Gradual alternations in scale's value.
        /// </summary>
        IEnumerator GradualScaleManipulation()
        {
            Vector3 startScaleInstance = new Vector3 (startScale, startScale, startScale);
            Vector3 finalScaleInstance = new Vector3 (finalScale, finalScale, finalScale);
            float elapsed = 0f;

            while(elapsed < scaleDuration)
            {   
                /*
                while((LevelManager.instance.currentLevelState == LevelState.Pause) || (LevelManager.instance.currentLevelState == LevelState.Intro))
                {
                    yield return null;
                }
                */

                float t = elapsed / scaleDuration;
                transform.localScale = Vector3.Lerp(startScaleInstance, finalScaleInstance, t);
                elapsed += Time.deltaTime;

                yield return null;
            }

            transform.localScale = finalScaleInstance;
        }

        /// <summary>
        /// The decrease in velocity.
        /// </summary>
        IEnumerator SpeedManipulation()
        {
            while(true)
            {
                /*
                while((LevelManager.instance.currentLevelState == LevelState.Pause) || (LevelManager.instance.currentLevelState == LevelState.Intro))
                {
                    yield return null;
                }
                */

                movementSpeed -= 0.001f;

                if(movementSpeed <= 1f) movementSpeed = 1f;
                
                yield return null;
            }
        }

        /// <summary>
        /// The widening of the amplitude;
        /// </summary>
        IEnumerator AmplitudeManipulation()
        {
            while(true)
            {
                /*
                while((LevelManager.instance.currentLevelState == LevelState.Pause) || (LevelManager.instance.currentLevelState == LevelState.Intro))
                {
                    yield return null;
                }
                */

                frequency -= 0.01f;
                amplitude += 0.001f;

                if(frequency <= 1f) frequency = 1f;
                if(amplitude >= 5f) frequency = 5f;
                
                yield return null;
            }
        }

    #endregion

}
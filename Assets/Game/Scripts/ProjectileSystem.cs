using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

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
public class ProjectileSystem : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]  
            public ProjectileOwner currentProjectileOwner = ProjectileOwner.Enemy;
            public Vector3 movementDirection = new Vector3(1f, 0f, 0f);
            public float defaultSpeed = 3f;
            public int damageAmount = 1;
            public float destroyObjIn = 10f;
            public AudioClip shootingSFX;
            public GameObject shootingVFX;
            private string targetTag;            
            
            [Space(10)] [Header("Adjustments")]
            [Tooltip("The route of a projectile.")]
            public DirectionType currentDirectionType;

                #region WAVE-BASED MOVEMENT

                    [Tooltip("The type of wave motion applied to the projectile (Sin, Cos).")]
                    public WaveDirectionType currentWaveType = WaveDirectionType.Sin;

                    [Tooltip("Controls how fast the wave oscillates. Higher values result in faster oscillations.")]
                    public float frequency = 20.0f;

                    [Tooltip("Determines the height of the wave. Higher values create wider movements.")]
                    public float amplitude = 0.3f;

                    [Tooltip("Time counter for waves.")]
                    private float timeCounter = 0f;

                    [Tooltip("If enabled, the wave amplitude increases as the projectile moves further.")]
                    public bool applyAmplitudeManipulation = false;

                    [Tooltip("If enabled, the projectile slows down as it moves further.")]
                    public bool applySpeedManipulation = false;

                #endregion

                #region AUTONAVIGATION
                
                    [Tooltip("The duration of the navigation in seconds.")]
                    public float endNavigationIn;
                    [Tooltip("The target that the projectile will move towards to.")]
                    public Transform target; //ShowOnly

                #endregion

            [Tooltip("The type of scale that determines the size of the projectile (Default, Gradual, Spontaneous).")]
            public ScaleType currentScaleType; // Controls the scale of a projectile

                #region GRADUAL SCALE MANIPULATIONS
        
                    [Tooltip("The duration of time from the initial to final scales in seconds.")]  
                    public float scaleDuration = 3f;
                    [Tooltip("The initial value of scale's x-y-z.")]  
                    public float startScale = 2f;
                    [Tooltip("The final value of scale's x-y-z.")]  
                    public float finalScale = 5f;   

                #endregion
            
            [Space(10)] [Header("Animation")]
            
            [Tooltip("The sequence of sprites of the projectile.")]
            public Sprite[] idleSprites;
            [Tooltip("The length of a single frame.")]
            public float frameDuration;
            [ShowOnly] public SpriteRenderer sr;
            public Coroutine animationCoroutine;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            SettingLevels();
            StartCoroutine(ProjectileAnimation());
            SetState();
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            // Add your per-frame logic here.
            // Example: Move objects, check user input, update animations, etc.
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {
            SetDirection();
        }

    #endregion

    #region CUSTOM METHODS

        public void SetState()
        {
            if(applySpeedManipulation)
            {
                StartCoroutine(SpeedManipulation());
            } 

            if(currentDirectionType == DirectionType.Wave)
            {
                timeCounter = 0f;

                if((currentWaveType == WaveDirectionType.Sin) || (currentWaveType == WaveDirectionType.Cos)) 
                {
                    if(applyAmplitudeManipulation)
                    {
                        StartCoroutine(AmplitudeManipulation());
                    }
                }
            }
            else if(currentDirectionType == DirectionType.AutoNavigation) 
            {
                StartCoroutine(EndNavigation());
            }

            if(currentScaleType == ScaleType.Gradual)
            {
                StartCoroutine(GradualScaleManipulation());
            }
        }
        void SetDirection()
        {
            switch(currentDirectionType)
            {
                case DirectionType.Straight:  
                    // 
                    break;
                case DirectionType.Wave:  
                    SetDirectionWave();
                    break;
                case DirectionType.AutoNavigation:  
                    HandleRotation();
                    break;
            }

            Movement();
        }
        
        /// <summary>
        /// Projectile's basic movement.
        /// </summary>
        void Movement()
        {
            Vector3 movement = movementDirection;
            transform.Translate(movement * defaultSpeed * Time.fixedDeltaTime); 
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
                y = defaultSpeed * Time.fixedDeltaTime;
            }
            else if(currentWaveType == WaveDirectionType.Cos)
            {
                x = Mathf.Sin(timeCounter * frequency) * amplitude;
                y = defaultSpeed * Time.fixedDeltaTime;
            }

            Vector3 movement = new Vector3(x, y, 0f);
            transform.Translate(movement); 
        }

        /// <summary>
        /// Rotation according to player's position.
        /// </summary>
        void HandleRotation()
        {
            target = GameObject.Find("Player").transform;
            Vector3 directionToTarget = target.position - transform.position;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0f, 0f, angle - 90f);
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
        /// The Coroutine to end an auto-navigation after a certain amount of time.
        /// </summary>
        IEnumerator EndNavigation()
        {
            float elapsedTime = 0f;
            
            while(elapsedTime < endNavigationIn)
            {
                /*
                while((LevelManager.instance.currentLevelState == LevelState.Pause) || (LevelManager.instance.currentLevelState == LevelState.Intro))
                {
                    yield return null;
                }
                */

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentDirectionType = DirectionType.Straight;
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

                defaultSpeed -= 0.001f;

                if(defaultSpeed <= 1f) defaultSpeed = 1f;
                
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

        void OnCollisionEnter2D(Collision2D obj) 
        {
            Instantiate(shootingVFX, transform.position, Quaternion.identity);

            if(currentProjectileOwner == ProjectileOwner.Player)
            {
                if(obj.gameObject.CompareTag("Cell"))
                {
                    PlatformerCell cellScript = obj.gameObject.GetComponent<PlatformerCell>();
                    cellScript.DamageCell(damageAmount);
                }
                if(obj.gameObject.CompareTag("Enemy"))
                {
                    IDamagable interfaceScript = obj.gameObject.GetComponent<IDamagable>();
                    if(interfaceScript != null) interfaceScript.TakeDamage(damageAmount);
                }
            }
            else
            {   
                if(obj.gameObject.CompareTag("Player"))
                {
                    IDamagable interfaceScript = obj.gameObject.GetComponent<IDamagable>();
                    if(interfaceScript != null) interfaceScript.TakeDamage(damageAmount);
                }
            }

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

        public void SettingLevels()
        {
            damageAmount = PlayerPrefs.GetInt("PlayerProjDamage", 2);

            if(PlayerPrefs.HasKey("PlayerDirectionWavy")) 
            {
                currentDirectionType = DirectionType.Wave;
            }
            else 
            {
                currentDirectionType = DirectionType.Straight;
            }
        }
        
        public void SetWavy()
        {
            currentDirectionType = DirectionType.Wave;
        
            PlayerPrefs.SetInt("PlayerDirectionWavy", true ? 1 : 0);
        }
        
        public void AddDamage()
        {
            damageAmount += 2;
        
            PlayerPrefs.SetInt("PlayerProjDamage", damageAmount);
        }

    #endregion

}
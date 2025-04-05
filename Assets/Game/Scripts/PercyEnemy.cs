using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class PercyEnemy : Enemy
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public int field;
            private Rigidbody2D rb;

            [Header("Attack")]
            public GameObject usualChargeProjectile;
            public int numberOfWaves;
            public Transform projectileSpawnTransform;
            public int numberOfProjectilesPerWave;
            public float delayBetweenProjectilesPerWave;
            public float delayBetweenWaves;
            public float shootingCooldown;
            [ShowOnly] public bool canShoot;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            Initialization();

            canShoot = true;
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if(canShoot)
            {
                Attack();
            }
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {
            Movement();
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        public override void Attack()
        {
            StartCoroutine(AttackCoroutine());
            StartCoroutine(ShootCooldown());
        }

        IEnumerator AttackCoroutine()
        {
            for(int i = 1; i <= numberOfWaves; i++)
            {
                for(int j = 1; j <= numberOfProjectilesPerWave; j++)
                {
                    GameObject projectileInstance = Instantiate(usualChargeProjectile, projectileSpawnTransform.position, Quaternion.identity);
                    ProjectileSystem projectileScript = projectileInstance.GetComponent<ProjectileSystem>();
                    
                    Vector3 rotationAngle = new Vector3(0f, 0f, 0f);
                    if(j == 1)
                    {
                        rotationAngle = new Vector3 (0f, 0f, 120f);
                    }
                    else if(j == 2)
                    {
                        rotationAngle = new Vector3 (0f, 0f, 90f);
                    }
                    else if(j == 3)
                    {
                        rotationAngle = new Vector3 (0f, 0f, 60f);
                    }

                    projectileInstance.transform.eulerAngles = transform.eulerAngles + rotationAngle;
                    projectileScript.movementDirection = new Vector3(1f, 0f, 0f);
                
                    //if(!isOnFlip) projectileScript.movementDirection = new Vector3(1f, 0f, 0f);
                    //else projectileScript.movementDirection = new Vector3(-1f, 0f, 0f);           

                    yield return new WaitForSeconds(delayBetweenProjectilesPerWave);
                }
                
                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }

        public override void Movement()
        {
            if(!isOnFlip) rb.linearVelocity = new Vector2(speedValue, rb.linearVelocity.y);
            else rb.linearVelocity = new Vector2(-speedValue, rb.linearVelocity.y);
        }

        IEnumerator ShootCooldown()
        {
            canShoot = false;

            yield return new WaitForSeconds(shootingCooldown);

            canShoot = true;
        }

    #endregion

}
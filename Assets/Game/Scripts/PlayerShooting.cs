using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class PlayerShooting : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Usual Charge")]
            public GameObject usualChargeProjectile;
            public int numberOfWaves;
            public int numberOfProjectilesPerWave;
            public float delayBetweenWaves;
            public float shootingCooldown;
            [ShowOnly] public bool canShoot;
            
            [Header("Super Charge")]
            public GameObject superChargeProjectile;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            // Initialize variables or cache references here.
            // Example: Setting up components or data before start is called.
            
            // Example of adding a component.
            // MyGame.Utils.AddComponent<SpriteRenderer>(out spriteRenderer, gameObject, this.GetType().Name);   
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            canShoot = true;
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if((Input.GetMouseButtonDown(0)) && (canShoot))
            {
                StartCoroutine(UsualCharge());
                StartCoroutine(ShootCooldown());
            }
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {
            // Add physics-related logic here.
            // Example: Rigidbody movement, applying forces, or collision detection.
        }

    #endregion

    #region CUSTOM METHODS
        IEnumerator UsualCharge()
        {
            for(int i = 1; i <= numberOfWaves; i++)
            {
                for(int j = 1; j <= numberOfProjectilesPerWave; j++)
                {
                    float angle = 360f / numberOfProjectilesPerWave;
    
                    GameObject projectileInstance = Instantiate(usualChargeProjectile, transform.position, Quaternion.identity);
                    ProjectileSystem projectileScript = projectileInstance.GetComponent<ProjectileSystem>();
                    
                    Vector3 rotationAngle = new Vector3 (0f, 0f, angle * j);
                    projectileInstance.transform.eulerAngles = transform.eulerAngles + rotationAngle;
                }
                
                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        void SuperCharge()
        {
            // Implement custom functionality here.
            // Example: Execute game-specific behavior or helper logic.
        }

        /// <summary>
        /// An example coroutine that waits for 2 seconds.
        /// </summary>
        IEnumerator ShootCooldown()
        {
            canShoot = false;

            yield return new WaitForSeconds(shootingCooldown);

            canShoot = true;
        }

    #endregion

}
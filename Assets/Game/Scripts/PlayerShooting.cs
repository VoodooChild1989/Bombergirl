using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public enum ShootingType
{
    Straight, Round
}

public class PlayerShooting : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Usual Charge")]
            public GameObject usualChargeProjectile;
            public ShootingType curShootingType;
            public int numberOfWaves;
            public int numberOfProjectilesPerWave;
            public float delayBetweenWaves;
            public float shootingCooldown;
            [ShowOnly] public bool canShoot;
            [ShowOnly] public bool isInShooting;
            const string ATTACKING_ANIMATION = "Player_Attacking";
            
            [Header("Super Charge")]
            public GameObject superChargeProjectile;
            public bool isShooting;
            public void MobileShoot() => isShooting = true;

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
            SettingLevels();
            canShoot = true;
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if(GameManager.instance.isInCutscene) return;

            if(canShoot)
            {
                if(isShooting)
                {
                    StartCoroutine(UsualCharge());
                    StartCoroutine(ShootCooldown());
                    //isShooting = false;
                }
                else if((Input.GetMouseButtonDown(0)) && (canShoot))
                {
                    //
                }
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
        
        /// <summary>
        /// Called once per frame after the Update().
        /// Use for logic that needs to be held after executing all other tasks.
        /// </summary>
        void LateUpdate()
        {
            if(isShooting) isShooting = false;
        }

    #endregion

    #region CUSTOM METHODS
        IEnumerator UsualCharge()
        {
            isInShooting = true;

            for(int i = 1; i <= numberOfWaves; i++)
            {
                GetComponent<PlayerMovement>().animationScript.ChangeAnimationState(ATTACKING_ANIMATION);

                for(int j = 1; j <= numberOfProjectilesPerWave; j++)
                {
                    if(curShootingType == ShootingType.Straight)
                    {
                        GameObject projectileInstance = Instantiate(usualChargeProjectile, transform.position, Quaternion.identity);
                        ProjectileSystem projectileScript = projectileInstance.GetComponent<ProjectileSystem>();

                        projectileScript.movementDirection = new Vector3(0f, 1f, 0f);
                        
                        PlayerMovement movementScript = GetComponent<PlayerMovement>();
                        if(movementScript.sr.flipX)
                        {
                            projectileInstance.transform.eulerAngles = new Vector3(0f, 180f, -90f);
                        }
                        else
                        {
                            projectileInstance.transform.eulerAngles = new Vector3(0f, 0f, -90f);
                        }

                        yield return new WaitForSeconds(0.1f);
                    }
                    else if(curShootingType == ShootingType.Round)
                    {
                        float angle = 360f / numberOfProjectilesPerWave;

                        GameObject projectileInstance = Instantiate(usualChargeProjectile, transform.position, Quaternion.identity);
                        ProjectileSystem projectileScript = projectileInstance.GetComponent<ProjectileSystem>();
                        Vector3 rotationAngle = new Vector3(0f, 0f, angle * j);

                        if(numberOfProjectilesPerWave == 2)
                        {
                            if(j == 1)
                            {
                                rotationAngle = new Vector3(0f, 180f, -90f);
                            }
                            else if(j == 2)
                            {
                                rotationAngle = new Vector3(0f, 0f, -90f);
                            }
                        }
                        
                        projectileInstance.transform.eulerAngles = rotationAngle;
                    }
                }
                
                yield return new WaitForSeconds(delayBetweenWaves);
            }
            
            isInShooting = false;
        }

        public void SettingLevels()
        {
            numberOfWaves = PlayerPrefs.GetInt("PlayerWave", 1);
            numberOfProjectilesPerWave = PlayerPrefs.GetInt("PlayerNumber", 2);
            delayBetweenWaves = PlayerPrefs.GetFloat("PlayerWaveDelay", 1f);
            shootingCooldown = PlayerPrefs.GetFloat("PlayerShootingCooldown", 1f);
        }

        public void AddWave()
        {   
            numberOfWaves++;
        
            PlayerPrefs.SetInt("PlayerWave", numberOfWaves);
        }

        public void AddNumber()
        {   
            numberOfProjectilesPerWave++;
        
            PlayerPrefs.SetInt("PlayerNumber", numberOfProjectilesPerWave);
        }

        public void ReduceWaveDelay()
        {   
            delayBetweenWaves -= 0.1f;
        
            PlayerPrefs.SetFloat("PlayerWaveDelay", delayBetweenWaves);
        }

        public void ReduceShotCooldown()
        {   
            shootingCooldown -= 0.1f;
        
            PlayerPrefs.SetFloat("PlayerShootingCooldown", shootingCooldown);
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
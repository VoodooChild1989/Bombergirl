using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class AirEnemy : Enemy
{
    
    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            base.Awake();
            rb.gravityScale = 0f;
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            PlayAnimation(idleSprites);
            
            float oscillation = (float)(rng.NextDouble() * 2.0 - 1.0);
            StartCoroutine(ToggleBool(() => canShoot, value => canShoot = value, shootingCooldown + oscillation));
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
                
                float oscillation = (float)(rng.NextDouble() * 2.0 - 1.0);
                StartCoroutine(ToggleBool(() => canShoot, value => canShoot = value, shootingCooldown + oscillation));
            }
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {            
            if((!isShooting) && (canMove))
            {
                Movement();

                if (rng.NextDouble() < 0.01f) StartCoroutine(ToggleBool(() => canMove, value => canMove = value, 1f));
            }
            else if(!isShooting)
            {
                PlayAnimation(idleSprites);
            }
        }

    #endregion

    #region CUSTOM METHODS

        public override void Movement()
        {
            PlayAnimation(runSprites);
            float direction = isOnFlip ? -1f : 1f;
            Vector3 movement = new Vector3(direction * speedValue * Time.fixedDeltaTime, 0f, 0f);
            transform.position += movement;
        }
        
        public override void Attack()
        {
            StartCoroutine(AttackRoutine());
        }

        IEnumerator AttackRoutine()
        {
            isShooting = true;
            PlayAnimation(shootSprites);

            for (int wave = 0; wave < numberOfWaves; wave++)
            {
                for (int i = 0; i < projectilesPerWave; i++)
                {
                    float angle = 360f / projectilesPerWave;
    
                    GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                    //GameObject projectileInstance = ProjectilePooling.instance.GetProjectile(name);
                    projectileInstance.transform.position = projectileSpawnPoint.position;
                    Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
                    
                    Vector3 rotationAngle = new Vector3 (0f, 0f, angle * i);
                    projectileInstance.transform.eulerAngles = transform.eulerAngles + rotationAngle;
                    projectileScript.movementDir = new Vector3(1f, 0f, 0f);

                    yield return new WaitForSeconds(delayBetweenProjectiles);
                }

                yield return new WaitForSeconds(delayBetweenWaves);
                StartCoroutine(ToggleBool(() => canMove, value => canMove = value, 2f));
            }

            isShooting = false;
        }

    #endregion

}
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class GroundEnemy : Enemy
{
    
    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            Debug.Log("Awake gr");
            base.Awake();
            rb.gravityScale = 1f;
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            Debug.Log("Start gr");
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
            // base.FixedUpdate();

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

            for(int wave = 0; wave < numberOfWaves; wave++)
            {
                for(int i = 0; i < projectilesPerWave; i++)
                {
                    GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                    //GameObject proj = ProjectilePooling.instance.GetProjectile(name);
                    proj.transform.position = projectileSpawnPoint.position;
                    Projectile ps = proj.GetComponent<Projectile>();

                    float angle = 90f + (i - 1) * 30f;
                    proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    ps.movementDir = Vector3.right;

                    yield return new WaitForSeconds(delayBetweenProjectiles);
                }

                yield return new WaitForSeconds(delayBetweenWaves);
                StartCoroutine(ToggleBool(() => canMove, value => canMove = value, 2f));
            }

            isShooting = false;
        }

    #endregion

}
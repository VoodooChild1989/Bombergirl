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

    [Header("NOTES")]
        [TextArea(4, 10)] 
        public string notes;

        [Header("Basic Settings")]
        public bool canMove = true;
        public bool isShooting;
        private Rigidbody2D rb;

        [Header("Attack Settings")]
        public GameObject projectilePrefab;
        public Transform projectileSpawnPoint;
        public int numberOfWaves = 3;
        public int projectilesPerWave = 3;
        public float delayBetweenProjectiles = 0.2f;
        public float delayBetweenWaves = 1f;
        public float shootingCooldown = 2f;
        [ShowOnly] public bool canShoot = true;

        [Header("Animation Settings")]
        public Sprite[] idleSprites;
        public Sprite[] runSprites;
        public Sprite[] shootSprites;
        public Sprite[] deathSprites;
        public float frameRate = 0.1f;

        [ShowOnly] public SpriteRenderer sr;
        private Coroutine currentAnimation;
        private Sprite[] currentAnimationSet;
        private System.Random rng = new System.Random();

    #endregion

    #region LIFE CYCLE METHODS
    
        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            Initialization();
            PlayAnimation(idleSprites);

            canMove = true;
            canShoot = true;
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            if (canShoot)
            {
                StartCoroutine(AttackRoutine());
                
                float oscillation = (float)(rng.NextDouble() * 2.0 - 1.0);
                StartCoroutine(ToggleBool(() => canShoot, value => canShoot = value, shootingCooldown + oscillation));
            }
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        private void FixedUpdate()
        {
            if (!isShooting && canMove)
            {
                Move();
                if (rng.NextDouble() < 0.01f)
                    StartCoroutine(ToggleBool(() => canMove, value => canMove = value, 0.5f));
            }
            else if (!isShooting)
            {
                PlayAnimation(idleSprites);
            }
        }

    #endregion

    #region CUSTOM METHODS

        public override void Movement() => Move();

        public override void Attack() => StartCoroutine(AttackRoutine());

        public override void Destruction() => StartCoroutine(EnemyDestruction());

        IEnumerator EnemyDestruction()
        {
            PlayAnimation(deathSprites);
            
            yield return new WaitForSeconds(deathSprites.Length * frameRate);

            DataManager.instance.AddCoins(5);
            Instantiate(base.destructionVFX, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        private void Move()
        {
            PlayAnimation(runSprites);
            float direction = isOnFlip ? -1f : 1f;
            Vector3 movement = new Vector3(direction * speedValue * Time.fixedDeltaTime, 0f, 0f);
            transform.position += movement;
        }

        private IEnumerator AttackRoutine()
        {
            isShooting = true;
            PlayAnimation(shootSprites);

            for (int wave = 0; wave < numberOfWaves; wave++)
            {
                for (int i = 0; i < projectilesPerWave; i++)
                {
                    GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                    ProjectileSystem ps = proj.GetComponent<ProjectileSystem>();

                    float angle = 90f + (i - 1) * 30f; // spread: 60°, 90°, 120°
                    proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    ps.movementDirection = Vector3.right;

                    yield return new WaitForSeconds(delayBetweenProjectiles);
                }

                yield return new WaitForSeconds(delayBetweenWaves);
                StartCoroutine(ToggleBool(() => canMove, value => canMove = value, 2f));
            }

            isShooting = false;
        }

        private IEnumerator ToggleBool(System.Func<bool> getter, System.Action<bool> setter, float duration)
        {
            setter(false);
            yield return new WaitForSeconds(duration);
            setter(true);
        }

        private void PlayAnimation(Sprite[] sprites)
        {
            if (currentAnimationSet == sprites) return;

            currentAnimationSet = sprites;

            if (currentAnimation != null)
                StopCoroutine(currentAnimation);

            currentAnimation = StartCoroutine(AnimationLoop(sprites));
        }

        private IEnumerator AnimationLoop(Sprite[] sprites)
        {
            int index = 0;
            while (true)
            {
                sr.sprite = sprites[index];
                index = (index + 1) % sprites.Length;
                yield return new WaitForSeconds(frameRate);
            }
        }

    #endregion

}
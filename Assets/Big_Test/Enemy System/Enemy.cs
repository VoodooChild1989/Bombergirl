using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void Attack();
}

public interface IDamagable
{
    void TakeDamage(int amount);
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour, IAttackable, IDamagable
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public string name = "Enemy_Name";
            public int killingPrice = 5;
            public int maxHealth = 100;
            [ShowOnly] public int curHealth = 0;
            [ShowOnly] public Rigidbody2D rb;
            [ShowOnly] public Collider2D col;
            public System.Random rng = new System.Random();

        [Space(20)] [Header("MOVEMENT")]
            
            [Header("Basic Variables")]
            public float speedValue = 4f;
            [ShowOnly] public bool canMove = true;
            [ShowOnly] public bool isOnFlip;

        [Space(20)] [Header("ATTACK")]
            
            [Header("Attack Settings")]
            public GameObject projectilePrefab;
            public Transform projectileSpawnPoint;
            public int numberOfWaves = 3;
            public int projectilesPerWave = 3;
            public float delayBetweenProjectiles = 0.2f;
            public float delayBetweenWaves = 1f;
            public float shootingCooldown = 2f;
            [ShowOnly] public bool canShoot;
            [ShowOnly] public bool isShooting;

        [Space(20)] [Header("ANIMATION")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;
            public Sprite[] runSprites;
            public Sprite[] shootSprites;
            public Sprite[] deathSprites;
            public Sprite[] curAnimationSet;
            public float frameDuration = 0.1f;
            public GameObject destructionVFX;
            [ShowOnly] public SpriteRenderer sr;
            [ShowOnly] public Camera camera;
            [ShowOnly] public bool onScreen;
            public Coroutine currentAnimation;

    #endregion

    #region INITIALIZATION

        protected virtual void Awake()
        {
            Initialization();
        }

        protected virtual void FixedUpdate()
        {
            // OnScreen();
        }

        public void Initialization()
        {
            sr = GetComponent<SpriteRenderer>();
            sr.sortingLayerName = "Character";
            sr.sortingOrder = 10;

            rb = GetComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            camera = FindObjectOfType<Camera>();
            col = GetComponent<Collider2D>();

            gameObject.layer = LayerMask.NameToLayer("Enemy");
            gameObject.tag = "Enemy";
            
            curHealth = maxHealth;
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
                rb.bodyType = RigidbodyType2D.Kinematic;
                col.enabled = false;
                sr.enabled = false;
                // StopAnimation();
            }
        }

    #endregion

    #region HEALTH

        public void TakeDamage(int amount)
        {
            curHealth -= amount;

            if(curHealth <= 0) StartCoroutine(Death());
        }

        IEnumerator Death()
        {
            Instantiate(destructionVFX, gameObject.transform.position, Quaternion.identity);
            PlayAnimation(deathSprites);
            
            yield return new WaitForSeconds(deathSprites.Length * frameDuration);

            KillingAward();

            Destroy(gameObject);
        }

        public virtual void KillingAward()
        {   
            DataManager.instance.AddCoins(killingPrice);
            DataManager.instance.AddKill();
        }  

    #endregion

    #region MOVEMENT
    
        public abstract void Movement();
        
        public abstract void Attack();

        public void Flip()
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y += 180f;
            transform.eulerAngles = currentRotation;
            isOnFlip = !isOnFlip;
        }        
        
        public IEnumerator ToggleBool(System.Func<bool> getter, System.Action<bool> setter, float duration)
        {
            setter(false);
            yield return new WaitForSeconds(duration);
            setter(true);
        }

        public void PlayAnimation(Sprite[] sprites)
        {
            if(curAnimationSet == sprites) return;

            curAnimationSet = sprites;

            if(currentAnimation != null) StopCoroutine(currentAnimation);

            currentAnimation = StartCoroutine(AnimationLoop(sprites));
        }

        public IEnumerator AnimationLoop(Sprite[] sprites)
        {
            int index = 0;

            while(true)
            {
                if(onScreen)
                {
                    sr.sprite = sprites[index];
                    index = (index + 1) % sprites.Length;
                }            

                yield return new WaitForSeconds(frameDuration);
            }
        }
        
    #endregion

}
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour, IDamagable, IAttackable
{
    #region FIELDS

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public GameObject destructionVFX;
            public float speedValue = 4f;
            public int maxHealth = 100;
            public int curHealth = 0;
            public bool isOnFlip;
    
    #endregion

    #region METHODS

        /// <summary>
        /// Constructor-like method.
        /// </summary>
        public void Initialization()
        {        
            curHealth = maxHealth;
        }

        /// <summary>
        /// Can be overriden as running or flying.
        /// </summary>
        public abstract void Movement();

        /// <summary>
        /// Can be overriden as shooting or kicking.
        /// </summary>
        public abstract void Attack();

        /// <summary>
        /// Health management method inherited from the interface.
        /// </summary>
        public void TakeDamage(int damage)
        {
            curHealth -= damage;
            
            if (curHealth <= 0)
            {
                Destruction();
            }
        }

        public void Flip()
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y += 180f;
            transform.eulerAngles = currentRotation;
            isOnFlip = !isOnFlip;
        }
        
        /// <summary>
        /// Virtual destruction method, which means it can be used as it is or overriden.
        /// </summary>
        public virtual void Destruction()
        {
            DataManager.instance.AddCoins(5);
            Instantiate(destructionVFX, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject); 
        }   
    
    #endregion
}
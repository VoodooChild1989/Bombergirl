using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour, IDamagable, IAttackable
{
    #region FIELDS

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public GameObject enemyProjectile;
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
            Debug.Log(name + " took " + damage + " damage. Remaining health: " + curHealth);
            
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
            Debug.Log(name + " is being destroyed!");
            Destroy(gameObject); 
        }   
    
    #endregion
}
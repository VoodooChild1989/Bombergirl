using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class PlayerHealth : MonoBehaviour, IDamagable
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public int maxHealth = 100;
            public int curHealth = 0;
            [ShowOnly] public bool isDying;
            [ShowOnly] Slider healthBar;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            healthBar = GameObject.Find("Health (Slider)").GetComponent<Slider>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            SettingLevels();
            curHealth = maxHealth;
            UpdateHealthBar();
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
            // Add physics-related logic here.
            // Example: Rigidbody movement, applying forces, or collision detection.
        }

    #endregion

    #region CUSTOM METHODS

        public void TakeDamage(int damage)
        {
            curHealth -= damage;
            UpdateHealthBar();
            
            if((curHealth <= 0) && (!isDying))
            {
                Destruction();
            }
        }

        void UpdateHealthBar()
        {
            healthBar.value = (float)curHealth / (float)maxHealth;
        }

        [ContextMenu("DIE")]
        void Destruction()
        {
            isDying = true;

            // GetComponent<WindowManager>().window = GameObject.Find("Death (Banner)");   
            // GetComponent<WindowManager>().OpenWindow(true);  

            GameObject.Find("Reset Base (Button)").GetComponent<Button>().onClick.Invoke();
            
            Destroy(gameObject); 
            //StartCoroutine(DelayDeath());
        }

        public void SettingLevels()
        {
            maxHealth = PlayerPrefs.GetInt("PlayerMaxHealth", 20);
        }

        public void AddMaxHealth()
        {   
            maxHealth += 3;
            curHealth = maxHealth;
        
            PlayerPrefs.SetInt("PlayerMaxHealth", maxHealth);
        }

        /// <summary>
        /// An example coroutine that waits for 2 seconds.
        /// </summary>
        IEnumerator DelayDeath()
        {
            yield return new WaitForSeconds(1f);

            Destroy(gameObject); 
        }

    #endregion

}
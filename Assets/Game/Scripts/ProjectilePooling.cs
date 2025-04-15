using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class ProjectilePooling : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public GameObject[] playerProjectiles;
            public GameObject[] enemyProjectiles;
            public GameObject projectilePrefab;
            public int percyCounter;
            public int fairyCounter;
            private List<GameObject> pool = new List<GameObject>();
            private List<GameObject> vfxPool = new List<GameObject>();
            public static ProjectilePooling instance;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this, false);
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            //
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

        public void SpawnProjs()
        {
            for (int i = 0; i < percyCounter; i++)
            {
                GameObject proj = Instantiate(enemyProjectiles[0]);
                proj.name = enemyProjectiles[0].name;
                proj.SetActive(false);
                pool.Add(proj);
            }   

            for (int i = 0; i < fairyCounter; i++)
            {
                GameObject proj = Instantiate(enemyProjectiles[1]);
                proj.name = enemyProjectiles[1].name;
                proj.SetActive(false);
                pool.Add(proj);
            }   
        }

        public GameObject GetProjectile(string characterName)
        {
            string projectileName = characterName + "_Projectile";

            foreach (GameObject proj in pool)
            {
                if ((projectileName == proj.name) && (!proj.activeInHierarchy))
                {
                    proj.SetActive(true);
                    return proj;
                }
            }

            return null;
        }

        public void ReturnProjectile(GameObject proj)
        {
            proj.SetActive(false);
            pool.Add(proj);
        }
        
    #endregion

}
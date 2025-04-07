using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class DoorSystem : MonoBehaviour, ITrigger
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public string sceneName;
            public bool isTransparent;
            [ShowOnly] public SpriteRenderer sr;
            [ShowOnly] public bool canInteract;
            [ShowOnly] public PlayerInteraction interactionScript;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            MyGame.Utils.AddComponent<SpriteRenderer>(out sr, gameObject, this.GetType().Name);   
            sr.color = new Color(1f, 1f, 1f, 0f);
            interactionScript = FindObjectOfType<PlayerInteraction>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            // Perform initial setup that occurs when the game starts.
            // Example: Initialize game state, start coroutines, load resources, etc.
            
            // Example of starting a coroutine.
            // StartCoroutine(ExampleCoroutine());
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if((FindObjectOfType<PlayerShooting>().isShooting) && (canInteract))
            {
                FindObjectOfType<PlayerShooting>().isShooting = false;

                canInteract = false;
                interactionScript.DisableIcon();
                FindObjectOfType<SceneTransitionSystem>().LoadNextLevel(sceneName);
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

        public void TriggerAction()
        {
            canInteract = true;   
            interactionScript.EnableIcon();

            if(!isTransparent) sr.color = new Color(1f, 1f, 1f, 1f);
        }

        void OnTriggerExit2D(Collider2D obj) 
        {
            if((canInteract) && (obj.gameObject.CompareTag("Player"))) 
            {
                canInteract = false;
                interactionScript.DisableIcon();
                sr.color = new Color(1f, 1f, 1f, 0f);
            }
        }

    #endregion

}
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class CutscenePlayer : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public string stringID;
            public bool isTriggered;
            public bool isRepeatative; // isTriggered = true, isPlayerMovable = false, stringID = ""
            [ShowOnly] public PlayableDirector cutsceneToPlay;
            [ShowOnly] public Collider2D triggerArea;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            MyGame.Utils.AddComponent<PlayableDirector>(out cutsceneToPlay, gameObject, this.GetType().Name);   
            
            if(isTriggered) MyGame.Utils.AddComponent<Collider2D>(out triggerArea, gameObject, this.GetType().Name);   
        }

        void OnEnable()
        {
            cutsceneToPlay.stopped += OnPlayableDirectorStopped;
        }

        void OnDisable()
        {
            cutsceneToPlay.stopped -= OnPlayableDirectorStopped;
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            if((!isTriggered) && (GameManager.instance.TryTriggerOncePersistent(stringID)))
            {
                PlayCutscene();
            }
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

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        void PlayCutscene()
        {
            cutsceneToPlay.Play();
            GameManager.instance.isInCutscene = true;
        }

        void OnPlayableDirectorStopped(PlayableDirector aDirector)
        {
            if (cutsceneToPlay == aDirector)
                GameManager.instance.isInCutscene = false;
        }

        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.gameObject.CompareTag("Player")) return;

            if(isTriggered)
            {
                if(isRepeatative)
                {
                    PlayCutscene();
                }
                else if(GameManager.instance.TryTriggerOncePersistent(stringID))
                {
                    PlayCutscene();
                }
            }   
        }

    #endregion

}
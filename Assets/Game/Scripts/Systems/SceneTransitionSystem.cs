using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class SceneTransitionSystem : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public Animator transition;
            public float transitionTime = 1f;
            public Transform pos;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            // Initialize variables or cache references here.
            // Example: Setting up components or data before start is called.
            
            // Example of adding a component.
            // MyGame.Utils.AddComponent<SpriteRenderer>(out spriteRenderer, gameObject, this.GetType().Name);   
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {         
            //if((UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Base") && (DataManager.instance.mainScenePos != Vector3.zero))
               // FindObjectOfType<PlayerInteraction>().gameObject.transform.position = DataManager.instance.mainScenePos;   
            if((UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Base") && (DataManager.instance.mainScenePos != Vector3.zero))
                FindObjectOfType<PlayerInteraction>().gameObject.transform.position = pos.position;   
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

        public void LoadNextLevel(string sceneName)
        {
            StartCoroutine(SceneTransition(sceneName, false));
        }

        public void RetryBase(string sceneName)
        {
            StartCoroutine(SceneTransition(sceneName, true));
        }

        /// <summary>
        /// An example coroutine that waits for 2 seconds.
        /// </summary>
        IEnumerator SceneTransition(string sceneName, bool isRetry = false)
        {
            if((UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Base") && (!isRetry))
                DataManager.instance.mainScenePos = FindObjectOfType<PlayerInteraction>().gameObject.transform.position;
            transition.gameObject.SetActive(true);
            transition.SetTrigger("Start");

            yield return new WaitForSeconds(transitionTime);

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

    #endregion

}
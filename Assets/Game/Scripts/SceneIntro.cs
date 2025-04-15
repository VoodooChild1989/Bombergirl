using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using DG.Tweening;

public class SceneIntro : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public RectTransform obj;

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

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        public void SceneStart()
        {
            obj.anchoredPosition = new Vector2(0, 300);
            obj.DOAnchorPos(Vector2.zero, 1.5f).SetEase(Ease.OutCubic);
        }

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        public void SceneEnd()
        {
            obj.anchoredPosition = Vector2.zero;
            obj.DOAnchorPos(new Vector2(0, 300), 1.5f).SetEase(Ease.OutCubic);
        }

        /// <summary>
        /// An example coroutine that waits for 2 seconds.
        /// </summary>
        IEnumerator ExampleCoroutine()
        {
            // Wait for 2 seconds before executing further code.
            yield return new WaitForSeconds(2f);

            Debug.Log("Action after 2 seconds.");
        }

    #endregion

}
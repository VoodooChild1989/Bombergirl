using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using Unity.Cinemachine;
using DG.Tweening;

public class PlayerInteraction : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public GameObject interactionObj;
            public Vector3 cameraOffset;
            public CinemachineCamera camera;
            private Tween cameraOffsetTween;

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
            DisableIcon();
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
        public void EnableIcon()
        {
            interactionObj.SetActive(true);   
        }

        public void DisableIcon()
        {
            interactionObj.SetActive(false);   
        }

        public void ChangeCameraOffset(float x, float y)
        {
            cameraOffset = new Vector3(x, y, -10f);

            if (cameraOffsetTween != null && cameraOffsetTween.IsActive())
            {
                cameraOffsetTween.Kill();
            }

            CinemachineFollow followScript = camera.GetComponent<CinemachineFollow>();
            cameraOffsetTween = DOTween.To(
                () => followScript.FollowOffset,
                value => followScript.FollowOffset = value,
                cameraOffset,
                0.4f
            ).SetEase(Ease.Linear);
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnTriggerEnter2D(Collider2D obj)
        {
            if(obj.gameObject.CompareTag("Trigger"))
            {
                obj.gameObject.GetComponent<ITrigger>().TriggerAction();
            }
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
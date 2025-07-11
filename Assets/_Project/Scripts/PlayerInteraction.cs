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

public interface IInteractable
{
    public void Interact();
}

public class PlayerInteraction : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public GameObject interactionObj;
            public Vector3 cameraOffset;
            [ShowOnly] public CinemachineCamera camera;
            private Tween cameraOffsetTween;
            public bool isInteracting;
            public void MobileInteraction() => isInteracting = true;
            public Image attackButtonImage;
            public Sprite attackIcon;
            public Sprite interactionIcon;
            public GameObject curObj;

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
            camera = FindObjectOfType<CinemachineCamera>();
            DisableIcon();
        }

        void Update()
        {
            if (curObj != null)
            {
                if (FindObjectOfType<PlayerShooting>().mobileShooting || Input.GetKeyDown(KeyCode.E))
                {
                    curObj.GetComponent<IInteractable>().Interact();
                }
            }
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void LateUpdate()
        {
            if(isInteracting) isInteracting = false;
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
            attackButtonImage.sprite = interactionIcon;
        }

        public void DisableIcon()
        {
            interactionObj.SetActive(false);   
            attackButtonImage.sprite = attackIcon;
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
        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Trigger"))
            {
                other.gameObject.GetComponent<ITrigger>().TriggerAction();
            }

            if (other.GetComponent<IInteractable>() != null) curObj = other.gameObject;
        }
        
        /// <summary>
        /// Sent when another object leaves a trigger collider attached to
        /// this object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        void OnTriggerExit2D(Collider2D other)
        {
            if (curObj != null && other.GetComponent<IInteractable>() != null) curObj = null;          
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
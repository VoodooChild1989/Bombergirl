using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using DG.Tweening;

public class WindowManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]            
            public GameObject window;  
            public float duration;
            public bool mirror;
        
        [Space(20)] [Header("ANIMATIONS")]
            
            [Header("Opening")]
            public Vector2 initialPosSOP;
            public Vector2 targetPosSOP;
            public Vector2 initialScaleSOP;
            public Vector2 targetScaleSOP;
            public float initialTransparencySOP;
            public float targetTransparencySOP;

            [Header("Closing")]
            public Vector2 initialPosSCL;
            public Vector2 targetPosSCL;
            public Vector2 initialScaleSCL;
            public Vector2 targetScaleSCL;
            public float initialTransparencySCL;
            public float targetTransparencySCL;

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
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
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
        /// Opening a window.
        /// </summary>
        public void OpenWindow(bool playAnimation)
        {
            if(playAnimation)
            {                
                if(mirror)
                {
                    MoveWindow(false, initialPosSOP, targetPosSOP, initialScaleSOP, targetScaleSOP, initialTransparencySOP, targetTransparencySOP);
                }
                else
                {     
                    MoveWindow(false, initialPosSOP, targetPosSOP, initialScaleSOP, targetScaleSOP, initialTransparencySOP, targetTransparencySOP);
                }
            }
            else
            {
                CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;   
            }
        }

        /// <summary>
        /// Closing a window.
        /// </summary>
        public void CloseWindow(bool playAnimation)
        {      
            if(playAnimation)
            {
                if(mirror)
                {
                    MoveWindow(true, targetPosSOP, initialPosSOP, targetScaleSOP, initialScaleSOP, targetTransparencySOP, initialTransparencySOP);
                }
                else
                {   
                    MoveWindow(true, initialPosSCL, targetPosSCL, initialScaleSCL, targetScaleSCL, initialTransparencySCL, targetTransparencySCL);
                }
            }
            else
            {
                CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// Moving a window.
        /// </summary>
        void MoveWindow(bool closing, Vector2 initialPos, Vector2 targetPos, Vector2 initialScale, Vector2 targetScale, float initialTransparency, float targetTransparency)
        {
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(targetTransparency, duration);

            RectTransform rectTransform = window.GetComponent<RectTransform>();

            // Scale
            rectTransform.localScale = initialScale;
            rectTransform.DOScale(targetScale, duration).SetEase(Ease.InSine);

            // Position
            rectTransform.anchoredPosition = initialPos;
            if(!closing)
            {
                rectTransform.DOAnchorPos(targetPos, duration);
            }
            else
            {
                rectTransform.DOAnchorPos(targetPos, duration).OnComplete(() => 
                {
                    // Code to execute when animation ends.
                });
            }
        }

        /// <summary>
        /// An example coroutine that waits for 2 seconds.
        /// </summary>
        IEnumerator BasicCoroutine()
        {
            // Wait for 2 seconds before executing further code.
            yield return new WaitForSeconds(2f);

            Debug.Log("Action after 2 seconds.");
        }

    #endregion

}
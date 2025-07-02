using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonChildControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            [ShowOnly] public List<Image> childImages;
            [ShowOnly] public List<TextMeshProUGUI> childTMPs;
            [ShowOnly] public Color[] childColors;
            
            [Header("Own Variables")]
            public bool useOwnColor;
            public bool useOwnOffset;
            public Color ownColor;
            public Vector3 ownOffset;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    Image image = child.gameObject.GetComponent<Image>();
                    childImages.Add(image);
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {                    
                    TextMeshProUGUI text = child.gameObject.GetComponent<TextMeshProUGUI>();
                    childTMPs.Add(text);
                }
            }

            childColors = new Color[transform.childCount];
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
        /// The method that is executed during the pressing of a button.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.gameObject.GetComponent<Image>())
                {
                    Image image = child.gameObject.GetComponent<Image>();
                    childColors[i] = image.color;

                    if(!useOwnColor)
                        image.color = GameManager.instance.pressedColor;
                    else
                        image.color = ownColor;
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    TextMeshProUGUI text = child.gameObject.GetComponent<TextMeshProUGUI>();
                    childColors[i] = text.color;

                    if(!useOwnColor)
                        text.color = GameManager.instance.pressedColor;
                    else
                        text.color = ownColor;
                }

                if(!useOwnOffset)
                    child.localPosition += GameManager.instance.buttonChildOffset;
                else
                    child.localPosition += ownOffset;
            }
        }

        /// <summary>
        /// The method that is executed after the pressing of a button.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().color = childColors[i];
                }
                else if (child.gameObject.GetComponent<TextMeshProUGUI>())
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().color = childColors[i];
                }

                if(!useOwnOffset)
                    child.localPosition -= GameManager.instance.buttonChildOffset;
                else
                    child.localPosition -= ownOffset;
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
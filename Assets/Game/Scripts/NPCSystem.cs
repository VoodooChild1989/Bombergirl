using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class NPCSystem : MonoBehaviour, ITrigger
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public bool isAutoTrigger;
            [ShowOnly] public bool canInteract;
            [ShowOnly] public DialogueSystem dialogueScript;
            [ShowOnly] public PlayerInteraction interactionScript;

            [Header("Animation")]
            public Sprite[] idleSprites;
            public float oneFrameDuration;
            private SpriteRenderer sr;
            [ShowOnly] public Material originalMaterial;
            [ShowOnly] public Material outlineMaterial;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            MyGame.Utils.AddComponent<DialogueSystem>(out dialogueScript, gameObject, this.GetType().Name);   
            MyGame.Utils.AddComponent<SpriteRenderer>(out sr, gameObject, this.GetType().Name);   
            originalMaterial = sr.material;
            outlineMaterial = Resources.Load<Material>("Materials/Outline");  
            interactionScript = FindObjectOfType<PlayerInteraction>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            StartCoroutine(IdleAnimation());
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if((Input.GetKeyDown(KeyCode.Q)) && (!dialogueScript.isDialogueActive) && (canInteract))
            {
                canInteract = false;
                dialogueScript.StartDialogue();
                interactionScript.DisableIcon();
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

        IEnumerator IdleAnimation()
        {
            while(true)
            {
                foreach(Sprite sprite in idleSprites)
                {
                    sr.sprite = sprite;
                    yield return new WaitForSeconds(oneFrameDuration);
                }
            }
        }

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        public void TriggerAction()
        {
            if(isAutoTrigger) 
            {
                dialogueScript.StartDialogue();
            
                //sr.material = outlineMaterial;
                StartCoroutine(SetMaterial());
            }
            else
            {
                canInteract = true;   
                interactionScript.EnableIcon();
                //sr.material = outlineMaterial;
                StartCoroutine(SetMaterial());
            }
        }

        void OnTriggerStay2D(Collider2D obj) 
        {
            if((!canInteract) && (!dialogueScript.isDialogueActive) && (obj.gameObject.CompareTag("Player"))) 
            {
                canInteract = true;   
                interactionScript.EnableIcon();
            }
        }

        void OnTriggerExit2D(Collider2D obj) 
        {
            if((canInteract) && (obj.gameObject.CompareTag("Player"))) 
            {
                canInteract = false;
                interactionScript.DisableIcon();
                sr.material = originalMaterial;
            }
        }

        IEnumerator SetMaterial()
        {
            yield return new WaitForSeconds(0.1f);
            sr.material = outlineMaterial;
        }

    #endregion

}
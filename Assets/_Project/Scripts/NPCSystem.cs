using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class NPCSystem : MonoBehaviour, ITrigger, IInteractable
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
            /*
            if((FindObjectOfType<PlayerShooting>().mobileShooting || Input.GetKeyDown(KeyCode.E)) && (canInteract) && (!dialogueScript.isDialogueActive))
            {
                FindObjectOfType<PlayerShooting>().mobileShooting = false;
                
                canInteract = false;
                dialogueScript.StartDialogue();
                interactionScript.DisableIcon();
            }
            */
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

        public void Interact()
        {
            if ((canInteract) && (!dialogueScript.isDialogueActive))
            {
                FindObjectOfType<PlayerShooting>().mobileShooting = false;
                
                canInteract = false;
                dialogueScript.StartDialogue();
                interactionScript.DisableIcon();
            }
        }

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
            
                if(!dialogueScript.isGlitched) sr.material = outlineMaterial;
            }
            else
            {
                canInteract = true;   
                interactionScript.EnableIcon();
                if(!dialogueScript.isGlitched) sr.material = outlineMaterial;
            }
        }

        void OnTriggerStay2D(Collider2D obj) 
        {
            if(!obj.gameObject.CompareTag("Player"))
            {
                canInteract = false;
                interactionScript.DisableIcon();
                if(!dialogueScript.isGlitched) sr.material = originalMaterial;   

                return;
            }

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
                if(!dialogueScript.isGlitched) sr.material = originalMaterial;
            }
        }

    #endregion

}
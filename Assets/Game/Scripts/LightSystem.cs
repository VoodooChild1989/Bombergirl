using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightSystem : MonoBehaviour, ITrigger
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Light")]
            public float cityLightIntensity;
            public float mineLightIntensity;
            public bool isUnderMine;
            public Light2D globalLight;

            [Header("Music")]
            public AudioSource audioSource;
            public AudioClip cityMusic;
            public AudioClip mineMusic;

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
            globalLight = GameObject.Find("Global Light 2D (Light)").GetComponent<Light2D>();
            audioSource = GameObject.Find("Music (Audio Source)").GetComponent<AudioSource>();
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

        void OnTriggerEnter2D(Collider2D obj)
        {
            if(obj.gameObject.CompareTag("Player"))
            {
                isUnderMine = obj.transform.position.y < transform.position.y;

                if(isUnderMine) ToMine();
                else ToCity();
            }
        }      
        
        void OnTriggerExit2D(Collider2D obj)
        {
            if(obj.gameObject.CompareTag("Player"))
            {
                isUnderMine = obj.transform.position.y < transform.position.y;

                if(isUnderMine) ToMine();
                else ToCity();
            }
        }

        void ToMine()
        {
            WeakerLight();

            audioSource.clip = mineMusic;
            audioSource.Play();
        }

        void ToCity()
        {
            StrongerLight();
            
            audioSource.clip = cityMusic;
            audioSource.Play();
        }

        void WeakerLight()
        {
            DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, mineLightIntensity, 0.5f);
        }

        void StrongerLight()
        {
            DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, cityLightIntensity, 0.5f);   
        }

        /// <summary>
        /// An example custom method.
        /// Replace with your own custom logic.
        /// </summary>
        public void TriggerAction()
        {
            //
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
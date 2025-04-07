using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public interface IButton
{
    public void ButtonAction();
}

public enum UpgradeType
{
    Speed, Jump, Health, JumpForce, Wave, Number, Damage, WaveDelay, ShotCooldown, Flying, WavyProj
}

public class UpgradeButton : MonoBehaviour, IButton
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public string upgradeName;
            public TMP_Text costTMP;
            public UpgradeType curUpgrade;
            public int[] costs;
            public PlayerMovement playerScript;
            public PlayerHealth playerHealthScript;
            public PlayerShooting playerShootingScript;
            [ShowOnly] public int curIndex;
            public GameObject proj;

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
            curIndex = PlayerPrefs.GetInt(upgradeName, 0);

            UpdateText();
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
        public void ButtonAction()
        {
            if(curIndex <= costs.Length - 1)
            {
                if(DataManager.instance.coins >= costs[curIndex])
                {
                    DataManager.instance.AddCoins(-costs[curIndex]);

                    if(curUpgrade == UpgradeType.Speed) playerScript.AddSpeed();
                    else if(curUpgrade == UpgradeType.Jump) playerScript.AddJump();
                    else if(curUpgrade == UpgradeType.JumpForce) playerScript.AddJumpForce();
                    else if(curUpgrade == UpgradeType.Health) playerHealthScript.AddMaxHealth();
                    else if(curUpgrade == UpgradeType.Wave) playerShootingScript.AddWave();
                    else if(curUpgrade == UpgradeType.Number) playerShootingScript.AddNumber();
                    else if(curUpgrade == UpgradeType.Damage) proj.GetComponent<ProjectileSystem>().SetWavy();
                    else if(curUpgrade == UpgradeType.WaveDelay) playerShootingScript.ReduceWaveDelay();
                    else if(curUpgrade == UpgradeType.ShotCooldown) playerShootingScript.ReduceShotCooldown();
                    else if(curUpgrade == UpgradeType.Flying) playerScript.ApplyFlight();
                    else if(curUpgrade == UpgradeType.WavyProj) proj.GetComponent<ProjectileSystem>().SetWavy();

                    curIndex++;
                    PlayerPrefs.SetInt(upgradeName, curIndex);

                    UpdateText();
                }
                else
                {
                    Debug.Log("Not enough");
                }
            }
        }

        void UpdateText()
        {
            if(curIndex <= costs.Length - 1)
            {
                costTMP.text = costs[curIndex].ToString();
            }
            else
            {
                costTMP.text = "-";
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
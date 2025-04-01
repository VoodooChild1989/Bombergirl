using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// The one of the most important scripts in the game.
/// Used to manage the game state.
/// </summary>
public class GameManager : MonoBehaviour
{    

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes = "Manager scripts - universal (singleton) \nSystem scripts - local";

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public bool isGamePlayed;
            public static GameManager instance;

            [Header("Data")]
            public int totalPlayTimeDuration;
            public int gameSessionDuration;

            [Header("UI")]
            public Color pressedColor;
            public Vector3 buttonChildOffset;

    #endregion

    #region LIFE CYCLE METHODS
    
        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        /// <seealso cref="Start"/>
        void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this);
            CheckGameState();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            StartCoroutine(GameTimeCounter());
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        /// <seealso cref="FixedUpdate"/>
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
        /// Checks whether game was lanched for the first time or not by getting the value from the PlayerPrefs.
        /// </summary>
        public void CheckGameState()
        {
            isGamePlayed = PlayerPrefs.GetInt("IsGamePlayed", 0) == 1; 

            if (isGamePlayed)
            {
                totalPlayTimeDuration = PlayerPrefs.GetInt("TotalPlayTimeDuration");

                MyGame.Utils.SystemComment($"The App Was Opened Not For The First Time! Last enter was {GetTimeAway()} seconds ago.");
            }
            else
            {
                MyGame.Utils.SystemComment("The App Was Opened For The First Time!");
            }

            PlayerPrefs.SetInt("IsGamePlayed", 1);
        }

        /// <summary>
        /// Restarts the game by reloading the current scene.
        /// </summary>
        [ContextMenu("Restart The Game")]
        public void RestartTheGame()
        {
            PlayerPrefs.DeleteAll();

            MyGame.Utils.SystemComment("The App Has Been Restarted!");
        }

        /// <summary>
        /// The method triggered when the user closes the game.
        /// </summary>
        public void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("TotalPlayTimeDuration", totalPlayTimeDuration);
            PlayerPrefs.SetString("LastQuitTime", DateTime.UtcNow.ToString());
            PlayerPrefs.Save();

            MyGame.Utils.SystemComment("The App Has Been Closed!");
        }

        /// <summary>
        /// Called whenever a new scene is loaded.
        /// </summary>
        /// <param name="scene">The newly loaded scene.</param>
        /// <param name="mode">The mode in which the scene was loaded.</param>
        void OnSceneChanged(Scene scene, LoadSceneMode mode)
        {
            MyGame.Utils.SystemComment($"The Loaded Scene Is: '{scene.name}'");
        }

        /// <summary>
        /// Counts seconds in the game.
        /// </summary>
        IEnumerator GameTimeCounter()
        {
            while(true)
            {
                yield return new WaitForSeconds(1f);
                gameSessionDuration++;   
                totalPlayTimeDuration++;   
            }
        }

        /// <summary>
        /// Calculating the time of absence.
        /// </summary>
        int GetTimeAway()
        {
            if(PlayerPrefs.HasKey("LastQuitTime"))
            {
                DateTime lastQuitTime;
                
                if(DateTime.TryParse(PlayerPrefs.GetString("LastQuitTime"), out lastQuitTime))
                {
                    return (int)(DateTime.UtcNow - lastQuitTime).TotalSeconds;
                }
            }
            return 0;
        }

    #endregion

}
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DataManager : MonoBehaviour
{
    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public int coins;
            public int killCount;
            public int pupCount;
            public TMP_Text coinsTMP;
            public Vector3 mainScenePos;
            public static DataManager instance;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this);
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            killCount = PlayerPrefs.GetInt("KillCount", 0);
            pupCount = PlayerPrefs.GetInt("PupCount", 0);
            
            coins = PlayerPrefs.GetInt("CoinCount", 200);
            UpdateCoinText();

            // Register for scene change events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Called when the scene has been loaded.
        /// This ensures that the TMP_Text is assigned correctly after each scene change.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Find the TMP_Text in the new scene and assign it
            coinsTMP = GameObject.Find("Coins (TMP)")?.GetComponent<TMP_Text>();
            UpdateCoinText(); // Update coins text after the scene has loaded
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            // You can add any logic that you want to run every frame
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {
            // Add physics-related logic here if necessary
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Add coins to the total and update the TMP_Text.
        /// </summary>
        public void AddCoins(int amount)
        {
            coins += amount;
            UpdateCoinText();
            PlayerPrefs.SetInt("CoinCount", coins);
        }

        public void AddKill()
        {
            killCount++;
            PlayerPrefs.SetInt("KillCount", killCount);
        }

        public void AddPup()
        {
            pupCount++;
            PlayerPrefs.SetInt("PupCount", pupCount);
        }

        /// <summary>
        /// Update the TMP_Text component that shows the coin count.
        /// </summary>
        private void UpdateCoinText()
        {
            if (coinsTMP != null)
            {
                coinsTMP.text = coins.ToString();
            }
        }

    #endregion
}
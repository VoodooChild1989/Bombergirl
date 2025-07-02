using UnityEngine;
using UnityEngine.UI;

public class RetryButton : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public int field;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                GameObject.Find("Scene Transitions (Collection)")
                    .GetComponent<SceneTransitionSystem>()
                    .LoadNextLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            });
        }

    #endregion
}
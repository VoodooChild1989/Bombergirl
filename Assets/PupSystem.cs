using System.Collections;
using UnityEngine;

public class PupSystem : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public int ID;
            public float frameRate = 0.4f;
            public Sprite[] idleSprites;
            private SpriteRenderer _sr;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            MyGame.Utils.AddComponent<SpriteRenderer>(out _sr, gameObject, this.GetType().Name);   

            if (PlayerPrefs.HasKey($"IsCollected_{ID}"))
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        IEnumerator Start()
        {
            while (true)
            {
                foreach (Sprite sprite in idleSprites)
                {
                    _sr.sprite = sprite;
                    yield return new WaitForSeconds(frameRate);
                }
            }
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                DataManager.instance.AddPup();

                PlayerPrefs.SetInt($"IsCollected_{ID}", 1);
                Destroy(gameObject);
            }   
        }

    #endregion

}
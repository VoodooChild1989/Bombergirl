using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public float offsetY;
            [ShowOnly] public GameObject playerObj;
            [ShowOnly] public Collider2D edgeCollider;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            MyGame.Utils.AddComponent<Collider2D>(out edgeCollider, gameObject, this.GetType().Name);   
            playerObj = FindObjectOfType<PlayerInteraction>().gameObject;
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if (transform.position.y > playerObj.transform.position.y + offsetY) edgeCollider.enabled = false;
            else edgeCollider.enabled = true;
        }

    #endregion

}
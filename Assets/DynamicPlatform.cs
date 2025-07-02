using UnityEngine;

public class DynamicPlatform : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public float speed = 3f;
            public Transform[] waypoints;
            private int _curIndex = 0;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            transform.position = waypoints[0].position;

            _curIndex = 1;
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[_curIndex].position, speed * Time.deltaTime);

            if (transform.position == waypoints[_curIndex].position)
            {
                if (_curIndex == waypoints.Length - 1) _curIndex = 0;
                else _curIndex++;
            }
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.SetParent(gameObject.transform);
            }   
        }

        /// <summary>
        /// Sent when a collider on another object stops touching this
        /// object's collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.parent = null;
            }   
        }
        
    #endregion

}
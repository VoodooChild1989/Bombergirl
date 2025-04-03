using UnityEngine;

public class AnimationSystem : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            private Animator animator;
            private string currentState;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {        
            animator = GetComponent<Animator>();
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Changing the animation state according to its name.
        /// </summary>
        public void ChangeAnimationState(string newState)
        {
            if(currentState == newState) return;
                    
            animator.Play(newState);

            currentState = newState;
        }

    #endregion

}
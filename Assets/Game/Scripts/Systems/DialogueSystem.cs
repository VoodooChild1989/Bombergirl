using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using Unity.Cinemachine;
using DG.Tweening;

/// <summary>
/// A system used to manage dialogues in both default and cutscene modes.
/// </summary>
public class DialogueSystem : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes = "'&' - Play a sound";

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Input")]
            public string characterName;
            public string[] lines;
            private int _currentIndexLine;
            public AudioClip[] sounds;
            private int _currentIndexSounds;
            public CinemachineCamera camera;
            public Vector3 offset;
            public AudioClip startVFX;

            [Header("Extra")]
            public PlayableDirector timeline;
            public GameObject banner;
            public TMP_Text characterNameTMP;
            public TMP_Text TMPtext;
            public bool isDialogueActive;
            public bool isLineAnimating;
            private Coroutine _lineAnimation;
            public float typingSpeed;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            //
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            EndDialogue();
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if((isDialogueActive) && (Input.GetKeyDown(KeyCode.E)))
            {
                if(isLineAnimating)
                {
                    isLineAnimating = false;
                    StopCoroutine(_lineAnimation);
                    SetClearLine();
                }
                else
                {
                    NextLine();
                }
            }   
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
        /// Initializing the dialogue state.
        /// </summary>
        public void StartDialogue()
        {
            // For cutscenes
            if(timeline != null) timeline.Pause();

            isDialogueActive = true;
            banner.SetActive(true);
            _currentIndexLine = 0;
            _lineAnimation = StartCoroutine(LineAnimation(lines[_currentIndexLine]));

            FindObjectOfType<PlayerMovement>().isInDialogue = true;
            characterNameTMP.text = characterName;
            camera.Follow = transform; 
            CinemachineFollow followScript = camera.GetComponent<CinemachineFollow>();
            DOTween.To(() => followScript.FollowOffset, 
                    x => followScript.FollowOffset = x, 
                    offset, 
                    1f)
                .SetEase(Ease.InOutQuad);
                
            SFXManager.PlaySFX(startVFX, transform, 1f);
        }
        
        /// <summary>
        /// Moving to the next line.
        /// </summary>
        void NextLine()
        {
            _currentIndexLine++;
            
            // If reached the last line, the dialogue ends
            if(_currentIndexLine == lines.Length)
            {
                EndDialogue();
            }
            else
            {
                _lineAnimation = StartCoroutine(LineAnimation(lines[_currentIndexLine]));
            }        
        }

        /// <summary>
        /// Ingoring all specific signs and assigning a new string.
        /// </summary>
        void SetClearLine()
        {
            TMPtext.text = "";

            for(int i = 0; i < lines[_currentIndexLine].Length; i++)
            {
                char character = lines[_currentIndexLine][i];

                if(character != '&')
                {
                    TMPtext.text += character;
                }
            }
        }

        /// <summary>
        /// Finishing the dialogue state.
        /// </summary>
        public void EndDialogue()
        {
            // For cutscenes
            if(timeline != null) timeline.Resume();

            isDialogueActive = false;
            banner.SetActive(false);
            TMPtext.text = "";
            
            FindObjectOfType<PlayerMovement>().isInDialogue = false;
            characterNameTMP.text = "";
            camera.Follow = FindObjectOfType<PlayerInteraction>().gameObject.transform;
            CinemachineFollow followScript = camera.GetComponent<CinemachineFollow>();
            DOTween.To(() => followScript.FollowOffset, 
                    x => followScript.FollowOffset = x, 
                    FindObjectOfType<PlayerInteraction>().cameraOffset, 
                    1f)
                .SetEase(Ease.InOutQuad);
        }

        /// <summary>
        /// Animating the characters of a line.
        /// </summary>
        /// <param name="line">The line to use.</param>
        IEnumerator LineAnimation(string line)
        {
            TMPtext.text = "";
            isLineAnimating = true;

            for(int i = 0; i < line.Length; i++)
            {
                char character = line[i];

                if(character == '&')
                {
                    if(_currentIndexSounds != sounds.Length - 1)
                    {
                        SFXManager.PlaySFX(sounds[_currentIndexSounds], transform, 1f);
                        _currentIndexSounds++;
                    }
                    
                    continue;
                }

                TMPtext.text += character;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            isLineAnimating = false;
        }

    #endregion

}
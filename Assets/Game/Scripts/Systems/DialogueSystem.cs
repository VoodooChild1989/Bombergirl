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
        public string notes = "Start with a name between two '-'.\n& - Play a sound.";

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Input")]
            public string[] lines;
            private int _currentIndexLine;
            public AudioClip[] sounds;
            private int _currentIndexSounds;

            [Header("Main")]
            public PlayableDirector timeline;
            public GameObject banner;
            public TMP_Text characterNameTMP;
            public TMP_Text TMPtext;
            public float typingSpeed;
            public AudioClip startVFX;
            public CinemachineCamera camera;
            public Vector3 offset;
            [ShowOnly] public GameObject activeEmoji;
            [ShowOnly ]public bool isDialogueActive;
            [ShowOnly] public bool isLineAnimating;
            private Coroutine _lineAnimation;

            [Header("Emojis")]
            public Sprite[] emojis;
            public Vector3 emojisOffset;
            public float emojisTargetScale;

            [Header("SFXs")]
            public AudioClip typingSFX;
            public float sfxVolume;
            [ShowOnly] public GameObject activeSource;

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
                    Destroy(activeSource);
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
            characterNameTMP.text = GetName(lines[_currentIndexLine]);
            ActivateEmoji(GetEmoji(lines[_currentIndexLine]));
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
            
            DeactivateEmoji();
            characterNameTMP.text = GetName(lines[_currentIndexLine]);
        }

        /// <summary>
        /// Ingoring all specific signs and assigning a new string.
        /// </summary>
        void SetClearLine()
        {
            TMPtext.text = "";
            int counter = 0;

            for(int i = 0; i < lines[_currentIndexLine].Length; i++)
            {
                char character = lines[_currentIndexLine][i];

                if((character == '-') && (counter < 3))
                {
                    counter++;
                }
                else if(counter == 3)
                {
                    if(character != '&')
                    {
                        TMPtext.text += character;
                    }
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
            int counter = 0;
            int numberOfSymbols = 0;

            // SFX
            AudioSource tempSource = new GameObject("Temporary Audio Source (Dialogue)").AddComponent<AudioSource>();
            if(SFXManager.instance.audioMixerGroup != null) tempSource.outputAudioMixerGroup = SFXManager.instance.audioMixerGroup;
            tempSource.clip = typingSFX;
            tempSource.volume = Mathf.Clamp01(sfxVolume);
            tempSource.loop = true;
            tempSource.Play();
            Object.Destroy(tempSource.gameObject, NumberOfSymbols(lines[_currentIndexLine]) * typingSpeed);
            activeSource = tempSource.gameObject;

            for(int i = 0; i < line.Length; i++)
            {
                char character = line[i];
                
                if((character == '-') && (counter < 3))
                {
                    counter++;
                }
                else if(counter == 3)
                {
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
            }
            
            isLineAnimating = false;
        }

        string GetName(string line)
        {
            string name = "";
            int counter = 0;

            for(int i = 0; i < line.Length; i++)
            {
                char character = line[i];

                if(character == '-')
                {
                    counter++;

                    if(counter == 2) return name;
                }
                else
                {
                    name += character;
                }
            }

            return "ABC";
        }

        int GetEmoji(string line)
        {
            string name = "";
            int counter = 0;

            for(int i = 0; i < line.Length; i++)
            {
                char character = line[i];

                if(character == '-')
                {
                    counter++;
                }
                else if(counter == 2)
                {
                    return character - '0';
                }
            }

            return 0;
        }

        int NumberOfSymbols(string line)
        {
            int counter = 0;
            int trueCounter = 0;

            for(int i = 0; i < line.Length; i++)
            {
                char character = line[i];
                
                if((character == '-') && (counter < 3))
                {
                    counter++;
                }
                else if(counter == 3)
                {
                    if(character != '&') trueCounter++;
                }
            }   

            return trueCounter;
        }

        void ActivateEmoji(int index)
        {     
            GameObject emojiObj = new GameObject("Emoji");
            activeEmoji = emojiObj;
            emojiObj.transform.position = transform.position + emojisOffset;
            emojiObj.transform.localScale = Vector3.one * 0.1f;

            SpriteRenderer sr = emojiObj.AddComponent<SpriteRenderer>();
            sr.sprite = emojis[index];
            sr.sortingLayerName = "VFX";
            sr.color = new Color(1f, 1f, 1f, 0f);

            emojiObj.transform.DOScale(emojisTargetScale, 0.4f).SetEase(Ease.OutBack);
            sr.DOFade(1f, 0.4f);
        }
        
        void DeactivateEmoji()
        {
            if (activeEmoji == null) return;

            SpriteRenderer sr = activeEmoji.GetComponent<SpriteRenderer>();
            if (sr == null) return;

            // Scale down and fade out
            Sequence hideSequence = DOTween.Sequence();
            hideSequence.Append(activeEmoji.transform.DOScale(0.1f, 0.3f).SetEase(Ease.InBack));
            hideSequence.Join(sr.DOFade(0f, 0.3f));
            hideSequence.OnComplete(() => 
            {
                activeEmoji.SetActive(false);
                ActivateEmoji(GetEmoji(lines[_currentIndexLine]));
            });
        }

    #endregion

}
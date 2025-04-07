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
            public bool isGlitched;
            public string characterID;
            public int killsToFree;
            public Material glitchMat;
            public SpriteRenderer sr;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Input")]
            public string[] lines;
            public string[] originalLines;
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
            private bool isEmojiAnimating = false;

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
            sr = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            if(PlayerPrefs.HasKey(characterID))
            {
                isGlitched = false;
            }

            if(isGlitched)
            {
                sr.material = glitchMat;
            }
            else
            {
                Material spriteLitMat = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
                sr.material = spriteLitMat;
            }

            originalLines = (string[])lines.Clone();

            EndDialogue(true);
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if((isDialogueActive) && (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
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

            if((isGlitched) && (DataManager.instance.killCount >= killsToFree)) 
            {
                isGlitched = false;
                PlayerPrefs.SetInt(characterID, isGlitched ? 1 : 0);

                Material spriteLitMat = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
                sr.material = spriteLitMat;
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

        void DistortText()
        {
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string originalLine = lines[lineIndex];
                int counter = 0;
                string newLine = "";

                for (int i = 0; i < originalLine.Length; i++)
                {
                    char character = originalLine[i];

                    if ((character == '-') && (counter < 3))
                    {
                        counter++;
                        newLine += character;
                    }
                    else if (counter == 3)
                    {
                        if (character != '&')
                        {
                            // Replace character with a random letter
                            char randomLetter = (char)UnityEngine.Random.Range('a', 'z' + 1);
                            newLine += randomLetter;
                        }
                        else
                        {
                            newLine += character;
                        }
                    }
                    else
                    {
                        newLine += character;
                    }
                }

                lines[lineIndex] = newLine;
            }
        }

        /// <summary>
        /// Initializing the dialogue state.
        /// </summary>
        public void StartDialogue()
        {
            if(isGlitched) DistortText();
            else lines = (string[])originalLines.Clone();

            // For cutscenes
            if (timeline != null)
            {
                Debug.Log("A");
                Debug.Log($"Timeline state before: {timeline.state}");
                Debug.Log($"Timeline asset: {timeline.playableAsset}");
                
                if (timeline.state == PlayState.Playing)
                    timeline.Pause();
                else
                    timeline.Play();

                Debug.Log("b");
            }

            isDialogueActive = true;

            // Animate the banner to fade in and scale up
            banner.SetActive(true);
            banner.transform.localScale = Vector3.zero;  // Start with scale 0 for animation
            banner.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

            // Fade in the banner if needed
            CanvasGroup bannerCanvasGroup = banner.GetComponent<CanvasGroup>();
            if (bannerCanvasGroup != null)
            {
                bannerCanvasGroup.alpha = 0f;  // Start from transparent
                bannerCanvasGroup.DOFade(1f, 0.5f);  // Fade in
            }

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
                DeactivateEmoji();
                characterNameTMP.text = GetName(lines[_currentIndexLine]);
            }        
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
        public void EndDialogue(bool start = false)
        {
            // For cutscenes
            if(timeline != null) 
            {
                Debug.Log("C");
                timeline.Resume();
                Debug.Log("D");
            }

            isDialogueActive = false;

            if(!start)
            {
                // Animate the banner to fade out and scale down
                CanvasGroup bannerCanvasGroup = banner.GetComponent<CanvasGroup>();
                if (bannerCanvasGroup != null)
                {
                    bannerCanvasGroup.DOFade(0f, 0.5f);  // Fade out
                }
                banner.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)
                    .OnComplete(() => banner.SetActive(false));  // Hide after animation completes
            }
            else
            {
                banner.SetActive(false);
            }

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

            DeactivateEmoji();
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
            emojiObj.transform.SetParent(gameObject.transform);
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
            SpriteRenderer sr = activeEmoji.GetComponent<SpriteRenderer>();
            if (sr == null) return;

            Sequence hideSequence = DOTween.Sequence();
            hideSequence.Append(activeEmoji.transform.DOScale(0.1f, 0.3f).SetEase(Ease.InBack));
            hideSequence.Join(sr.DOFade(0f, 0.3f));
            hideSequence.OnComplete(() =>
            {
                Destroy(activeEmoji);

                int emojiIndex = GetEmoji(lines[_currentIndexLine]);
                if (emojiIndex >= 0)
                {
                    ActivateEmoji(emojiIndex);
                }
            });
        }

    #endregion

}
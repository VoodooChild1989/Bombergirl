using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// The main audio manager.
/// </summary>
public class AudioManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public AudioMixer audioMixer;
            public static AudioManager instance;

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
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {            
            SetVolumes();
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Setting up the volume values for the channels.
        /// Note: You can't directly assign the value of a channel via script, it needs to be done by sliders.
        /// </summary>
        public void SetVolumes()
        {
            Slider masterSlider = GameObject.Find("Master (Slider)").GetComponent<Slider>();
            Slider musicSlider = GameObject.Find("Music (Slider)").GetComponent<Slider>();
            Slider sfxSlider = GameObject.Find("SFX (Slider)").GetComponent<Slider>();

            masterSlider.value = PlayerPrefs.GetFloat("MasterVolumeValue", 1.0f);
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolumeValue", 1.0f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolumeValue", 1.0f);

            masterSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.RemoveAllListeners();
            
            masterSlider.onValueChanged.AddListener(delegate { SetMasterVolume(masterSlider.value); });
            musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(musicSlider.value); });
            sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(sfxSlider.value); });

            SetMasterVolume(masterSlider.value);
            SetMusicVolume(musicSlider.value);
            SetSFXVolume(sfxSlider.value);
        }

        /// <summary>
        /// Adjusting the value of master channel.
        /// </summary>
        /// <param name="level">The value of the volume.</param>
        public void SetMasterVolume(float level)
        {
            // Applying the values
            level = Mathf.Clamp(level, 0.0001f, 1.0f); 
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);

            // Saving the data
            PlayerPrefs.SetFloat("MasterVolumeValue", level);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Adjusting the value of music channel.
        /// </summary>
        /// <param name="level">The value of the volume.</param>
        public void SetMusicVolume(float level)
        {
            // Applying the values
            level = Mathf.Clamp(level, 0.0001f, 1.0f);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);

            // Saving the data
            PlayerPrefs.SetFloat("MusicVolumeValue", level);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Adjusting the value of sfx channel.
        /// </summary>
        /// <param name="level">The value of the volume.</param>
        public void SetSFXVolume(float level)
        {
            // Applying the values
            level = Mathf.Clamp(level, 0.0001f, 1.0f);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
            
            // Saving the data
            PlayerPrefs.SetFloat("SFXVolumeValue", level);
            PlayerPrefs.Save();
        }

    #endregion

}
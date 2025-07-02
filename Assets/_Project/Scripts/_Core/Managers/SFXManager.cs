using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Manages sound effects in the game.
/// </summary>
public class SFXManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public AudioMixerGroup audioMixerGroup;
            public static SFXManager instance;

    #endregion

    #region BASIC METHODS

        private void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this);
        }

        /// <summary>
        /// Plays a sound effect at a specified position with a given volume.
        /// Example: 
        /// AudioClip sfxClip;
        /// SFXManager.PlaySFX(sfxClip, transform, 1f);
        /// </summary>
        /// <param name="audioClip">The audio clip to play.</param>
        /// <param name="spawnTransform">The position to play the sound effect.</param>
        /// <param name="volume">The volume of the sound effect (0.0 - 1.0).</param>
        public static void PlaySFX(AudioClip audioClip, Transform spawnTransform, float volume)
        {
            if (audioClip == null)
            {
                MyGame.Utils.SystemComment("AudioClip is null! Cannot play sound effect!");
                return;
            }

            // Create a temporary audio source to play the sound
            AudioSource tempSource = new GameObject("Temporary Audio Source").AddComponent<AudioSource>();

            // Adding the audio source to a group
            if(instance.audioMixerGroup != null) tempSource.outputAudioMixerGroup = instance.audioMixerGroup;

            // Datas
            tempSource.clip = audioClip;
            tempSource.volume = Mathf.Clamp01(volume);
            tempSource.transform.position = spawnTransform.position;

            // Play the sound and destroy the GameObject after the clip finishes
            tempSource.Play();
            Object.Destroy(tempSource.gameObject, audioClip.length);
        }

    #endregion
}
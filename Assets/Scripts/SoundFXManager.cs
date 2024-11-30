using UnityEngine;

public class SoundFXManger : MonoBehaviour
{
    public static SoundFXManger Instance;
    [SerializeField] private AudioSource soundFXObject;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float length = audioSource.clip.length;

        Destroy(audioSource.gameObject, length);
    }
}

using UnityEngine;

public class StepSoundController : PlayerSystem
{
    public AudioClip StepSound;

    private Transform CharacterTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterTransform = player.transform;
    }
    public void PlayStepSound()
    {
        SoundFXManger.Instance.PlaySoundFXClip(StepSound, CharacterTransform, 1f);
    }
}

using UnityEngine;

public class AnimationSoundController : PlayerSystem
{
    public AudioClip StepSound;
    public AudioClip SwingFishingRod;
    private Transform CharacterTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterTransform = player.transform;
    }
    public void PlayStepSound()
    {
        SoundFXManger.Instance.PlaySoundFXClip(StepSound, CharacterTransform, 2f);
    }
    public void PlaySwingFishingRod()
    {
        SoundFXManger.Instance.PlaySoundFXClip(SwingFishingRod, CharacterTransform, 1f);
    }
}

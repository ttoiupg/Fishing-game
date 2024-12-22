using System.Collections;
using UnityEngine;

public class FishCatchAnimationHandler : MonoBehaviour
{
    public Animator fishAnimator;
    public ParticleSystem whiteMiddle;
    public ParticleSystem leftSmall;
    public ParticleSystem leftBig;
    public ParticleSystem leftBlue;
    public ParticleSystem rightSmall;
    public ParticleSystem rightBig;
    public ParticleSystem rightBlue;

    public IEnumerator PlayFirstCatchAnimation()
    {
        fishAnimator.enabled = true;
        whiteMiddle.Play();
        leftSmall.Play();
        leftBig.Play();
        leftBlue.Play();
        rightSmall.Play();
        rightBig.Play();
        rightBlue.Play();
        yield return new WaitForSeconds(2.5f);
        fishAnimator.enabled = false;
        whiteMiddle.Stop();
        leftSmall.Stop();
        leftBig.Stop();
        leftBlue.Stop();
        rightSmall.Stop();
        rightBig.Stop();
        rightBlue.Stop();
    }
}

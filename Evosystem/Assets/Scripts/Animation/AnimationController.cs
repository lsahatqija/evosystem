using System;
using UnityEngine;

public abstract class AnimationController : MonoBehaviour
{
    const float k_crossfadeDuration = 0.1f;

    Animator animator;
    CountdownTimer timer;

    float animationLength;

    [HideInInspector] public int locomotionClip = Animator.StringToHash("Locomotion");
    [HideInInspector] public int speedHash = Animator.StringToHash("Speed");
    [HideInInspector] public int attackClip = Animator.StringToHash("Attack");
    [HideInInspector] public int layClip = Animator.StringToHash("Lay");
    [HideInInspector] public int eatClip = Animator.StringToHash("Eat");
    [HideInInspector] public int hitClip = Animator.StringToHash("Hit");
    [HideInInspector] public int deathClip = Animator.StringToHash("Death");
    [HideInInspector] public int mateClip = Animator.StringToHash("Mate");

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        SetLocomotionClip();
        SetAttackClip();
        SetSpeedHash();
        SetHitClip();
        SetEatClip();
        SetLayClip();
    }

    public void SetSpeed(float speed) => animator.SetFloat(speedHash, speed);
    public void Attack() => PlayAnimationUsingTimer(attackClip);
    public void Eat() => PlayAnimationUsingTimer(eatClip);
    public void Lay() => PlayAnimationUsingTimer(layClip);
    public void Hit() => PlayAnimationUsingTimer(hitClip);

    public void Death() => animator.Play(deathClip);
    public void Mate() => PlayAnimationUsingTimer(mateClip);

    void Update() => timer?.Tick(Time.deltaTime);

    void PlayAnimationUsingTimer(int clipHash)
    {
        timer = new CountdownTimer(GetAnimationLength(clipHash));
        timer.OnTimerStart += () => animator.CrossFade(clipHash, k_crossfadeDuration);
        timer.OnTimerStop += () => animator.CrossFade(locomotionClip, k_crossfadeDuration);
        timer.Start();
    }

    public float GetAnimationLength(int hash)
    {
        if (animationLength > 0) return animationLength;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == hash)
            {
                animationLength = clip.length;
                return clip.length;
            }
        }

        return -1f;
    }

    protected abstract void SetLocomotionClip();
    protected abstract void SetAttackClip();
    protected abstract void SetSpeedHash();
    protected abstract void SetLayClip();
    protected abstract void SetEatClip();
    protected abstract void SetHitClip();
    protected abstract void SetDeathClip();
    protected abstract void SetMateClip();


    public void SetAnimator(Animator animator) => this.animator = animator;

}
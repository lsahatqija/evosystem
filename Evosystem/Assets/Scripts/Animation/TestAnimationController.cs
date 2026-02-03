using UnityEngine;

public class TestAnimationController : AnimationController
{
    protected override void SetLocomotionClip()
    {
        locomotionClip = Animator.StringToHash("Locomotion");
    }

    protected override void SetAttackClip()
    {
        attackClip = Animator.StringToHash("Attack");
    }

    protected override void SetSpeedHash()
    {
        speedHash = Animator.StringToHash("Speed");
    }

    protected override void SetLayClip()
    {
        layClip = Animator.StringToHash("Lay");
    }

    protected override void SetEatClip()
    {
        eatClip = Animator.StringToHash("Eat");
    }

    protected override void SetHitClip()
    {
        hitClip = Animator.StringToHash("Hit");
    }

    protected override void SetDeathClip()
    {
        hitClip = Animator.StringToHash("Death");
    }

    protected override void SetMateClip()
    {
        hitClip = Animator.StringToHash("Bounce");
    }
}
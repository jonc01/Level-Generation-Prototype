using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [Header("References / Setup")]
    public Animator anim;
    public PlayerMovement movement;
    public PlayerCombat combat;
    [SerializeField] private float landAnimDuration = 0.3f;
    //[SerializeField] private float dashAnimTime = 0.15f;

    //State Checks
    private float lockedTill;
    [SerializeField] private bool attacking;
    private bool playingDeath;
    Coroutine AttackCO;

    #region Cached
    private int currentState;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Move = Animator.StringToHash("Run");

    //Ground Attacks
    private static readonly int Attack1 = Animator.StringToHash("Attack1");
    private static readonly int Attack2 = Animator.StringToHash("Attack2"); //Reusing this anim for a second attack
    private static readonly int Attack3 = Animator.StringToHash("Attack3");

    /*private static readonly int Attack1_HS = Animator.StringToHash("Attack1_HS");
    private static readonly int Attack2_HS = Animator.StringToHash("AirAttack1_HS");
    private static readonly int Attack3_HS = Animator.StringToHash("Attack2_HS");*/

    //Movement
    //private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Falling = Animator.StringToHash("Falling");
    private static readonly int Landing = Animator.StringToHash("Land");
    private static readonly int Crouch = Animator.StringToHash("Crouch");
    //private static readonly int UnCrouch = Animator.StringToHash("CrouchEnd");
    private static readonly int LedgeGrab = Animator.StringToHash("LedgeGrab");
    private static readonly int Death = Animator.StringToHash("Death");
    //private static readonly int LoopDeath = Animator.StringToHash("LoopDeath");
    #endregion

    private void Awake()
    {
        //Get references
        if (anim == null) anim = GetComponent<Animator>();

        playingDeath = false;
        attacking = false;
    }

    private void Update()
    {
        //Death/Stun override
        /*if (!combat.isAlive)
        {
            if(!playingDeath) PlayDeathAnim();
            return;
        }*/
        //if (movement.isDashing) StopAttackCO();

        if (combat.isStunned)
        {
            //No stun animation :(
            attacking = false;
            return;
        }

        //Attack override
        if (attacking) return;

        //State Checks
        var state = GetState();

        if (state == currentState) return;
        anim.CrossFade(state, 0, 0);
        currentState = state;
    }

    int LockState(int s, float t)
    {
        lockedTill = Time.time + t;
        return s;
    }

    void StopAttackCO()
    {
        if (AttackCO != null) StopCoroutine(AttackCO); //Stops current attackCO if attack speed overrides
        attacking = false;
    }

    public void PlayAttackAnim(int attackNum, float animTime, bool hitStop = false)
    {
        StopAttackCO();
        attacking = true;

        //if (hitStop) attackNum += 3;
        switch (attackNum)
        {
            case 1:
                //anim.Play(Attack1);
                anim.CrossFade(Attack1, 0, 0); //is crossfade needed?
                break;
            case 2:
                //anim.Play(Attack2);
                anim.CrossFade(Attack2, 0, 0);
                break;
            case 3:
                //anim.Play(Attack3);
                anim.CrossFade(Attack3, 0, 0);
                break;
            /*case 4:
                anim.CrossFade(Attack1_HS, 0, 0);
                break;
            case 5:
                anim.CrossFade(Attack2_HS, 0, 0);
                break;
            case 6:
                anim.CrossFade(Attack3_HS, 0, 0);
                break;*/
            default:
                break;
        }
        //float test = anim.GetCurrentAnimatorStateInfo(0).length;
        AttackCO = StartCoroutine(AttackState(animTime));
    }

    IEnumerator AttackState(float duration) //Timer
    {
        yield return new WaitForSeconds(duration);
        attacking = false;
        if (movement.horizontal == 0) anim.CrossFade(Idle, 0, 0);
        else anim.CrossFade(Move, 0, 0);
        //Makes no sense, but otherwise GetState GetsStuck
    }

    private int GetState()
    {
        if (!combat.isAlive) return Death;
        if (Time.time < lockedTill/* && !movement.isDashing*/) return currentState;

        //if (movement.isDashing) return LockState(Dash, dashAnimTime);

        if (movement.isLanding && movement.canPlayLanding) return LockState(Landing, landAnimDuration);
        if (movement.isJumping) return Jump;
        if (movement.isFalling) return Falling;

        if (movement.isGrounded) return movement.horizontal == 0 ? Idle : Move;
        return movement.rb.velocity.y > 0 ? Jump : Fall;
    }

    void PlayDeathAnim()
    {
        anim.Play(Death);
        attacking = false;
        playingDeath = true;
    }

}

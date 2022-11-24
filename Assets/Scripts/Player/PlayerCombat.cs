using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References/Setup")]
    public PlayerMovement movement;
    public AnimatorManager animator;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private bool showGizmos = false;

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Material mWhiteFlash;
    private Material mDefault;

    [SerializeField] float attackMoveDelay = .0f;

    [Header("Ground Attack Animator Setup")]
    //array of attack anim time (ground)
    public int totalNumGroundAttacks;
    public float[] groundAttackDelayAnimTimes;
    public float[] groundAttackFullAnimTimes;
    public float[] groundAttackDamageMultipliers;

    //Controls
    public bool allowInput = true; //toggled when player is stunned, or can't attack

    //Stats
    public float maxHP;
    public float currentHP;

    [SerializeField]
    float attackDamage,
        attackSpeed;

    //Attack Bools and Counters
    public int currAttackIndex; //Used to reference hitboxes
    public int currentAttack;
    float timeSinceAttack;
    public bool isAttacking;

    bool dashImmune;

    //Bools
    public bool isStunned;
    public bool isAlive;
    public bool isKnockedback;

    //Coroutines - Stored to allow interrupts
    Coroutine AttackingCO;
    Coroutine StunnedCO;
    Coroutine KnockbackCO;


    void Start()
    {
        //sr = GetComponentInChildren<SpriteRenderer>();
        mDefault = sr.material;

        isAlive = true;
        isStunned = false;

        currentHP = maxHP;

        allowInput = true; //may not need (using Update return checks)

        isAttacking = false;
        currentAttack = 0;
        currAttackIndex = 0;
        dashImmune = false;
    }

    private void Update()
    {
        if (!isAlive) return;
        timeSinceAttack += Time.deltaTime;
        if (isStunned) return;

        if (Input.GetKeyDown(KeyCode.Y)) //TODO: temp testing
        {
            HealPlayer(20f);
        }

        if (movement.isDashing)
        {
            dashImmune = true;
            return;
        }

        AttackMoveCheck();

        dashImmune = false;
    }

    private void AttackMoveCheck()
    {
        //Delay after attacking to resume movement
        float delay = attackSpeed + attackMoveDelay;
        if (timeSinceAttack <= delay) //air attacks not affected
        {
            movement.ToggleCanMove(false);
        }
        else movement.ToggleCanMove(true);
    }

    //Attacks
    public void Attack1()
    {
        if (!isAlive) return;
        if (timeSinceAttack > attackSpeed && !movement.isDashing)// && !isAttacking && !isAirAttacking) //allowInput
        {
            currentAttack++;

            //Cycle through 3 attacks
            if (currentAttack > totalNumGroundAttacks) currentAttack = 1;
            //Reset Attack after timer reaches 2s
            if (timeSinceAttack > 2.0f) currentAttack = 1;

            //Coroutine
            currAttackIndex = currentAttack - 1; //[0-2]
            AttackingCO = StartCoroutine(Attacking(currentAttack));

            //Reset timer
            timeSinceAttack = 0.0f;
        }
    }

    IEnumerator Attacking(int currentAttack)
    {
        isAttacking = true;

        int currIndex = currentAttack - 1;
        float attackDelay = groundAttackDelayAnimTimes[currIndex];
        float attackAnimFull = groundAttackFullAnimTimes[currIndex];
        float attackAnimEnd = attackAnimFull - attackDelay;

        animator.PlayAttackAnim(currentAttack, attackAnimFull);
        //movement.ToggleCanMove(false); //Disable movement

        yield return new WaitForSeconds(attackDelay);
        CheckAttack(groundAttackDamageMultipliers[currIndex], attackAnimFull);

        yield return new WaitForSeconds(attackAnimEnd); //attackAnimEnd
        //movement.ToggleCanMove(true); //Enable movement
        isAttacking = false;
    }

    void CheckAttack(float damageMultiplier, float attackAnimFull, bool groundAttack = true)
    {
        float damageDealt = attackDamage * damageMultiplier;
        
        //if (damageMultiplier > 1) knockbackStrength = 6; //TODO: set variable defintion in Inspector

        /*foreach (Collider2D player in hitEnemies)
        {
            if (player.GetComponent<Base_EnemyCombat>() != null)
            {
                player.GetComponent<Base_EnemyCombat>().TakeDamage(damageDealt, true, knockbackStrength);
                HitStopAnim(attackAnimFull, groundAttack);

                if (isAirAttacking) movement.Float(.3f);
                //ScreenShakeListener.Instance.Shake(1); //TODO: if Crit
                //hitStop.Stop(.1f); //Successful hit
            }
        }*/
    }

    /*void HitStopAnim(float attackAnimFull, bool ground)
    {
        if (ground) animator.PlayAttackAnim(currentAttack, attackAnimFull, true);
        else animator.PlayAirAttackAnim(currentAirAttack, attackAnimFull, true);
    }*/

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    public void GetKnockback(bool enemyToRight, float strength = 8, float delay = .5f)
    {
        if (!isAlive) return;
        KnockbackNullCheckCO();

        isKnockedback = true;

        float temp = enemyToRight != true ? 1 : -1; //get knocked back in opposite direction of player
        Vector2 direction = new Vector2(temp, movement.rb.velocity.y);
        movement.rb.AddForce(direction * strength, ForceMode2D.Impulse);

        KnockbackCO = StartCoroutine(KnockbackReset(delay));
    }

    IEnumerator KnockbackReset(float delay, float recoveryDelay = .1f)
    {
        yield return new WaitForSeconds(delay);
        movement.rb.velocity = Vector3.zero;
        movement.canMove = false;
        yield return new WaitForSeconds(recoveryDelay); //delay before allowing move again
        movement.canMove = true;
        isKnockedback = false;
    }

    void KnockbackNullCheckCO()
    {
        if (KnockbackCO == null) return;
        StopCoroutine(KnockbackCO);
        movement.canMove = true;
        isKnockedback = false;
    }

    public void GetStunned(float stunDuration)
    {
        //Interrupt player attack
        if (AttackingCO != null) StopCoroutine(AttackingCO);

        isAttacking = false;

        StunnedCO = StartCoroutine(StunTimer(stunDuration));
    }

    IEnumerator StunTimer(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    public void TakeDamage(float damageTaken)
    {
        if (!isAlive) return;
        if (dashImmune) return; //TODO: needs testing

        HitFlash();

        //Shake screen based on how much damage is taken (% of max HP)
        //float damageToHealth = damageTaken / maxHP;
        /*int shakeNum;

        if (damageToHealth >= .3f) shakeNum = 2;
        else if (damageToHealth >= .15f) shakeNum = 1;
        else shakeNum = 0;*/

        //ScreenShakeListener.Instance.Shake(shakeNum);
        //

        currentHP -= damageTaken;
        CheckDie();
    }

    public void HealPlayer(float healAmount)
    {
        if (!isAlive) return;

        if (currentHP < maxHP) currentHP += healAmount;
        if (currentHP > maxHP) currentHP = maxHP;

    }

    void HitFlash()
    {
        sr.material = mWhiteFlash;
        Invoke("ResetMaterial", .1f);
    }

    void ResetMaterial()
    {
        sr.material = mDefault;
    }

    public void CancelAttack()
    {
        if (AttackingCO != null) StopCoroutine(AttackingCO);
    }


    void CheckDie()
    {
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isAlive = false; //Triggers death anim in AnimatorManager

        //Stop Coroutines
        //if (AttackingCO != null) StopCoroutine(AttackingCO);
        //if (StunnedCO != null) StopCoroutine(StunnedCO);
        StopAllCoroutines();

        movement.ToggleCanMove(false);

        isAlive = false;
    }

}

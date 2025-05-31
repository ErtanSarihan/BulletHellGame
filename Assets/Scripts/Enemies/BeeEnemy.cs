using System;
using System.Collections;
using UnityEngine;

namespace Enemies {
  public class BeeEnemy : EnemyBase {
    [Header("Bee Specific")]
    [SerializeField]
    private Animator animator;
    
    private const string MoveState = "Move";
    private const string AttackState = "Attack";
    private const string HorizontalParam = "Horizontal";
    private const string VerticalParam = "Vertical";

    protected override void Start() {
      base.Start();
      if (!animator)
        animator = GetComponent<Animator>();
    }

    protected override void HandleMovement() {
      if (!playerTransform || isAttacking) {
        SetAnimatorMove(false);
        return;
      }

      // Only move if not in attack range OR if attack is on cooldown and player is out of range
      if (!isInAttackRange || (!canAttack && !isInAttackRange)) {
        // Calculate direction to player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        // Move towards player
        rb.linearVelocity = direction * moveSpeed;
        
        // Update animator parameters with actual direction values
        animator.SetFloat(HorizontalParam, direction.x);
        animator.SetFloat(VerticalParam, direction.y);
        
        SetAnimatorMove(true);
      } else {
        // Stop moving if in attack range and can attack, or if attack is on cooldown but still in range
        rb.linearVelocity = Vector2.zero;
        SetAnimatorMove(false);
      }
    }

    protected override void StartAttack() {
      base.StartAttack();
      SetAnimatorAttack(true);
    }

    protected override void DealDamage() {
      base.DealDamage();
      SetAnimatorAttack(false);
    }
    
    private void SetAnimatorMove(bool moving) {
      if (animator)
        animator.SetBool(MoveState, moving);
    }

    private void SetAnimatorAttack(bool attacking) {
      if (animator)
        animator.SetBool(AttackState, attacking);
    }
  }
}
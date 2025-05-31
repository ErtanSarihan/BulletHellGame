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

      // Calculate direction to player
      Vector2 direction = (playerTransform.position - transform.position).normalized;
      
      // Move towards player
      rb.linearVelocity = direction * moveSpeed;
      
      // Update animator parameters with actual direction values
      animator.SetFloat(HorizontalParam, direction.x);
      animator.SetFloat(VerticalParam, direction.y);
      
      SetAnimatorMove(true);
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
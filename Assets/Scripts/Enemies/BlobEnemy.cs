using UnityEngine;

namespace Enemies {
  public class BlobEnemy : EnemyBase {
    protected override void HandleMovement() {
      if (!playerTransform) return;
            
      // Calculate direction to player
      Vector2 direction = (playerTransform.position - transform.position).normalized;
            
      // Move towards player
      rb.linearVelocity = direction * moveSpeed;
      
    }
  }
}
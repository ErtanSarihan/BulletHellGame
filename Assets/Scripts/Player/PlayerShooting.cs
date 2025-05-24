using System.Collections.Generic;
using UnityEngine;

namespace Player {
  public class PlayerShooting : MonoBehaviour {
    [Header("Shooting Settings")]
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private float fireDelay = 0.5f;

    [SerializeField]
    private float range = 10f;

    [SerializeField]
    private LayerMask enemyLayer = 1 << 6;

    private float _nextFireTime;
    private Transform _currentTarget;
    private List<Collider2D> _enemyBuffer;
    private ContactFilter2D _enemyFilter;

    void Awake() {
      // Pre-allocate the list once
      _enemyBuffer = new List<Collider2D>();
            
      // Setup contact filter for enemy layer
      _enemyFilter = new ContactFilter2D {
        useLayerMask = true, layerMask = enemyLayer,  useTriggers = true
      };
    }
    
    void Update() {
      // Find nearest enemy
      _currentTarget = FindNearestEnemy();

      // Auto shoot if we have a target and enough time has passed
      if (_currentTarget && Time.time >= _nextFireTime) {
        Shoot();
        _nextFireTime = Time.time + fireDelay;
      }
    }

    Transform FindNearestEnemy() {
      // Find all enemies within range - only checks colliders on layer 6 (Enemy layer)
      // In newer Unity versions, OverlapCircle uses List<Collider2D> with ContactFilter2D
      // This avoids allocations by reusing the same list
      int enemyCount = Physics2D.OverlapCircle(transform.position, range, _enemyFilter, _enemyBuffer);
            
      if (enemyCount == 0) return null;
            
      // Find the closest enemy
      Transform nearest = null;
      float minDistance = float.MaxValue;
            
      // Iterate through the enemies found
      foreach (Collider2D enemy in _enemyBuffer) {
        if (enemy) {
          float distance = Vector2.Distance(transform.position, enemy.transform.position);
          if (distance < minDistance) {
            minDistance = distance;
            nearest = enemy.transform;
          }
        }
      }
            
      return nearest;
    }

    void Shoot() {
      if (!bulletPrefab || !firePoint) {
        Debug.LogWarning("Bullet prefab or fire point not set!");
        return;
      }
      if (!_currentTarget) {
        Debug.LogWarning("No target to shoot at!");
        return;
      }
      
      GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
      Bullet bulletScript = bullet.GetComponent<Bullet>();
            
      if (bulletScript) {
        // Calculate direction to target
        Vector2 direction = (_currentTarget.position - firePoint.position).normalized;
        bulletScript.SetDirection(direction);
      } else {
        Debug.LogError("Bullet prefab is missing Bullet script component!");
        Destroy(bullet);
      }
    }
  }
}
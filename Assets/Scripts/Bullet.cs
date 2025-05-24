using System;
using UnityEngine;

public class Bullet : MonoBehaviour {
  [SerializeField]
  private float speed = 10f;

  [SerializeField]
  private float damage = 1f;

  [SerializeField]
  private float lifeTime = 3f;

  private Rigidbody2D _rigidbody;

  private void Awake() {
    _rigidbody = GetComponent<Rigidbody2D>();
  }

  private void Start() {
    Destroy(gameObject, lifeTime);
  }

  public void SetDirection(Vector2 direction) {
    if (!_rigidbody) {
      Debug.LogError("Bullet is missing Rigidbody2D component!");
      return;
    }
    _rigidbody.linearVelocity = direction.normalized * speed;
    
    // rotate bullet to face the direction
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  }

  void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Enemy")) {
      var enemy = other.GetComponent<Enemies.Enemy>();
      if (enemy) {
        enemy.TakeDamage(damage);
      }
      Destroy(gameObject);
    }
  }

}
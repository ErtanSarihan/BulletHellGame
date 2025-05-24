using UnityEngine;

namespace Enemies {
  public class Enemy : MonoBehaviour {
    private float _health = 10f;
    [SerializeField]
    private float moveSpeed = 1f;
    
    private Transform _player;
    private Rigidbody2D _rigidbody;

    private void Start() {
      _rigidbody = GetComponent<Rigidbody2D>();
      GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
      if (playerObject) {
        _player = playerObject.transform;
      }
    }
    private void Update() {
      if (_player) {
        Vector2 direction = (_player.position - transform.position).normalized;
        _rigidbody.linearVelocity = direction * moveSpeed;
      }
    }

    public void TakeDamage(float damage) {
      _health -= damage;
      if (_health <= 0) {
        Destroy(gameObject);
      }
    }

    // Getter for auto aim
    public Transform GetTransform() {
      return transform;
    }
  }
}
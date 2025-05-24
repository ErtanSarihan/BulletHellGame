using UnityEngine;

namespace Enemies {
  public class Enemy : MonoBehaviour {
    [SerializeField]
    private float _health = 10f;
    [SerializeField]
    private float _maxHealth = 10f;
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

      _maxHealth = _health;
      Debug.Log($"[ENEMY SPAWN] {gameObject.name} spawned with {_health} health");
    }

    private void Update() {
      if (_player) {
        Vector2 direction = (_player.position - transform.position).normalized;
        _rigidbody.linearVelocity = direction * moveSpeed;
      }
    }

    public void TakeDamage(float damage) {
      _health -= damage;
      Debug.Log($"[ENEMY DAMAGE] {gameObject.name} took {damage} damage! Health: {_health}/{_maxHealth}");
      if (_health <= 0) {
        Debug.Log($"[ENEMY DEATH] {gameObject.name} has been destroyed!");
        Destroy(gameObject);
      }
    }

    // Getter for auto aim
    public Transform GetTransform() {
      return transform;
    }
  }
}
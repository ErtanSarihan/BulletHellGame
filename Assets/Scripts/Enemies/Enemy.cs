using System.Collections;
using UnityEngine;

namespace Enemies {
  public class Enemy : MonoBehaviour {
    [SerializeField]
    private float health = 10f;
    private float _maxHealth = 10f;
    [SerializeField]
    private float moveSpeed = 1f;

    private Transform _player;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    // <-----Death animation----->
    private float _deathFadeDuration = 0.5f;
    private bool _isDying = false; 
    // turnWhiteOnDeath -> always true for now can be extracted
    private Collider2D _collider;
    
    // <-----------Flash on Hit-------------->
    private Color _originalColor;
    private Color _damageColor = Color.red;
    private Coroutine _flashCoroutine;
    private float _flashDuration = 0.2f;

    
    private void Start() {
      _rigidbody = GetComponent<Rigidbody2D>();
      _spriteRenderer = GetComponent<SpriteRenderer>();
      GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
      _collider = GetComponent<Collider2D>();

      if (playerObject) _player = playerObject.transform;
      if (_spriteRenderer) _originalColor = _spriteRenderer.color;

      _maxHealth = health;
      // Debug.Log($"[ENEMY SPAWN] {gameObject.name} spawned with {health} health");
    }

    private void Update() {
      if (_isDying) return;
      
      if (_player) {
        Vector2 direction = (_player.position - transform.position).normalized;
        _rigidbody.linearVelocity = direction * moveSpeed;

        // Optional: Rotate to face player
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
      }
    }

    public void TakeDamage(float damage) {
      if (_isDying) return;
      
      health -= damage;
      if (_flashCoroutine != null) {
        StopCoroutine(_flashCoroutine);
      }
      _flashCoroutine = StartCoroutine(DamageFlash());
      // Debug.Log($"[ENEMY DAMAGE] {gameObject.name} took {damage} damage! Health: {health}/{_maxHealth}");
      if (health <= 0) {
        // Debug.Log($"[ENEMY DEATH] {gameObject.name} has been destroyed!");
        Die();
      }
    }

    IEnumerator DamageFlash() {
      if (_spriteRenderer) {
        _spriteRenderer.color = _damageColor;
        yield return new WaitForSeconds(_flashDuration);
        _spriteRenderer.color = _originalColor;
      }
    }
    
    void Die() {
      if (_isDying) return;
      _isDying = true;
      _rigidbody.linearVelocity = Vector2.zero;
      if (_collider) _collider.enabled = false;
      // Stop any ongoing flash
      if (_flashCoroutine != null) {
        StopCoroutine(_flashCoroutine);
      }

      StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation() {
      if (!_spriteRenderer) {
        Destroy(gameObject);
        yield break;
      }

      _spriteRenderer.color = Color.white;
      yield return new WaitForSeconds(0.1f);
      
      float elapsedTime = 0f;
      Color startColor = _spriteRenderer.color;
      
      while (elapsedTime < _deathFadeDuration) {
        elapsedTime += Time.deltaTime;
        float normalizedTime = elapsedTime / _deathFadeDuration;
        float alpha = 1f - normalizedTime;
        Color currentColor = startColor;
        currentColor.a = alpha;
        _spriteRenderer.color = currentColor;
        yield return null;
      }
      
      Destroy(gameObject);
    }
    
    void OnDestroy() {
      // Clean up coroutines
      if (_flashCoroutine != null) {
        StopCoroutine(_flashCoroutine);
      }
    }

    // Getter for auto aim
    public Transform GetTransform() {
      return transform;
    }
  }
}
using System.Collections;
using Player;
using UnityEngine;

namespace Enemies {
  public abstract class EnemyBase : MonoBehaviour {
    [Header("Stats")]
    [SerializeField]
    protected float healthPoints = 10f;
    [SerializeField]
    protected float moveSpeed = 10f;
    [SerializeField]
    protected float experienceGiven = 10f;

    [Header("Visual Effects")]
    [SerializeField]
    protected float flashDurationOnDamageTaken = 0.2f;
    [SerializeField]
    protected Color damagedColor = Color.red;
    [SerializeField]
    protected float fadeDurationOnDeath = 0.3f;

    // <--------COMPONENTS---------->
    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D col;

    // <---------RUNTIME VARIABLES---------->
    protected float maxHealthPoints;
    protected Color originalColor;
    protected bool isDying;
    private Coroutine _flashCoroutine;

    protected virtual void Start() {
      InitializeComponents();
      FindPlayer();
      SetupEnemy();
    }

    protected virtual void Update() {
      if (isDying) return;
      HandleMovement();
    }

    protected abstract void HandleMovement();

    public virtual void TakeDamage(float damage) {
      if (isDying) return;

      healthPoints -= damage;
      OnDamageTaken(damage);

      if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
      _flashCoroutine = StartCoroutine(DamageFlash());

      if (healthPoints <= 0) {
        Die();
      }
    }

    protected virtual void InitializeComponents() {
      rb = GetComponent<Rigidbody2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      col = GetComponent<Collider2D>();

      if (spriteRenderer) originalColor = spriteRenderer.color;
    }

    protected virtual void FindPlayer() {
      GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
      if (playerObject) playerTransform = playerObject.transform;
    }

    protected virtual void SetupEnemy() {
      maxHealthPoints = healthPoints;
    }

    protected virtual void OnDamageTaken(float damage) {
      // display damage taken
      // remove hp from hp bar 
    }

    protected virtual void Die() {
      if (isDying) return;
      isDying = true;

      OnDeath();
      DisableEnemy();

      if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
      StartCoroutine(DeathAnimation());
    }

    protected virtual void OnDeath() {
      
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      if (!player) return;
      
      PlayerStats playerStats = player.GetComponent<PlayerStats>();
      if (!playerStats) return;
      
      playerStats.AddXp(experienceGiven);
      
      Debug.Log("Player gained XP: " + experienceGiven);
    }

    protected virtual void DisableEnemy() {
      rb.linearVelocity = Vector2.zero;
      if (col) col.enabled = false;
    }

    protected virtual IEnumerator DeathAnimation() {
      if (!spriteRenderer) {
        Destroy(gameObject);
        yield break;
      }

      spriteRenderer.color = Color.white;
      yield return new WaitForSeconds(0.1f);

      float elapsedTime = 0f;
      Color startColor = spriteRenderer.color;

      while (elapsedTime < fadeDurationOnDeath) {
        elapsedTime += Time.deltaTime;
        float normalizedTime = elapsedTime / fadeDurationOnDeath;
        float alpha = 1f - normalizedTime;

        Color currentColor = startColor;
        currentColor.a = alpha;
        spriteRenderer.color = currentColor;
        yield return null;
      }

      Destroy(gameObject);
    }

    protected virtual IEnumerator DamageFlash() {
      if (spriteRenderer) {
        spriteRenderer.color = damagedColor;
        yield return new WaitForSeconds(flashDurationOnDamageTaken);
        if (!isDying) spriteRenderer.color = originalColor;
      }
    }

    protected virtual void OnDestroy() {
      if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
    }

    public Transform GetTransform() => transform;
    public float GetHealth() => healthPoints;
    public float GetMaxHealth() => maxHealthPoints;
    public bool IsDying() => isDying;
  }
}
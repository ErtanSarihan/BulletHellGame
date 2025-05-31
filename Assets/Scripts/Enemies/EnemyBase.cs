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

    [Header("Attack Settings")]
    [SerializeField]
    protected float attackRange = 1.5f;
    [SerializeField]
    protected float attackDamage = 1f;
    [SerializeField]
    protected float attackCooldown = 1f;
    [SerializeField]
    protected float attackDuration = 0.5f;
    protected bool canAttack = true;
    protected bool isAttacking = false;

    [Header("Visual Effects")]
    [SerializeField]
    protected float flashDurationOnDamageTaken = 0.2f;
    [SerializeField]
    protected Color damagedColor = Color.red;
    [SerializeField]
    protected float fadeDurationOnDeath = 0.3f;

    // <--------COMPONENTS---------->
    protected Transform playerTransform;
    protected PlayerStats playerStats;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D col;

    // <---------RUNTIME VARIABLES---------->
    protected float maxHealthPoints;
    protected Color originalColor;
    protected bool isDying;
    private Coroutine _flashCoroutine;
    private Coroutine _attackCooldownCoroutine;
    private Coroutine _attackDurationCoroutine;

    protected virtual void Start() {
      InitializeComponents();
      FindPlayer();
      SetupEnemy();
    }

    protected virtual void Update() {
      if (isDying) return;
      HandleMovement();

      if (canAttack && playerTransform && !isAttacking) {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange) {
          StartAttack();
        }
      }
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
      if (playerObject) {
        playerTransform = playerObject.transform;
        playerStats = playerObject.GetComponent<PlayerStats>();
      }
    }

    protected virtual void SetupEnemy() {
      maxHealthPoints = healthPoints;
    }

    protected virtual void OnDamageTaken(float damage) {
      // Override in derived classes for specific damage effects
    }

    protected virtual void DealDamage() {
      if (!playerTransform || !playerStats) return;
      var distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
      if (distanceToPlayer <= attackRange) {
        playerStats.TakeDamage(attackDamage);
      }
    }

    protected virtual void StartAttack() {
      if (isAttacking) return;
      isAttacking = true;
      canAttack = false;
      
      if (rb) rb.linearVelocity = Vector2.zero;
      
      if (_attackDurationCoroutine != null) StopCoroutine(_attackDurationCoroutine);
      _attackDurationCoroutine = StartCoroutine(AttackDurationRoutine());
    }

    private IEnumerator AttackDurationRoutine() {
      yield return new WaitForSeconds(attackDuration);
      DealDamage();
      isAttacking = false;
      StartAttackCooldown();
    }
    
    protected void StartAttackCooldown() {
      if (_attackCooldownCoroutine != null) {
        StopCoroutine(_attackCooldownCoroutine);
      }
      _attackCooldownCoroutine = StartCoroutine(AttackCooldownRoutine());
    }

    private IEnumerator AttackCooldownRoutine() {
      canAttack = false;
      yield return new WaitForSeconds(attackCooldown);
      canAttack = true;
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
      if (!playerStats) return;

      playerStats.AddXp(experienceGiven);
      XPUIManager xpUI = FindFirstObjectByType<XPUIManager>();
      if (xpUI) {
        xpUI.ShowFloatingXpText(transform.position, experienceGiven);
      }
    }

    protected virtual void DisableEnemy() {
      if (rb) rb.linearVelocity = Vector2.zero;
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
      if (_attackCooldownCoroutine != null) StopCoroutine(_attackCooldownCoroutine);
      if (_attackDurationCoroutine != null) StopCoroutine(_attackDurationCoroutine);
    }

    public Transform GetTransform() => transform;
    public float GetHealth() => healthPoints;
    public float GetMaxHealth() => maxHealthPoints;
    public bool IsDying() => isDying;
  }
}
using Input;
using UnityEngine;

namespace Player {
  public class PlayerMovement : MonoBehaviour {
    [SerializeField]
    private float moveSpeed = 5f;
  
    private Vector2 _movement;
  
    private Rigidbody2D _rb;
    private Animator _animator;
    
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";
    private const string LastHorizontal = "LastHorizontal";
    private const string LastVertical = "LastVertical";
    
    private void Awake() {
      _rb = GetComponent<Rigidbody2D>();
      _animator = GetComponent<Animator>();
    }

    private void Update() {
      _movement.Set(InputManager.Movement.x, InputManager.Movement.y);
      
      _rb.linearVelocity = _movement * moveSpeed;
      
      _animator.SetFloat(Horizontal, _movement.x);
      _animator.SetFloat(Vertical, _movement.y);

      if (_movement != Vector2.zero) {
        _animator.SetFloat(LastHorizontal, _movement.x);
        _animator.SetFloat(LastVertical, _movement.y);
      }
      
    }
  }
}
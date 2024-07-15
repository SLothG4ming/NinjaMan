using UnityEngine;
using System.Collections;

public class FrogController : MonoBehaviour
{
    private Rigidbody2D _frog2d;
    private Animator _animator;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float _jumpingForceX = 5f;
    [SerializeField] private float _jumpingForceY = 5f;
    [SerializeField] private float _jumpDuration = 1f;
    [SerializeField] private float _idleTime = 2f;
    [SerializeField] private Character_Attack_Sounds _deathSounds;
    private bool _isFacingRight = true;
    private bool _isJumping;
    private int _currentLife;
    private bool _isDead;
    private Collider2D _collider;
    private void Awake()
    {
        _frog2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        StartCoroutine(FrogBehavior());
    }

    private IEnumerator FrogBehavior()
    {
        while (true)
        {
            // Randomly decide whether to flip or not
            if (Random.Range(0, 2) == 0)
            {
                Flip();
            }

            yield return new WaitForSeconds(_idleTime);

            // Set jump state and trigger animation
            _isJumping = true;
            _animator.SetBool("isJumping", true);

            // Apply jump force
            Jump();

            // Wait for jump duration
            yield return new WaitForSeconds(_jumpDuration);

            // Reset jump state and animation
            _isJumping = false;
            _animator.SetBool("isJumping", false);
        }
    }

    private void Jump()
    {
        // Determine jump direction based on facing direction
        float jumpDirection = _isFacingRight ? 1 : -1;
        _frog2d.velocity = new Vector2(_jumpingForceX * jumpDirection, _jumpingForceY);
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    public void TakeDamage(int damage)
    {
        if (_isDead)
        {
            return;
        }

        _currentLife -= damage;



        if (_currentLife <= 0)
        {
            Die();
            _deathSounds.PlayDeathSound(); // Play death sound
        }

    }

    private void Die()
    {
        _isDead = true;

        animator.SetBool("isDead", _isDead);


        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }


        // Stop the frog from moving
        _frog2d.velocity = Vector2.zero;

        Destroy(gameObject, 5f);
    }
}
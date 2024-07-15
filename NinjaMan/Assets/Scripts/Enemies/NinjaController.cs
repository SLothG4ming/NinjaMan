using UnityEngine;

public class NinjaController : MonoBehaviour
{
    private int _maxLife = 100;
    private int _currentLife;
    private float _movementSpeed = 2f;
    private bool _isDead = false;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D enemy2d;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Character_Attack_Sounds _attackSounds;

    private PlayerController _player;
    private bool _isFacingRight = true;
    private bool _isWaiting = false;
    private float _waitTime = 3f;
    private float _waitTimer = 0f;
    private int _attackDamage = 25;
    private float _groundCheckRadius = 0.2f;
    [SerializeField] private float knockbackForce;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>(); // Assign the _player reference in the Awake method
        _currentLife = _maxLife;
        enemy2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
       
    }

    private void FixedUpdate()
    {
        animator.SetFloat("isRunning", Mathf.Abs(enemy2d.velocity.x));
        if (!_isDead)
        {
            Debug.DrawLine(GroundCheck.position, GroundCheck.position + Vector3.down * _groundCheckRadius * 2, Color.red);
            Debug.DrawLine(transform.position, transform.position + Vector3.right * (_isFacingRight ? 1f : -1f) * _movementSpeed, Color.blue);

            Patrol();
        }
    }

    private bool IsGrounded()
    {
        Vector2 raycastOrigin = new Vector2(GroundCheck.position.x, GroundCheck.position.y);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, 0.1f, whatIsGround);
        Debug.DrawRay(raycastOrigin, Vector2.down * 0.1f, Color.green);
        return hit.collider != null;
    }

    private void Patrol()
    {
      
        if (_isWaiting)
        {
            _waitTimer += Time.fixedDeltaTime;
            if (_waitTimer >= _waitTime)
            {
                _isWaiting = false;
                _waitTimer = 0f;
                Flip();
             
            }
        }
        else
        {
            if (!IsGrounded())
            {
                _isWaiting = true;
                enemy2d.velocity = Vector2.zero;
              
            }
            else
            {
                float moveDirection = _isFacingRight ? 1f : -1f;
                enemy2d.velocity = new Vector2(moveDirection * _movementSpeed, enemy2d.velocity.y);
              
            }
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {           
            _player.TakeDamage(_attackDamage); // Hurt the player
        }
    }
    public bool IsPlayerInFront(Vector3 playerPosition)
    {
        float playerDirection = playerPosition.x - transform.position.x;
        return (_isFacingRight && playerDirection > 0) || (!_isFacingRight && playerDirection < 0);
    }


    public void StartAttack()
    {
       animator.SetTrigger("isAttacking");
    }
    public void TakeDamage(int damage)
    {
        if (_isDead)
        {
            return;
        }

        _currentLife -= damage;
       
        animator.SetTrigger("isHurt");

        if (_currentLife <= 0)
        {
            Die();
            _attackSounds.PlayDeathSound(); // Play death sound
        }
        else
        {
           _attackSounds.PlayHurtSound(); // Play hurt sound
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
        enemy2d.velocity = Vector2.zero;
        Destroy(gameObject, 5f);
    }
}

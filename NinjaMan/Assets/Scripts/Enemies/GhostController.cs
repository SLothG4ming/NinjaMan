using UnityEngine;
using System.Collections;
public class GhostController : MonoBehaviour
{
    [SerializeField] private float _hoverAmplitude = 1f;
    [SerializeField] private float _hoverFrequency = 1f;
    [SerializeField] private float _chaseRange = 20f;
    [SerializeField] private float _returnSpeed = 2f;
    [SerializeField] private int _maxLife = 50;
    [SerializeField] private int _attackDamage = 10;
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private Character_Attack_Sounds _attackSounds;

    private PlayerController _player;
    private Rigidbody2D _ghost2D;
    private Vector2 _originalPosition;
    private bool _isFacingRight = true;
    private bool _isDead = false;
    private int _currentLife;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();
        _ghost2D = GetComponent<Rigidbody2D>();
        _currentLife = _maxLife;
    }

    private void Start()
    {
        _originalPosition = transform.position;
        StartCoroutine(HoverCoroutine());
    }

    private IEnumerator HoverCoroutine()
    {
        while (!_isDead)
        {
            float hoverY = _originalPosition.y + _hoverAmplitude * Mathf.Sin(_hoverFrequency * Time.time);
            _ghost2D.velocity = new Vector2(_ghost2D.velocity.x, hoverY - transform.position.y);
            yield return null;
        }
    }

    private void Update()
    {
        if (!_isDead)
            ChasePlayer();
    }

    private void ChasePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(_playerTag);
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= _chaseRange)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            _ghost2D.velocity = new Vector2(direction.x * _returnSpeed, _ghost2D.velocity.y);

            if (player.transform.position.x > transform.position.x && !_isFacingRight)
                Flip();
            else if (player.transform.position.x < transform.position.x && _isFacingRight)
                Flip();
        }
        else
        {
            Vector2 returnDirection = (_originalPosition - (Vector2)transform.position).normalized;
            _ghost2D.velocity = new Vector2(returnDirection.x * _returnSpeed, _ghost2D.velocity.y);
        }
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
        if (_isDead) return;

        _currentLife -= damage;
    

        if (_currentLife <= 0)
        {
            Die();
            _attackSounds.PlayDeathSound();
        }
        else
        {
            _attackSounds.PlayHurtSound();
        }
    }

    private void Die()
    {
        _isDead = true;
        
        _ghost2D.velocity = Vector2.zero;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _player.TakeDamage(_attackDamage);
        }
    }
}

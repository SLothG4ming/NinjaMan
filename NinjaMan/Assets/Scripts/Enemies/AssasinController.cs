using System.Collections;
using UnityEngine;

public class AssasinController : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _player;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private float checkInterval = 3f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private float jumpDamageRadius = 1.5f;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private int attackDamage = 20;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = FindObjectOfType<PlayerController>();
        StartCoroutine(CheckForPlayerRoutine());
    }

    private IEnumerator CheckForPlayerRoutine()
    {
        while (true)
        {
            CheckPlayerVisibility();

            // Wait for the specified interval before checking again
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void CheckPlayerVisibility()
    {
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            AttackPlayer();
        }
    }

    private bool CanSeePlayer()
    {
        // Check bottom left and bottom right for the player
        Collider2D playerColliderLeft = Physics2D.OverlapCircle(groundCheck.position - Vector3.right, attackDistance, whatIsPlayer);
        Collider2D playerColliderRight = Physics2D.OverlapCircle(groundCheck.position + Vector3.right, attackDistance, whatIsPlayer);

        return playerColliderLeft != null || playerColliderRight != null;
    }

    private void AttackPlayer()
    {
        if (isAttacking)
        {
            return;
        }

        // Stop moving
        _animator.SetBool("isJumping", true);

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Wait for jump cooldown
        yield return new WaitForSeconds(jumpCooldown);

        // Check if player is within the damage radius
        float distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);
        if (distanceToPlayer <= jumpDamageRadius)
        {
            // Apply damage to the player
            _player.TakeDamage((int)attackDamage);
        }

        // Resume normal behavior
        isAttacking = false;
        _animator.SetBool("isJumping", false);
    }
}

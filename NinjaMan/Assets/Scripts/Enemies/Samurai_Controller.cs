using UnityEngine;

public class Samurai_Controller : MonoBehaviour
{
    public MagicController magicController;
    public Animator animator;

    private bool isFacingRight = true;
    private bool isDead = false;
    private int _currentLife = 100;
    private bool _isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isDead)
        {
            FlipAtIntervals();
        }
    }

    private void FlipAtIntervals()
    {
        if (Random.Range(0, 100) < 1)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void Attack()
    {
        if (magicController.CanThrowMagic())
        {
            magicController.ThrowMagic(isFacingRight);
        }
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
           
        }

    }
    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        animator.SetBool("isDead", isDead);
        Destroy(gameObject, 5f);
    }
}

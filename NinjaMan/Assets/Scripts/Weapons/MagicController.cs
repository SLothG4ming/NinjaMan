using System.Collections;
using UnityEngine;

public class MagicController : MonoBehaviour
{
    [SerializeField] private float throwRange = 15f;
    [SerializeField] private GameObject magicProjectilePrefab;
    public float magicSpeed = 32f;

    public bool canThrowMagic = true;

    private void Start()
    {
        StartCoroutine(ThrowMagicCoroutine());
    }

    private IEnumerator ThrowMagicCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 3f));
            if (canThrowMagic)
            {
                ThrowMagic(false);
            }

            yield return new WaitForSeconds(3f);
        }
    }

    public bool CanThrowMagic()
    {
        return canThrowMagic;
    }

    public void ThrowMagic(bool isFacingRight)
    {
        GameObject magicProjectile = Instantiate(magicProjectilePrefab, transform.position, Quaternion.identity);

        float projectileSpeed = magicSpeed;
        if (!isFacingRight)
        {
            projectileSpeed = -projectileSpeed;
            magicProjectile.GetComponent<SpriteRenderer>().flipX = true;
        }

        Rigidbody2D rb = magicProjectile.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(projectileSpeed, rb.velocity.y);

        canThrowMagic = false;
        StartCoroutine(ResetMagicThrowCoroutine());
    }

    public float GetThrowRange()
    {
        return throwRange;
    }

    private IEnumerator ResetMagicThrowCoroutine()
    {
        yield return new WaitForSeconds(3f);
        canThrowMagic = true;
    }
}

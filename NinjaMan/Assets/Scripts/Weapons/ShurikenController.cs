using UnityEngine;

public class ShurikenController : MonoBehaviour
{
    [SerializeField] public float throwspeed = 16f;
  

    public PlayerController player;
    [SerializeField] private int shurikenDamage = 25;
    [SerializeField] private float maxThrowRange = 25f;

    // Start is called before the first frame update
    void Start()
    {
        // We find our enemy
        player = FindObjectOfType<PlayerController>();
        if (player.transform.localScale.x < 0)
        {
            throwspeed = -throwspeed;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        Destroy(gameObject, 5);
    }
    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(throwspeed, GetComponent<Rigidbody2D>().velocity.y);

        // Check if shuriken has reached its maximum throw range
        if (Vector2.Distance(transform.position, player.transform.position) >= maxThrowRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Check if the other object has a NinjaController script
            NinjaController ninjaEnemy = other.GetComponent<NinjaController>();
            if (ninjaEnemy != null)
            {
                ninjaEnemy.TakeDamage(shurikenDamage);
            }

            // Check if the other object has a Samurai_Controller script
            Samurai_Controller samuraiEnemy = other.GetComponent<Samurai_Controller>();
            if (samuraiEnemy != null)
            {
                samuraiEnemy.TakeDamage(shurikenDamage);
            }

            // Check if the other object has a FrogController script
            FrogController frogEnemy = other.GetComponent<FrogController>();
            if (frogEnemy != null)
            {
                frogEnemy.TakeDamage(shurikenDamage);
            }

            // Check if the other object has a GhostController script
            GhostController ghostEnemy = other.GetComponent<GhostController>();
            if (ghostEnemy != null)
            {
                ghostEnemy.TakeDamage(shurikenDamage);
            }

            // Destroy the shuriken regardless of enemy type
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("IgnoreCollision"))
        {
            Destroy(gameObject);
        }
    }
}


using UnityEngine;

public class ShurikanController : MonoBehaviour
{
    [SerializeField] public float throwspeed = 16f;
    public bool thrown;

    public PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        //We find Our Ninja
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
    }

}


using UnityEngine;

public class OnCollisionEnter : MonoBehaviour
{
    public bool OnWall { get; private set; }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnWall = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        OnWall = false;
    }
}

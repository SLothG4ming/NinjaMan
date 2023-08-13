
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    private int _maxLife = 100;
    private int _currentLife;
    private float movementSpeed = 0.2f;


    // Start is called before the first frame update
    void Awake()
    {
        _currentLife = _maxLife;

    }

    private void FixedUpdate()
    {
        transform.position = transform.position + new Vector3( movementSpeed * Time.deltaTime,0f);
    }


    public void TakeDamange(int damage)
    {
        _currentLife -= damage;
        Debug.Log("Enemy lost Hp!");
        //set Enemys Hurt Anim
        if (_currentLife <= 0)
        {
            _currentLife = 0;
            Die();
           
        }
    }

    public void Die()
    {
        Debug.Log("Enemy Died!");
        Destroy(gameObject);
        //set Enemys Death Anim

    }

}

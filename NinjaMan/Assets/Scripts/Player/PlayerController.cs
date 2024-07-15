using UnityEngine.InputSystem;
using UnityEngine;

using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //UI
    /*.....................................................*/
    [Header("UI Related")]
    [SerializeField] private ShurikenCounter shurikenCounter;
    [SerializeField] private HealthBar healthBar;



    [SerializeField] private Character_Attack_Sounds _attackSounds;

   
    
    //Rigidbody and Animator 
    /*.....................................................*/
    [SerializeField] private Rigidbody2D player2d;
    [SerializeField] private Animator animator;

    //Transforms
    /*.....................................................*/
    [SerializeField] private Transform GroundCheck;

    [SerializeField] private Transform Muzzle;

    private OnCollisionEnter WallCheck;

    //Game Objects 
    /*.....................................................*/
    public GameObject Projectile;

    //Layers 
    /*....................... ..............................*/

    [SerializeField] private Transform groundRaycastPoint;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundRaycastDistance = 0.1f;
    
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsEnemy;
    //Movement 
    /*.....................................................*/
    private float _horizontal;

    [SerializeField,Min(0)] private float _movementspeed = 8f;
   
    [SerializeField] private float _moveSpeedMultiplier = 1f;
    //Jumping
    /*.....................................................*/
    private int _numberOfJumps = 0;
    [SerializeField,Range(0,5)] private int _maxjumps = 2;
    [SerializeField] private float _jumppower = 16f;
    private float _coyoteTimeCounter;
    [SerializeField] private float _coyoteTime = 0.2f;
    // [SerializeField] private float _aircontrol = 3f;
    [SerializeField] private Vector2 _wallJumpingPower = new(8f, 16f);
    //Wall sliding
    /*.....................................................*/
    //  [SerializeField] private float _wallSlidingSpeed = 5f;

    private float _wallJumpingDirection;
    //Grabbing
    /*.....................................................*/
    private const float _grabExhaustTime = 1.1f;
    private float _timeSinceLastGrab = 0f;

    //Attacking
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int _attackDamage = 40;
    private int _kills = 0;

    //Shurikan
    /*.....................................................*/
    [SerializeField] private int _curKnifeStock = 7 ;
    [SerializeField] private int _maxKnifeStock = 25;
    private int shurikenCount = 0;


   

    //Health
    [SerializeField] private int _maxlife = 100;
    [SerializeField] private int _currentLife;
    [SerializeField] private float _airspeed = 4f;
    public int CurrentLife { get; set; }
    public int MaxLife { get; set; }
    //Booleans
    /*.....................................................*/
    private bool _isGrounded = true;
    private bool _wasGrounded = false;
    private bool _isFacingRight = true;
    private bool _isJumping = false;
    private bool _isGrabbing = false;
    private bool _jumpPrepare = false;
    private bool _isWallSliding;
    private bool _isFalling;
    private bool _isDead = false;
    private bool _lockHorizontalMovement = false;
    Collider2D[] hitEnemies;
    [SerializeField] private float knockbackForce = 3f;
    private bool canMove = true;
    private float deathAnimationDuration = 2;


    // Start is called before the first frame update

    private void Start()
    {
        if (shurikenCounter == null)
        {
            Debug.LogError("ShurikenCounter is not assigned in the Inspector. Assign it to update the UI.");
        }

        if (healthBar == null)
        {
            Debug.LogError("HealthBar is not assigned in the Inspector. Assign it to update the UI.");
        }
        else
        {
            // Initialize the UI with the current shuriken count and current health
            shurikenCounter.SetShurikenCount(_curKnifeStock);
            healthBar.SetHealth(_currentLife, _maxlife);
        }

        _curKnifeStock = 7;
        shurikenCount = _curKnifeStock;
        shurikenCounter.SetShurikenCount(_curKnifeStock);
    }
    void Awake()
    {
        animator = GetComponent<Animator>();
        player2d = GetComponent<Rigidbody2D>();
        WallCheck = GetComponentInChildren<OnCollisionEnter>();
        _currentLife = _maxlife;
    }

    //Update is called once per frame
    void Update()
    {
        WallSlide();
        Flip();
       
        //Animation
        animator.SetFloat("isRunning", Mathf.Abs(player2d.velocity.x) / _movementspeed);
        animator.SetBool("isJumping", _isJumping);
        animator.SetBool("isFalling", IsFalling());
        animator.SetBool("isWallSliding", _isWallSliding);
        animator.SetBool("isGrabbing", _isGrabbing);
        animator.SetBool("isDead", _isDead);
        
        animator.SetBool("jumpPrepare", _jumpPrepare);
        
    }

    public void PlayHurtSound()
    {
        if (_attackSounds != null)
        {
            _attackSounds.PlayHurtSound();
        }
    }
    private void FixedUpdate()
    {
        

        WallGrab();

        //Denies the player to move if OnWall 

        if (!_lockHorizontalMovement && !_isDead && canMove)
        {
            player2d.velocity = new Vector2(_horizontal * _movementspeed * _moveSpeedMultiplier, player2d.velocity.y);
        }


        //Set conditions for landing
        _wasGrounded = _isGrounded;
        _isGrounded = IsGroundedRaycast();
        Debug.Log("IsGrounded: " + _isGrounded);

        if (!_wasGrounded && _isGrounded)
        {
            Landed();
        }

        
        Debug.Log(_currentLife);
        ////////////////////////////////////////////////////////////////////////////////////////
        //if (WallCheck.OnWall && _isGrabbing || _isWallSliding && //Input weg von der Wand*) 
        ////////////////////////////////////////////////////////////////////////////////////////
   
    }
   
    //Returns horizontal as context Menu & reads its X Value
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x; ;
    }
    public bool IsFacingRight()
    {
        return _isFacingRight;
    }
    //Flips the character sprite according to its facing direction
    public void Flip()
    {
        if (!_lockHorizontalMovement && (_isFacingRight && _horizontal < 0f || !_isFacingRight && _horizontal > 0f))
        {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    //Lowers movementspeed by _movementSpeedMultiplier        
    public void Stealth(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveSpeedMultiplier = 0.3f;
        }
        if (context.canceled)
        {
            _moveSpeedMultiplier = 1f;
        }
    }

    //Set boolean for isGrounded
    private bool IsGroundedRaycast()
    {
        // Cast a ray downward from the groundRaycastPoint
        RaycastHit2D hit = Physics2D.Raycast(groundRaycastPoint.position, Vector2.down, groundRaycastDistance, whatIsGround);
        if (hit.collider != null)
        {
            Debug.Log("Ground Hit: " + hit.collider.name);
        }
       
        // If the ray hits something on the ground layer, return true (player is grounded)
        return hit.collider != null;
    }


    //Resets the number of jumps done
    private void Landed()
    {
        _numberOfJumps = 0;
        _lockHorizontalMovement = false;

    }

    private bool IsFalling()
    {

        if (!_isGrounded && player2d.velocity.y < 0f)

        {
            _isFalling = true;
        }
        else
        {
            _isFalling = false;
        }
        return _isFalling;
    }

    // walljumping fixen  door fixen 
    //Method for jumping

    //What if Space is held pressed? 
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_numberOfJumps < _maxjumps || CoyoteTime() > 0f || _isGrounded)
            {
                player2d.velocity = new Vector2(player2d.velocity.x, _jumppower);
                _isJumping = true;
                _lockHorizontalMovement = false;
                _numberOfJumps++;
            }
            if (_isWallSliding)
            {
                _wallJumpingDirection = -transform.localScale.x;

                // Apply wall jump force
                player2d.velocity = new Vector2(_wallJumpingPower.x * _wallJumpingDirection, _wallJumpingPower.y);

                // Exit wall sliding state
                _isWallSliding = false;
            }
        }
        if (context.canceled && !_isGrounded && player2d.velocity.y > 0f)
        {
            _coyoteTimeCounter = 0f;

            _isJumping = false;
            player2d.velocity = new Vector2(player2d.velocity.x * _airspeed, player2d.velocity.y * 0.1f);
        }
    
    }


    //Checks if player may jump shortly after leaving ground 
    private float CoyoteTime()
    {
        if (_isGrounded)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
        return _coyoteTimeCounter;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            StartCoroutine(BlockInputCountdown());
         
                Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;

                // Apply knockback force to the player's Rigidbody2D
               // player2d.velocity = Vector2.zero; // Reset player's velocity
                player2d.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            
                // Deal damage to the enemy
                collision.gameObject.GetComponent<NinjaController>().TakeDamage(_attackDamage);
           
        }
       
        

    }
    IEnumerator BlockInputCountdown() 
    {
        canMove = false;
        yield  return new WaitForSeconds(0.5f);
        canMove = true;

    }

    //Handels the WallGrab 
    private void WallGrab()
    {
        if (WallCheck.OnWall && !_isGrounded)
        {
            
            if (_timeSinceLastGrab == 0)
            {
                _isJumping = false;
                _isGrabbing = true;
                _lockHorizontalMovement = true;
                _numberOfJumps = 0;
                player2d.velocity = new Vector2(0f, 0f);
                player2d.gravityScale = 0f;
                Debug.Log("casted");
            }

            _timeSinceLastGrab += Time.deltaTime;

            if (_timeSinceLastGrab >= _grabExhaustTime)
            {
                _isGrabbing = false;
                player2d.gravityScale = 1.5f;

            }
        }

        else
        {
            _isGrabbing = false;
            player2d.gravityScale = 1.5f;
        }

        if (_isGrounded || !WallCheck.OnWall)
        {
            _timeSinceLastGrab = 0f;
        }
    }

    //WallSlide
    /// <summary>
    /// Sets _isWallSliding condition and clamps the players y velocity 
    /// </summary>
    private void WallSlide()
    {
        if (WallCheck.OnWall && !_isGrounded && !_isGrabbing && player2d.velocity.y > 0)
        {
            _isWallSliding = true;
            _lockHorizontalMovement = true;
        }
        else
        {
            _isWallSliding = false;
        }
    }

    //Atack
    /// <summary>
    /// Controls attack animation & checks for hitted enemies and collects them in a array to give out dmg to each of them. 
    /// </summary>
    /// <param name="context"></param>
    private void AttackEnemies(Vector2 position)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(position, attackRange, whatIsEnemy);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Check if the other object has a NinjaController script
            NinjaController ninjaEnemy = enemy.GetComponent<NinjaController>();
            if (ninjaEnemy != null)
            {
                ninjaEnemy.TakeDamage(_attackDamage);
                IncrementKills(); // Increment the kill count
            }

            // Check if the other object has a Samurai_Controller script
            Samurai_Controller samuraiEnemy = enemy.GetComponent<Samurai_Controller>();
            if (samuraiEnemy != null)
            {
                samuraiEnemy.TakeDamage(_attackDamage);
                IncrementKills(); // Increment the kill count
            }

            // Check if the other object has a FrogController script
            FrogController frogEnemy = enemy.GetComponent<FrogController>();
            if (frogEnemy != null)
            {
                frogEnemy.TakeDamage(_attackDamage);
                IncrementKills(); // Increment the kill count
            }

            // Check if the other object has a GhostController script
            GhostController ghostEnemy = enemy.GetComponent<GhostController>();
            if (ghostEnemy != null)
            {
                ghostEnemy.TakeDamage(_attackDamage);
                IncrementKills(); // Increment the kill count
            }

            Debug.Log("We hit " + enemy.name);
        }
    }
    public void Slash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("isAttacking");

            // Check if Muzzle is not null before using it
            if (Muzzle != null)
            {
                AttackEnemies(Muzzle.position);
            }
        }
    }
    private void IncrementKills()
    {
        _kills++;
    }
   
    //Rangechecks
    /// <summary>
    /// Collection of Rangechecks: Muzzle
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (Muzzle == null)
            return;
        Gizmos.DrawWireSphere(Muzzle.position, attackRange);

    }

    //Throw 
    /// <summary>
    /// Controls Animation of isThrowing and spawns a new projectile from Muzzle.
    /// </summary>
    /// <param name="context"></param>
    public void Shurikan(InputAction.CallbackContext context)
    {
       
        if (context.performed && _curKnifeStock > 0)
        {
            animator.SetTrigger("isThrowing");
            GameObject mProjectile = Instantiate(Projectile, Muzzle.position, Muzzle.rotation);
            mProjectile.transform.parent = GameObject.Find("GameManager").transform;
            mProjectile.GetComponent<Renderer>().sortingLayerName = "Player";
           
            _curKnifeStock--;
            shurikenCount = _curKnifeStock;
            shurikenCounter.SetShurikenCount(_curKnifeStock);
            
            

            AttackEnemies(Muzzle.position);
        }
    }

    public void AddShurikans(int count)
    {
        _curKnifeStock += count;
        _curKnifeStock = Mathf.Clamp(_curKnifeStock, 0, _maxKnifeStock);
        shurikenCount = _curKnifeStock;

        // Update the UI when adding shurikens
        UpdateShurikenUI();
    }

    public void UpdateShurikenUI()
    {
        // Ensure you have a reference to the shurikenCounter component in your Unity Inspector
        if (shurikenCounter != null)
        {
            shurikenCounter.SetShurikenCount(_curKnifeStock);
        }
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

        // Update the health bar
        healthBar.SetHealth(_currentLife, _maxlife);
    }

    void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        
        animator.SetTrigger("isDead");

        player2d.velocity = Vector2.zero; // Stop the player from moving

        // Add any other necessary logic here, like triggering death animations or effects

        StartCoroutine(LoadGameOverSceneAfterAnimation()); // Load Game Over scene after death animation
    }

    IEnumerator LoadGameOverSceneAfterAnimation()
    {
        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimationDuration); // Replace with the actual duration of the death animation

        // Load the Game Over scene
        SceneManager.LoadScene("GameOver"); // Make sure the scene name is correct
    }

    public int Kills => _kills;
}

 
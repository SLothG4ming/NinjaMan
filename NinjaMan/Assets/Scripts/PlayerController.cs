
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    //Input Actions
    /*.....................................................*/

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
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsEnemy;
    //Movement 
    /*.....................................................*/
    private float _horizontal;

    [SerializeField] private float _movementspeed = 8f;
    [SerializeField] private float _groundCheckRadius = 0.02f;
    [SerializeField] private float _moveSpeedMultiplier = 1f;
    //Jumping
    /*.....................................................*/
    private int _numberOfJumps = 0;
    [SerializeField] private int _maxjumps = 2;
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


    //Shurikan
    /*.....................................................*/
    [SerializeField] private int _maxKnifeStock = 25;
    [SerializeField] private int _curKnifeStock = 8;
    private float _timeSinceLastThrow = 0f;
    private float throwRange = 2f;

    //Health
    [SerializeField] private int _maxlife = 100;
    [SerializeField] private int _currentLife;
    [SerializeField] private int _dmgamout = 0;
    [SerializeField] private float _airspeed = 4f;

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
   

    // Start is called before the first frame update
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
        animator.SetBool("isDead", _isDead);
        animator.SetBool("jumpPrepare", _jumpPrepare);

    }
   

    private void FixedUpdate()
    {
        WallGrab();

        //Denies the player to move if OnWall 
       
        if (!_lockHorizontalMovement && !_isDead)
        {
            player2d.velocity = new Vector2(_horizontal * _movementspeed * _moveSpeedMultiplier, player2d.velocity.y);
        }
        //Set conditions for landing
        _wasGrounded = _isGrounded;
        _isGrounded = IsGrounded();

        if (!_wasGrounded && _isGrounded)
        {
            Landed();
        }
      
       
        ////////////////////////////////////////////////////////////////////////////////////////
        //if (WallCheck.OnWall && _isGrabbing || _isWallSliding && //Input weg von der Wand*) 
        ////////////////////////////////////////////////////////////////////////////////////////
        //  if (GetDamgeByEnemy Or Walk into the Hitbox of the enemie) 
        //  {
        //      TakeDamge(_dmgamout);
        //  }
        ////////////////////////////////////////////////////////////////////////////////////////
    }

    //Returns horizontal as context Menu & reads its X Value
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x; ;
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
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, _groundCheckRadius, whatIsGround);
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
        return  _isFalling;
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
            }

            if (!_isGrounded && WallCheck.OnWall)
            {
                _lockHorizontalMovement = false;
                _numberOfJumps =0 ;
                Debug.Log("isWalljumping");
                player2d.velocity = new Vector2( _wallJumpingPower.x, _wallJumpingPower.y ) * _wallJumpingDirection;
                
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
    public void Slash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("isAttacking");

            hitEnemies = Physics2D.OverlapCircleAll(Muzzle.position, attackRange, whatIsEnemy);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyController>().TakeDamange(_attackDamage);
                Debug.Log("We hit" + enemy.name);
            }
        }

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
        if (context.performed && _curKnifeStock <= _maxKnifeStock )
        {
            animator.SetTrigger("isThrowing");
            GameObject mProjectile = Instantiate(Projectile, Muzzle.position, Muzzle.rotation);
            mProjectile.transform.parent = GameObject.Find("GameManager").transform;
            mProjectile.GetComponent<Renderer>().sortingLayerName = "Player";
           // _timeSinceLastThrow += Time.deltaTime;
            _curKnifeStock--;

            ////////////////////////////////////////////////////////////////////////////////////////
            //NEED A DIFFERENT OVERLAP HERE 
            hitEnemies = Physics2D.OverlapCircleAll(Muzzle.position, throwRange, whatIsEnemy);
            ///////////////////////////////////////////////////////////////////////////////////////
            ///
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyController>().TakeDamange(_attackDamage);
                Debug.Log("We hit" + enemy.name);
            }
        }
      

    }

    // Bool for deathstatus
    /// <summary>
    /// Returns Death Value
    /// </summary>
    /// <returns></returns>
    private bool Dead()
    {
        return _isDead = true;
    }

    //Damage 
    /// <summary>
    /// Reduces Life until Dead and sets Animation Trigger "isHurt". 
    /// </summary>
    /// <param name="damage"></param>
    private void TakeDamge(int damage)
    {
        _currentLife -= damage;
        animator.SetTrigger("isHurt");

        if (_currentLife <= 0)
        {
            Dead();
        }
    }


}




/*  Unity CheatSheet  */
//
//
//
//
/*MonoBehaviour.Start() */
/*----------------------------*/
//
//Start is called on the frame when a script is enabled just beforeany of the Update methods are called the first time.
//Start is called exactly once in the lifetime of the script.
/*........................................................................................................................................*/
//
//
//
//
//MonoBehaviour.Awake()
/*----------------------------*
//Awake is called when an enabled script instance is being loaded.
//Awake is called either when an active GameObject that contains the script is initialized when a Scene loads, or when a previously inactive
//GameObject is set to active, or after a GameObject created with Object.Instantiate is initialized.
//Use Awake to initialize variables or states before the application starts.
//Awake is called even if the script is a disabled component of an active GameObject.
//Awake can not act as a coroutine.
//Use Awake instead of the constructor for initialization, as the serialized state of the component is undefined at construction time. Awake is called once, just like the constructor.
/*........................................................................................................................................*/
//
//
//
//
//MonoBehaviour.OnEnable()
/*----------------------------*\
//This function is called when the object becomes enabled and active.
//Because this function is executed when the object is enabled, it will be executed whenever entering the Play Mode (with the object enabled).
/*........................................................................................................................................*/
//
//
//
//
//MonoBehaviour.OnDisable()
/*----------------------------*
//This function is called when the behaviour becomes disabled.
//
/7This is also called when the object is destroyed and can be used for any cleanup code.
// When scripts are reloaded after compilation has finished, OnDisable will be called, followed by an OnEnable after the script has been loaded.
/*........................................................................................................................................*/
//
//
//
//
/*MonoBehaviour.LateUpdate() */
/*----------------------------*/
//
//LateUpdate is called every frame, if the Behaviour is enabled.
//LateUpdate is called after all Update functions have been called. This is useful to order script execution.
//For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.
/*........................................................................................................................................*/
//
//
//
//
///*MonoBehaviour.Update() */
//Update is called every frame, if the MonoBehaviour is enabled.
//
//Update is the most commonly used function to implement any kind of game script. Not every MonoBehaviour script needs Update.
/*........................................................................................................................................*/
//
//
//
//
/*MonoBehaviour.FixedUpdate() */
/*----------------------------*/
//
//Compute Physics system calculations after FixedUpdate. 0.02 seconds (50 calls per second) is the default time between calls. Use Time.fixedDeltaTime to access this value.
//Alter it by setting it to your preferred value within a script, or, navigate to Edit > Settings > Time > Fixed Timestep and set it there.
//Use FixedUpdate when using Rigidbody. Set a force to a Rigidbody and it applies each fixed frame.
//FixedUpdate occurs at a measured time step that typically does not coincide with MonoBehaviour.Update.
/*........................................................................................................................................*/
//
//
//
//



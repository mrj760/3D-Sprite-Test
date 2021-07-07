using System.Collections;
using UnityEngine;



public class Player : MonoBehaviour 
{
    private float hinp, vinp;
    private Vector3 movDir;
    private Transform tx;
    private CharacterController cc;
    
    [SerializeField] private Transform ctx;
    [SerializeField] private float moveSpeed=20f, jumpSpeed=5f, jumpTime = .33f;
    
    [Header("Gravity")]
    [SerializeField] private 
        float gravAccel = 9.81f;
    [SerializeField, 
     Range(-100,0)] private 
        float gravMax=-30f;
    [SerializeField] private 
        bool gravEnabled = true;
    
    [Header("Weapon")]
    [SerializeField] private Weapon weapon;
    private Animator weaponAnimator;

    private static readonly 
        int moveBool = Animator.StringToHash("Move");
    private static readonly 
        int attackTrigger = Animator.StringToHash("Attack");

    public enum MoveState
    {
        Idle, Moving
    }

    private MoveState moveState;
    
    public enum AirState
    {
        Ground, Jumping, Falling
    }
    private AirState airState;

    void Start()
    {
        tx = transform;
        cc = GetComponent<CharacterController>();
        weaponAnimator = 
            weapon.GetComponent<Animator>();
        
        Cursor.lockState = 
            CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GetInput();
        Move();
        Animate();
        // Stabilize();
    }

    private void LateUpdate()
    {
        CheckGrav();
    }

    private void GetInput()
    {
        // Movement
        vinp = 0f; hinp = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            vinp += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vinp -= 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            hinp -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            hinp += 1f;
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Attacking
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    private void Move()
    {
        var dir = 
            tx.forward * vinp + tx.right * hinp;
        dir.Normalize();

        movDir.y = 
            airState == AirState.Ground ? 
                0 : movDir.y + FallAccel();
        dir.y = movDir.y;
        
        movDir = dir;
        cc.Move(movDir * (moveSpeed * Time.deltaTime));
    }

    // Disables gravity and sets a positive y-speed for a brief moment
    private void Jump()
    {
        if (!cc.isGrounded)
        {
            Debug.Log("No");
            return;
        }
        
        movDir.y = jumpSpeed;
        gravEnabled = false;
        StartCoroutine(nameof(EndJump));
    }

    private IEnumerator EndJump()
    {
        yield return new WaitForSeconds(jumpTime);
        gravEnabled = true;
        movDir.y = 0;
    }

    private float FallAccel()
    {
        if (!gravEnabled || movDir.y <= gravMax) return 0;

        return -gravAccel * Time.deltaTime;
    }

    private void CheckGrav()
    {
        if (cc.isGrounded)
        {
            airState = AirState.Ground;
        }
        else
        {
            airState = 
                movDir.y > 0 ? 
                    AirState.Jumping : AirState.Falling;
        }
    }

    private void Animate()
    {
        bool hasMovementInput = 
            (Mathf.Abs(hinp) > 0f || Mathf.Abs(vinp) > 0f);
        
        weaponAnimator.SetBool(moveBool, hasMovementInput);
    }

    private void Attack()
    {
        if (weapon.Attack(ctx))
        {
            weaponAnimator.SetTrigger(attackTrigger);
        }
    }

    private void StabilizeXZ()
    {
        Vector3 ea = tx.eulerAngles;
        tx.eulerAngles = new Vector3(0f, tx.eulerAngles.y, 0f);
    }
}

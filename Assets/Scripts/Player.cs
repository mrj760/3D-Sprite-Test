using System.Collections;
using UnityEngine;



public class Player : MonoBehaviour 
{
    private float hinp, vinp, gravVel;
    private Vector3 movDir;
    private Transform tx;
    private CharacterController cc;
    
    [SerializeField] private Transform ctx;
    [SerializeField] private float moveSpeed=20f, jumpVelocity=5f;
    
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
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }

        // Attacking
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    private void Move()
    {
        movDir = 
            tx.forward * vinp + tx.right * hinp;
        movDir.Normalize();
        movDir.y += Fall();
        cc.Move(movDir * (moveSpeed * Time.deltaTime));
    }

    private void Jump()
    {
        cc.Move(Vector3.up * (jumpVelocity * Time.deltaTime));
    }

    private float Fall()
    {
        if (gravEnabled)
        {
            gravVel -= 
                gravAccel * Time.deltaTime;
            gravVel = 
                Mathf.Max(gravVel, gravMax);
            return gravVel;
        }

        return movDir.y;
    }

    private void CheckGrav()
    {
        if (cc.isGrounded)
        {
            gravVel = 0;
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

    private bool canAttack = true;
    private void Attack()
    {
        if (!canAttack) return;
        
        weapon.Attack(ctx);
        weaponAnimator.SetTrigger(attackTrigger);
        
        StartCoroutine(nameof(AttackTime));
    }
    private IEnumerator AttackTime()
    {
        canAttack = false;
        yield return new WaitForSeconds(weapon.attackTime);
        canAttack = true;
    }

    private void StabilizeXZ()
    {
        Vector3 ea = tx.eulerAngles;
        tx.eulerAngles = new Vector3(0f, tx.eulerAngles.y, 0f);
    }
}

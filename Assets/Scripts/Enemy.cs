using System;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    protected GameObject playerObj;
    protected Rigidbody rb;
    [SerializeField] protected float moveSpeed=5f;
    [SerializeField] protected float stopDistance=.3f;
    [SerializeField] protected float sightDistance = 10f;
    [SerializeField] protected float health;
    
    [SerializeField] protected Sprite[] sprites;
    protected int curSprite = 0;
    protected WorldSprite worldSprite;
    protected SpriteRenderer spriteRen;

    protected float reverseTime = 1f;
    protected float reverseTimer;
    
    
    protected void Start()
    {
        playerObj = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        
        worldSprite = GetComponentInChildren<WorldSprite>();
        spriteRen = GetComponentInChildren<SpriteRenderer>();
        
        spriteRen.sprite = sprites[curSprite];

        reverseTimer = reverseTime;
    }

    void FixedUpdate()
    {
        SeekTarget(playerObj.transform.position);
    }

    [SerializeField, Range(0f,1f)] private float 
        changeVelocitySmoothness = 1f;

    protected virtual void SeekTarget(Vector3 target, float moveMult = 1f, bool checkStopDistance = true)
    {
        if (SeesTarget())
        {
            // Add force toward target. stop if too close
            var toTarget = 
                (target - transform.position);
            
            if (checkStopDistance && toTarget.magnitude < stopDistance) 
                return;

            toTarget = 
                toTarget.normalized * (moveSpeed * moveMult);
            
            toTarget.y = 
                rb.velocity.y;
            
            rb.velocity = toTarget;
        
            // sprite flips 180 after a certain amount of time of moving 
            reverseTimer -= 
                Time.fixedDeltaTime;
            if (reverseTimer <= 0f)
            {
                worldSprite.Reverse();
                reverseTimer = reverseTime;
            }
        }
    }
    
    protected bool SeesTarget()
    {
        float distanceToTarget = 
            Vector3.Distance(transform.position, playerObj.transform.position);
        
        return (distanceToTarget < sightDistance);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected void UpdateSprite()
    {
        spriteRen.sprite = sprites[curSprite];
    }
}

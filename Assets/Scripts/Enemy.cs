using System;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    protected GameObject playerObj;
    protected CharacterController cc;
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
        cc = GetComponent<CharacterController>();
        
        worldSprite = GetComponentInChildren<WorldSprite>();
        spriteRen = GetComponentInChildren<SpriteRenderer>();
        
        spriteRen.sprite = sprites[curSprite];

        reverseTimer = reverseTime;
    }

    void Update()
    {
        FallCheck();
    }

    private float yvel = 0f;
    private void FallCheck()
    {
        
        yvel = cc.isGrounded ? 0 : yvel - Time.deltaTime;
        cc.Move(new Vector3(0, yvel, 0));
    }
    
    protected virtual void SeekTarget(Vector3 target, float moveMult = 1f, bool checkStopDistance = true)
    {
        if (SeesTarget())
        {
            // Add force toward target. stop if too close
            var toTarget = 
                (target - transform.position);
            
            if (checkStopDistance && toTarget.magnitude < stopDistance) 
                return;

            toTarget.y = 0;
            toTarget = 
                toTarget.normalized * (moveSpeed * moveMult * Time.deltaTime);
            cc.Move(toTarget);
        
            // sprite flips 180 after a certain amount of time of moving 
            reverseTimer -= 
                Time.deltaTime;
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

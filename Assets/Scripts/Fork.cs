using System;
using UnityEngine;

public class Fork : Weapon
{
    [SerializeField] private float range, radius;

    public override Type wep_t
    {
        get => Type.Melee;
        protected set { }
    }

    protected override void DoAttack(Transform ctx)
    {
        
        
        // First check along a line from the camera, then if nothing found try a wider range cast.
        // A regular raycast is needed for when checking immediately in front of the player.
        //      -- the sphere cast will miss anything that is inside it at its time of creation.
        
        if (Physics.Raycast(ctx.position, ctx.forward, out var hit, range))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
                return;
            }
        }
        
        if (Physics.SphereCast(ctx.position, radius, ctx.forward, out hit, range))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
    }

}

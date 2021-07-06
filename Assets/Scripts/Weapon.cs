using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] public float attackTime;

    public enum Type
    {
        Melee, Gun, Nade
    }
    public abstract Type wep_t { get; protected set; }

    public enum AttackState
    {
        Ready, Attacking, Prepping
    }
    public AttackState attackState { get; protected set; }

    public abstract void Attack(Transform ctx);
}

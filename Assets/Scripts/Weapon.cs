using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] public float attackTime;

    protected AudioSource audsc;
    [SerializeField] protected AudioClip attackSound;

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

    private void Start()
    {
        audsc = GetComponent<AudioSource>();
    }

    public bool Attack(Transform cameraTransform)
    {
        if (!canAttack) return false;
        audsc.PlayOneShot(attackSound);
        DoAttack(cameraTransform);
        StartCoroutine(AttackCooldown());
        return true;
    }

    protected abstract void DoAttack(Transform ctx);

    private bool canAttack = true;
    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackTime);
        canAttack = true;
    }
}

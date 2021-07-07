using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class HookHairHead : Enemy
{
    private const float 
        painTime = .33f, scaredTime = 1.5f;

    [SerializeField] private 
        ParticleSystem bloodfx;

    private AudioSource audsc;
    [SerializeField] private 
        AudioClip hurtSound, dieSound;

    private enum LifeState
    {
        Alive, Dying
    }
    private LifeState 
        lifeState = LifeState.Alive;

    private enum Face
    {
        happy = 0,
        pain = 1,
        scared = 2
    }
    private Face 
        face = Face.happy;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        audsc = 
            GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move away/toward player if in range and not in pain.
        if (SeesTarget())
        {
            // doesnt check to limit distance if scared and moving backwards
            SeekTarget();
        }
    }

    void SeekTarget()
    {
        if (face == Face.happy)
        {
            SeekTarget(playerObj.transform.position, 1.5f);
        }
        else
        {
            var away = 
                transform.position + 
                (transform.position - playerObj.transform.position);
            switch (face)
            {
                case Face.scared:
                    // Move away from player after hit
                    base.SeekTarget(away, 1f, false);
                    break;
                
                case Face.pain:
                    // short, quick knockback
                    base.SeekTarget(away, 3f, false);
                    break;
                
                default:
                    break;
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        // Set up for either normal hit or dying
        StopCoroutine(nameof(BloodSpurt));
        SetFace(Face.pain);
        base.TakeDamage(damage);
        
        if (lifeState != LifeState.Alive) return;
        
        // Still alive after taking hit, do normal hit reaction
        StartCoroutine(nameof(BloodSpurt));
        audsc.pitch =    
            Random.Range(.95f, 1.05f);
        audsc.PlayOneShot(hurtSound);
    }

    IEnumerator BloodSpurt()
    {
        bloodfx.Play();
        yield return new WaitForSeconds(.5f);
        bloodfx.Stop();
    }

    void SetFace(Face newFace)
    {
        face = newFace;
        curSprite = (int) face;
        UpdateSprite();
        
        switch (face)
        {
            case Face.pain:
                // Stay in pain for a bit then become scared
                StopCoroutine(nameof(PainFace));
                StopCoroutine(nameof(ScaredFace));
                StartCoroutine(nameof(PainFace));
                break;
            
            case Face.scared:
                // Stay scared for a bit then revert back to normal
                StopCoroutine(nameof(ScaredFace));
                StartCoroutine(nameof(ScaredFace));
                break;
            
            default:
                break;
        }
    }

    IEnumerator PainFace ()
    {
        yield return new WaitForSeconds(painTime);
        SetFace(Face.scared);
    }

    IEnumerator ScaredFace()
    {
        yield return new WaitForSeconds(scaredTime);
        SetFace(Face.happy);
    }

    protected override void Die()
    {
        if (lifeState == LifeState.Dying) { return; }
        
        lifeState = LifeState.Dying;

        StopCoroutine(nameof(BloodSpurt));
        bloodfx.transform.localScale *= 2f;
        var main = bloodfx.main;
        /**/    main.simulationSpeed *= 3f;
        bloodfx.Play();

        audsc.PlayOneShot(dieSound);
        
        StartCoroutine(nameof(Die_CR));
    }
    
    private IEnumerator Die_CR()
    {
        yield return new WaitForSeconds(1f);
        base.Die();
    }
}

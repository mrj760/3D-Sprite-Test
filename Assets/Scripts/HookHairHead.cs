using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class HookHairHead : Enemy
{
    private const float 
        painTime = .5f, scaredTime = 1.5f;

    [SerializeField] private 
        ParticleSystem bloodfx;

    private AudioSource audsc;
    [SerializeField] private 
        AudioClip hurtSound;

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
    new void FixedUpdate()
    {
        // Move away/toward player if in range and not in pain.
        if (SeesTarget() && face != Face.pain)
        {
            // doesnt check to limit distance if scared and moving backwards
            CheckToMove();
        }
    }

    void CheckToMove()
    {
        switch (face)
        {
            case Face.happy:
                // move toward player with grin
                SeekTarget(playerObj.transform.position);
                break;
            
            case Face.scared:
                // Move away from player after hit
                var vec = 
                    transform.position + 
                    (transform.position - playerObj.transform.position);
                SeekTarget(vec, 2f);
                break;
            
            case Face.pain:
                // Stand in pain without moving
                break;
        }
    }

    public override void TakeDamage(float damage)
    {
        SetFace(Face.pain);
        base.TakeDamage(damage);

        if (lifeState != LifeState.Alive) return;
        
        StartCoroutine(nameof(BloodSpurt));
        audsc.Stop();
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
            
            case Face.happy:
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
        
        bloodfx.transform.localScale *= 3f;
        var main = bloodfx.main;
        /**/    main.simulationSpeed *= 3f;
        bloodfx.Play();
        
        StartCoroutine(nameof(Die_CR));
    }
    
    private IEnumerator Die_CR()
    {
        yield return new WaitForSeconds(2f);
        base.Die();
    }
}

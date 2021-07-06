using System;
using UnityEngine;

public class WorldSprite : MonoBehaviour
{
    /**
     * Just always look at the camera (or directly away for that flip effect)
     */
    
    private GameObject playerCam;
    private Transform tx;

    enum State
    {
        Forward, Reverse, Free
    }

    private State state = 
        State.Forward;
    
    // Start is called before the first frame update
    void Start()
    {
        playerCam = 
            GameObject.FindWithTag("MainCamera");
        tx = transform;
    }

    // Update is called once per frame
    void Update()
    {
        //
        switch (state)
        {
            //
            case State.Reverse:
                state = 
                    State.Reverse;
                ReverseLookAtPlayerCamera();
                break;
            //
            case State.Forward:
                state = 
                    State.Forward;
                LookAtPlayerCamera();
                break;
            //
            case State.Free:
                break;
        }
    }

    void LookAtPlayerCamera()
    {
        //
        tx.LookAt(playerCam.transform);
        tx.eulerAngles = 
            new Vector3(0f, tx.eulerAngles.y, 0f);
    }

    // same thing but with y+180
    void ReverseLookAtPlayerCamera()
    {
        tx.LookAt(playerCam.transform);
        tx.eulerAngles = 
            new Vector3(0f, tx.eulerAngles.y+180, 0f);
    }

    public void Reverse()
    {
        state = 
            state == State.Forward ? 
                State.Reverse : State.Forward;
    }
}

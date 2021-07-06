using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform ptx;
    private float hMouse, vMouse, xRot = 0f;

    [SerializeField] private float lookSens = 100f, yLookMult = 1f, xLookMult = 1f;
    
    void Update()
    {
        GetInput();
        Look();
    }

    private void GetInput()
    {
        var deltaSpeed = 
            lookSens * Time.deltaTime;
        
        vMouse = 
            Input.GetAxis("Mouse Y") * deltaSpeed * yLookMult;
        hMouse = 
            Input.GetAxis("Mouse X") * deltaSpeed * xLookMult;
    }
    
    private void Look()
    {
        xRot -= vMouse;
        xRot = 
            Mathf.Clamp(xRot, -80f, 80f);

        transform.localRotation = 
            Quaternion.Euler(xRot, 0f, 0f);
        ptx.Rotate(Vector3.up * hMouse);   
    }
}

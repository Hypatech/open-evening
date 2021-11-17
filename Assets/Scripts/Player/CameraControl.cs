using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform pivot;

    public float sensX, sensY;

    [HideInInspector]
    public Vector3 offset;

    Vector3 lookVec;

    [Header("Shake settings")]
    public float trauma;
    public float traumaRecovery;
    public float frequency;
    public float shake => trauma * trauma;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePos();
        HandleRot();
        HandleCursor();
    }

    void HandlePos(){
        var targetPos = pivot.position + pivot.TransformDirection(offset);
        var targetVec = (targetPos - pivot.transform.position);

        RaycastHit hit;
        if(Physics.Raycast(pivot.transform.position, targetVec.normalized, out hit, targetVec.magnitude + 1, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore)){
            transform.position = hit.point - targetVec.normalized;
        } else{
            transform.position = targetPos;
        }
    }

    void HandleRot(){
        transform.forward = pivot.forward;

        var inputX = Input.GetAxis("Mouse X") * sensX;
        var inputY = Input.GetAxis("Mouse Y") * -sensY;

        lookVec += new Vector3(inputY, inputX, 0);
        lookVec.x = Mathf.Clamp(lookVec.x, -60, 60);
        pivot.rotation = Quaternion.Euler(lookVec);

        var pitch = shake * (Mathf.PerlinNoise(0, Time.time * frequency)*2 - 1);
        var yaw = shake * (Mathf.PerlinNoise(1000, Time.time * frequency)*2 - 1);
        var roll = shake * (Mathf.PerlinNoise(2000, Time.time * frequency)*2 - 1);

        transform.forward += new Vector3(pitch, yaw, roll);

        trauma = Mathf.Lerp(trauma, 0, traumaRecovery * Time.deltaTime);
    }

    void HandleCursor(){
        if(Input.GetKey(KeyCode.Escape)){
            Cursor.lockState = CursorLockMode.None;
        }

        if(Input.GetMouseButton(0)){
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}

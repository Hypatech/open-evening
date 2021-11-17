using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum State{
        Move,
        Shield,
        Attack
    }

    public Transform pivot;
    public Transform model;
    public PlayerEvents playerEvents;
    public CharacterController chr;
    public CameraControl cam;
    public Animator anim;
    public PlayerSword sword;

    public GameObject shield;

    public Action<float> Damaged;
    public Action Deflected;

    public float maxHealth = 3;
    float health;

    [Header("Physics")]
    public float speed = 25;
    public float jumpSpeed = 25;
    public float gravity = 10;
    public float accel = 10;
    
    [Header("Combat")]
    public float attackForward = 50f;

    [Header("Graphical")]
    public float modelTurnSpeed = 5;
    public Vector3 regularOffset;
    public Vector3 aimOffset;

    public Vector3 velocity;
    public Vector3 horizontalVelocity{
        get => new Vector3(velocity.x, 0, velocity.z);
        set => velocity = new Vector3(value.x, velocity.y, value.z);
    }
    public State currentState {get; private set;}

    Vector3 inputVec;
    bool jumping;

    void OnEnable(){
        playerEvents.Landed += Land;
        playerEvents.AttackReleased += AttackReleased;
        playerEvents.AttackEnd += AttackEnd;

        Damaged += TakeDamage;
        health = maxHealth;

        Deflected += Deflect;
    }

    void OnDisable(){
        playerEvents.Landed -= Land;
        playerEvents.AttackReleased -= AttackReleased;
        playerEvents.AttackEnd -= AttackEnd;

        Damaged -= TakeDamage;
        Deflected += Deflect;
    }

    void TakeDamage(float dmg){
        health -= dmg;
    }

    void Deflect(){
        cam.trauma += 0.3f;
    }

    void AttackReleased(float val){
        model.forward = Vector3.ProjectOnPlane(cam.pivot.forward, Vector3.up);

        sword.enabled = true;
        horizontalVelocity = inputVec * val;
        anim.SetBool("Attack", false);
        currentState = State.Attack;
    }

    void AttackEnd(){
        sword.enabled = false;
        currentState = State.Move;
        
    }

    void Land(){
        horizontalVelocity = Vector3.zero;
    }

    void Update()
    {
        var adjustedPivotForward = Vector3.ProjectOnPlane(pivot.forward, Vector3.up).normalized;
        var adjustedPivotRight = Vector3.ProjectOnPlane(pivot.right, Vector3.up).normalized;
        inputVec = ((adjustedPivotForward * Input.GetAxisRaw("Vertical")) + (adjustedPivotRight * Input.GetAxisRaw("Horizontal"))).normalized;

        if(Input.GetMouseButtonDown(0) && chr.isGrounded){
            anim.SetBool("Attack", true);
        }
        

        if(currentState == State.Move){
            
            

            cam.offset = Vector3.Lerp(cam.offset, regularOffset, 10 * Time.deltaTime);

            HandleMove();
            anim.SetBool("Rising", velocity.y > 0);
            anim.SetFloat("Speed", horizontalVelocity.magnitude / speed);
            anim.SetBool("Grounded", chr.isGrounded);
            anim.SetBool("Aiming", false);

            shield.SetActive(false);

            //TODO: checkAim method
            currentState = Input.GetMouseButton(1) ? State.Shield : State.Move;

            
        } else if(currentState == State.Shield){
            cam.offset = Vector3.Lerp(cam.offset, aimOffset, 10 * Time.deltaTime);
            model.forward = Vector3.Slerp(model.forward, Vector3.ProjectOnPlane(cam.pivot.forward, Vector3.up), modelTurnSpeed * Time.deltaTime);

            if(chr.isGrounded){
                velocity = Vector3.Lerp(velocity, Vector3.zero, 10 * Time.deltaTime);
            } else{
                velocity.y -= gravity * Time.deltaTime;
            }

            anim.SetBool("Aiming", true);

            shield.SetActive(true);

            currentState = Input.GetMouseButton(1) ? State.Shield : State.Move;
        } else{
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, accel * Time.deltaTime);
            // model.forward = Vector3.Slerp(model.forward, Vector3.ProjectOnPlane(inputVec, Vector3.up), modelTurnSpeed * 0.05f * Time.deltaTime);
        }

        chr.Move(velocity * Time.deltaTime);
    }

    void HandleMove(){
        

        if(chr.isGrounded){
            if(!jumping){
                velocity.y = -5;

                if(Input.GetKeyDown(KeyCode.Space)){
                    anim.SetTrigger("Jump");
                    jumping = true;
                    velocity.y = jumpSpeed;
                }
            }
        } else{
            jumping = false;
            velocity.y -= gravity * Time.deltaTime;
        }

        


        horizontalVelocity = Vector3.Lerp(horizontalVelocity, inputVec * speed, accel * Time.deltaTime);

        if(inputVec.magnitude > 0){
            model.forward = Vector3.Slerp(model.transform.forward, inputVec, modelTurnSpeed * Time.deltaTime);
        } else{
            model.forward = Vector3.Slerp(model.forward, Vector3.ProjectOnPlane(model.forward, Vector3.up), modelTurnSpeed * Time.deltaTime);        
        }
    }

    public void Hitstop(float ms){
        StartCoroutine(HitstopRoutine(ms));
    }

    IEnumerator HitstopRoutine(float ms){
        anim.speed = 0f;
        yield return new WaitForSeconds(ms / 1000);
        anim.speed = 1;
    }
}

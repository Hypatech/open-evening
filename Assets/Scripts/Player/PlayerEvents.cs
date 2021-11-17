using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public Animator anim;
    public Action Landed, AttackEnd, WindupStart;
    public Action<float> AttackReleased;

    void Start(){
        anim = GetComponent<Animator>();
    }

    void LandedAnimEvent(){
        Landed?.Invoke();
    }

    void AttackStartAnimEvent(AnimationEvent value){
        AttackReleased?.Invoke(value.floatParameter);
    }

    void AttackEndAnimEvent(){
        AttackEnd?.Invoke();
    }

    void WindupStartAnimEvent(){
        WindupStart?.Invoke();
    }

    void FinalAttackAnimEvent(){
        anim.SetBool("Attack", false);
    }
}

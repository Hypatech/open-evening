using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float maxLifetime;
    float lifetime;

    Vector3 lastPos;

    public float dmg;
    public float speed;

    bool friendly;

    // Start is called before the first frame update
    void Start()
    {
        lifetime = maxLifetime;
    }

    // Update is called once per frame
    void Update()
    {
        lastPos = transform.position;
        transform.position += transform.forward * speed * Time.deltaTime;

        RaycastHit hit;
        var diff = transform.position - lastPos;
        if(Physics.SphereCast(lastPos, 1.5f, diff.normalized, out hit, diff.magnitude, LayerMask.GetMask("Player", "Default", "Entity"), QueryTriggerInteraction.Ignore)){
            transform.position = hit.point - transform.forward*0.5f;

            if(friendly){
                hit.transform.gameObject.GetComponentInParent<Entity>()?.Damaged?.Invoke(999, hit.point, transform.forward * speed);
                Destroy(this.gameObject);
            } else{
                var plr = hit.transform.gameObject.GetComponentInParent<Player>();
                if(plr){
                    if(plr.currentState == Player.State.Shield){
                        friendly = true;
                        transform.forward = -transform.forward;
                        plr.Deflected?.Invoke();
                    } else{
                        plr.Damaged?.Invoke(dmg);
                        Destroy(this.gameObject);
                    }
                } else{
                    Destroy(this.gameObject);
                }
            }
        }

        lifetime -= Time.deltaTime;
        if(lifetime <= 0){
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public Player plr;

    public Transform top;
    public Transform bottom;
    public float damage = 1;
    public float ssPerHit = 0.3f;

    public ParticleSystem particle;

    public int pointAmount = 10;
    Vector3 lastTop;

    List<GameObject> objectsHit = new List<GameObject>();

    public void Refresh(){
        objectsHit.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        lastTop = top.position;
    }

    void OnEnable(){
        Refresh();

        var em = particle.emission;
        em.rateOverDistanceMultiplier = 1;
    }

    private void OnDisable() {
        var em = particle.emission;
        em.rateOverDistanceMultiplier = 0;
    }

    // Update is called once per frame
    void Update()
    {

        var diff = top.position - lastTop;
        for(float i = 0; i < pointAmount; i ++){
            var dist = i / pointAmount;
            var curPos = Vector3.Lerp(bottom.position, top.position, dist);

            Debug.DrawLine(curPos, curPos - diff.normalized, Color.green, 2);

            RaycastHit hit;
            if(Physics.SphereCast(curPos, 1 / pointAmount, -diff.normalized, out hit, diff.magnitude, LayerMask.GetMask("Entity"), QueryTriggerInteraction.Ignore)){
                if(!objectsHit.Contains(hit.transform.gameObject)){
                    

                    objectsHit.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponentInParent<Entity>()?.Damaged?.Invoke(damage, hit.point, diff / Time.deltaTime);
                    plr.cam.trauma += ssPerHit;
                    plr.Hitstop(50);
                    // plr.velocity = Vector3.zero;
                }
            }       
        }

        lastTop = top.position;
    }
}

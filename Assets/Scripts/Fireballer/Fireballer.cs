using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireballer : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float maxCooldown;
    float cooldown;

    Player plr;

    Material myMat;

    // Start is called before the first frame update
    void Start()
    {
        plr = FindObjectOfType<Player>();
        myMat = new Material(GetComponentInChildren<MeshRenderer>().material);
        GetComponentInChildren<MeshRenderer>().material = myMat;
        myMat.EnableKeyword("_EMISSION");
    }

    private void OnEnable() {
        cooldown = Random.Range(maxCooldown / 2, maxCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        var targ = plr.transform.position + Vector3.up*2;
        var targLook = targ - (transform.position + Vector3.up * 2);
        transform.forward = Vector3.ProjectOnPlane(targLook, Vector3.up);

        cooldown -= Time.deltaTime;    
        if(cooldown <= 0){
            cooldown = Random.Range(maxCooldown / 2, maxCooldown);

            var go = GameObject.Instantiate(fireballPrefab);
            go.transform.position = transform.position + Vector3.up*2 + transform.forward;
            go.transform.rotation = Quaternion.LookRotation(targ - go.transform.position);
            go.SetActive(true);
        }

        Debug.Log(maxCooldown - cooldown);

        myMat.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.red, (maxCooldown - cooldown) / maxCooldown));
    }
}

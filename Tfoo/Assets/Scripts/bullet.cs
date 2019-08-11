using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float bulletspeed = 100f;
    private GameObject target;
    public float damage = 5;
    public GameObject bullethole;
    public GameObject hitsparks;
    public GameObject raycastpoint;
    public bool hashit = false;
    private GameObject FX1, FX2;
    private float destroydelay = 0.1f;

    void Start()
    {
        hashit = false;
        Instantiatehiteffets();
    }

    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * bulletspeed); 
        if(hashit == true)
        {
            FX1.SetActive(true);
            FX2.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        hashit = true;
        bulletspeed = 0;      
        if (other.tag != "bullet")
        {
            StartCoroutine(destroybullet());
        }
    }    

    void Instantiatehiteffets()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastpoint.transform.position, raycastpoint.transform.forward, out hit))
        {
            Debug.Log(hit.transform.name);
            GameObject Bullethole =  Instantiate(bullethole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            GameObject Hitsparks = Instantiate(hitsparks, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            FX1 = Bullethole;
            FX2 = Hitsparks;

            FX1.SetActive(false);
            FX2.SetActive(false);
        }
    }

    public IEnumerator destroybullet()
    {
        yield return new WaitForSeconds(destroydelay);
        Destroy(gameObject);
    }
}
    

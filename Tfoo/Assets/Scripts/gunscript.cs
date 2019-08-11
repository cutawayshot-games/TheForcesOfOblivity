using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gunscript : MonoBehaviour
{
    [Header("ComponentsGetters")]
    public AudioSource audsource;
    public Animator animator;
    public Camera fpscam;

    [Header("Properties")]
    public float damage = 10f;
    public float range = 150f;
    public float Minfirerate = 600f;
    public float Maxfirerate = 600f;
    private float firedelay = 0f;
    public int magazinesize = 5;
    public int loadedbullets = 5;
    public int totalammo = 50;
    public float scopeFov = 15f;
    public float normalFov = 60f;   
    public GameObject BulletPrefab;
    public bool isreloading;
    public bool isshooting;
    public enum shootmode { Auto, Semi }
    public shootmode shootingmode;
    private bool shootinput;
    public GameObject Scopeobject;
    public bool WeaponHasScope;
    public GameObject Partstodisableuponscope;

    [Header("Audioclips")]
    public AudioClip shootingsound;
    public AudioClip ammorefil;

    [Header("Recoil")]
    public GameObject recoilrotation;
    private Vector3 recoilangle;
    public float currentaccX, currentaccY;
    public float currentsnapX, currentsnapY;
    public float hipAccuracy;
    public float ADSAccuracy;
    public float Verticalsnap = 1f;
    public float Horizontalsnap = 1f;
    public float kickback = 0.5f;
    public float recoilspeed = 1f;
    public GameObject weaponobject;

    [Header("Aimpositioning")]
    public Vector3 hipposition;
    public Vector3 adsposition;
    public bool isaiming;
    public float aimspeed = 1f;
    public Transform shootpoint;

    [Header("UI")]
    public Text ammotext;

    void Start()
    {
        audsource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        updateammotext();
    }

    private void FixedUpdate()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        isreloading = info.IsName("reload");

        if (isaiming == true)
        {
            weaponobject.transform.localPosition = Vector3.Lerp(weaponobject.transform.localPosition, adsposition, Time.deltaTime * aimspeed);
        }

        else if (isaiming == false)
        {
            weaponobject.transform.localPosition = Vector3.Lerp(weaponobject.transform.localPosition, hipposition, Time.deltaTime * aimspeed);
        }
    }

    void Update()
    {
        if (!isaiming)
        {
            Scopeobject.SetActive(false);
            Partstodisableuponscope.SetActive(true);
            fpscam.fieldOfView = normalFov;
        }

        switch (shootingmode)
        {
            case shootmode.Auto:
                shootinput = Input.GetButton("Fire1");
                break;

            case shootmode.Semi:
                shootinput = Input.GetButtonDown("Fire1");
                break;
        }       

        if (shootinput && Time.time >= firedelay)
        {
            if (loadedbullets > 0)
                shoot();
        }

        if (Input.GetButton("Fire2"))
        {
            if (isreloading == true)
                return;

            aim();
            currentaccX = Random.Range(-ADSAccuracy, ADSAccuracy);
            currentaccY = Random.Range(-ADSAccuracy, ADSAccuracy);
        }
        else
        {
            isaiming = false;
            currentaccX = Random.Range(-hipAccuracy, hipAccuracy);
            currentaccY = Random.Range(-hipAccuracy, hipAccuracy);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (loadedbullets < magazinesize && totalammo > 0)


                Doreload();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            recoilrotation.transform.localRotation = Quaternion.identity;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            recoilrotation.transform.localRotation = Quaternion.identity;
        }

        updateammotext();
    }


    private void shoot()
    {
        if (loadedbullets < 1 || isreloading)
            return;

        float Currentfirerate = Random.RandomRange(Minfirerate,Maxfirerate);

        firedelay = Time.time + 60f / Currentfirerate;

        Instantiate(BulletPrefab, shootpoint.position, shootpoint.rotation);

        currentsnapX = Random.Range(-Horizontalsnap, Horizontalsnap);
        currentsnapY = Random.Range(-Verticalsnap, Verticalsnap);

        Vector3 weaponobjectlocalposition = weaponobject.transform.localPosition;
        Quaternion weaponobjectlocalrotation = weaponobject.transform.localRotation;
        weaponobjectlocalposition.x = weaponobjectlocalposition.x + currentsnapX;
        weaponobjectlocalposition.y = weaponobjectlocalposition.y + currentsnapY;
        weaponobjectlocalposition.z = weaponobjectlocalposition.z - kickback;
        weaponobject.transform.localPosition = weaponobjectlocalposition;

        recoilangle.x = currentaccX;
        recoilangle.y = currentaccY;
        recoilrotation.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(recoilrotation.transform.localEulerAngles), Quaternion.Euler(recoilangle), Time.deltaTime);

        audsource.PlayOneShot(shootingsound);
        //animator.CrossFadeInFixedTime("shoot", 0.01f); 
        loadedbullets--;
    }

    public void reload() //Never call this function directly, instead call the 'Doreload' function and this will be called with it
    {
        if (totalammo < 1)
            return;

        int bulletstoret = loadedbullets;
        int bulletsintotal = totalammo;

        int bulletstoload = magazinesize - loadedbullets;
        int bulletstodeduct = (totalammo >= bulletstoload) ? bulletstoload : totalammo;

        totalammo -= bulletstodeduct;
        loadedbullets += bulletstodeduct;

        updateammotext();
    }

    void Doreload()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (isreloading)
        {
            return;
        }

        GetComponent<Animator>().Play("reload");
        isaiming = false;
    }

    void updateammotext()
    {
        ammotext.text = loadedbullets + " / " + totalammo;
    }

    public void addammo()
    {
        totalammo += 30;
        audsource.PlayOneShot(ammorefil);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ammobox")
        {
            addammo();
        }
    }

    void aim()
    {
        if (!WeaponHasScope)
        {
            isaiming = true;
        }

        if (WeaponHasScope)
        {
            isaiming = true;
            StartCoroutine(OnScoped());
        }   
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(.15f);
        Scopeobject.SetActive(true);
        Partstodisableuponscope.SetActive(false);
        fpscam.fieldOfView = scopeFov;    
    }
}


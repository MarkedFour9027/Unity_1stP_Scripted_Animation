using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsCtrl : MonoBehaviour
{

    [Header ("Ammo")]
    [SerializeField] private int magLimit = 30;
    [SerializeField] private int currentMag;



    private float nextTimeToFire = 0f;

    [Header ("Guns Function")]
    [SerializeField] private bool isBurst = false;
    [SerializeField] private bool boltLockAtEnd = false;
    [SerializeField] private bool saveOneBullet = false;
    [SerializeField] private bool isSemi = false;
    private float timeBetweenShot = 0.05f;
    private float lastShotTime = 0f;
    private bool reloading = false;
    public bool isAim = false;
    //private bool isEmpty = false;
    [SerializeField] private float rateFire = 5f;
    private float fAutoROF;
    [SerializeField] private float loadTime = 1f;
    [SerializeField] private float caseVelo = 150f;
    public float fSemiInt;
    public float fAutoInt;
    float fSpent = 0.0f;
    int fHash;
    bool Inspecting = false;

    [Header("Case Settings")]
    public bool isFlop;

    [Header("Reference")]
    public Animator gAnim;
    //public GameObject camObj;
    public GameObject muzzFlash;
    public Rigidbody spentCase;
    public AudioClip fireSFX;
    public AudioSource magDSC;
    public AudioSource magASC;
    public AudioSource fireSC;
    public AudioSource boltpullSC;
    public AudioSource boltReleaseSC;
    public AudioSource additionalSC;
    public AudioSource additionalSC2;
    public Transform muzzle;
    public Transform receiver;
    public RecScript recoil;
    public Transform fSelectionRot;


    void Start()
    {
        currentMag = magLimit;
        InstantiateAudio(fireSFX);
        fAutoROF = rateFire;
        //fHash = Animator.StringToHash("FireSpent");
    }

    private void InstantiateAudio(AudioClip clip)
    {
        fireSC = gameObject.AddComponent<AudioSource>();
        fireSC.clip = clip;
    }

    public void playSound()
    {
        fireSC.PlayOneShot(fireSFX);
    }

    public void MagRemove()
    {
        magDSC.Play();
    }

    public void MagInsert()
    {
        magASC.Play();
    }

    public void PullCHandle()
    {
        boltpullSC.Play();
    }

    public void RelCHandle()
    {
        boltReleaseSC.Play();
    }

    public void addSFX()
    {
        additionalSC.Play();
    }

    public void addSFX2()
    {
        additionalSC2.Play();
    }
    void Update()
    {
        if (reloading)
        {
            return;
        }

        /*if (isSemi)
        {
            rateFire = 2f;
        }
        else
        {
            rateFire = fAutoROF;
        }*/

        if(isSemi)
        {
            if(Input.GetButtonDown("Fire1") && Time.time >= lastShotTime + timeBetweenShot) 
            {
                lastShotTime = Time.time;
                Fire();
            }
        }
        else
        {
            
            if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                Fire();
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            StartCoroutine(DumpLoad());
        }

        if(Input.GetKey(KeyCode.Q))
        {
            StartCoroutine(FastLoad());
        }

        if (Input.GetButtonDown("Fire2"))
        {
            ADS();
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            UnADS();
        }
        if (Input.GetKey(KeyCode.L))
        {
            gAnim.SetBool("Holster", true);
        }
        
        if (Input.GetKey(KeyCode.I))
        {
            Inspecting = true;
        }
        else
        {
            Inspecting = false;
        }

        if(Input.GetKey(KeyCode.T))
        {
            if(isSemi)
            {
                if(isAim)
                {
                    gAnim.SetBool("ADSSToA", true);
                }
                gAnim.SetBool("SToA", true);
            }
            else
            {
                if(isAim)
                {
                    gAnim.SetBool("ADSAToS", true);
                }
                gAnim.SetBool("AToS", true);
            }
            
        }

        if(Inspecting)
        {
            gAnim.SetBool("Inspect", true);
        }
        else
        {
            gAnim.SetBool("Inspect", false);
        }
    }


    void Fire()
    {
        if(currentMag > 0)
        {
            nextTimeToFire = Time.time + 1f / rateFire;
            currentMag--;
            ShellCasing();
            playSound();
            MuzzleFlash();
            recoil.RecFire();
            if (!isAim)
            {
                gAnim.CrossFadeInFixedTime("Fire", 0.1f);
                /*if(fSpent == 0.1f)
                {
                    gAnim.CrossFadeInFixedTime("Fire_bM1", 0.1f);
                }*/
                if(boltLockAtEnd && currentMag < 1)
                {
                    gAnim.CrossFadeInFixedTime("Fire End", 0.1f);
                }
            }
            else if (isAim)
            {
                gAnim.CrossFadeInFixedTime("ADSFire", 0.1f);
                if(boltLockAtEnd && currentMag < 1)
                {
                    gAnim.CrossFadeInFixedTime("ADSFire End", 0.1f);
                }
            }
            if(currentMag > 12)
            {
                fSpent = 0.0f;

            }
            else if(currentMag < 12)
            {
                fSpent = 0.1f;
            }
            else if(currentMag < 10)
            {
                fSpent = 0.2f;
            }
            else if(currentMag < 9)
            {
                fSpent = 0.3f;
            }
            //gAnim.SetFloat(fHash, fSpent);
        }
        else if(currentMag == 0)
        {
            StartCoroutine(Reload());
        }

    }
    void ADS()
    {
        isAim = true;
        gAnim.SetBool("ADS", true);
    }

    void UnADS()
    {
        isAim = false;
        gAnim.SetBool("ADS", false);
    }

    void ShellCasing()
    {
        if(!isFlop)
        {
            Rigidbody cartridgePhysics;
            cartridgePhysics = Instantiate(spentCase, receiver.position, Quaternion.Euler(90, Random.Range(0, 90), 180)) as Rigidbody;
            cartridgePhysics.AddForce(receiver.right * caseVelo);
            receiver.localRotation = Quaternion.Euler(0, Random.Range(-15, 15), Random.Range(0, 30));
        }
        else
        {
            Rigidbody cartridgePhysics;
            cartridgePhysics = Instantiate(spentCase, receiver.position, Quaternion.Euler(0, Random.Range(0, 90), 180)) as Rigidbody;
            cartridgePhysics.AddForce(receiver.right * caseVelo);
            receiver.localRotation = Quaternion.Euler(0, Random.Range(-15, 15), Random.Range(0, 30));
        }
       
    }

    void MuzzleFlash()
    {
        GameObject muzzObj = Instantiate(muzzFlash, muzzle.position, muzzle.rotation);
        muzzObj.transform.parent = gameObject.transform;

        Destroy(muzzObj, 1f);
    }

    IEnumerator Reload()
    {
        if(isAim)
        {
            gAnim.SetBool("ADSReload", true);
            //gAnim.SetBool("ADS", false);
            //isAim = false;
            fSpent = 0.0f;
            yield return new WaitForSeconds(.1f);
            reloading = true;
            yield return new WaitForSeconds(loadTime - .25f);
            reloading = false;
            currentMag = magLimit;
            gAnim.SetBool("ADSReload", false);
        }
        else
        {
            gAnim.SetBool("Reload", true);
            //gAnim.SetBool("ADS", false);
            //isAim = false;
            fSpent = 0.0f;
            yield return new WaitForSeconds(.1f);
            reloading = true;
            yield return new WaitForSeconds(loadTime - .25f);
            reloading = false;
            currentMag = magLimit;
            gAnim.SetBool("Reload", false);
        }
        
    }

    IEnumerator DumpLoad()
    {
        if(isAim)
        {
            gAnim.SetBool("ADSDumpLoad", true);
            yield return new WaitForSeconds(.1f);
            reloading = true;
            yield return new WaitForSeconds(loadTime - .25f);
            reloading = false;
            currentMag = magLimit;
            if (saveOneBullet)
            {
                currentMag = magLimit + 1;
            }
            gAnim.SetBool("ADSDumpLoad", false);
        }
        else
        {
            gAnim.SetBool("DumpLoad", true);
            yield return new WaitForSeconds(.1f);
            reloading = true;
            yield return new WaitForSeconds(loadTime - .25f);
            reloading = false;
            currentMag = magLimit;
            if (saveOneBullet)
            {
                currentMag = magLimit + 1;
            }
            gAnim.SetBool("DumpLoad", false);
        }

    }

    IEnumerator FastLoad()
    {
        gAnim.SetBool("FastLoad", true);
        yield return new WaitForSeconds(.1f);
        reloading = true;
        yield return new WaitForSeconds(loadTime - .25f);
        reloading = false;
        currentMag = magLimit;
        if (saveOneBullet)
        {
            currentMag = magLimit + 1;
        }
        gAnim.SetBool("FastLoad", false);
    }

    public void FSChange_AToS()
    {
        fSelectionRot.localRotation = Quaternion.Euler(fSemiInt, 0, 0);
        isSemi = true;
        gAnim.SetBool("AToS", false);
        gAnim.SetBool("ADSAToS", false);
        Debug.Log("Changed to Semi");
    }

    public void FSChange_SToA()
    {
        fSelectionRot.localRotation = Quaternion.Euler(fAutoInt, 0, 0);
        isSemi = false;
        gAnim.SetBool("SToA", false);
        gAnim.SetBool("ADSSToA", false);
        Debug.Log("Changed to Auto");
    }
}

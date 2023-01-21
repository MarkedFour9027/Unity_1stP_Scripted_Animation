using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecScript : MonoBehaviour
{

    public GunsCtrl ADSRecFunc;
    private bool takeAim;

    private Vector3 currentRot;
    private Vector3 targetRot;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float ADSRecoilX;
    [SerializeField] private float ADSRecoilY;
    [SerializeField] private float ADSRecoilZ;


    [SerializeField] private float snap;
    [SerializeField] private float returnSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        takeAim = ADSRecFunc.isAim;
        targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRot = Vector3.Slerp(currentRot, targetRot, snap * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRot);
    }

    public void RecFire()
    {
        if(takeAim) targetRot += new Vector3(Random.Range(0,ADSRecoilX), Random.Range(-ADSRecoilY, ADSRecoilY), Random.Range(-ADSRecoilZ, ADSRecoilZ));
        else targetRot += new Vector3(Random.Range(-recoilX,recoilX), Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}

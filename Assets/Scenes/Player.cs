using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public struct equippedGun
{
    public Gun gunItem;
    public int currentAmmo;

    public int clipAmmo;
    
}
public class Player : MonoBehaviour
{
    [HideInInspector] public float f_JumpPower = 120.0f;
    protected CharacterController moveController;
    protected Camera mainCam;
    private bool isGrounded_b;
    private Vector3 moveDir = Vector3.zero;
    public List<equippedGun> gunsInStore;
    public TMP_Text _ammoText;
    public Gun primaryGun;
    public float mouseSensitivity= 2.0f;
    private int chosenGunId = 0;
    private float XRotation;
    private float YRotation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunsInStore = new List<equippedGun>();
        moveController = GetComponent<CharacterController>();
        mainCam = GetComponentInChildren<Camera>();
        AddGun(primaryGun);
        Cursor.visible = false;
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jumped");
        if (moveController.isGrounded)
        {
            moveDir.y = f_JumpPower;

        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        Vector2 InputDelta = context.ReadValue<Vector2>();
        //XRotation -= InputDelta.y;
        XRotation = Mathf.Clamp(XRotation - (InputDelta.y * (mouseSensitivity/5.0f)), -90.0f, 90.0f);
        YRotation += (InputDelta.x/5.0f);
        
        mainCam.transform.localRotation = Quaternion.Euler(XRotation, YRotation, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (moveController.isGrounded)
        {
            //bla bla bla
        }
        moveDir.y -= 9.8f * Time.deltaTime;
        moveController.Move(moveDir * Time.deltaTime);

    }
    public void AddGun(Gun _gun)
    {
        gunsInStore.Add(new equippedGun { gunItem = _gun, currentAmmo = _gun.maxAmmo - _gun.maxClipAmmo, clipAmmo = _gun.maxClipAmmo });
        chosenGunId = gunsInStore.Count - 1;
        _ammoText.text = gunsInStore[chosenGunId].clipAmmo + "/" + gunsInStore[chosenGunId].currentAmmo;
        
        
    }
    void Reload()
    {
        //make delay bla bla
        equippedGun repelishedGun = gunsInStore[chosenGunId];
        repelishedGun.clipAmmo = Math.Clamp(repelishedGun.currentAmmo, 0, repelishedGun.gunItem.maxClipAmmo);
        repelishedGun.currentAmmo -= repelishedGun.clipAmmo;
        gunsInStore[chosenGunId] = repelishedGun;
        _ammoText.text = gunsInStore[chosenGunId].clipAmmo + "/" + gunsInStore[chosenGunId].currentAmmo;

    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started){
        equippedGun depletedGun = gunsInStore[chosenGunId];
        depletedGun.clipAmmo = gunsInStore[chosenGunId].clipAmmo-1;
        gunsInStore[chosenGunId] = depletedGun;
        _ammoText.text = gunsInStore[chosenGunId].clipAmmo + "/" + gunsInStore[chosenGunId].currentAmmo;
        if (gunsInStore[chosenGunId].clipAmmo <= 0)
        {
            Reload();
        }}
    }

    }

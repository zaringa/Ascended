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
    public InputActionReference jump;
    public InputActionReference shoot;
    protected CharacterController moveController;
    protected Camera mainCam;
    private bool isGrounded_b;
    private Vector3 moveDir = Vector3.zero;
    public List<equippedGun> gunsInStore;
    public TMP_Text _ammoText;
    public Gun primaryGun;
    private int chosenGunId = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunsInStore = new List<equippedGun>();
        moveController = GetComponent<CharacterController>();
        mainCam = GetComponentInChildren<Camera>();
        AddGun(primaryGun);
    }
    void Jump(InputAction.CallbackContext obj)
    {
        Debug.Log("Jumped");
        if (moveController.isGrounded)
        {
            moveDir.y = f_JumpPower;

        }
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
    void OnEnable()
    {
        jump.action.started += Jump;
        shoot.action.started += Shoot;
    }
    void OnDisable()
    {
        jump.action.started -= Jump;
        shoot.action.started -= Shoot;

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
    void Shoot(InputAction.CallbackContext obj)
    {
        equippedGun depletedGun = gunsInStore[chosenGunId];
        depletedGun.clipAmmo = gunsInStore[chosenGunId].clipAmmo-1;
        gunsInStore[chosenGunId] = depletedGun;
        _ammoText.text = gunsInStore[chosenGunId].clipAmmo + "/" + gunsInStore[chosenGunId].currentAmmo;
        if (gunsInStore[chosenGunId].clipAmmo <= 0)
        {
            Reload();
        }
    }

    }

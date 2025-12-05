using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInputHandler : MonoBehaviour
{
    [Header("Ссылка на оружие")]
    [SerializeField] private BaseWeapon weapon;

    [Header("Настройки ввода")]
    [SerializeField] private bool useMouseButton = true; // ЛКМ
    [SerializeField] private Key alternativeFireKey = Key.Space; // Альтернативная клавиша

    private Mouse mouse;
    private Keyboard keyboard;
    private bool wasFiring = false;

    private void Start()
    {
        mouse = Mouse.current;
        keyboard = Keyboard.current;
        if (weapon == null)
        {
            weapon = GetComponent<BaseWeapon>();
        }
        if (weapon == null)
        {
            weapon = GetComponentInChildren<BaseWeapon>();
        }

        if (weapon == null)
        {
            Debug.LogError("[WeaponInputHandler] Оружие не найдено! Назначьте BaseWeapon в инспекторе.");
        }
        else
        {
            Debug.Log($"[WeaponInputHandler] Найдено оружие: {weapon.gunInfo.name}");
            Debug.Log("ЛКМ - стрелять/атаковать, R - перезарядка");
        }
    }

    private void Update()
    {
        //TODO Нужен фикс
        /*if (weapon == null) return; - Требуется фикс
        if (mouse == null || keyboard == null) return;

        if (useMouseButton)
        {
            if (weapon is SemiAutoRifle)
            {
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    weapon.TryToFire();
                }
            }
            else if (weapon is DoubleBarrelShotgun)
            {
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    weapon.TryToFire();
                }
            }
            else if (weapon is Katana)
            {
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    weapon.TryToFire();
                }
            }
            else if (weapon.WeaponType == GunInfo.WeaponType.Firearm)
            {
                if (mouse.leftButton.isPressed) // ЛКМ удержание
                {
                    if (!wasFiring) 
                    {
                        weapon.OnFireButtonPressed();
                        wasFiring = true;
                    }
                    weapon.TryToFire();
                }
                else if (wasFiring) 
                {
                    weapon.OnFireButtonReleased();
                    wasFiring = false;
                }
            }
            else
            {
                if (mouse.leftButton.wasPressedThisFrame) // ЛКМ нажатие
                {
                    weapon.TryToFire();
                }
            }
        }

        // Перезарядка на R
        if (keyboard[Key.R].wasPressedThisFrame)
        {
            weapon.StartReload();
            return;
        }
        if (keyboard[alternativeFireKey].isPressed && alternativeFireKey != Key.R)
        {
            weapon.TryToFire();
        }*/
    }

    // Отображение информации на экране
    private void OnGUI()
    {
        //TODO Нужен фикс
        /*if (weapon == null) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 18;
        style.normal.textColor = Color.white;

        GUI.Box(new Rect(10, 10, 250, 120), "");
        GUI.Label(new Rect(20, 15, 300, 30), $"Оружие: {weapon.gunInfo.name}", style);

        if (weapon.WeaponType == GunInfo.WeaponType.Firearm)
        {
            GUI.Label(new Rect(20, 45, 300, 30),
                $"Патроны: {weapon.CurrentAmmo}/{weapon.MaxAmmo}", style);

            if (weapon.IsReloading)
            {
                style.normal.textColor = Color.yellow;
                GUI.Label(new Rect(20, 75, 300, 30), "ПЕРЕЗАРЕЖАЮСЬ", style);
                style.normal.textColor = Color.white;
            }
        }
        else
        {
            GUI.Label(new Rect(20, 45, 300, 30), "Холодное оружие", style);
        }

        if (weapon.IsAttacking)
        {
            style.normal.textColor = Color.red;
            GUI.Label(new Rect(20, 75, 300, 30), "РАТАТАТАТАТА", style);
            style.normal.textColor = Color.white;
        }
        style.fontSize = 14;
        GUI.Label(new Rect(20, 105, 300, 20), "ЛКМ - Атака | R - Перезарядка", style);*/
    }
}
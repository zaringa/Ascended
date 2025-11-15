using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Основные импланты")]
    [SerializeField] public ArmsImplant armsImplant;
    [SerializeField] public BodyImplant bodyImplant;
    [SerializeField] public LegsImplant legsImplant;

    [Header("Малые и гига-импланты")]
    [SerializeField] public List<(SmallImplant, int)> smallImplants = new();
    [SerializeField] public List<(GigaImplant, int)> gigaImplants = new();

    [Header("Основное и второстепенное оружие")]
    [SerializeField] public GunInfo primaryWeapon;
    [SerializeField] public GunInfo secondaryWeapon;

    [Header("Расходники")]
    [SerializeField] public IConsumable[] craftedConsumables = new IConsumable[3];
    [SerializeField] public List<(IConsumable, int)> consumables;

    /// <summary> Значения конкретных изменяемых имплантами параметров по умолчанию и значени после внесения изменений </summary>
    private List<(Implant.Stat stat, float defaultValue, float currentValue)> stats = new();

    /// <summary> Добавляет новый малый имплант </summary>
    /// <param name="smallImplant"> Добавляемый имплант </param>
    /// <param name="count"> Количество имплантов добавляемого типа </param>
    public void AddSmallImplant(SmallImplant smallImplant, int count = 1)
    {
        for (int i = 0; i < smallImplants.Count; i++)
            if (smallImplants[i].Item1.GetType() == smallImplant.GetType())
                smallImplants[i] = (smallImplants[i].Item1, smallImplants[i].Item2 + count);
        if (!stats.Exists(x => x.stat == smallImplant.StatType))
            SetStat(smallImplant.StatType);
        List<(Implant, int)> list = new();
        smallImplants.ForEach(x => list.Add(x));
        ApplyImplantStats(list);
    }

    /// <summary> Добавляет новый гига-имплант </summary>
    /// <param name="gigaImplant"> Добавляемый имплант </param>
    /// <param name="count"> Количество имплантов добавляемого типа </param>
    public void AddGigaImplant(GigaImplant gigaImplant, int count = 1)
    {
        for (int i = 0; i < gigaImplants.Count; i++)
            if (gigaImplants[i].Item1.GetType() == gigaImplant.GetType())
                gigaImplants[i] = (gigaImplants[i].Item1, gigaImplants[i].Item2 + count);
        if (!stats.Exists(x => x.stat == gigaImplant.StatType))
            SetStat(gigaImplant.StatType);
        List<(Implant, int)> list = new();
        gigaImplants.ForEach(x => list.Add(x));
        ApplyImplantStats(list);
    }

    /// <summary> Вычисляет новые значения бонусов, даваемые имплантами </summary>
    /// <param name="implants"> Список имплантов, у которых обновляются бонусы </param>
    private void ApplyImplantStats(List<(Implant implant, int count)> implants)
    {
        stats.ForEach(x => UpdateStat(x.stat, false));
        for (int i = 0; i < implants.Count; i++)
            if (implants[i].implant.hasStat)
                stats[i] = (implants[i].implant.StatType, stats[i].defaultValue,
                    (implants[i].implant.stat * 0.5f * (implants[i].count + 1)) + stats[i].defaultValue);
        stats.ForEach(x => UpdateStat(x.stat, true));
    }

    private void SetStat(Implant.Stat stat)
    {
        switch (stat)
        {
            case Implant.Stat.Health:
                // Логика изменения параметра здоровья игрока
                break;
            case Implant.Stat.AttackDamage:
                //stats.Add((stat, primaryWeapon.damage, primaryWeapon.damage));
                break;
            case Implant.Stat.MeleeDamage:
                //
                break;
            case Implant.Stat.BulletsDamage:
                //
                break;
            case Implant.Stat.RunSpeed:
                //float f = GetComponent<PlayerController>().movementSpeed;
                //stats.Add((stat, f, f));
                break;
            case Implant.Stat.JumpHeight:
                //float f = GetComponent<PlayerController>().jumpHeight;
                //stats.Add((stat, f, f));
                break;
            case Implant.Stat.DashSpeed:
                //float f = GetComponent<PlayerController>().dashSpeed;
                //stats.Add((stat, f, f));
                break;
            default:
                break;
        }
    }

    /// <summary> Обновляет значение параметра </summary>
    /// <param name="stat"> Изменяемый параметр </param>
    /// <param name="changingFromDefaultToChanged"> Обновление параметра на значение по умолчанию или на изменённое значение </param>
    private void UpdateStat(Implant.Stat stat, bool changingFromDefaultToChanged)
    {
        float f = changingFromDefaultToChanged ? stats.Find(x => x.stat == stat).defaultValue : stats.Find(x => x.stat == stat).currentValue;
        switch (stat)
        {
            case Implant.Stat.Health:
                // Логика изменения параметра здоровья игрока
                break;
            case Implant.Stat.AttackDamage:
                primaryWeapon.damage = f;
                secondaryWeapon.damage = f;
                break;
            case Implant.Stat.MeleeDamage:
                //if (primaryWeapon.type == GunInfo.WeaponType.Melee)
                //    primaryWeapon.damage = f;
                //if (secondaryWeapon.type == GunInfo.WeaponType.Melee)
                //    secondaryWeapon.damage = f;
                break;
            case Implant.Stat.BulletsDamage:
                //if (primaryWeapon.type == GunInfo.WeaponType.Firearm)
                //    primaryWeapon.damage = f;
                //if (secondaryWeapon.type == GunInfo.WeaponType.Firearm)
                //    secondaryWeapon.damage = f;
                break;
            case Implant.Stat.RunSpeed:
                //GetComponent<PlayerController>().movementSpeed = d;
                break;
            case Implant.Stat.JumpHeight:
                //GetComponent<PlayerController>().jumpHeight = d;
                break;
            case Implant.Stat.DashSpeed:
                //GetComponent<PlayerController>().dashSpeed = d;
                break;
            default:
                break;
        }
    }
}
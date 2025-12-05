using UnityEngine;

public class Katana : BaseWeapon
{
    private float attackCooldown = 1f; // 1 удар в секунду

    protected override void Start()
    {
        base.Start();
        gunInfo.damage = 20f;
    }

    public override bool TryToFire()
    {
        //TODO Нужен фикс
        /*if (isAttacking || Time.time - lastFireTime < attackCooldown)
            return false;

        lastFireTime = Time.time;
        isAttacking = true;
        InvokeWeaponAttack();
        PlaySound(gunInfo.fireSound);

        // Логика ближнего боя
        PerformMeleeAttack();

        isAttacking = false;*/
        return true;
    }

    private void PerformMeleeAttack()
    {
        // Реализация ближнего боя (например, проверка столкновений с врагами)
        Debug.Log("Katana attack performed!");
    }
}
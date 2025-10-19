/// <summary> Представляет параметр здоровья </summary>
public class Health
{
    Int_Stat health;

    public Health()
    {
        health = new Int_Stat(100, 100, 0);
    }

    #region INFO
    /// <summary> Возвращает информацию о здоровье в отформатированном виде </summary>
    /// <returns> Отформатированные данные о здоровье </returns>
    public string GetInfo() =>
        $"{health.GetInfo()}";

    /// <summary> Выводит в консоль отформатированные данные о здоровье </summary>
    public void LogInfo() =>
        UnityEngine.Debug.Log(GetInfo());
    #endregion

    #region HEALTH
    /// <summary> Увеличивает текущее здоровье на велинину </summary>
    /// <param name="healAmount"> Величина лечения </param>
    public void Heal(uint healAmount) =>
        health.ChangeStat((int)healAmount);

    /// <summary> Уменьшает текущее здоровье на велинину </summary>
    /// <param name="damageAmount"> Величина нанесённого урона </param>
    public void TakeDamage(uint damageAmount) =>
        health.ChangeStat(-(int)damageAmount);

    /// <summary> Увеличивает текущее здоровье до максимума </summary>
    public void FullHeal() =>
        health.SetStat(health.GetMaxStat());
    #endregion

    #region MAX_HEALTH
    /// <summary> Изменяет максимальное здоровье на величину </summary>
    /// <param name="maxHealthAmount"> Величина изменения максимального здоровья </param>
    public void ChangeMaxHealth(int maxHealthAmount) =>
        health.ChangeMaxStat(maxHealthAmount);

    /// <summary> Устанавливает новое максимальное здоровье </summary>
    /// <param name="maxHealthAmount"> Новое максимальное здоровье </param>
    public void SetMaxHealth(int maxHealthAmount) =>
        health.SetMaxStat(maxHealthAmount);
    #endregion
}
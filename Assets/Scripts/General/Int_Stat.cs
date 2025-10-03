/// <summary> Представляет определённый параметр с максимальным и минимальным значениями </summary>
public class Int_Stat
{
    /// <summary> Максимальное значение параметра </summary>
    protected int maxStat;

    /// <summary> Минимальное значение параметра </summary>
    protected int minStat;

    /// <summary> Текущее значение параметра </summary>
    protected int stat;

    public Int_Stat(int maxStat = 100, int stat = 0, int minStat = 0)
    {
        if (minStat >= maxStat)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MIN_ERROR);
        else
        {
            this.maxStat = maxStat;
            this.stat = stat;
            this.minStat = minStat;
            ClampStat();
        }
    }

    /// <summary> Ограничивает текущее значение параметра его максимальным и минимальным значениями </summary>
    public void ClampStat() =>
        stat = System.Math.Clamp(stat, minStat, maxStat);

    #region INFO
    /// <summary> Возвращает информацию о параметре в отформатированном виде </summary>
    /// <returns> Отформатированные данные о параметре </returns>
    public string GetInfo() =>
        $"{minStat}-->{stat}-->{maxStat}";

    /// <summary> Выводит в консоль отформатированные данные о параметре </summary>
    public void LogInfo() =>
        UnityEngine.Debug.Log(GetInfo());
    #endregion

    #region GETTERS_AND_SETTERS
    /// <summary> Возвращает текущее значение параметра </summary>
    /// <returns> Текущее значение параметра </returns>
    public int GetStat() => stat;

    /// <summary> Возвращает нормированное текущее значение параметра </summary>
    /// <returns> Относительное к максимальному и минимальному значение параметра (между 0 и 1) </returns>
    public float GetNormalizedStat() =>
        (float)stat / (maxStat - minStat);

    /// <summary> Возвращает максимальное значение параметра </summary>
    /// <returns> Максимальное значение параметра </returns>
    public int GetMaxStat() => maxStat;

    /// <summary> Возвращает минимальное значение параметра </summary>
    /// <returns> Минимальное значение параметра </returns>
    public int GetMinStat() => minStat;

    /// <summary> Устанавливает новое значение параметра </summary>
    /// <param name="stat"> Новое текущее значение параметра </param>
    public void SetStat(int stat)
    {
        this.stat = stat;
        ClampStat();
    }

    /// <summary> Устанавливает новое максимальное значение параметра </summary>
    /// <param name="maxStat"> Новое максимальное значение параметра </param>
    public void SetMaxStat(int maxStat)
    {
        if (maxStat <= minStat)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MAX_ERROR);
        else
        {
            this.maxStat = maxStat;
            ClampStat();
        }
    }

    /// <summary> Устанавливает новое минимальноге значение параметра </summary>
    /// <param name="minStat"> Новое минимальное значение параметра </param>
    public void SetMinStat(int minStat)
    {
        if (minStat >= maxStat)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MIN_ERROR);
        else
        {
            this.minStat = minStat;
            ClampStat();
        }
    }
    #endregion

    #region CHANGERS

    #region CHANGERS_ONCE
    /// <summary> Увеличивает текущее значение параметра </summary>
    /// <param name="difference"> Величина изменения текущего значения параметра </param>
    public void ChangeStat(int difference) =>
        ChangeByValue(ref stat, difference);

    /// <summary> Умножает текущее значения параметра </summary>
    /// <param name="factor"> Фактор изменения текущего значения параметра </param>
    public void ChangeStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            ChangeByFactor(ref stat, factor);
    }

    /// <summary> Увеличивает максимальное значение параметра </summary>
    /// <param name="difference"> Величина изменения максимального значения параметра </param>
    public void ChangeMaxStat(int difference)
    {
        if (maxStat + difference <= minStat)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MAX_ERROR);
        else
            ChangeByValue(ref maxStat, difference);
    }

    /// <summary> Умножает максимальное значения параметра </summary>
    /// <param name="factor"> Фактор изменения максимального значения параметра </param>
    public void ChangeMaxStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            ChangeByFactor(ref maxStat, factor);
    }

    /// <summary> Увеличивает минимальное значение параметра </summary>
    /// <param name="difference"> Величина изменения минимального значения параметра </param>
    public void ChangeMinStat(int difference)
    {
        if (minStat + difference >= maxStat)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MIN_ERROR);
        else
            ChangeByValue(ref minStat, difference);
    }

    /// <summary> Умножает минимальное значения параметра </summary>
    /// <param name="factor"> Фактор изменения минимального значения параметра </param>
    public void ChangeMinStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            ChangeByFactor(ref minStat, factor);
    }

    /// <summary> Увеличивает переданный параметр на величину </summary>
    /// <param name="value"> Изменяетмый параметр </param>
    /// <param name="difference"> Величина изменения параметра </param>
    private void ChangeByValue(ref int value, int difference)
    {
        value += difference;
        ClampStat();
    }

    /// <summary> Умножает переданный параметр на фактор </summary>
    /// <param name="value"> Изменяетмый параметр </param>
    /// <param name="factor"> Фактор изменения параметра </param>
    private void ChangeByFactor(ref int value, float factor)
    {
        value = (int)(value * factor);
        ClampStat();
    }
    #endregion
    
    #endregion
}
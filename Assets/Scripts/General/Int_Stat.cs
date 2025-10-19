/// <summary> Представляет определённый параметр с максимальным и минимальным значениями </summary>
public class Int_Stat
{
    private int maxStat;
    /// <summary> Максимальное значение параметра </summary>
    public int MaxStat
    {
        get { return maxStat; }
        set
        {
            if (value <= MinStat)
                Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MAX_ERROR);
            else
            {
                maxStat = value;
                ClampStat();
            }
        }
    }

    private int stat;
    /// <summary> Текущее значение параметра </summary>
    public int Stat
    {
        get { return stat; }
        set
        {
            stat = value;
            ClampStat();
        }
    }

    private int minStat;
    /// <summary> Минимальное значение параметра </summary>
    public int MinStat
    {
        get { return minStat; }
        set
        {
            if (value >= MaxStat)
                Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MIN_ERROR);
            else
            {
                minStat = value;
                ClampStat();
            }
        }
    }

    public Int_Stat(int maxStat = 100, int stat = 0, int minStat = 0)
    {
        this.minStat = minStat;
        MaxStat = maxStat;
        Stat = stat;
    }

    /// <summary> Ограничивает текущее значение параметра его максимальным и минимальным значениями </summary>
    public void ClampStat() =>
        stat = System.Math.Clamp(stat, minStat, maxStat);

    /// <summary> Возвращает нормированное текущее значение параметра </summary>
    /// <returns> Относительное к максимальному и минимальному значение параметра (между 0 и 1) </returns>
    public float GetNormalizedStat() =>
        (float)Stat / (MaxStat - MinStat);

    #region INFO
    /// <summary> Возвращает информацию о параметре в отформатированном виде </summary>
    /// <returns> Отформатированные данные о параметре </returns>
    public string GetInfo() =>
        $"{MinStat}-->{Stat}-->{MaxStat}";

    /// <summary> Выводит в консоль отформатированные данные о параметре </summary>
    public void LogInfo() =>
        UnityEngine.Debug.Log(GetInfo());
    #endregion

    #region CHANGERS
    /// <summary> Умножает текущее значения параметра </summary>
    /// <param name="factor"> Фактор изменения текущего значения параметра </param>
    public void ChangeStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            Stat = (int)(Stat * factor);
    }

    /// <summary> Умножает максимальное значения параметра </summary>
    /// <param name="factor"> Фактор изменения максимального значения параметра </param>
    public void ChangeMaxStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            MaxStat = (int)(MaxStat * factor);
    }

    /// <summary> Умножает минимальное значения параметра </summary>
    /// <param name="factor"> Фактор изменения минимального значения параметра </param>
    public void ChangeMinStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            MinStat = (int)(MinStat * factor);
    }
    #endregion
}
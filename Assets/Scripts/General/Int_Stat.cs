/// <summary> ������������ ����������� �������� � ������������ � ����������� ���������� </summary>
public class Int_Stat
{
    /// <summary> ������������ �������� ��������� </summary>
    protected int maxStat;

    /// <summary> ����������� �������� ��������� </summary>
    protected int minStat;

    /// <summary> ������� �������� ��������� </summary>
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

    /// <summary> ������������ ������� �������� ��������� ��� ������������ � ����������� ���������� </summary>
    public void ClampStat() =>
        stat = System.Math.Clamp(stat, minStat, maxStat);

    #region INFO
    /// <summary> ���������� ���������� � ��������� � ����������������� ���� </summary>
    /// <returns> ����������������� ������ � ��������� </returns>
    public string GetInfo() =>
        $"{minStat}-->{stat}-->{maxStat}";

    /// <summary> ������� � ������� ����������������� ������ � ��������� </summary>
    public void LogInfo() =>
        UnityEngine.Debug.Log(GetInfo());
    #endregion

    #region GETTERS_AND_SETTERS
    /// <summary> ���������� ������� �������� ��������� </summary>
    /// <returns> ������� �������� ��������� </returns>
    public int GetStat() => stat;

    /// <summary> ���������� ������������� ������� �������� ��������� </summary>
    /// <returns> ������������� � ������������� � ������������ �������� ��������� (����� 0 � 1) </returns>
    public float GetNormalizedStat() =>
        (float)stat / (maxStat - minStat);

    /// <summary> ���������� ������������ �������� ��������� </summary>
    /// <returns> ������������ �������� ��������� </returns>
    public int GetMaxStat() => maxStat;

    /// <summary> ���������� ����������� �������� ��������� </summary>
    /// <returns> ����������� �������� ��������� </returns>
    public int GetMinStat() => minStat;

    /// <summary> ������������� ����� �������� ��������� </summary>
    /// <param name="stat"> ����� ������� �������� ��������� </param>
    public void SetStat(int stat)
    {
        this.stat = stat;
        ClampStat();
    }

    /// <summary> ������������� ����� ������������ �������� ��������� </summary>
    /// <param name="maxStat"> ����� ������������ �������� ��������� </param>
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

    /// <summary> ������������� ����� ������������ �������� ��������� </summary>
    /// <param name="minStat"> ����� ����������� �������� ��������� </param>
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
    /// <summary> ����������� ������� �������� ��������� </summary>
    /// <param name="difference"> �������� ��������� �������� �������� ��������� </param>
    public void ChangeStat(int difference) =>
        ChangeByValue(ref stat, difference);

    /// <summary> �������� ������� �������� ��������� </summary>
    /// <param name="factor"> ������ ��������� �������� �������� ��������� </param>
    public void ChangeStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            ChangeByFactor(ref stat, factor);
    }

    /// <summary> ����������� ������������ �������� ��������� </summary>
    /// <param name="difference"> �������� ��������� ������������� �������� ��������� </param>
    public void ChangeMaxStat(int difference)
    {
        if (maxStat + difference <= minStat)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MAX_ERROR);
        else
            ChangeByValue(ref maxStat, difference);
    }

    /// <summary> �������� ������������ �������� ��������� </summary>
    /// <param name="factor"> ������ ��������� ������������� �������� ��������� </param>
    public void ChangeMaxStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            ChangeByFactor(ref maxStat, factor);
    }

    /// <summary> ����������� ����������� �������� ��������� </summary>
    /// <param name="difference"> �������� ��������� ������������ �������� ��������� </param>
    public void ChangeMinStat(int difference)
    {
        if (minStat + difference >= maxStat)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.MIN_ERROR);
        else
            ChangeByValue(ref minStat, difference);
    }

    /// <summary> �������� ����������� �������� ��������� </summary>
    /// <param name="factor"> ������ ��������� ������������ �������� ��������� </param>
    public void ChangeMinStat(float factor)
    {
        if (factor <= 0)
            Error_Handler.ErrorLog(Error_Handler.ErrorCodes.FACTOR_ERROR);
        else
            ChangeByFactor(ref minStat, factor);
    }

    /// <summary> ����������� ���������� �������� �� �������� </summary>
    /// <param name="value"> ����������� �������� </param>
    /// <param name="difference"> �������� ��������� ��������� </param>
    private void ChangeByValue(ref int value, int difference)
    {
        value += difference;
        ClampStat();
    }

    /// <summary> �������� ���������� �������� �� ������ </summary>
    /// <param name="value"> ����������� �������� </param>
    /// <param name="factor"> ������ ��������� ��������� </param>
    private void ChangeByFactor(ref int value, float factor)
    {
        value = (int)(value * factor);
        ClampStat();
    }
    #endregion
    
    #endregion
}
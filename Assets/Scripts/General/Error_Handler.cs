/// <summary> ������������ ����������� ���������� ��� ��������� �� ������� </summary>
static public class Error_Handler
{
    /// <summary> ���� ������ </summary>
    public enum ErrorCodes
    {
        MAX_ERROR,
        MIN_ERROR,
        FACTOR_ERROR
    }

    /// <summary> ������ ��������� �� ������� </summary>
    readonly static string[] Errors = {
        "������������ �������� ������ ��� ����� ������������!",
        "����������� �������� ������ ��� ����� �������������!",
        "������ ������ ���� ������ ����!" };

    /// <summary> ������������ ��� ������ � ������� Unity ��������� �� ������� </summary>
    /// <param name="code"> ��� ������ </param>
    static public void ErrorLog(ErrorCodes code)
        => UnityEngine.Debug.Log(Errors[(int)code]);
}
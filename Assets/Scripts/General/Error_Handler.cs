/// <summary> Представляет минимальный функционал для сообщений об ошибках </summary>
static public class Error_Handler
{
    /// <summary> Коды ошибок </summary>
    public enum ErrorCodes
    {
        MAX_ERROR,
        MIN_ERROR,
        FACTOR_ERROR
    }

    /// <summary> Массив сообщений об ошибках </summary>
    readonly static string[] Errors = {
        "Максимальное значение меньше или равно минимальному!",
        "Минимальное значение больше или равно максимальному!",
        "Фактор должен быть больше нуля!" };

    /// <summary> Используется для вывода в консоль Unity сообщений об ошибках </summary>
    /// <param name="code"> Код ошибки </param>
    static public void ErrorLog(ErrorCodes code)
        => UnityEngine.Debug.Log(Errors[(int)code]);
}
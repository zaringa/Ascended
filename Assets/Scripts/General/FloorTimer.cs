using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorTimer : MonoBehaviour
{
    // Метод для вызова таймера, который задаёт время и вызываемый метод
    public Coroutine TimerStart(float duration, Action callback)
    {
        return StartCoroutine(TimerRoutine(duration, callback));
    }

    // Тело таймера, который по завершении вызывает заданный метод
    private IEnumerator TimerRoutine(float duration, Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
    }
}
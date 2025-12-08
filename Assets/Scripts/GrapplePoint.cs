using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [Header("Grapple Parameters")]
    public float minDistance = 3f;
    public float maxDistance = 25f;

    public float maxViewAngle = 10f;
    public float minHeightDifference = 1f;

    [Header("UI")]
    public GameObject hintUI; // Иконка "Можно притянуться"
}

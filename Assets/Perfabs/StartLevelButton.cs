using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class StartLevelButton : UseableItem
{
    public string targetScene;
    public float DelayTime = 3F;
    [HideInInspector]
    public float timeLeft = 0F;
    public TMP_Text DisplayTimer;
    private bool isTriggered = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isTriggered)
        {
            DisplayTimer.text = timeLeft.ToString();
            timeLeft -= Time.deltaTime;
            if( timeLeft <= 0F)
            {
                isTriggered = false;
                DisplayTimer.text = "Get Ready";
                try
                {
                    SceneManager.LoadScene(targetScene);
                }
                catch
                {
                    Debug.LogError("Конечного уровня не существует в сборке!");
                }
            }
        }
    }
    public override void Execute()
    {
        base.Execute();
        timeLeft = DelayTime;
        isTriggered = true;

    }
}

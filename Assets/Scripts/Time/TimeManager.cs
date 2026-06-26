using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Настройки")]
    [SerializeField] private float realSecondsPerMinute = 0.2f;


    public int currentHour = 6;
    public int currentMinute = 0;
    public int currentDay = 1;

    private float timer;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= realSecondsPerMinute)
        {
            timer = 0;
            AddMinute(1);
        }
    }

    private void AddMinute(int minutes)
    {
        currentMinute += minutes;

        if (currentMinute >= 60)
        {
            currentMinute = 0;
            currentHour++;

            if(currentHour >= 24)
            {
                currentHour = 0;
                currentDay++;
            }
        }
    }

    public string GetTimeString()
    {
        return $"{currentHour:00} : {currentMinute:00}";
    }

    public float GetTimeOfDay()
    {
        return (currentHour + currentMinute / 60f) / 24f;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Image darknessOverlay;
    [SerializeField] private float maxDarkness = 0.8f;
    [SerializeField] private float minDarkness = 0f;

    void Update()
    {
        float timeOfDay = TimeManager.Instance.GetTimeOfDay();
        float darkness = CalculateDarkness(timeOfDay);
        darknessOverlay.color = new Color(0, 0, 0, darkness);
    }

    private float CalculateDarkness(float time)
    {
        if(time < 0.35f)
        {
            return Mathf.Lerp(maxDarkness, minDarkness, time/0.35f);

        }
        else if(time < 0.75f)
        {
            return minDarkness;
        }
        else
        {
            return Mathf.Lerp(minDarkness, maxDarkness, (time - 0.75f));
        }
    }
}

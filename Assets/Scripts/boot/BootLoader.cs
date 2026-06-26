using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    void Start()
    {
        Debug.Log("BootLoader: загружаю House");
        SceneManager.LoadScene("House");
    }
}
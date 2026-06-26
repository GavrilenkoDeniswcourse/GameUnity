using UnityEngine;

public class DoorTeleporter : MonoBehaviour
{
    public string targetScene;
    public string targetSpawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneTransitionManager.Instance.GoToScene(targetScene, targetSpawnPoint);
        }
    }
}
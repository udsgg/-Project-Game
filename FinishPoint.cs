using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (SceneController.instance != null)
            {
                SceneController.instance.NextLevel();
            }
            else
            {
                Debug.LogError("SceneController instance is missing!");
            }
        }
    }
}
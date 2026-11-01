
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

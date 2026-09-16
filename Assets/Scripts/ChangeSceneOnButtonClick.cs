using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnButtonClick : MonoBehaviour
{
    // This function is called when the button is clicked
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

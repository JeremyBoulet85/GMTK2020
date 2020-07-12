using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Toggle toggle = GameObject.FindGameObjectWithTag("mode").GetComponent<Toggle>();
        GameManager.instance.IsHardMode = !toggle.isOn;
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}

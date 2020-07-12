using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().Play("UIclick");
        Toggle toggle = GameObject.FindGameObjectWithTag("mode").GetComponent<Toggle>();
        GameManager.instance.IsHardMode = !toggle.isOn;
        SceneManager.LoadScene("MainScene");
    }

    public void MakeSound()
    {
        FindObjectOfType<AudioManager>().Play("UIclick");
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("UIclick");
        Debug.Log("Quit!");
        Application.Quit();
    }

}

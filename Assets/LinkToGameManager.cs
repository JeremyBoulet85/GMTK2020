using UnityEngine;
using UnityEngine.UI;

public class LinkToGameManager : MonoBehaviour
{
    void Start()
    {
        switch (gameObject.name)
        {
            case "ResumeButton":
                gameObject.GetComponent<Button>().onClick.AddListener(delegate () 
                {
                    FindObjectOfType<AudioManager>().Play("UIclick");
                    GameManager.instance.Resume();
                });
                break;
            case "Menu":
                gameObject.GetComponent<Button>().onClick.AddListener(delegate () 
                {
                    FindObjectOfType<AudioManager>().Play("UIclick");
                    GameManager.instance.LoadMenu(); 
                });
                break;
            case "Quit":
                gameObject.GetComponent<Button>().onClick.AddListener(delegate () 
                {
                    FindObjectOfType<AudioManager>().Play("UIclick");
                    GameManager.instance.QuitGame();
                });
                break;
        }
    }
}

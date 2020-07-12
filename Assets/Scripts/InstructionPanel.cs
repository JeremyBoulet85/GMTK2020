using UnityEngine;

public class InstructionPanel : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            gameObject.SetActive(false);
            GameManager.instance.StartGame();
            // play a little boop sound
        }
    }
}

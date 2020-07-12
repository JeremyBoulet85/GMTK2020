using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeHud : MonoBehaviour
{

    private int time;
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        time = GameManager.instance.IsHardMode ? (int)GameManager.instance.HardModeTimer : 999;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.IsHardMode) 
        {
            time = (int)GameManager.instance.HardModeTimer;
            UpdateText();
        }
    }

    void UpdateText() 
    {
        text = gameObject.GetComponent<Text>();
        text.text = $"{time}";
    }
}

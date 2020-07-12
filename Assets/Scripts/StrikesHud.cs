using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrikesHud : MonoBehaviour
{
    private int totalStrikes;
    private int strikesCount;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        totalStrikes = GameManager.instance.TotalStrikes;
        strikesCount = GameManager.instance.StrikeCount;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.StrikeCount != strikesCount)
        {
            strikesCount = GameManager.instance.StrikeCount;
            UpdateText();
        }
    }

    void UpdateText()
    {
        text = gameObject.GetComponent<Text>();
        text.text = $"{strikesCount}/{totalStrikes}";
    }
}

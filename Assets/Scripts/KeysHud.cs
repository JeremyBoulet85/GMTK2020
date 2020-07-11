using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeysHud : MonoBehaviour
{
    private int totalKeys;
    private int currentKeys;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        totalKeys = GameManager.instance.TotalKeys;
        currentKeys = GameManager.instance.CurrentKeys;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.CurrentKeys != currentKeys)
        {
            currentKeys = GameManager.instance.CurrentKeys;
            UpdateText();
        }
    }

    void UpdateText()
    {
        text = gameObject.GetComponent<Text>();
        text.text = $"{currentKeys}/{totalKeys}";
    }
}

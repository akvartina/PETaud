using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    public TextMeshProUGUI screenText;
    void Start()
    {
        screenText.text = "Hi";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

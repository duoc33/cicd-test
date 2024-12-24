using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TMPro.TextMeshProUGUI text = GetComponent<TMPro.TextMeshProUGUI>();
        text.text = "Hello , World!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

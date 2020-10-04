using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITitle : MonoBehaviour
{
    public Button StartBtn;

    // Start is called before the first frame update
    void Start()
    {
        StartBtn.onClick.AddListener(OnStartClick);
    }

    void OnStartClick()
    {
        GameManager.instance.FirstInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

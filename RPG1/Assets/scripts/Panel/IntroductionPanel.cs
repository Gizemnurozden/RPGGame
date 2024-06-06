using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionPanel : MonoBehaviour
{
    public GameObject introductionPanel; // Paneli tanımlayın

    void Start()
    {
        // Oyun başlarken paneli göster
        introductionPanel.SetActive(true);
    }

    void Update()
    {
        // Kullanıcı herhangi bir tuşa bastığında paneli gizle
        if (Input.anyKeyDown)
        {
            introductionPanel.SetActive(false);
        }
    }
}

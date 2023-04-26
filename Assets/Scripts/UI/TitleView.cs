using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tilteTxt;

    [SerializeField] private GameObject normalBkg;
    [SerializeField] private GameObject falseBkg;
    [SerializeField] private GameObject realBkg;

    public void SetTitle(Title title)
    {
        tilteTxt.text = title.TitleText;
    }

    public void SetNormal()
    {
        normalBkg.SetActive(true);
        falseBkg.SetActive(false);
        realBkg.SetActive(false);
    }

    public void SetFalse()
    {
        normalBkg.SetActive(false);
        falseBkg.SetActive(true);
        realBkg.SetActive(false);
    }

    public void SetReal()
    {
        normalBkg.SetActive(false);
        falseBkg.SetActive(false);
        realBkg.SetActive(true);
    }
}

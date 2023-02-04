using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tilteTxt;
    [SerializeField] private TextMeshProUGUI realTilteText;

    [SerializeField] private GameObject normalBkg;
    [SerializeField] private GameObject falseBkg;
    [SerializeField] private GameObject realBkg;

    public void SetTitle(string title)
    {
        tilteTxt.text = title;
    }

    public void SetNormal()
    {
        normalBkg.SetActive(true);
        falseBkg.SetActive(false);
        realBkg.SetActive(false);

        realTilteText.gameObject.SetActive(false);
    }
    public void SetFalse(List<string> realPlayers)
    {
        normalBkg.SetActive(false);
        falseBkg.SetActive(true);
        realBkg.SetActive(false);

        realTilteText.gameObject.SetActive(true);
        realTilteText.text = "";
        foreach (var p in realPlayers)
            realTilteText.text += p + " ";
    }
    public void SetReal()
    {
        normalBkg.SetActive(false);
        falseBkg.SetActive(false);
        realBkg.SetActive(true);

        realTilteText.gameObject.SetActive(true);
        realTilteText.text = "Titlu real";
    }
}

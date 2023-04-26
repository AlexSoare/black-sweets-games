using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowingTitlesPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private TitleView titlePrefab;
    [SerializeField] private Transform leftTitlesParent;
    [SerializeField] private Transform rightTitlesParent;

    [SerializeField] private Image drawingImg;

    private List<TitleView> currentTitles;

    public void Init(List<Title> titles, Sprite drawing)
    {
        if (currentTitles != null)
        {
            foreach (var t in currentTitles)
                Destroy(t.gameObject);

            currentTitles.Clear();
        }
        else currentTitles = new List<TitleView>();

        drawingImg.sprite = drawing;

        for(int i = 0;i< titles.Count; i++)
        {
            TitleView tempTitle;

            if (i%2==0)
                tempTitle = Instantiate(titlePrefab, leftTitlesParent);
            else
                tempTitle = Instantiate(titlePrefab, rightTitlesParent);

            tempTitle.gameObject.SetActive(true);
            tempTitle.SetTitle(titles[i]);

            currentTitles.Add(tempTitle);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetTimer(string time)
    {
        timerText.text = time;
    }
}

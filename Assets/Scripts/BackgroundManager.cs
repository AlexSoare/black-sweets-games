using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    private static BackgroundManager instance;
    public static BackgroundManager Instance { get { return instance; } }

    [SerializeField] private List<Sprite> spritesPool;
    [SerializeField] private List<Image> gridImages;

    bool forceUpdating = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        instance = this;

        gridImages = new List<Image>();
        var alphas = GetComponentsInChildren<AlphaTween>();
        foreach (var a in alphas)
            gridImages.Add(a.GetComponent<Image>());

        yield return StartCoroutine(UpdateGridRow(0.05f,1f));
        //StartCoroutine(UpdateGridRandom());
        foreach (var i in gridImages)
            StartCoroutine(UpdateGridImage2(i));
    }

    private IEnumerator UpdateGrid()
    {
        foreach (var i in gridImages)
        {
            StartCoroutine(UpdateGridImage(i));
            yield return new WaitForSeconds(0.1f);
        }

        /*while (true)
        {
            var random = Random.Range(0, 2);
            if(random == 0)
            {
                StartCoroutine(UpdateGridRow());
            }
            else
            {
                StartCoroutine(UpdateGridLine());
            }
            yield return new WaitForSeconds(1);
        }*/
    }

    public IEnumerator UpdateGridRow(float step, float alphaTime)
    {
        forceUpdating = true;
        var totalLines = 4;
        var totalRows = 7;

        for (int row = 0; row < totalRows; row++)
        {
            for (int line = 0; line < totalLines; line++)
            {
                var index = line * totalRows + row;
                StartCoroutine(UpdateGridImage(gridImages[index], alphaTime, false, true));
                yield return new WaitForSeconds(step);
            }
        }
        forceUpdating = false;
    }
    private IEnumerator UpdateGridLine()
    {
        var totalLines = 4;
        var totalRows = 7;


        for (int line = 0; line < totalLines; line++)
        {
            for (int row = 0; row < totalRows; row++)
            {
                var index = line * totalRows + row;
                StartCoroutine(UpdateGridImage(gridImages[index]));
                yield return new WaitForSeconds(2f);
            }
        }

    }

    private IEnumerator UpdateGridRandom()
    {
        while (true)
        {
            var randomImg = gridImages[Random.Range(0, gridImages.Count)];
            StartCoroutine(UpdateGridImage(randomImg));

            yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
        }
    }

    private IEnumerator UpdateGridImage(Image img, float tweenDuration = 1, bool entryTween = true, bool exitTween = true)
    {
        if (img.GetComponent<AlphaTween>() == null)
            yield break;

        if (entryTween)
        {
            img.GetComponent<AlphaTween>().duration = tweenDuration;
            img.GetComponent<AlphaTween>().PlayReverse();
            yield return new WaitForSeconds(tweenDuration);
        }

        img.sprite = spritesPool[Random.Range(0, spritesPool.Count)];

        if (exitTween)
        {
            img.GetComponent<AlphaTween>().PlayForward();
        }
    }
    private IEnumerator UpdateGridImage2(Image img)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.2f, 10f));

            while (forceUpdating)
                yield return null;

            img.GetComponent<AlphaTween>().PlayReverse();
            yield return new WaitForSeconds(1f);

            img.sprite = spritesPool[Random.Range(0, spritesPool.Count)];
            img.GetComponent<AlphaTween>().PlayForward();

        }
    }

}

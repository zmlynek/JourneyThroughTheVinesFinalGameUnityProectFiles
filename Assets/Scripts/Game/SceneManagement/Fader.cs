using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fader : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float time)
    {
        yield return image.DOFade(1, time).WaitForCompletion();
    }
    public IEnumerator FadeOut(float time)
    {
        yield return image.DOFade(0, time).WaitForCompletion();
    }
}

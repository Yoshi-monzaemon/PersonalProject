using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_ColorChanger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField,Range(1,100)]private int ChangeEach = 5;

    [Header("Palette")]
    [SerializeField]private Color[] ColorPalette;

    [Header("References")]
    [SerializeField]private Text ScoreText;
    [SerializeField]private Image Background;
        [SerializeField]private AudioClip ChangeColorSound;

    private int Count;
    private int CurrentColorID;
    private Color CurrentColor;
    private AudioSource Source;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        Source = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnBounce()
    {
        Count++;
        if(Count >= ChangeEach)
        {
            ChangeColor();
            Count = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ChangeColor()
    {
        if (ChangeColorSound)
        {
            Source.clip = ChangeColorSound;
            Source.Play();
        }
        StartCoroutine(CrossFadeColor());
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnGameOver()
    {
        Count = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator CrossFadeColor()
    {
        float percent = 0;
        Color c = GetDifferentColor;
        while (percent < 1)
        {
            percent += 0.005f;
            CurrentColor = Color.Lerp(CurrentColor, c, percent);
            ScoreText.color = CurrentColor;
            Background.color = CurrentColor;
            yield return null;
        }
        CurrentColor = c;
    }

    /// <summary>
    /// 
    /// </summary>
    private Color GetDifferentColor
    {
        get
        {
            int r = Random.Range(0, ColorPalette.Length);
            int max = 10;
            while (r == CurrentColorID)
            {
                r = Random.Range(0, ColorPalette.Length);
                max--;
                if (max <= 0) { break; }//avoid loop
            }
            CurrentColorID = r;
            return ColorPalette[r];
        }
    }
}
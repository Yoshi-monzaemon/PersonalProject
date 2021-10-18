using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using PersonalProjectExtension;

public class bl_GameManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(25,500)]public float PlatformSpeed = 75;
    [Range(1,15)]public float BallSpeed = 3.3f;
    [SerializeField,Range(1,50)]private int IncreaseBallSpeedEach = 5;
    [SerializeField,Range(0.1f,20)]private float IncreaseBallSpeedPerHit = 2;

    [Header("References")]
    [SerializeField]private Text ScoreText;
    [SerializeField]private Text BestScoreMenuText;
    [SerializeField]private Text GamePlayedText;
    [SerializeField]private Text GameOverScoreText;
    [SerializeField]private Text BestScoreText;
    [SerializeField]private Image AudioImage;
    [SerializeField]private Sprite AudioEnableIcon;
    [SerializeField]private Sprite AudioDisableIcon;
    [SerializeField]private Animator CircleAnim;
    [SerializeField]private GameObject GameRoot;
    [SerializeField]private Animator GameOverAnim;
    [SerializeField]private Animator MenuAnim;
    [SerializeField]private AudioClip GameOverSound;
    [SerializeField]private Button StartButton;
    [SerializeField]private Button RetryButton;
    [SerializeField]private Button RightButton;
    [SerializeField]private Button LeftButton;
    [SerializeField]private bl_Control platForm;

    private int CurrentScore;
    public static bool GameStarted = false;
    private bl_Ball m_Ball;
    private bl_Control m_Platform;
    private bl_ColorChanger ColorChanger;
    private int GamesPlayed = 0;
    private int BestScore = 0;
    private const string GamesPlayedKey = "lovattostudio.circlepong.gamesplayed";
    private const string BestScoreKey = "lovattostudio.circlepong.bestscore";
    private const string AudioEnableKey = "lovattostudio.circlepong.audio";
    private AudioSource Source;
    private int BounceCount = 0;
    private float defaultBallSpeed;
    private bool audioEnable = true;
    private bl_Ad Ad;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        m_Ball = FindObjectOfType<bl_Ball>();
        m_Platform = FindObjectOfType<bl_Control>();
        ColorChanger = FindObjectOfType<bl_ColorChanger>();
        Ad = FindObjectOfType<bl_Ad>();
        Source = GetComponent<AudioSource>();
        GameRoot.SetActive(false);
        GameOverAnim.gameObject.SetActive(false);
        GameStarted = false;
        GamesPlayed = PlayerPrefs.GetInt(GamesPlayedKey, 0);
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        audioEnable = (PlayerPrefs.GetInt(AudioEnableKey, 1) == 1) ? true : false;
        BestScoreMenuText.text = string.Format("BEST SCORE: {0}", BestScore);
        GamePlayedText.text = string.Format("GAMES PLAYED: {0}", GamesPlayed);
        defaultBallSpeed = BallSpeed;
        AudioListener.volume = (audioEnable) ? 1 : 0;
        AudioImage.sprite = (audioEnable) ? AudioEnableIcon : AudioDisableIcon;
        
        StartButton.OnClickAsObservable()
            .Do(_ => StartGame())
            .Subscribe();

        RetryButton.OnClickAsObservable()
            .Do(_ => TryAgain())
            .Subscribe();

        RightButton.OnPointerDownAsObservable()
            .Do(_ => platForm.SetButton(false))
            .Subscribe();

        RightButton.OnPointerUpAsObservable()
            .Do(_ => platForm.SetButtonUp())
            .Subscribe();

        LeftButton.OnPointerDownAsObservable()
            .Do(_ => platForm.SetButton(true))
            .Subscribe();

        LeftButton.OnPointerUpAsObservable()
            .Do(_ => platForm.SetButtonUp())
            .Subscribe();
    }

    /// <summary>
    /// First time
    /// </summary>
    public void StartGame()
    {
        StartCoroutine(StartWait());
        SetPlayTime();
        ColorChanger.ChangeColor();
    }

    /// <summary>
    /// Try again
    /// </summary>
    public void TryAgain()
    {
        CurrentScore = 0;
        ScoreText.text = CurrentScore.ToString();
        StartCoroutine(TryAgainWait());
        SetPlayTime();
        ColorChanger.ChangeColor();
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnGameOver()
    {
        if (!GameStarted)
            return;

        GameStarted = false;
        BounceCount = 0;
        BallSpeed = defaultBallSpeed;
        GameOverScoreText.text = CurrentScore.ToString();
        BestScoreText.text = string.Format("BEST SCORE: {0}", BestScore);
        GameOverAnim.gameObject.SetActive(true);
        ColorChanger.OnGameOver();
        if (GameOverSound)
        {
            Source.clip = GameOverSound;
            Source.Play();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnBounce()
    {
        CurrentScore++;
        BounceCount++;
        ScoreText.text = CurrentScore.ToString();
        CircleAnim.Play("Point", 0, 0);
        //check for best score.
        if(CurrentScore > BestScore)
        {
            BestScore = CurrentScore;
            PlayerPrefs.SetInt(BestScoreKey, BestScore);
        }
        ColorChanger.OnBounce();
        //Increment the difficult of game
        if(BounceCount >= IncreaseBallSpeedEach)
        {
            BallSpeed += IncreaseBallSpeedPerHit;
            BounceCount = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void SetPlayTime()
    {
        GamesPlayed++;
        PlayerPrefs.SetInt(GamesPlayedKey, GamesPlayed);
        if (isMobile)
        {
            Ad.SetTry();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetAudio()
    {
        audioEnable = !audioEnable;
        AudioListener.volume = (audioEnable) ? 1 : 0;
        AudioImage.sprite = (audioEnable) ? AudioEnableIcon : AudioDisableIcon;
        PlayerPrefs.SetInt(AudioEnableKey, (audioEnable) ? 1 : 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator StartWait()
    {
        MenuAnim.SetTrigger("start");
        yield return new WaitForSeconds(MenuAnim.GetCurrentAnimatorStateInfo(0).length);
        MenuAnim.gameObject.SetActive(false);
        GameRoot.SetActive(true);
        yield return new WaitForSeconds(1);
        GameStarted = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool isMobile
    {
        get
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
            return true;
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator TryAgainWait()
    {
        m_Ball.ResetPosition();
        m_Platform.ResetRotation();
        GameOverAnim.SetTrigger("hide");
        yield return new WaitForSeconds(GameOverAnim.GetCurrentAnimatorStateInfo(0).length);
        GameOverAnim.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        m_Ball.Init();
        GameStarted = true;
    }
}
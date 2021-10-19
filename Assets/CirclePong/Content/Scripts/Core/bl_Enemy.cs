using System.Collections.Generic;
using UnityEngine;

public class bl_Enemy : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(1, 10)] private float DeathRadius = 5;
    [SerializeField] private Vector2 VariantRange = new Vector2(-0.5f, 0.5f);

    [Header("References")]
    [SerializeField] private AudioClip BounceSound;
    [SerializeField] private GameObject BounceParticles;
    [SerializeField] private List<Vector2> enemyPositionList;

    private Transform m_Transform;
    private RectTransform m_RectTransform;
    private Vector2 velocity;
    private bl_GameManager GameManager;
    private AudioSource Source;
    private Vector3 defaultPosition;
    private Animator Anim;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        m_Transform = transform;
        m_RectTransform = GetComponent<RectTransform>();
        GameManager = FindObjectOfType<bl_GameManager>();
        Source = GetComponent<AudioSource>();
        Anim = GetComponent<Animator>();
        resetDefaultPosition();
        Init();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Init()
    {
        ResetPosition();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetPosition()
    {
        resetDefaultPosition();
        //m_Transform.position = defaultPosition;
        m_RectTransform.localPosition = defaultPosition;
        velocity = Vector2.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (Vector2.Distance(m_Transform.parent.position, m_Transform.position) >= DeathRadius)
        {
            GameManager.OnGameOver();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        if (!bl_GameManager.GameStarted)
            return;

        m_Transform.Translate((velocity * Time.fixedDeltaTime));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!bl_GameManager.GameStarted)
            return;

        if (other.tag == "Player")
        {
            OnBounce(other.transform);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnBounce(Transform platform)
    {
        float variant = Random.Range(VariantRange.x, VariantRange.y);
        Vector3 v = new Vector3(0f, variant, 0f);
        Vector2 vector = m_Transform.position - v;
        velocity = -vector.normalized + ((Vector2.Reflect(transform.position - v.normalized, platform.up).normalized * GameManager.BallSpeed / 2));
        velocity += (new Vector2(variant, variant));
        //GameManager.OnBounce();
        Source.clip = BounceSound;
        if (BounceParticles)
        {
            Vector3 position = m_Transform.position;
            position.z -= 1;
            GameObject p = Instantiate(BounceParticles, position, Quaternion.identity) as GameObject;
            Destroy(p, 2);
        }
        Source.Play();
        Anim.Play("hit", 0, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.parent.position, new Vector3(transform.position.x, DeathRadius, transform.position.z));
    }

    void resetDefaultPosition()
    {
        var positionNum = Random.Range(1, enemyPositionList.Count + 1);
        var enemyPosition = enemyPositionList[positionNum];
        defaultPosition = enemyPositionList[positionNum];
    }
}
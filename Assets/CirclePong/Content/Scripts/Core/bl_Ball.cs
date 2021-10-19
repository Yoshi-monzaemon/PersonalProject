using UnityEngine;

public class bl_Ball : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField,Range(1,10)]private float DeathRadius = 5;
    [SerializeField]private Vector2 VariantRange = new Vector2(-0.5f,0.5f);

    [Header("References")]
    [SerializeField]private AudioClip BounceSound;
    [SerializeField]private GameObject BounceParticles;

    private Transform m_Transform;
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
        GameManager = FindObjectOfType<bl_GameManager>();
        Source = GetComponent<AudioSource>();
        Anim = GetComponent<Animator>();
        defaultPosition = m_Transform.position;
        Init();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Init()
    {     
        velocity = (Vector3.down * GameManager.BallSpeed);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetPosition()
    {
        m_Transform.position = defaultPosition;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if(Vector3.Distance(m_Transform.parent.position,m_Transform.position) >= DeathRadius)
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
        else if (other.tag == "Enemy")
        {
            OnBounce(other.transform, false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnBounce(Transform platform, bool isPlayer = true)
    {
        float variant = Random.Range(VariantRange.x, VariantRange.y);
        Vector3 v = new Vector3(0f, variant, 0f);
        Vector2 vector = m_Transform.position - v;
        velocity = -vector.normalized + ((Vector2.Reflect(transform.position - v.normalized, platform.up).normalized * GameManager.BallSpeed));
        velocity += (new Vector2(variant, variant));
        if(isPlayer) GameManager.OnBounce();
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
}
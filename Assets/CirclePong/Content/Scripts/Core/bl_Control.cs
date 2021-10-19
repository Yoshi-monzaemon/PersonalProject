﻿using UnityEngine;
using UnityEngine.UI;

public class bl_Control : MonoBehaviour
{
    [SerializeField] private Image line;
 
    private Transform m_Transform;
    private float horizontal;
    private Vector3 defaultRotation;
    private bl_GameManager GameManager;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        m_Transform = transform;
        GameManager = FindObjectOfType<bl_GameManager>();
        defaultRotation = m_Transform.eulerAngles;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetRotation()
    {
        m_Transform.eulerAngles = defaultRotation;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (!bl_GameManager.GameStarted)
            return;
        if (!GameManager.isMobile)
        {
            KeyboardControl();
        }
        RotatePlatform();
    }

    /// <summary>
    /// 
    /// </summary>
    void KeyboardControl()
    {
        horizontal = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isLeft"></param>
    public void SetButton(bool isLeft)
    {
        horizontal = (isLeft) ? -1 : 1;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetButtonUp()
    {
        horizontal = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    void RotatePlatform()
    {
        m_Transform.Rotate(new Vector3(0, 0, -horizontal) * Time.deltaTime * GameManager.PlatformSpeed, Space.Self);
    }

    public void AdjustImageSize(float circleRate = 0.1f)
    {
        line.fillAmount = circleRate;
    }
}
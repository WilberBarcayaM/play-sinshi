using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour
{
    public static MobileControls Instance { get; private set; }

    [Header("Estado de Botones")]
    public bool leftPressed = false;
    public bool rightPressed = false;
    public bool jumpPressed = false;
    public bool punchPressed = false;
    public bool swordPressed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnLeftButtonDown()
    {
        leftPressed = true;
    }

    public void OnLeftButtonUp()
    {
        leftPressed = false;
    }

    public void OnRightButtonDown()
    {
        rightPressed = true;
    }

    public void OnRightButtonUp()
    {
        rightPressed = false;
    }

    public void OnJumpButtonDown()
    {
        jumpPressed = true;
    }

    public void OnJumpButtonUp()
    {
        jumpPressed = false;
    }

    public void OnPunchButton()
    {
        punchPressed = true;
    }

    public void OnSwordButton()
    {
        swordPressed = true;
    }

    public float GetHorizontalInput()
    {
        if (leftPressed && !rightPressed)
            return -1f;
        if (rightPressed && !leftPressed)
            return 1f;
        return 0f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header(" Elements ")]
    [SerializeField] private CrowdSystem crowdSystem;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header(" Settings ")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float roadWidth;
    private bool canMove;

    [Header(" Control ")]
    [SerializeField] private float slideSpeed;
    private Vector3 clickedScreenPosition;
    private Vector3 clickedPlayerPosition;

    [Header(" Hard Mode Settings ")]
    [SerializeField] private float hardModeSpeedMultiplier = 1.5f;
    private bool isHardMode = false;
    private float originalMoveSpeed;
    private float originalSlideSpeed;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.onGameStateChanged += GameStateChangedCallback;
        
        // Store original speeds
        originalMoveSpeed = moveSpeed;
        originalSlideSpeed = slideSpeed;
        
        // Check if hard mode is enabled
        isHardMode = PlayerPrefs.GetInt("HardMode", 0) == 1;
        
        Debug.Log("PlayerController - Hard Mode: " + isHardMode);
        Debug.Log("PlayerController - Original moveSpeed: " + moveSpeed);
        Debug.Log("PlayerController - Original slideSpeed: " + slideSpeed);
        
        // Apply hard mode speed multiplier if enabled
        if (isHardMode)
        {
            moveSpeed *= hardModeSpeedMultiplier;
            slideSpeed *= hardModeSpeedMultiplier;
            
            Debug.Log("PlayerController - Hard Mode Applied!");
            Debug.Log("PlayerController - New moveSpeed: " + moveSpeed);
            Debug.Log("PlayerController - New slideSpeed: " + slideSpeed);
        }
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            MoveForward();
            ManageControl();
        }
    }

    private void GameStateChangedCallback(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.Game)
            StartMoving();
        else if (gameState == GameManager.GameState.Gameover || gameState == GameManager.GameState.LevelComplete)
            StopMoving();
    }

    private void StartMoving()
    {
        canMove = true;

        playerAnimator.Run();
    }

    private void StopMoving()
    {
        canMove = false;

        playerAnimator.Idle();
    }

    private void MoveForward()
    {
        transform.position += Vector3.forward * Time.deltaTime * moveSpeed;
    }

    private void ManageControl()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clickedScreenPosition = Input.mousePosition;
            clickedPlayerPosition = transform.position;
        }
        else if (Input.GetMouseButton(0))
        {
            float xScreenDifference = Input.mousePosition.x - clickedScreenPosition.x;

            xScreenDifference /= Screen.width;
            xScreenDifference *= slideSpeed;

            Vector3 position = transform.position;
            position.x = clickedPlayerPosition.x + xScreenDifference;

            position.x = Mathf.Clamp(position.x, -roadWidth / 2 + crowdSystem.GetCrowdRadius(),
                roadWidth / 2 - crowdSystem.GetCrowdRadius());

            transform.position = position;

            //transform.position = clickedPlayerPosition + Vector3.right * xScreenDifference;
        }
    }

    public bool IsHardMode()
    {
        return isHardMode;
    }
    
    public void ToggleHardMode()
    {
        isHardMode = !isHardMode;
        
        if (isHardMode)
        {
            // Apply hard mode speeds
            moveSpeed = originalMoveSpeed * hardModeSpeedMultiplier;
            slideSpeed = originalSlideSpeed * hardModeSpeedMultiplier;
            Debug.Log("PlayerController - Hard Mode Enabled! Speed: " + moveSpeed);
        }
        else
        {
            // Restore normal speeds
            moveSpeed = originalMoveSpeed;
            slideSpeed = originalSlideSpeed;
            Debug.Log("PlayerController - Normal Mode Enabled! Speed: " + moveSpeed);
        }
        
        // Save the state
        PlayerPrefs.SetInt("HardMode", isHardMode ? 1 : 0);
    }
}

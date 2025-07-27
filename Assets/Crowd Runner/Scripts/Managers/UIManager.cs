using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header(" Managers ")]
    [SerializeField] private ShopManager shopManager;


    [Header(" Elements ")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject tutPanel;

    [SerializeField] private Slider progressBar;
    [SerializeField] private Text levelText;
    [SerializeField] private Text hardModeText;
    [SerializeField] private Button hardModeButton;

    // Start is called before the first frame update
    void Start()
    {
        progressBar.value = 0;

        gamePanel.SetActive(false);
        gameoverPanel.SetActive(false);
        settingsPanel.SetActive(false);
        HideShop();
        HideTut();

        levelText.text = "Level " + (ChunkManager.instance.GetLevel() + 1);
        
        // Initialize hard mode UI
        UpdateHardModeUI();

        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProgressBar();
    }

    private void GameStateChangedCallback(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.Gameover)
            ShowGameover();
        else if (gameState == GameManager.GameState.LevelComplete)
            ShowLevelComplete();
    }

    public void RetryButtonPressed()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowGameover()
    {
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(true);
    }

    private void ShowLevelComplete()
    {
        gamePanel.SetActive(false);
        levelCompletePanel.SetActive(true);
    }

    public void PlayButtonPressed()
    {
        // Reset hard mode flag for normal mode
        PlayerPrefs.SetInt("HardMode", 0);

        GameManager.instance.SetGameState(GameManager.GameState.Game);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void HardModeButtonPressed()
    {
        Debug.Log("Hard Mode Button Pressed!");
        
        // Toggle hard mode flag
        int currentHardMode = PlayerPrefs.GetInt("HardMode", 0);
        int newHardMode = currentHardMode == 1 ? 0 : 1;
        PlayerPrefs.SetInt("HardMode", newHardMode);
        
        Debug.Log("Hard Mode flag toggled to: " + newHardMode);

        // Update the PlayerController speed immediately
        if (PlayerController.instance != null)
        {
            PlayerController.instance.ToggleHardMode();
        }
        
        // Update UI to show hard mode status
        UpdateHardModeUI();
    }
    
    private void UpdateHardModeUI()
    {
        bool isHardMode = PlayerPrefs.GetInt("HardMode", 0) == 1;
        
        // Update hard mode text if available
        if (hardModeText != null)
        {
            if (isHardMode)
            {
                hardModeText.text = "HARD MODE ON";
                hardModeText.gameObject.SetActive(true);
            }
            else
            {
                hardModeText.text = "NORMAL MODE";
                hardModeText.gameObject.SetActive(false);
            }
        }
        
        // Update button text if available
        if (hardModeButton != null)
        {
            Text buttonText = hardModeButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isHardMode ? "HARD MODE" : "NORMAL MODE";
            }
        }
        
        Debug.Log("Hard Mode UI Updated - Hard Mode: " + isHardMode);
    }

    public void UpdateProgressBar()
    {
        if (!GameManager.instance.IsGameState())
            return;

        float progress = PlayerController.instance.transform.position.z / ChunkManager.instance.GetFinishZ();
        progressBar.value = progress;
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void HideSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void ShowShop()
    {
        shopPanel.SetActive(true);
        shopManager.UpdatePurchaseButton();
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
    }

    public void ShowTut()
    {
        if (tutPanel != null)
            tutPanel.SetActive(true);
    }

    public void HideTut()
    {
        if (tutPanel != null)
            tutPanel.SetActive(false);
    }
}

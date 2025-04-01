using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Lofelt.NiceVibrations;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;

    public UIView startView;
    public UIView winView;
    public UIView loseView;
    public UIView yumView;

    public Button restartButton;
    public Button loseButton;

    public bool won;
    public bool gameStarted;

    public event Action OnGameStarted;
    public event Action OnGameEnded;

    public float levelTimer = 0;
    public float maxLevelTime = 120;
    public TMP_Text timerText;

    // Kill counter for player's kills.
    public int killCount = 0;
    public TMP_Text killCountText;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        
        //Lazy reset of timer etc
        StopAllCoroutines();
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        restartButton.onClick.AddListener(Reset);
        loseButton.onClick.AddListener(Reset);

        // Format text in mm:ss
        timerText.text = TimeSpan.FromSeconds(maxLevelTime).ToString(@"mm\:ss");
        UpdateKillUI();
    }

    void Start()
    {
        ShowStartView();
    }

    public void StartGame()
    {
        gameStarted = true;
        HideStartView();

        OnGameStarted?.Invoke();

        StartTimer();
    }

    void StartTimer()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        if (won || !gameStarted)
            yield break;

        while (levelTimer < maxLevelTime)
        {
            yield return new WaitForSeconds(1f);
            levelTimer += 1f;
            timerText.text = TimeSpan.FromSeconds(maxLevelTime - levelTimer).ToString(@"mm\:ss");
        }

        Lost();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    public void Won()
    {
        ShowWinView();
        OnGameEnded?.Invoke();
        won = true;
    }

    public void Lost()
    {
        ShowLoseView();
        OnGameEnded?.Invoke();
    }

    void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RegisterKill()
    {
        killCount++;
        UpdateKillUI();
        
        yumView.Hide(true);
        yumView.Show();
    }

    void UpdateKillUI()
    {
        if (killCountText != null)
            killCountText.text = killCount.ToString();
    }

    public void ShowStartView()
    {
        startView.Show();
        winView.Hide();
        loseView.Hide();
    }

    public void HideStartView()
    {
        startView.Hide();
    }

    public void ShowLoseView()
    {
        winView.Hide();
        loseView.Show();
    }

    public void ShowWinView()
    {
        winView.Show();
        loseView.Hide();
    }

    public void HapticLight()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
    }
    
    public void HapticHeavy()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
    }
}
using UnityEngine;
using System;
using TMPro;
using System.Collections;
using DG.Tweening;

public class Hole : MonoBehaviour
{
    [SerializeField] private bool isPlayer = false;
    [Space]
    [SerializeField] private int score = 0; // current fullness
    [SerializeField] private int level = 1; // current level
    [SerializeField] private int currentThreshold = 10;
    [SerializeField] private float growthScaleFactor = 1.1f; // scale multiplier on level-up

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text scoreText;
    
    [SerializeField] private SlicedFilledImage scoreFill;
    
    // Material tweening parameters
    [SerializeField] private MeshRenderer levelUpRenderer;
    private Material levelUpMaterial;
    [SerializeField] private float tweenDuration = 0.5f;
    
    public event Action<int> OnScoreChanged;

    private void Awake()
    {
        ScorePoint scorePoint = GetComponentInChildren<ScorePoint>();
        if (scorePoint != null)
            scorePoint.OnItemEaten += AddScore;
        
        if (levelUpRenderer != null)
            levelUpMaterial = levelUpRenderer.material;
        
        UpdateUI();

        if (!isPlayer)
        {
            //Set name of hole to a random name
            nameText.text = "Player " + UnityEngine.Random.Range(2, 9);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        CheckLevelUp();
        OnScoreChanged?.Invoke(score);
        UpdateUI();
    }

    private void CheckLevelUp()
    {
        while (score >= currentThreshold)
        {
            score -= currentThreshold;
            LevelUp();
            currentThreshold += 7;
        }
    }

    private void LevelUp()
    {
        transform.localScale *= growthScaleFactor;
        level++;
        
        if (levelUpMaterial != null)
            TweenTextureOffset(levelUpMaterial);
    }

    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "LVL " + level;
        if (scoreText != null)
            scoreText.text = score + " / " + currentThreshold;
        if (scoreFill != null)
            scoreFill.fillAmount = (float)score / currentThreshold;
    }

    private void TweenTextureOffset(Material mat)
    {
        // Tween the texture's y offset from its current value (assumed 0.5) to 0 and back.
        DOTween.To(() => mat.mainTextureOffset,
                x => mat.mainTextureOffset = x,
                new Vector2(mat.mainTextureOffset.x, 0f),
                tweenDuration)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                // Ensure the final offset is exactly 0.5
                Vector2 offset = mat.mainTextureOffset;
                offset.y = 0.5f;
                mat.mainTextureOffset = offset;
            });
    }
}

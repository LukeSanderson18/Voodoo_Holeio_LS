using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Hole playerHole;

    public SlicedFilledImage levelCompleteFill;
    int totalScore = 0;
    public int scoreToFill = 100;
    public TMP_Text scoreToFillText;
    
    

    private void Awake()
    {
        playerHole.OnScoreChanged += UpdateScore;
        scoreToFillText.text = scoreToFill + " PTS";
    }

    private void UpdateScore(int newScore)
    {
        totalScore += newScore;
        
        float a = (float)totalScore / (float)scoreToFill;
        levelCompleteFill.fillAmount = a;
        
        if(totalScore >= scoreToFill)
        {
            Debug.Log("Level Complete");
            totalScore = 0;
            levelCompleteFill.fillAmount = 1;
            GameManager.Instance.Won();
        }
    }

    private void OnDestroy()
    {
        playerHole.OnScoreChanged -= UpdateScore;
    }
}
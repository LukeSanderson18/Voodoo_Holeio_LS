// ScoreDisplay.cs

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Hole playerHole;  // Assign the player's hole in the inspector
    //public TMP_Text scoreText;

    public SlicedFilledImage levelCompleteFill;
    public int scoreToFill = 100;

    private void Awake()
    {
        playerHole.OnScoreChanged += UpdateScore;
    }

    private void UpdateScore(int newScore)
    {
        float a = (float)newScore / (float)scoreToFill;
        
        Debug.Log(a);
        //scoreText.text = newScore + "PTS";
        levelCompleteFill.fillAmount = a;
    }

    private void OnDestroy()
    {
        playerHole.OnScoreChanged -= UpdateScore;
    }
}
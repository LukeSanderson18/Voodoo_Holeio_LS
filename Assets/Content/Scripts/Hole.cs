using UnityEngine;
using System;
using TMPro;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;

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
    [SerializeField] private float tweenSizeGrowDuration = 0.2f;

    [SerializeField] private float baseSpeed = 5f;
    public float MovementSpeed => baseSpeed * Size;
    public float Size => transform.localScale.x;

    [HideInInspector] public bool gameStarted = false;
    
    public float minX = -20f, maxX = 250f;
    public float minZ = 20f, maxZ = 220f;

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
            // Set name of hole to a random name for AI-controlled holes.
            nameText.text = "Player " + UnityEngine.Random.Range(2, 9);
        }
    }

    public void Start()
    {
        GameManager.Instance.OnGameStarted += HandleGameStarted;
        GameManager.Instance.OnGameEnded += HandleGameEnded;

        if (!isPlayer)
        {
            StartCoroutine(AIMove());
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted -= HandleGameStarted;
            GameManager.Instance.OnGameEnded -= HandleGameEnded;
        }
    }

    private void HandleGameStarted()
    {
        gameStarted = true;
    }

    private void HandleGameEnded()
    {
        gameStarted = false;
    }

    public void AddScore(int points, Transform other = null)
    {
        score += points;
        CheckLevelUp();

        if (isPlayer)
        {
            OnScoreChanged?.Invoke(points);

            UpdateUI();
            
            //Eatscore (the little +1 that pops up when you eat something)
            GameObject eatScore = ObjectPooler.Instance.GetPooledObject("EatScore");
            if (eatScore != null)
            {
                eatScore.transform.position = transform.position;
                TMP_Text text = eatScore.GetComponent<TMP_Text>();
                text.text = "+" + points;
                text.color = Color.white;
                text.DOColor(new Color(1, 1, 1, 0), 0.75f);
                eatScore.transform.DOMoveY(transform.position.y + 1f, 0.75f)
                    .OnComplete(() => eatScore.SetActive(false));

                eatScore.SetActive(true);
            }
            
            if(other != null)
                StarUIManager.Instance.SpawnStar(other.position);
            
            GameManager.Instance.HapticLight();

        }
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
        level++;
        
        Vector3 targetScale = transform.localScale + Vector3.one * growthScaleFactor;
        transform.DOScale(targetScale, tweenSizeGrowDuration).SetEase(Ease.OutQuad);
    
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
        DOTween.To(() => mat.mainTextureOffset,
                x => mat.mainTextureOffset = x,
                new Vector2(mat.mainTextureOffset.x, 0f),
                tweenDuration)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                Vector2 offset = mat.mainTextureOffset;
                offset.y = 0.5f;
                mat.mainTextureOffset = offset;
            });
    }

    IEnumerator AIMove()
    {
        while (true)
        {
            if (gameStarted)
            {
                float randomX = UnityEngine.Random.Range(minX, maxX);
                float randomZ = UnityEngine.Random.Range(minZ, maxZ);
                Vector3 targetPos = new Vector3(randomX, transform.position.y, randomZ);

                while (Vector3.Distance(transform.position, targetPos) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, MovementSpeed * Time.deltaTime);
                    if(gameStarted == false)
                    {
                        break;
                    }
                    yield return null;
                }

                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return null;
            }
        }
    }
    
    // Handle collision between holes.
    private void OnTriggerEnter(Collider other)
    {
        // Only the player's hole processes collision with other holes.
        if (!isPlayer)
            return;
        
        
        
        Hole otherHole = other.GetComponent<Hole>();
        if (otherHole != null && otherHole != this)
        {
            if (this.Size > otherHole.Size)
            {
                // Player is bigger: eat the other hole.
                GameManager.Instance.RegisterKill();
                Destroy(otherHole.gameObject);
                GameManager.Instance.HapticHeavy();
            }
            else if (this.Size < otherHole.Size)
            {
                Destroy(gameObject);
                GameManager.Instance.Lost();
                GameManager.Instance.HapticHeavy();
            }
        }
    }
}

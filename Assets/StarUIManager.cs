// StarUIManager.cs
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StarUIManager : MonoBehaviour
{
    public static StarUIManager Instance { get; private set; }
    public RectTransform targetUI;
    public string starPoolTag = "StarUI"; // Ensure this matches the pooled object's name
    public Canvas canvas;
    public float animationDuration = 0.5f;
    
    private Camera mainCam;

    void Awake()
    {
        Instance = this;

        mainCam = Camera.main;
    }
    public void SpawnStar(Vector3 worldPosition)
    {
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPosition);
        GameObject star = ObjectPooler.Instance.GetPooledObject(starPoolTag);
        if (star != null)
        {
            star.transform.SetParent(canvas.transform, false);
            star.SetActive(true);
            RectTransform starRect = star.GetComponent<RectTransform>();
            starRect.position = screenPos;
            StartCoroutine(AnimateStar(starRect));
        }
    }

    IEnumerator AnimateStar(RectTransform starRect)
    {
        Vector3 startPos = starRect.position;
        Vector3 endPos = targetUI.position;
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            starRect.position = Vector3.Lerp(startPos, endPos, elapsed / animationDuration);
            yield return null;
        }

        targetUI.DOKill();
        targetUI.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        starRect.position = endPos;
        starRect.gameObject.SetActive(false);
    }
}
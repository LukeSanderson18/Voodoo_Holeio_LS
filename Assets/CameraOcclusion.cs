using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraOcclusion : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask occlusionMask;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private string environmentTag = "Environment";
    private string fadePropertyName = "_Fade";

    private Dictionary<Renderer, Material> occluderMaterials = new Dictionary<Renderer, Material>();
    private Dictionary<Renderer, Sequence> activeSequences = new Dictionary<Renderer, Sequence>();
    private Dictionary<Transform, Renderer> transformRendererCache = new Dictionary<Transform, Renderer>();
    private float nextCheckTime = 0f;

    private void Start()
    {
        nextCheckTime = 0f;
        CacheEnvironmentRenderers();
    }

    private void CacheEnvironmentRenderers()
    {
        GameObject[] environmentObjects = GameObject.FindGameObjectsWithTag(environmentTag);
        foreach (GameObject obj in environmentObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null && renderer.material.HasProperty(fadePropertyName))
            {
                transformRendererCache[obj.transform] = renderer;
            }
        }

        Debug.Log($"Cached {transformRendererCache.Count} environment renderers");
    }

    private void OnDisable()
    {
        RestoreAllMaterials();
    }

    private void Update()
    {
        if (Time.time < nextCheckTime)
            return;

        nextCheckTime = Time.time + checkInterval;
        CheckOcclusion();
    }

    private void CheckOcclusion()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;
        direction.Normalize();

        HashSet<Renderer> newOccluders = new HashSet<Renderer>();
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, occlusionMask);

        foreach (RaycastHit hit in hits)
        {
            //get cached
            if (transformRendererCache.TryGetValue(hit.transform, out Renderer renderer))
            {
                newOccluders.Add(renderer);
                ProcessOccluder(renderer, true);
            }
        }

        List<Renderer> renderersToRemove = new List<Renderer>();
        foreach (var occluder in occluderMaterials.Keys)
        {
            if (!newOccluders.Contains(occluder))
            {
                ProcessOccluder(occluder, false);

                if (occluder == null || !occluder.enabled)
                {
                    renderersToRemove.Add(occluder);
                }
            }
        }

        foreach (var renderer in renderersToRemove)
        {
            occluderMaterials.Remove(renderer);
            activeSequences.Remove(renderer);
        }
    }

    private void ProcessOccluder(Renderer renderer, bool isOccluding)
    {
        if (renderer == null || !renderer.enabled)
            return;

        Material material = renderer.material;
        if (!material.HasProperty(fadePropertyName))
            return;

        if (!occluderMaterials.ContainsKey(renderer))
        {
            occluderMaterials[renderer] = new Material(material);
        }

        if (activeSequences.TryGetValue(renderer, out Sequence sequence) && sequence != null)
        {
            sequence.Kill();
            activeSequences.Remove(renderer);
        }

        float targetValue = isOccluding ? 0f : 1f;
        sequence = DOTween.Sequence();
        sequence.Append(material.DOFloat(targetValue, fadePropertyName, fadeDuration));
        sequence.SetEase(Ease.InOutQuad);
        sequence.OnComplete(() =>
        {
            if (sequence != null && activeSequences.ContainsKey(renderer))
            {
                activeSequences.Remove(renderer);
            }
        });

        activeSequences[renderer] = sequence;
    }

    private void RestoreAllMaterials()
    {
        foreach (var sequence in activeSequences.Values)
        {
            if (sequence != null)
                sequence.Kill();
        }
        activeSequences.Clear();

        foreach (var entry in occluderMaterials)
        {
            Renderer renderer = entry.Key;
            if (renderer != null && renderer.enabled && renderer.material.HasProperty(fadePropertyName))
            {
                renderer.material.SetFloat(fadePropertyName, 1f);
            }
        }
        occluderMaterials.Clear();
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
        RestoreAllMaterials();
    }
}
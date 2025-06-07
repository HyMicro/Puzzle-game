using UnityEngine;

public class ParticleEffectManager : MonoBehaviour
{
    public static ParticleEffectManager Instance;

    [Header("Particle Prefabs")]
    public GameObject crystalCollectEffect;
    public GameObject pipeConnectEffect;
    public GameObject piecePlaceEffect;

    void Awake()
    {
        Instance = this;
    }

    public void PlayEffect(GameObject effectPrefab, Vector3 position)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);

            // Auto destroy after particle duration
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effect, 2f);
            }
        }
    }

    public void CrystalCollected(Vector3 position)
    {
        PlayEffect(crystalCollectEffect, position);
    }

    public void PipeConnected(Vector3 position)
    {
        PlayEffect(pipeConnectEffect, position);
    }

    public void PiecePlaced(Vector3 position)
    {
        PlayEffect(piecePlaceEffect, position);
    }
}
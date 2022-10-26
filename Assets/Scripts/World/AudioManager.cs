using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private Transform _audioSourcePrefab;
    [Tooltip ("Default = 0, PlayerDeath = 1, EnemyDeath = 2, AstroidDeath = 3, MiniBossDeath = 4," +
        "BossDeath = 5, Laser = 6, Tripple = 7, ShieldOn = 8, ShieldOff = 9, SpeedBoost = 10, PickUp = 11")]
    [SerializeField] private List<AudioClip> _clipAssets;
    private List<DisableOnComplete> _clipPool;
    private int _poolMax = 10;
    [Space]
    private Type.SFX _sfx;
    private void Awake()
    {
        PopulateClipPool();
        if(Instance)
        {
            Destroy(gameObject);
        }else
        {
            Instance = this;
        }
    }

    private void PopulateClipPool()
    {
        _clipPool = new(10);
        for (int i = 0; i < _poolMax; i++)
        {
            Transform prefab = Instantiate(_audioSourcePrefab, transform);
            _clipPool.Add(prefab.GetComponent<DisableOnComplete>());
        }
    }

    public void PlayAudioOneShot(Type.SFX sfx)
    {
        _sfx = sfx;
        if (_clipAssets.Count > 0)
        {
            for (int i = 0; i < _clipPool.Count; i++)
            {
                if (!_clipPool[i].AudioSource.clip)
                {
                    _clipPool[i].PlayAudioOneShot(AssignAudioClip(sfx));
                    return;
                }
            }
            _clipPool.Add(Instantiate(_audioSourcePrefab, transform).GetComponent<DisableOnComplete>());
            _clipPool.LastOrDefault().PlayAudioOneShot(AssignAudioClip(sfx));
        }
    }
    private AudioClip AssignAudioClip(Type.SFX sfx)
    {
        //Default = 0, PlayerDeath = 1, EnemyDeath = 2, AstroidDeath = 3, MiniBossDeath = 4,
        //BossDeath = 5, Laser = 6, Tripple = 7, ShieldOn = 8, ShieldOff = 9, SpeedBoost = 10, PickUp = 11
        int clip = 0;
        switch (sfx)
        {
            case Type.SFX.Default:
                clip = 0;
                break;
            case Type.SFX.PlayerDeath:
                clip = 1;
                break;
            case Type.SFX.EnemyDeath:
                clip = 2;
                break;
            case Type.SFX.AstroidDeath:
                clip = 3;
                break;
            case Type.SFX.MiniBossDeath:
                clip = 4;
                break;
            case Type.SFX.BossDeath:
                clip = 5;
                break;
            case Type.SFX.Laser:
                clip = 6;
                break;
            case Type.SFX.Tripple:
                clip = 7;
                break;
            case Type.SFX.ShieldOn:
                clip = 8;
                break;
            case Type.SFX.ShieldOff:
                clip = 9;
                break;
            case Type.SFX.SpeedBoost:
                clip = 10;
                break;
            case Type.SFX.PickUp:
                clip = 11;
                break;
        }
        if (clip > _clipAssets.Count - 1) clip = 0;
        return _clipAssets[clip];
    }
}

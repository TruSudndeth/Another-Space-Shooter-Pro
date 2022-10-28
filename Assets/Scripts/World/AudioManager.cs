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
    private Types.SFX _sfx;
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

    public void PlayAudioOneShot(Types.SFX sfx)
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
    private AudioClip AssignAudioClip(Types.SFX sfx)
    {
        //Default = 0, PlayerDeath = 1, EnemyDeath = 2, AstroidDeath = 3, MiniBossDeath = 4,
        //BossDeath = 5, Laser = 6, Tripple = 7, ShieldOn = 8, ShieldOff = 9, SpeedBoost = 10, PickUp = 11
        int clip = 0;
        switch (sfx)
        {
            case Types.SFX.Default:
                clip = 0;
                break;
            case Types.SFX.PlayerDeath:
                clip = 1;
                break;
            case Types.SFX.EnemyDeath:
                clip = 2;
                break;
            case Types.SFX.AstroidDeath:
                clip = 3;
                break;
            case Types.SFX.MiniBossDeath:
                clip = 4;
                break;
            case Types.SFX.BossDeath:
                clip = 5;
                break;
            case Types.SFX.Laser:
                clip = 6;
                break;
            case Types.SFX.Tripple:
                clip = 7;
                break;
            case Types.SFX.ShieldOn:
                clip = 8;
                break;
            case Types.SFX.ShieldOff:
                clip = 9;
                break;
            case Types.SFX.SpeedBoost:
                clip = 10;
                break;
            case Types.SFX.PickUp:
                clip = 11;
                break;
        }
        if (clip > _clipAssets.Count - 1) clip = 0;
        return _clipAssets[clip];
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : DontDestroyHelper<AudioManager>
{
    //Todo: Add a negative sound Effect for negative powerups.
    public delegate void MusicVolume(float musicVolume);
    public static event MusicVolume UpdateMusicVolumeEvent;
    
    [Space]
    [SerializeField] private Transform _audioSourcePrefab;
    
    [Tooltip ("Default = 0, PlayerDeath = 1, EnemyDeath = 2, AstroidDeath = 3, MiniBossDeath = 4," +
        "BossDeath = 5, Laser = 6, Tripple = 7, ShieldOn = 8, ShieldOff = 9, SpeedBoost = 10, PickUp = 11" +
        "LaserDamage01 = 12, LaserDamage02 = 13, LaserDamage03 = 14, LaserDamage04 = 15")]
    [SerializeField] private List<AudioClip> _clipAssets;
    private List<DisableOnComplete> _clipPool;
    private int _poolMax = 10;
    
    [Space]
    private Types.SFX _sfx;
    private float _clipDuplicates = 0.1f;
    protected override void Awake()
    {
        base.Awake();
        
        PopulateClipPool();
    }
    public void PopulateClipPool()
    {
        _clipPool = new(10);
        if(!_audioSourcePrefab)
        {
            Debug.Log("_audioSourcePrefab is null ");
            return;
        }
        for (int i = 0; i < _poolMax; i++)
        {
            Transform prefab = Instantiate(_audioSourcePrefab, transform);
            _clipPool.Add(prefab.GetComponent<DisableOnComplete>());
        }
    }

    public void PlayAudioOneShot(Types.SFX sfx)
    { //Todo: Volume Controll when instance is called
        // if (_clipPool.Any(x => x.gameObject.activeSelf && x.AudioSource.clip == _clipAssets[(int)sfx] && x.AudioSource.time <= _clipDuplicates)) return;
        if (_clipAssets.Count > (int)sfx && _clipPool.Any(x => x.gameObject.activeSelf && x.AudioSource.clip == _clipAssets[(int)sfx] && x.AudioSource.time <= _clipDuplicates))
        {
            return;
        }
        _sfx = sfx;
        if (_clipAssets.Count > 0)
        {
            for (int i = 0; i < _clipPool.Count; i++)
            {
                if (!_clipPool[i].AudioSource.clip) // fix: MissingRefrence on destroy - check for null X3
                {
                    _clipPool[i].PlayAudioOneShot(AssignAudioClip(sfx));
                    return;
                }
            }
            _clipPool.Add(Instantiate(_audioSourcePrefab, transform).GetComponent<DisableOnComplete>());
            _clipPool.LastOrDefault().PlayAudioOneShot(AssignAudioClip(sfx));
        }
    }
    public void UpdateMusicVolume(float musicVolume)
    {
        UpdateMusicVolumeEvent?.Invoke(musicVolume);
    }
    public void UpdateSFXVolume(float soundVolume)
    {
        for (int i = 0; i < _clipPool.Count; i++)
        {
            _clipPool[i].AudioSource.volume = soundVolume;
        }
    }
    private AudioClip AssignAudioClip(Types.SFX sfx)
    {
        //Default = 0, PlayerDeath = 1, EnemyDeath = 2, AstroidDeath = 3, MiniBossDeath = 4,
        //BossDeath = 5, Laser = 6, Tripple = 7, ShieldOn = 8, ShieldOff = 9, SpeedBoost = 10, PickUp = 11
        //Debug: REPLACE SWITCH WITH FACTORY PATTERN
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
            case Types.SFX.LaserDamage01:
                clip = 12;
                break;
            case Types.SFX.LaserDamage02:
                clip = 13;
                break;
            case Types.SFX.LaserDamage03:
                clip = 14;
                break;
            case Types.SFX.LaserDamage04:
                clip = 15;
                break;
            case Types.SFX.LaserDamage05:
                clip = 16;
                break;
            case Types.SFX.EnemyLaser:
                clip = 17;
                break;
            case Types.SFX.MiniBossLaser:
                clip = 18;
                break;
            case Types.SFX.BossLaser:
                clip = 19;
                break;
            case Types.SFX.BombAlert:
                clip = 20;
                break;
            case Types.SFX.ErrorSound:
                clip = 21;
                break;
            case Types.SFX.UI_Hover:
                clip = 22;
                break;
            case Types.SFX.UI_Click:
                clip = 23;
                break;
        }
        if (clip > _clipAssets.Count - 1) clip = 0;
        return _clipAssets[clip];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossColliderParts : MonoBehaviour
{
    public delegate void BossColliderPartsDelegate(BossParts part);
    public static event BossColliderPartsDelegate OnBossColliderParts;

    [SerializeField]
    private List<BossParts> _stage1Parts;
    [SerializeField]
    private List<BossParts> _Stage2Parts;
    [SerializeField]
    private List<BossParts> _Stage3Parts;
    [SerializeField]
    private BossParts _bossColliderParts;
        
    private List<List<BossParts>> _stages;

    private void Awake()
    {
        _stages = new List<List<BossParts>> { _stage1Parts, _Stage2Parts, _Stage3Parts };
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Types.LaserTag.PlayerLaser.ToString()))
        {
            other.gameObject.SetActive(false);
            //Play a laser particle effect
            //??Play a laser sound effect??
            if(NonePartCheck())
            OnBossColliderParts?.Invoke(_bossColliderParts);
        }
    }
    private bool NonePartCheck()
    {
        if (_bossColliderParts == BossParts.None)
        {
            Debug.Log("BossColliderParts.cs: BossParts is set to None", transform);
            return false;
        }
        return true;
    }
}
public enum BossStage
{
    None, Stage1, Stage2, Stage3
}

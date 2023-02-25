using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossColliderParts : MonoBehaviour
{
    public delegate void BossColliderPartsDelegate(BossParts part);
    public static event BossColliderPartsDelegate OnBossColliderParts;

    [SerializeField]
    private BossParts _bossColliderParts;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

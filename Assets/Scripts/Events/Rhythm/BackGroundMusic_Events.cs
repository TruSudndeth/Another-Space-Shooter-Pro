using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic_Events : RhythmListener
{
    public delegate void BGM_Events();
    public static BGM_Events _BGM_events;
    public override void BPMEvent(RhythmEventData data)
    {
        throw new System.NotImplementedException();
    }

    public override void RhythmEvent(RhythmEventData data)
    {
        if(data.layer.layerName == "BaseBeat")
        {
            _BGM_events?.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic_Events : RhythmListener
{
    public delegate void BGMEvents();
    public static BGMEvents BGM_Events;
    public override void BPMEvent(RhythmEventData data)
    {
        throw new System.NotImplementedException();
    }

    public override void RhythmEvent(RhythmEventData data)
    {
        //Note: layerName keeps resetting to default Channel 0_0
        if (data.layer.layerName == "Channel 0_0")
        {
            data.layer.layerName = "BaseBeat";
        }
        if (data.layer.layerName == "BaseBeat")
        {
            BGM_Events?.Invoke();
        }
    }
}

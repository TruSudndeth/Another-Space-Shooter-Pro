using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RhythmListener : MonoBehaviour
{
    /// <summary>
    ///  The rhythm event received
    /// </summary>
    /// <param name="data">The data containing the information about the triggered hit</param>
    public abstract void RhythmEvent(RhythmEventData data);
    /// <summary>
    /// The BPM event, if it was enabled in the Rhythm object
    /// </summary>
    /// <param name="data">The data containing the information about the triggered hit</param>
    public abstract void BPMEvent(RhythmEventData data);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittinListener : RhythmListener
{

    public GameObject lead, lead2, bass;

    Vector3 leadTarget, lead2Target, bassTarget;

    private void Start()
    {
        leadTarget = lead.transform.position;
        lead2Target = lead2.transform.position;
        bassTarget = bass.transform.position;
    }

    public override void BPMEvent(RhythmEventData data)
    {
       
    }

    public override void RhythmEvent(RhythmEventData data)
    {
        switch (data.layer.layerName) {
            case "h1":
                if(data.objects.Count > 0) {
                    int pitch = (int)data.objects[0];
                    leadTarget.y = (pitch - 60) / 20f * 5;
                }
                break;
            case "h6":
                if (data.objects.Count > 0) {
                    int pitch = (int)data.objects[0];
                    bassTarget.y = (pitch - 60) / 20f * 5;
                }
                break;
            case "h7":
                if (data.objects.Count > 0) {
                    int pitch = (int)data.objects[0];
                    lead2Target.x = (pitch - 60) / 20f * 5;
                }
                break;
        }
    }

    private void Update()
    {
        Vector3 v = lead.transform.position;
        v += (leadTarget - v) / 10f;
        lead.transform.position = v;

        v = lead2.transform.position;
        v += (lead2Target - v) / 10f;
        lead2.transform.position = v;

        v = bass.transform.position;
        v += (bassTarget - v) / 10f;
        bass.transform.position = v;
    }
}

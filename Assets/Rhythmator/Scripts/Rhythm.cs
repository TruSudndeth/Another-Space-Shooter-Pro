using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "New Rhythm", menuName = "Rhythmator/Rhythm", order = 1)]
[System.Serializable]
public class Rhythm : ScriptableObject
{

    public AudioClip clip;
    public float BPM = 100;
    public float BPMdelay;
    public bool BPMenabled;

    [HideInInspector]
    [SerializeField]
    public List<MarkupLayer> layers;

    // Editor only
    [HideInInspector]
    public float needlePosition;
    [HideInInspector]
    public float timelineScroll;
    [HideInInspector]
    public float timelineZoom = 1;
    [HideInInspector]
    public bool snapToBPM;

    [System.Serializable]
    public class Markup
    {
        public float timer;
        public float length;
        public List<RhythmEventData.Primitive> additionalParameters;
    }


    [System.Serializable]
    public class MarkupLayer
    {
        public string layerName;
        public List<Markup> markups;
        //Editor specific
        public bool visible = true;
        public Color color;
    }

    public void InitializeLayers()
    {
        if (layers == null) {
            layers = new List<MarkupLayer>();
            layers.Add(new MarkupLayer() { layerName = "Default", markups = new List<Markup>() });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

[RequireComponent(typeof(AudioSource))]
public class Rhythmator : MonoBehaviour
{
    AudioSource audioSource;
    public Rhythm rhythm;

    [HideInInspector]
    public List<Rhythm.Markup> processed;
    [HideInInspector]
    public List<Rhythm.Markup> processedLong;

    public RhythmListener[] listeners;

    // Start is called before the first frame update
    void Awake()
    {
        processed = new List<Rhythm.Markup>();
        processedLong = new List<Rhythm.Markup>();
        if (rhythm != null) {
            rhythm.InitializeLayers();
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = rhythm.clip;
        }
    }


    float oldBeat = 0;
    /// <summary>
    /// 
    /// Returns the time in secods for the next markup to be triggered
    /// 
    /// </summary>
    /// <param name="layerName">The layer name that you want to search for the next Hit</param>
    /// <returns>The time in seconds to the next hit, or -1 if there is no next hit</returns>
    public float TimeForNextHit(string layerName)
    {
        Rhythm.MarkupLayer layer = null;
        foreach(Rhythm.MarkupLayer l in rhythm.layers) {
            if(l.layerName == layerName) {
                layer = l;
                break;
            }
        }
        Rhythm.Markup m = null;
        layer.markups.Sort((a, b) => (int)((a.timer - b.timer)*100f));
        for(int i = 0; i < layer.markups.Count; i++) {
            if(layer.markups[i].timer > audioSource.time) {
                m = layer.markups[i];
                break;
            }
        }
        if(m != null)
        return m.timer - audioSource.time;

        return -1;
    }

    /// <summary>
    /// Returns the time in seconds for which the last markup was triggered
    /// </summary>
    /// <param name="layerName">The layer name that you want to search for the previous Hit</param>
    /// <returns>The time in seconds passed from the last hit triggered, or -1 if it doesnt exist</returns>
    public float TimeFromPreviousHit(string layerName)
    {
        Rhythm.MarkupLayer layer = null;
        foreach (Rhythm.MarkupLayer l in rhythm.layers) {
            if (l.layerName == layerName) {
                layer = l;
                break;
            }
        }
        Rhythm.Markup m = null;
        layer.markups.Sort((a, b) => (int)((a.timer - b.timer) * 100f));
        for (int i = 0; i < layer.markups.Count; i++) {
            if (layer.markups[i].timer > audioSource.time) {
                break;
            }
            else {
                m = layer.markups[i];
            }
        }
        if(m != null)
            return audioSource.time - m.timer;
        return -1;
    }
    float lastPos = 0;
    void Update()
    {

        if (lastPos > audioSource.time) {
            processed.Clear();
            processedLong.Clear();
        }

        //BPM
        if (audioSource.isPlaying && rhythm.BPMenabled) {
            float bps = 60 / rhythm.BPM;
            float delayedTime = Mathf.Max(audioSource.time - rhythm.BPMdelay, 0);
            int beat = (int)Mathf.Ceil(delayedTime / bps);

            if (beat != oldBeat) {
                RhythmEventData data = new RhythmEventData() {
                    layer = null,
                    markup = null,
                    parent = rhythm,
                    type = RhythmEventData.RhythmDataType.BPM,
                    factor = 1
                };
                foreach (RhythmListener listener in listeners) {
                    listener.BPMEvent(data);
                }
            }

            oldBeat = beat;
        }


        //MARCAÇÕES
        if (audioSource.isPlaying) {
            foreach (Rhythm.MarkupLayer layer in rhythm.layers) {
                foreach (Rhythm.Markup markup in layer.markups) {

                    List<object> additionalParameters = new List<object>();
                    for (int i = 0; i < markup.additionalParameters.Count; i++) {
                        additionalParameters.Add(markup.additionalParameters[i].GetValue());
                    }

                    if (markup.length > 0) {
                        //Hold
                        if (audioSource.time >= markup.timer && !processed.Contains(markup)) {
                            RhythmEventData data = new RhythmEventData() {
                                layer = layer,
                                markup = markup,
                                parent = rhythm,
                                type = RhythmEventData.RhythmDataType.Begin,
                                factor = 0,
                                objects = additionalParameters
                            };
                            foreach (RhythmListener listener in listeners) {
                                listener.RhythmEvent(data);
                            }
                            processed.Add(markup);
                        }
                        if (audioSource.time >= markup.timer && audioSource.time < markup.timer + markup.length) {
                            RhythmEventData data = new RhythmEventData() {
                                layer = layer,
                                markup = markup,
                                parent = rhythm,
                                type = RhythmEventData.RhythmDataType.Stay,
                                factor = (audioSource.time - markup.timer) / markup.length,
                                objects = additionalParameters
                            };
                            foreach (RhythmListener listener in listeners) {
                                listener.RhythmEvent(data);
                            }
                        }
                        if (audioSource.time >= markup.timer + markup.length && !processedLong.Contains(markup)) {
                            RhythmEventData data = new RhythmEventData() {
                                layer = layer,
                                markup = markup,
                                parent = rhythm,
                                type = RhythmEventData.RhythmDataType.End,
                                factor = 1,
                                objects = additionalParameters
                            };
                            foreach (RhythmListener listener in listeners) {
                                listener.RhythmEvent(data);
                            }
                            processedLong.Add(markup);
                        }
                    }
                    else {
                        //Hit
                        if (audioSource.time >= markup.timer && !processed.Contains(markup)) {
                            RhythmEventData data = new RhythmEventData() {
                                layer = layer,
                                markup = markup,
                                parent = rhythm,
                                type = RhythmEventData.RhythmDataType.Hit,
                                factor = 1,
                                objects = additionalParameters
                            };
                            foreach (RhythmListener listener in listeners) {
                                listener.RhythmEvent(data);
                            }
                            processed.Add(markup);
                        }
                    }
                }
            }
        }

        lastPos = audioSource.time;
    }
}

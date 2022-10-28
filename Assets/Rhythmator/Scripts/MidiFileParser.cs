#if UNITY_EDITOR

using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MidiFileParser
{
    public static void Parse(Rhythm rhythm, Object midiAsset)
    {
        MidiFile mf = MidiFile.Read(AssetDatabase.GetAssetPath(midiAsset));
        mf.GetTempoMap();
        foreach(FourBitNumber fbn in mf.GetChannels()) {
            Rhythm.MarkupLayer l = GetOrCreateLayerByName(rhythm, "Channel " + fbn);
            l.markups.Clear();

        }
        Dictionary<int, Dictionary<long, List<Note>>> noteMap = new Dictionary<int, Dictionary<long, List<Note>>>() ;
        foreach (Note n in mf.GetNotes()) {
            if (!noteMap.ContainsKey((int)n.Channel)) {
                noteMap.Add((int)n.Channel, new Dictionary<long, List<Note>>());
            }
            Dictionary<long, List<Note>> channelNoteMap = noteMap[(int)n.Channel];
            if (!channelNoteMap.ContainsKey(n.Time)) {
                channelNoteMap.Add(n.Time, new List<Note>());
            }
            List<Note> cat = channelNoteMap[n.Time];
            cat.Add(n);
        }

        foreach (FourBitNumber fbn in mf.GetChannels()) {
            Dictionary<long, List<Note>> channelNoteMap = noteMap[(int)fbn];

            foreach (long tick in channelNoteMap.Keys) {
                List<Note> noteGroup = channelNoteMap[tick];

                int idx = 0;
                foreach (Note n in noteGroup) {
                    Rhythm.MarkupLayer l = GetOrCreateLayerByName(rhythm, "Channel " + fbn + "_" + idx);

                    Rhythm.Markup m = new Rhythm.Markup();
                    float timer = n.TimeAs<MetricTimeSpan>(mf.GetTempoMap()).TotalMicroseconds / 1000000f;
                    float length = n.LengthAs<MetricTimeSpan>(mf.GetTempoMap()).TotalMicroseconds / 1000000f;
                    m.timer = timer;
                    m.length = length;
                    m.additionalParameters = new List<RhythmEventData.Primitive>();
                
                    RhythmEventData.Primitive p = new RhythmEventData.Primitive();
                    p.SetValue((int)n.NoteNumber);
                    m.additionalParameters.Add(p);
                    l.markups.Add(m);

                    idx++;
                }
            }
        }

        
    }

    static Rhythm.MarkupLayer GetOrCreateLayerByName(Rhythm rhythm, string layerName)
    {
        Rhythm.MarkupLayer layer = null;
        foreach(Rhythm.MarkupLayer l in rhythm.layers) {
            if(l.layerName == layerName) {
                layer = l;
                break;
            }
        }
        if(layer == null) {
            layer = new Rhythm.MarkupLayer() { layerName = layerName, markups = new List<Rhythm.Markup>(), visible = true, color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) };
            rhythm.layers.Add(layer);
        }
        return layer;
    }
}
#endif
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AubioNoteDetector
{

    public static Rhythm Detect(Rhythm rhythm, AudioClip clip, bool pitchData)
    {
        try {
            Rhythm.MarkupLayer layer = null;
            for (int i = 0; i < rhythm.layers.Count; i++) {
                if (rhythm.layers[i].layerName == clip.name) {
                    layer = rhythm.layers[i];
                    break;
                }
            }
            if (layer == null) {
                layer = new Rhythm.MarkupLayer() {
                    color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)),
                    layerName = clip.name,
                    markups = new List<Rhythm.Markup>(),
                    visible = true
                };
                rhythm.layers.Add(layer);
            }

            ProcessLayer(layer, AssetDatabase.GetAssetPath(clip), pitchData);
            return rhythm;
        }
        
        catch {
            return null;
        }
    }

    static void ProcessLayer(Rhythm.MarkupLayer layer, string audiopath, bool pitchData)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo(Application.dataPath + "/Rhythmator/Libraries/aubio/bin/"+(pitchData ? "aubionotes" : "aubioonset")+".exe", "\"" + audiopath + "\"") {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };
        Process process = Process.Start(processInfo);

        StreamReader reader = process.StandardOutput;
        string output = reader.ReadToEnd();

        UnityEngine.Debug.Log(output);

        layer.markups.Clear();
        float off = 0;
        foreach (string l in output.Split('\n')) {
            string[] times = l.Split('\t');
            if (pitchData) {
                if (times.Length == 1) {
                    if (off == 0) {
                        try {
                            off = float.Parse(l, CultureInfo.InvariantCulture);
                        }
                        catch {
                            UnityEngine.Debug.Log("Error trying to parse value " + l);
                        }
                        UnityEngine.Debug.Log("First length 1 val: " + off);
                    }
                    try {
                        float t = float.Parse(times[0], CultureInfo.InvariantCulture) - off;
                        Rhythm.Markup r = new Rhythm.Markup() {
                            timer = t,
                            length = 0,
                            additionalParameters = new List<RhythmEventData.Primitive>()
                        };
                        layer.markups.Add(r);
                    }
                    catch {
                        UnityEngine.Debug.Log("Could not parse string " + l);
                    }

                }
                if (times.Length == 3) {
                    try {
                        float t = float.Parse(times[1], CultureInfo.InvariantCulture) - off;
                        int nt = (int)float.Parse(times[0], CultureInfo.InvariantCulture);
                        Rhythm.Markup r = new Rhythm.Markup() {
                            timer = t,
                            length = 0,
                            additionalParameters = new List<RhythmEventData.Primitive>()
                        };
                        r.additionalParameters.Add(new RhythmEventData.Primitive() { type = 0 });
                        r.additionalParameters[0].SetValue(nt);
                        layer.markups.Add(r);
                    }
                    catch {
                        UnityEngine.Debug.Log("Could not parse string " + l);
                    }
                }
            }
            else {
                if (times.Length == 1) {
                    try {
                        float t = float.Parse(l, CultureInfo.InvariantCulture);
                        Rhythm.Markup r = new Rhythm.Markup() {
                            timer = t,
                            length = 0,
                            additionalParameters = new List<RhythmEventData.Primitive>()
                        };
                        layer.markups.Add(r);
                    }
                    catch (Exception e){
                        UnityEngine.Debug.LogError(e);
                        UnityEngine.Debug.Log("Could not parse string " + l);
                    }
                }
            }
        }
    }
}
#endif
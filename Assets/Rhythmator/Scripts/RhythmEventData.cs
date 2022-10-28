using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;

[System.Serializable]
public class RhythmUnityEvent : UnityEvent<RhythmEventData>
{
    public RhythmUnityEvent Clone()
    {
        return JsonUtility.FromJson<RhythmUnityEvent>(JsonUtility.ToJson(this));
    }
}

public class RhythmEventData
{
    public enum RhythmDataType
    {
        Hit,
        Begin,
        Stay,
        End,
        BPM
    }

    [System.Serializable]
    public class Primitive
    {
        public int type;
        [SerializeField]
        int integer;
        [SerializeField]
        float floateger;
        [SerializeField]
        string text;
        [SerializeField]
        bool boolean;
        [SerializeField]
        GameObject gameObject;

        public object GetValue()
        {
            switch (type) {
                case 0: return integer;
                case 1: return floateger;
                case 2: return text;
                case 3: return boolean;
                case 4: return gameObject;
            }
            return null;
        }

        public Primitive Clone()
        {
            return new Primitive() { type = this.type, boolean = this.boolean, floateger = this.floateger, text = this.text, integer = this.integer, gameObject = this.gameObject };
        }

        public static object GetSample(int type)
        {
            if (type == 0) return 0;
            if (type == 1) return 0f;
            if (type == 2) return "";
            if (type == 3) return false;
            return null;
        }

        public void SetType(int type)
        {
            if(this.type != type) {
                this.type = type;
                SetValue(GetSample(this.type));
            }


        }
        public void SetValue(object obj)
        {
            if (type == 0) integer = (int)obj;
            if (type == 1) floateger = (float)obj;
            if (type == 2) text = (string)obj;
            if (type == 3) boolean = (bool)obj;
            if (type == 4) gameObject = (GameObject)obj;
        }
    }

    /// <summary>
    /// The parent Rhythm object that contains this hit
    /// </summary>
    public Rhythm parent;
    /// <summary>
    ///  The layer where the hit resides (or null for BPM Event)
    /// </summary>
    public Rhythm.MarkupLayer layer;
    /// <summary>
    /// The markup object beign triggered
    /// </summary>
    public Rhythm.Markup markup;

    /// <summary>
    /// The type of hit:
    /// Hit: hits that doesnt have length (length = 0)
    /// Begin: A hit that has length > 0 has just started
    /// Stay: Every frame that is inside a hit with length > 0
    /// End: Just exited the hit that has length > 0
    /// BPM: A beat event
    /// </summary>
    public RhythmDataType type;
    /// <summary>
    /// For hits where length > 0, the current progress of the hit, where 0 is the start, and 1 is the end
    /// </summary>
    public float factor;
    /// <summary>
    /// All the aditional parameters specified in the Editor for this current hit
    /// </summary>
    public List<object> objects;
}

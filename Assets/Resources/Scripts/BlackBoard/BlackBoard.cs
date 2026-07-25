using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class BlackBoard : IBlackBoard
{
    [System.Serializable]
    public class Entry
    {
        public string key;
        public EntryType type;
        public string stringVal;
        public int intVal;
        public float floatVal;
        public bool boolVal;
        public Vector3 vector3Val;

        public object GetValue()
        {
            switch (type)
            {
                case EntryType.String: return stringVal;
                case EntryType.Int: return intVal;
                case EntryType.Float: return floatVal;
                case EntryType.Bool: return boolVal;
                case EntryType.Vector3: return vector3Val;
                default: return null;
            }
        }
    }

    public enum EntryType { String, Int, Float, Bool, Vector3, Vector2 }

    [SerializeField]
    public List<Entry> entries = new List<Entry>();

    private Dictionary<string, object> runtimeData;

    private void InitializeIfNeeded()
    {
        if (runtimeData == null)
        {
            runtimeData = new Dictionary<string, object>();
            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.key))
                {
                    runtimeData[entry.key] = entry.GetValue();
                }
            }
        }
    }

    public void Set<T>(string key, T value)
    {
        InitializeIfNeeded();
        runtimeData[key] = value;
#if UNITY_EDITOR
        UpdateEntryInInspector(key, value);
#endif
    }

#if UNITY_EDITOR
    private void UpdateEntryInInspector(string key, object value)
    {
        var entry = entries.Find(x => x.key == key);
        if (entry == null)
        {
            entry = new Entry { key = key };
            entries.Add(entry);
        }

        if (value is int iVal) { entry.type = EntryType.Int; entry.intVal = iVal; }
        else if (value is float fVal) { entry.type = EntryType.Float; entry.floatVal = fVal; }
        else if (value is bool bVal) { entry.type = EntryType.Bool; entry.boolVal = bVal; }
        else if (value is string sVal) { entry.type = EntryType.String; entry.stringVal = sVal; }
        else if (value is Vector3 v3Val) { entry.type = EntryType.Vector3; entry.vector3Val = v3Val; }
    }
#endif

    public T Get<T>(string key)
    {
        InitializeIfNeeded();
        if (runtimeData.TryGetValue(key, out var val))
        {
            if (val is T tVal) return tVal;
            try
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch
            {
                Debug.LogWarning($"BlackBoard: Failed to convert key '{key}' value '{val}' to type {typeof(T)}");
            }
        }
        return default;
    }

    public bool HasKey(string key)
    {
        InitializeIfNeeded();
        return runtimeData.ContainsKey(key);
    }

    public void Remove(string key)
    {
        InitializeIfNeeded();
        runtimeData.Remove(key);
    }

    public void Clear()
    {
        InitializeIfNeeded();
        runtimeData.Clear();
    }
}

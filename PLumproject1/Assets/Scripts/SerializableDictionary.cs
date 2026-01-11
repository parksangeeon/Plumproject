using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity JsonUtility가 Dictionary를 지원하지 않으므로, 
/// 직렬화 가능한 Dictionary를 위한 래퍼 클래스
/// </summary>
[Serializable]
public class SerializableDictionary
{
    [Serializable]
    public class KeyValuePair
    {
        public string key;
        public string value;

        public KeyValuePair(string k, string v)
        {
            key = k;
            value = v;
        }
    }

    public List<KeyValuePair> items = new List<KeyValuePair>();

    public void Add(string key, string value)
    {
        items.Add(new KeyValuePair(key, value));
    }

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.key))
            {
                dict[item.key] = item.value;
            }
        }
        return dict;
    }

    public static SerializableDictionary FromDictionary(Dictionary<string, string> dict)
    {
        SerializableDictionary result = new SerializableDictionary();
        if (dict != null)
        {
            foreach (var kvp in dict)
            {
                result.Add(kvp.Key, kvp.Value);
            }
        }
        return result;
    }
}




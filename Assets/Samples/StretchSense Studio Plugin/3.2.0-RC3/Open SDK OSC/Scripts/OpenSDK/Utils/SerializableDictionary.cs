using System.Collections.Generic;
using System.Linq;

namespace StretchSense
{
    [System.Serializable]
    public class SerializableDictionaryEntry<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
    }

    [System.Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        public List<SerializableDictionaryEntry<TKey, TValue>> entries = new List<SerializableDictionaryEntry<TKey, TValue>>();

        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var entry in entries)
            {
                dictionary.Add(entry.Key, entry.Value);
            }
            return dictionary;
        }

        public static SerializableDictionary<TKey, TValue> FromDictionary(Dictionary<TKey, TValue> dictionary)
        {
            var serializableDictionary = new SerializableDictionary<TKey, TValue>();
            foreach (var kvp in dictionary)
            {
                serializableDictionary.entries.Add(new SerializableDictionaryEntry<TKey, TValue> { Key = kvp.Key, Value = kvp.Value });
            }
            return serializableDictionary;
        }

        public void Set(TKey key, TValue value)
        {
            Dictionary<TKey, TValue> dictionary = ToDictionary();
            dictionary[key] = value;
            entries = FromDictionary(dictionary).entries;
        }

        public bool Find(TKey key, out TValue value)
        {
            Dictionary<TKey, TValue> dictionary = ToDictionary();

            dictionary.TryGetValue(key, out value);

            return (value != null);
        }

        public void Update(TKey key, TValue value)
        {
            Set(key, value);
        }

        public List<TValue> All
        {
            get { return entries.Select(entry => entry.Value).ToList(); }
        }
    }
}



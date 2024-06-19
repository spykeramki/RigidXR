using UnityEditor;

namespace StretchSense
{
    [CustomEditor(typeof(SerializableDictionary<,>))] // Generic editor for any SerializableDictionary
    public class SerializableDictionaryEditor : Editor
    {
        SerializedProperty entries;

        private void OnEnable()
        {
            entries = serializedObject.FindProperty("entries");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(entries, true); // Shows the list of entries

            serializedObject.ApplyModifiedProperties();
        }
    }
}
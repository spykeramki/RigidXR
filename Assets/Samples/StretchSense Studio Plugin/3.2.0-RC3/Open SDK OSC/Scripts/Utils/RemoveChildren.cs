using UnityEngine;

namespace StretchSense
{
    public class RemoveChildren : MonoBehaviour
    {
        public GameObject parentObject;
        public bool removeOnStart = true;

        private void Start()
        {
            if(parentObject == null)
            {
                parentObject = gameObject;
            }

            if(removeOnStart)
            {
                RemoveAllChildren();
            }
        }

        void RemoveAllChildren()
        {
            // Loop backwards as we're removing items
            for (int i = parentObject.transform.childCount - 1; i >= 0; i--)
            {
                // DestroyImmediate is important if you're not in play mode
                DestroyImmediate(parentObject.transform.GetChild(i).gameObject);
            }
        }
    }
}
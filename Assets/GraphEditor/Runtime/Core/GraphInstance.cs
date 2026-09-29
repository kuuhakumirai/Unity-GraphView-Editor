using UnityEngine;

namespace GraphEditor.Core
{
    public class GraphInstance : MonoBehaviour
    {
        public GraphObject GraphObject;
        public GraphCompiler Compiler { get; private set;  }

        void Awake()
        {
            if (GraphObject == null)
            {
                Debug.LogError("There is no GraphObject in the GraphInstance!");
                return;
            }
            Compiler = new(GraphObject);
        }
    }
}
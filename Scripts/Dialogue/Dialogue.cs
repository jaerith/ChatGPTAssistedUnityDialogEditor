using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private readonly Vector2 NewNodeOffset = new Vector2(250, 0);

        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        [SerializeField]
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            OnValidate();
        }
#endif

        private void OnValidate()
        {
            nodeLookup.Clear();
            nodes.ForEach(x => nodeLookup[x.name] = x);
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode() { return nodes[0]; }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (var childId in parentNode.GetChildren())
            {
                yield return (nodeLookup.ContainsKey(childId) ? nodeLookup[childId] : null);
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode parentNode)
        {
            foreach (var childId in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childId) && !nodeLookup[childId].isPlayerSpeaking)
                    yield return nodeLookup[childId];
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode parentNode)
        {
            foreach (var childId in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childId) && nodeLookup[childId].isPlayerSpeaking)
                    yield return nodeLookup[childId];
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count <= 0)
            {
                var newChild = MakeNode(null);
                AddNode(newChild);
            }

            if (!String.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                GetAllNodes().ToList()
                             .Where(x => String.IsNullOrEmpty(AssetDatabase.GetAssetPath(x))).ToList()
                             .ForEach(x => AssetDatabase.AddObjectToAsset(x, this));
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            // NOTE: This method will likely never be implemented, but throwing the NotImplementedException causes an error in the Unity compiler
            // throw new NotImplementedException();
        }

        #region UNITY_EDITOR_REGION
#if UNITY_EDITOR

        private void AddNode(DialogueNode newChild)
        {
            nodes.Add(newChild);

            OnValidate();
        }

        public void CreateNode(DialogueNode parent)
        {
            var newChild = MakeNode(parent);

            Undo.RegisterCreatedObjectUndo(newChild, "Created Dialogue Node");

            Undo.RecordObject(this, "Added Dialogue Node");

            AddNode(newChild);
        }

        public void DeleteNode(DialogueNode targetNode)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");

            nodes.Remove(targetNode);

            OnValidate();

            RemoveDanglingChildren(targetNode);

            Undo.DestroyObjectImmediate(targetNode);
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            var newChild = CreateInstance<DialogueNode>();

            newChild.name = Guid.NewGuid().ToString();

            if (parent != null)
            {
                newChild.SetRectPosition(parent.GetRect().position + NewNodeOffset);

                newChild.isPlayerSpeaking = !parent.isPlayerSpeaking;

                parent.AddChild(newChild.name);
            }

            return newChild;
        }

        private void RemoveDanglingChildren(DialogueNode targetNode)
        {
            GetAllNodes().ToList().ForEach(x => x.RemoveChild(targetNode.name));
        }

#endif
        #endregion
    }
}

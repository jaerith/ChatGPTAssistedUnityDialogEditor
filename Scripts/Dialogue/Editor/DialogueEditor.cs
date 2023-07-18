using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        public const float CANVAS_SIZE_WIDTH  = 4000f;
        public const float CANVAS_SIZE_HEIGHT = 4000f;

        public const float BACKGROUND_DIM_SIZE = 50f;

        Dialogue selectedDialogue = null; 

        [NonSerialized]
        GUIStyle nodeStyle;

        [NonSerialized] 
        GUIStyle nodeChatGPTStyle;

        [NonSerialized]
        GUIStyle playerNodeStyle;

        [NonSerialized]
        DialogueNode draggingNode   = null;

        [NonSerialized]
        Vector2      draggingOffset = Vector2.zero;

        [NonSerialized]
        DialogueNode creatingNode = null;

        [NonSerialized]
        DialogueNode deletingNode = null;

        [NonSerialized]
        DialogueNode linkingParentNode = null;

        Vector2 scrollPosition;

        [NonSerialized]
        bool draggingCanvas = false;

        [NonSerialized]
        Vector2 draggingCanvasOffset;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogueInstance = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

            if (dialogueInstance != null)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor  = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border  = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);

            nodeChatGPTStyle = new GUIStyle();
            nodeChatGPTStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            nodeChatGPTStyle.normal.textColor = Color.white;
            nodeChatGPTStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeChatGPTStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged() 
        {
            Dialogue dialogueInstance = Selection.activeObject as Dialogue;

            if (dialogueInstance != null)
            {
                selectedDialogue = dialogueInstance;

                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(CANVAS_SIZE_WIDTH, CANVAS_SIZE_HEIGHT);
                DrawBackground(canvas);

                foreach (var node in selectedDialogue.GetAllNodes()) 
                {
                    DrawConnections(node);
                }

                foreach (var node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void DrawBackground(Rect canvas)
        {            
            var backgroundTexture = Resources.Load("background") as Texture2D;

            Rect textureCoords =
                new Rect(0, 0, canvas.width / backgroundTexture.width, canvas.height / backgroundTexture.height);

            GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                if (childNode != null)
                {
                    Vector3 endPosition = new Vector3(childNode.GetRect().xMin, childNode.GetRect().center.y, 0);

                    Vector3 controlPointOffset = endPosition - startPosition;
                    controlPointOffset.y = 0;
                    controlPointOffset.x *= 0.8f;

                    Handles.DrawBezier(startPosition,
                                       endPosition,
                                       startPosition + controlPointOffset,
                                       endPosition - controlPointOffset,
                                       Color.white,
                                       null,
                                       4f);
                }
            }
        }

        private void ProcessEvents()
        {
            if ((Event.current.type == EventType.MouseDown) && (draggingNode == null))
            {
                Vector2 relativeMousePosition = Event.current.mousePosition + scrollPosition;

                draggingNode = GetNodeAtPoint(relativeMousePosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;

                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas       = true;
                    draggingCanvasOffset = relativeMousePosition;

                    Selection.activeObject = selectedDialogue;
                }
            }
            else if ((Event.current.type == EventType.MouseDrag) && (draggingNode != null))
            {
                draggingNode.SetRectPosition(Event.current.mousePosition + draggingOffset);

                GUI.changed = true;
            }
            else if ((Event.current.type == EventType.MouseDrag) && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }
            else if ((Event.current.type == EventType.MouseUp) && (draggingNode != null))
            {
                draggingNode   = null;
                draggingOffset = Vector2.zero;
            }
            else if ((Event.current.type == EventType.MouseUp) && draggingCanvas)
            {
                draggingCanvas       = false;
                draggingCanvasOffset = Vector2.zero;
            }
        }

        private void DrawLinkButton(DialogueNode targetNode)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = targetNode;
                }
            }
            else
            {
                if (linkingParentNode.GetChildren().Contains(targetNode.name))
                {
                    if (GUILayout.Button("unlink"))
                    {
                        linkingParentNode.RemoveChild(targetNode.name);
                        linkingParentNode = null;
                    }
                }
                else if (targetNode.name != linkingParentNode.name)
                {
                    if (GUILayout.Button("child"))
                    {
                        linkingParentNode.AddChild(targetNode.name);
                        linkingParentNode = null;
                    }
                }
                else
                {
                    if (GUILayout.Button("cancel"))
                    {
                        linkingParentNode = null;
                    }
                }
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle currentStyle =
                node.isPlayerSpeaking ? playerNodeStyle : (node.isChatGPTSpeaking ? nodeChatGPTStyle : nodeStyle );

            GUILayout.BeginArea(node.GetRect(), currentStyle);

            var oldText = node.text;
            var newText = EditorGUILayout.TextField(oldText);
            node.text   = newText;

            EditorUtility.SetDirty(selectedDialogue);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }

            DrawLinkButton(node);

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();            
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            var absoluteMousePosition = mousePosition;

            return selectedDialogue.GetAllNodes().LastOrDefault(x => x.GetRect().Contains(absoluteMousePosition));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using System;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        private bool _isPlayerSpeaking = false;

        [SerializeField]
        private bool _isChatGPTSpeaking = false;

        [SerializeField]
        private string _text;

        [SerializeField]
        private ChatGPTPromptEmotionEnum _promptEmotionChoice = ChatGPTPromptEmotionEnum.None;

        /**
         ** NOTE: More experimentation will be needed in order to make this work
         **
        [SerializeField]
        private string _customPrompt;
         **/

        [SerializeField]
        private List<string> _children = new List<string>();

        [SerializeField]
        private Rect _rect = new Rect(0,0,200,200);

        [SerializeField]
        private string onEnterAction;

        [SerializeField]
        private string onExitAction;

        public string onEnterActionName
        {
            get { return onEnterAction; }
            // set { onEnterAction = value; }
        }

        public string onExitActionName
        {
            get { return onExitAction; }
            // set { onExitAction = value; }
        }

        public ChatGPTPromptEmotionEnum promptEmotionChoice
        {
            get { return _promptEmotionChoice; }

#if UNITY_EDITOR
            set
            {
                if ((_promptEmotionChoice != value) && isChatGPTSpeaking)
                {
                    Undo.RecordObject(this, "Update Prompt Emotion");

                    _promptEmotionChoice = value;

                    EditorUtility.SetDirty(this);
                }
            }
#endif
        }

        /**
         ** NOTE: More experimentation will be needed in order to make this work
         **
        public string customPrompt
        {
            get { return _customPrompt; }

#if UNITY_EDITOR
            set
            {
                if ((_customPrompt != value) && isChatGPTSpeaking)
                {
                    Undo.RecordObject(this, "Update Prompt Text");

                    _customPrompt = value;

                    EditorUtility.SetDirty(this);
                }
            }
#endif

        }
         **/

        public string text
        { 
            get { return _text; }

            set
            {
                if ((_text != value) && isChatGPTSpeaking)
                {
#if UNITY_EDITOR
                    Undo.RecordObject(this, "Update Dialogue Text");
#endif

                    _text = value;

#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
            }

        }

        public bool isChatGPTSpeaking
        {
            get 
            {
                return (!_isPlayerSpeaking ? _isChatGPTSpeaking : false);
            }

#if UNITY_EDITOR
            set
            {
                if (!_isPlayerSpeaking && (_isChatGPTSpeaking != value))
                {
                    Undo.RecordObject(this, "Update ChatGPT Speaking Flag");
                    _isChatGPTSpeaking = value;

                    EditorUtility.SetDirty(this);
                }
            }
#endif
        }

        public bool isPlayerSpeaking
        {
            get { return _isPlayerSpeaking; }

#if UNITY_EDITOR
            set
            {
                if (_isPlayerSpeaking != value)
                {
                    Undo.RecordObject(this, "Update Player Speaking Flag");
                    _isPlayerSpeaking = value;

                    EditorUtility.SetDirty(this);
                }
            }
#endif
        }

        public List<string> GetChildren()
        {
            return new List<string>(_children);
        }

        /*
        public List<string> children
        {
            get { return _children; }

            set
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                _text = value;
            }
        }
        */

        /*
        public Rect rect 
        { 
            get { return _rect; } 
        }
        */

        public Rect GetRect()
        {
            return _rect;
        }

        public Vector2 GetRectPosition()
        {
            return _rect.position;
        }

        #region UNITY EDITOR SECTION

#if UNITY_EDITOR
        public void AddChild(string childName)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            _children.Add(childName);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childName)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            _children.Remove(childName);
            EditorUtility.SetDirty(this);
        }

        public void SetRectPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            _rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

#endif

        #endregion
    }

}

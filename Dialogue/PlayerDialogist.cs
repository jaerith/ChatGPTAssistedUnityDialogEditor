using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using RPG.Dialogue.ChatGPT;

namespace RPG.Dialogue
{
    public class PlayerDialogist : MonoBehaviour, IUpdateDialogue
    {
        [SerializeField]
        bool debugFlag = false;

        [SerializeField]
        string playerName = "Hiro Protagonist";

        Dialogue     currentDialogue;
        DialogueNode currentNode = null;

        AIDialogist  currentAIDialogist = null;

        bool isChoosing = false;

        public event Action onDialogueUpdated;

        /**
         ** NOTE: Could be useful in some situations
         **
        IEnumerator Start()
        {
            yield return new WaitForSeconds(2);            
        }
         **/

        public bool IsActive()
        {
            return (currentDialogue != null);
        }

        public void StartDialogue(AIDialogist newDialogist, Dialogue selectedDialogue)
        {
            currentAIDialogist = newDialogist;
            currentDialogue    = selectedDialogue;
            currentNode        = currentDialogue?.GetRootNode();

            if (currentAIDialogist != null)
            {
                var chatGPTController = currentAIDialogist.GetComponent<ChatGPTController>();

                chatGPTController?.ResetConversation();
            }

            currentDialogue.GetAllNodes()
                           .Where(x => x.isChatGPTSpeaking)
                           .ToList().ForEach(x => x.text = "Hmmm...");

            AddMessageToChatGPTConversation(currentNode?.text, false);

            TriggerEnterAction();

            onDialogueUpdated();
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();

            currentAIDialogist = null;
            currentNode        = null;
            isChoosing         = false;
            onDialogueUpdated();
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetCurrentSpeaker()
        {
            return IsChoosing() ? playerName : (currentAIDialogist?.CharacterName ?? string.Empty);
        }

        public string GetText()
        {
            if (currentDialogue == null)
            {
                return string.Empty;
            }

            return currentNode.text;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue?.GetPlayerChildren(currentNode) ?? Enumerable.Empty<DialogueNode>();
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;

            AddMessageToChatGPTConversation(currentNode?.text, true);

            TriggerEnterAction();

            isChoosing = false;

            Next();
        }

        public void Next()
        {
            var playerChildren = currentDialogue?.GetPlayerChildren(currentNode);

            if (debugFlag)
            {
                Debug.Log("In Next(), count of player children is [" + playerChildren.Count() + "]");
            }

            isChoosing = playerChildren?.Count() > 0;
            if (isChoosing) 
            {
                TriggerExitAction();

                onDialogueUpdated();

                if (debugFlag)
                {
                    Debug.Log("First player child has text (" + playerChildren.First().text + ")");
                }

                return;
            }

            var aiChildren = currentDialogue?.GetAIChildren(currentNode).ToArray();

            TriggerExitAction();

            currentNode = 
                aiChildren?.Length > 0 ? aiChildren[UnityEngine.Random.Range(0, aiChildren.Length)] : null;

            if (currentNode.isChatGPTSpeaking)
            {
                if (currentNode.promptEmotionChoice != ChatGPTPromptEmotionEnum.None) 
                {
                    AddMessageToChatGPTConversation(currentNode.promptEmotionChoice);
                }
                /**
                 ** NOTE: More experimentation will be needed in order to make this work
                 **
                else if (!String.IsNullOrEmpty(currentNode.customPrompt))
                {
                    Debug.Log("DEBUG: Provided prompt is (" + currentNode.customPrompt + ")");

                    AddMessageToChatGPTConversation(currentNode.customPrompt, false);
                }
                 **/

                GetChatGPTResponse(this);
            }

            AddMessageToChatGPTConversation(currentNode?.text, false);

            TriggerEnterAction();

            onDialogueUpdated();
        }

        public bool HasNext()
        {
            if (debugFlag)
            {
                Debug.Log("Current Node (" + currentNode.text + ") has [" +
                          currentDialogue?.GetAllChildren(currentNode).Count() + "] kids -> .Any results in (" +
                          currentDialogue?.GetAllChildren(currentNode).Any() + ")");
            }

            var children = currentDialogue?.GetAllChildren(currentNode);
            return children?.Any() ?? false;
        }

        public void UpdateDialogueText(string newText)
        {
            if (currentNode != null)
            {
                currentNode.text = newText;
                onDialogueUpdated();
            }
        }

        private void AddMessageToChatGPTConversation(string newMessage, bool isUserRole)
        {
            if (!String.IsNullOrEmpty(newMessage) && (currentAIDialogist != null)) 
            {
                var chatGPTController = currentAIDialogist.GetComponent<ChatGPTController>();

                chatGPTController?.AddMessage(newMessage, isUserRole);
            }
        }

        private void AddMessageToChatGPTConversation(ChatGPTPromptEmotionEnum promptEmotion)
        {
            if ((currentNode.promptEmotionChoice != ChatGPTPromptEmotionEnum.None) && (currentAIDialogist != null))
            {
                var chatGPTController = currentAIDialogist.GetComponent<ChatGPTController>();

                string assembledPrompt = chatGPTController.AssemblePrompt(currentNode.promptEmotionChoice);

                if (debugFlag)
                {
                    Debug.Log("DEBUG: Assembled prompt is (" + assembledPrompt + ")");
                }

                chatGPTController?.AddMessage(assembledPrompt, false);
            }
        }

        private string GetChatGPTResponse(IUpdateDialogue targetNode)
        {
            string newAIResponse = string.Empty;

            if (currentAIDialogist != null)
            {
                var chatGPTController = currentAIDialogist.GetComponent<ChatGPTController>();

                chatGPTController?.GetChatGPTResponse(null, targetNode);
            }

            return newAIResponse;
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                if (debugFlag)
                {
                    Debug.Log("For Node(" + currentNode.text + "), Enter Action Called -> (" + currentNode.onEnterActionName + ")");
                }

                TriggerAction(currentNode.onEnterActionName);
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                if (debugFlag)
                {
                    Debug.Log("For Node(" + currentNode.text + "), Exit Action Called -> (" + currentNode.onExitActionName + ")");
                }

                TriggerAction(currentNode.onExitActionName);
            }
        }

        private void TriggerAction(string actionName)
        {
            if ((currentAIDialogist == null) || String.IsNullOrEmpty(actionName))
            {
                return;
            }

            foreach (var trigger in currentAIDialogist.GetComponents<DialogueTrigger>()) 
            {
                if (trigger != null)
                {
                    trigger.Trigger(actionName);
                }
            }
        }
    }
}

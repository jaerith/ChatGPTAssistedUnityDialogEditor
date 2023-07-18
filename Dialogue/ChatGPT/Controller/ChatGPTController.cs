using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using TMPro;
using RPG.Dialogue;
using UnityEditor;

namespace RPG.Dialogue.ChatGPT
{
    public class ChatGPTController : MonoBehaviour
    {
        private const string RoleUser   = "user";
        private const string RoleSystem = "system";

        private const string EmotionPlaceholder = "{emotion}";

        private ChatGPTPromptEmotionEnum _defaultPromptEmotion = ChatGPTPromptEmotionEnum.None;

        public string defaultPromptMessage { get; protected set; }

        public string defaultPromptMessageSuffix { get; protected set; }

        public ChatGPTPromptEmotionEnum defaultPromptEmotion
        {
            get { return _defaultPromptEmotion; }

            protected set
            {
                _defaultPromptEmotion = value;
            }
        }

        [SerializeField]
        bool debugFlag = false;

        [SerializeField]
        TextAsset jsonConfigFile = null;

        ChatGPTCharacterConfig characterConfig = null;

        ChatGPTSettings          settings     = null;
        List<ChatGPTChatMessage> conversation = null;

        void Start()
        {
            ResetConversation();
        }

        public void AddMessage(string newMessage, bool isUserMessage)
        {
            if (conversation != null)
            {
                conversation.Add(new ChatGPTChatMessage { Role = isUserMessage? RoleUser : RoleSystem, Content = newMessage });
            }
        }

        public string AssemblePrompt(ChatGPTPromptEmotionEnum choiceEmotion)
        {
            ChatGPTPromptEmotionEnum promptEmotionEnum 
                = (choiceEmotion != ChatGPTPromptEmotionEnum.None) ? choiceEmotion : defaultPromptEmotion;

            string emotionValue = Enum.GetName(typeof(ChatGPTPromptEmotionEnum), choiceEmotion);

            StringBuilder prompt = new StringBuilder(defaultPromptMessage.Replace(EmotionPlaceholder, emotionValue));
            prompt.Append(" ").Append(defaultPromptMessageSuffix);

            return prompt.ToString();
        }

        public void GetChatGPTResponse(string newUserMessage = null, IUpdateDialogue updateDialogue = null)
        {
            string newAIResponse = string.Empty;

            if (!string.IsNullOrEmpty(newUserMessage))
            {
                if (debugFlag)
                {
                    Debug.Log("Inside ChatGPTController -> Response message [" + newUserMessage + "] has been provided.");
                }

                conversation.Add(new ChatGPTChatMessage { Role = RoleUser, Content = newUserMessage });
            }

            StartCoroutine(ChatGPTClient.AskAsync(settings, conversation.ToArray(), (response) =>
            {
                try
                {
                    var lastChatGPTResponseCache = response;

                    var responseTimeText = $"Time: {response.ResponseTotalTime} ms";

                    if (debugFlag)
                    {
                        Debug.Log(responseTimeText);
                    }

                    var choices = lastChatGPTResponseCache.Choices;
                    if (choices.Count > 0)
                    {
                        newAIResponse = choices[0].Message?.Content ?? "blank";

                        if (debugFlag)
                        {
                            Debug.Log("Inside ChatGPTController -> The response from ChatGPT is: (" + newAIResponse + ")");
                        }

                        updateDialogue?.UpdateDialogueText(newAIResponse);
                    }

                    conversation.Add(new ChatGPTChatMessage() { Role = "system", Content = newAIResponse });
                }
                catch (System.Exception ex)
                {
                    Debug.Log("ERROR!  Problem with asking ChatGPT -> " + ex.Message);
                }
            }));

            return;
        }

        public void ResetConversation()
        {
            if ((jsonConfigFile != null) && (characterConfig == null))
            {
                ChatGPTCharacterConfig characterConfig = JsonUtility.FromJson<ChatGPTCharacterConfig>(jsonConfigFile.text);

                if ((characterConfig != null) && (characterConfig.SeedMessages.Length > 0))
                {
                    defaultPromptMessage       = characterConfig.DefaultPromptMessage;
                    defaultPromptMessageSuffix = characterConfig.DefaultPromptMessageSuffix;
                    defaultPromptEmotion       = ChatGPTPromptEmotionEnum.None;

                    Enum.TryParse(characterConfig.DefaultPromptEmotion, out _defaultPromptEmotion);

                    if (!string.IsNullOrEmpty(defaultPromptMessage) && 
                        (defaultPromptEmotion != ChatGPTPromptEmotionEnum.None) &&
                        !defaultPromptMessage.Contains(EmotionPlaceholder))
                    {
                        Debug.Log("WARNING!  The emotion placeholder is not in your default prompt message.");
                    }

                    if (!string.IsNullOrEmpty(defaultPromptMessageSuffix) &&
                        !characterConfig.SeedMessages[0].Content.Contains(defaultPromptMessageSuffix))
                    {
                        Debug.Log("WARNING!  The default prompt message suffix is not mentioned in your introductory content message.");
                    }

                    settings = new ChatGPTSettings();
                    settings.apiKey   = characterConfig.ApiKey;
                    settings.apiModel = characterConfig.Model;
                    settings.apiURL   = characterConfig.ApiUrl;

                    settings.debug = debugFlag;
                }

                conversation = new List<ChatGPTChatMessage>();
                foreach (var seedMessage in characterConfig.SeedMessages)
                {
                    conversation.Add(new ChatGPTChatMessage() { Role = seedMessage.Role, Content = seedMessage.Content });
                }
            }
        }
    }
}
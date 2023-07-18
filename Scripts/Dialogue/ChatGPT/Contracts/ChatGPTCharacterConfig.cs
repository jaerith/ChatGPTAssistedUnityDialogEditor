using System;

namespace RPG.Dialogue.ChatGPT
{
    [Serializable]
    public class ChatGPTCharacterConfig
    {
        public string Model;
        public string ApiKey;
        public string ApiUrl;
        public string DefaultPromptMessage;
        public string DefaultPromptMessageSuffix;
        public string DefaultPromptEmotion;

        public ChatGPTCharConfigSeedMessage[] SeedMessages;
    }
}

using Newtonsoft.Json;

namespace RPG.Dialogue.ChatGPT
{
    public class ChatGPTRequest
    {
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public ChatGPTChatMessage[] Messages { get; set; }
    }
}
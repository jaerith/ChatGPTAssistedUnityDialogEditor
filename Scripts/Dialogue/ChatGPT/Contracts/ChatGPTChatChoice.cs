using System;
using UnityEngine;

namespace RPG.Dialogue.ChatGPT
{
    [Serializable]
    public class ChatGPTChatChoice
    {
        [field: SerializeField]
        public int Index { get; set; }

        [field: SerializeField]
        public ChatGPTChatMessage Message { get; set; }

        [field: SerializeField]
        public string FinishReason { get; set; }
    }
}
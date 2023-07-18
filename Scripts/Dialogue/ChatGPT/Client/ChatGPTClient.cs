using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RPG.Dialogue.ChatGPT
{
    public class ChatGPTClient
    {
        static public IEnumerator AskAsync(ChatGPTSettings providedChatGPTSettings, ChatGPTChatMessage[] messages, Action<ChatGPTResponse> callBack)
        {
            var url = providedChatGPTSettings.debug ? $"{providedChatGPTSettings.apiURL}?debug=true" : providedChatGPTSettings.apiURL;

            using(UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                var requestParams = JsonConvert.SerializeObject(new ChatGPTRequest
                {
                    Model    = providedChatGPTSettings.apiModel,
                    Messages = messages
                });

                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestParams);
            
                request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.disposeDownloadHandlerOnDispose    = true;
                request.disposeUploadHandlerOnDispose      = true;
                request.disposeCertificateHandlerOnDispose = true;

                request.SetRequestHeader("Content-Type", "application/json");

                // required to authenticate against OpenAI
                request.SetRequestHeader("Authorization", $"Bearer {providedChatGPTSettings.apiKey}");

                var requestStartDateTime = DateTime.Now;

                yield return request.SendWebRequest();

                if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.Log(request.error);
                }
                else if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("ERROR: " + request.error);
                    Debug.Log("Chat GPT Response text is (" + request.downloadHandler.text);
                }
                else
                {
                    string responseInfo = request.downloadHandler.text;

                    var response = JsonConvert.DeserializeObject<ChatGPTResponse>(responseInfo);

                    response.ResponseTotalTime = (DateTime.Now - requestStartDateTime).TotalMilliseconds;

                    callBack(response);
                }
            }
        }

    }
}

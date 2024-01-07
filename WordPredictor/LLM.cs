using Newtonsoft.Json;
using RestSharp;
using WordPredictor.Models;

namespace WordPredictor
{
    public static class LLM
    {
        static readonly string systemPrompt = "You are a helpufl terminal assistant. Do not answer anything unrelated to terminal. Output code as normal text. Provide short and succinct ansers";
        static readonly string API_URL = "http://localhost:1234/v1";

        public static async Task<string> GetResponse(string prompt = "", Conversation? conversation = null, double temperature = 0.7)
        {
            // Create conversation
            if (conversation == null)
            {
                conversation = new Conversation();
                conversation.AddSystemMessage(systemPrompt);
                conversation.AddUserMessage(prompt);
            }

            // Create client
            var client = new RestClient(API_URL);

            var request = new RestRequest("chat/completions", Method.Post);
            request.AddHeader("Content-Type", "application/json");

            // Create JSON request
            var requestData = new
            {
                model = "local-model",
                messages = conversation.messages,
                temperature = temperature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            // Execute request asynchronously
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content);

                string assistantResponse = jsonResponse.choices[0].message.content;

                return assistantResponse;
            }
            else
            {
                throw new Exception("Request failed");
            }
        }

        public async static Task<bool> IsLLMAvailable()
        {
            try
            {
                await GetResponse("test");
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}

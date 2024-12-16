using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter the unformatted text:");
        string userInput = Console.ReadLine();

        string correctedText = await SendTextToChatGPT(userInput);

        Console.WriteLine("\nCorrected/Formatted Text:");
        Console.WriteLine(correctedText);
        Console.ReadLine();
    }

    static async Task<string> SendTextToChatGPT(string inputText)
    {
        //GIT does not allow you to push secrets to a repository

        string apiKey = "your-secret"; // Replace with your actual API key
        string apiUrl = "https://api.openai.com/v1/chat/completions";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var body = new
            {
                //depending on your openAI license type,
                //your requests will succeed when using a paid subscription
                //or return an insufficient funds message when your free license has run out of monthly credits
                
                model = "gpt-3.5-turbo",    //or gpt-4
                messages = new[]
                {
                    new { role = "system", content = "You are a text formatter. Correct and format the input text properly." },
                    new { role = "user", content = inputText }
                },
                max_tokens = 1000,
                temperature = 0.7
            };

            string jsonBody = System.Text.Json.JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // DEBUGGING: Log the request
            //Console.WriteLine($"Request URL: {apiUrl}");
            //Console.WriteLine($"Request Body: {jsonBody}");

            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseContent);
                return jsonResponse["choices"][0]["message"]["content"].ToString();
            }
            else
            {
                Console.WriteLine($"Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                return "Error communicating with ChatGPT.";
            }
        }
    }
}

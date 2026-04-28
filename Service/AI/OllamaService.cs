using System.Text;
using System.Text.Json;

namespace Service.AI;

public interface IOllamaService
{
    Task<string> GetCompletionAsync(string modelName, string systemPrompt, List<ViewModel.Chat.ChatMessageVM> history, string newMessage);
}

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private const string OllamaEndpoint = "http://localhost:11434/api/chat";

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetCompletionAsync(string modelName, string systemPrompt, List<ViewModel.Chat.ChatMessageVM> history, string newMessage)
    {
        if (string.IsNullOrWhiteSpace(modelName))
        {
            throw new ArgumentException("Model name must be provided from configuration.", nameof(modelName));
        }

        var messages = new List<object>
        {
            new { role = "system", content = systemPrompt }
        };

        foreach (var msg in history)
        {
            messages.Add(new { role = msg.Role, content = msg.Content });
        }

        messages.Add(new { role = "user", content = newMessage });

        var payload = new
        {
            model = modelName,
            messages = messages,
            stream = false
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(OllamaEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            
            return doc.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            // Log exception here if we had a logger
            throw new Exception("Could not communicate with Ollama AI model.", ex);
        }
    }
}

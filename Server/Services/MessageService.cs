using System.Text.Json;
using Server.Interfaces;

namespace Server.Services;
using System.Net.Http;

public class MessageService : IMessageService
{
    private HttpClient client;
    private string _token;

    public MessageService(HttpClient _client, IConfiguration configuration)
    {
        client = _client;
        _token = configuration["TELEGRAM_TOKEN"];
    }

    public async Task<bool> SendMessage(long chatId, string content)
    {

        var payload = new 
        {
            chat_id = chatId,
            text = content,
            parse_mode = "HTML"
        };

        
        var url = $"https://api.telegram.org/bot{_token}/sendMessage";
        var response = await client.PostAsJsonAsync(url, payload);
        
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        
        return false;
        

    }
    public async Task<long> FindChatId(string targetUsername)
    {
        var httpResponse = await client.GetAsync($"https://api.telegram.org/bot{_token}/getUpdates");
        var response = await httpResponse.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(response);
        var updates = jsonDocument.RootElement.GetProperty("result");
        
        foreach (var update in updates.EnumerateArray())
        {
            if (update.TryGetProperty("message", out var message))
            {
                var username = message.GetProperty("from").GetProperty("username").GetString();
        
                if (username == targetUsername)
                {
                    return message.GetProperty("chat").GetProperty("id").GetInt64();
                    
                }
            }
        }

        throw new Exception();
    }
}
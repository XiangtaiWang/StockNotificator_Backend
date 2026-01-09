using System.Text;

namespace Server.Models;

public class TelegramMessageModel(string chatId, string text)
{
    public string chat_id = chatId;
    public string text = text;
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        foreach (var fieldInfo in this.GetType().GetFields().Where(f=> f.IsPublic))
        {
            stringBuilder.Append(fieldInfo.Name);
            stringBuilder.Append('=');
            stringBuilder.Append(this.GetType().GetField(fieldInfo.Name));
        }
        return stringBuilder.ToString();
    }
}
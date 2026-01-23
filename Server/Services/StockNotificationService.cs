using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using Server.Helpers;
using Server.Interfaces;
using DateTime = System.DateTime;

namespace Server.Services;

public class StockNotificationService : IStockNotificationService
{
    private IDataCenterService _dataCenterService;
    private IMessageService _messageService;

    public StockNotificationService(IDataCenterService dataCenterService, IMessageService messageService)
    {
        _messageService = messageService;
        _dataCenterService = dataCenterService;
    }

    public async Task Notify(IEnumerable<string> userList)
    {
        var stockInfo =  _dataCenterService.GetStockInfo();
        var systemUpdateTime = stockInfo.UpdateTime;
        foreach (var user in userList)
        {
            var messageContent = new StringBuilder();
            var notifyStockList = new List<string>();

            messageContent.Append($"系統更新時間: {stockInfo.UpdateTime.ConvertToTaipeiTimeZone().ToString("yyyy/M/d HH:mm:ss")} \n");
            var userLatestNotification = _dataCenterService.GetUserLatestNotification(user);
            var notificationSettings = await _dataCenterService.GetUserNotificationSettings(user);
            foreach (var notificationSetting in notificationSettings.StockNotificationSettings)
            {
                if (!userLatestNotification.LatestNotificationInfos.TryGetValue(notificationSetting.StockCode,
                        out var lastSentTime))
                {
                    lastSentTime = DateTime.UtcNow.AddDays(-1);
                }
                var systemUpdateDateTimeWithoutSeconds = new DateTime(systemUpdateTime.Year, systemUpdateTime.Month, systemUpdateTime.Day, systemUpdateTime.Hour, systemUpdateTime.Minute, 0);
                var lastSentTimeWithoutSeconds = new DateTime(lastSentTime.Year, lastSentTime.Month, lastSentTime.Day, lastSentTime.Hour, lastSentTime.Minute, 0);
                var nextShouldSendDateTimeWithoutSeconds = lastSentTimeWithoutSeconds.AddMinutes(notificationSetting.IntervalMinute);
                if (systemUpdateDateTimeWithoutSeconds>=nextShouldSendDateTimeWithoutSeconds)
                {
                    notifyStockList.Add(notificationSetting.StockCode);
                }
            }

            if (!notifyStockList.IsNullOrEmpty())
            {
                var chatId = await _dataCenterService.GetChatId(user);
                var now = DateTime.UtcNow;
                foreach (var stockCode in notifyStockList)
                {
                    stockInfo.StockDict.TryGetValue(stockCode, out var stock);
                    messageContent.Append($"股票 {stockCode} {stock.Name} 價格為 {stock.Price}\n");
                    
                    userLatestNotification.LatestNotificationInfos[stockCode] = now;
                }
                
                await _messageService.SendMessage(chatId, messageContent.ToString());
                _dataCenterService.UpdateUserLatestNotification(userLatestNotification);

            }
        }
    }
}
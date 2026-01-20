using System.Text.Json;
using Server.Interfaces;
using Server.Models;

namespace Server.Services;

public class DataCenterService:IDataCenterService
{
    // private IRepository _repository;
    private IStockService _stockService;
    private IUserService _userService;
    private ICacheService _cacheService;
    private string stockInfoCacheKey = "StockInfo";
    private IUserRepository _userRepository;

    public DataCenterService(IUserService userService, ICacheService cacheService, IStockService stockService, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _userService = userService;
        _cacheService = cacheService;
        _stockService = stockService;
    }
    public async Task UpdateStockInfoJob()
    {
        var stockList = await _userService.GetStockList();
        var twseStockInfos = await _stockService.FetchStocksInfo(stockList);
        var stockInfo = new StockInfo()
        {
            Stocks = twseStockInfos.Select(s => new Stock()
            {
                Code = s.Code,
                Name = s.Name,
                Price = Convert.ToDecimal(s.Price)
            }),
            UpdateTime = DateTime.UtcNow
        };

        
        _cacheService.Write(stockInfoCacheKey, JsonSerializer.Serialize(stockInfo));

    }

    public StockInfo GetStockInfo()
    {
        var stockInfo = JsonSerializer.Deserialize<StockInfo>(_cacheService.Read(stockInfoCacheKey));
        stockInfo.StockDict = new Dictionary<string, Stock>();

        foreach (var stock in stockInfo.Stocks)
        {
            stockInfo.StockDict.Add(stock.Code, stock);
        }
        return stockInfo;
    }

    public async Task<UserStockNotificationSetting> GetUserNotificationSettings(string user)
    {
        var key = $"UserStockNotificationSetting_{user}";
        if (!_cacheService.Exist(key))
        {
            var userNotificationSettings = await _userRepository.GetUserNotificationSettings(user);
            _cacheService.Write(key, JsonSerializer.Serialize(userNotificationSettings));
        }
        var value = _cacheService.Read(key);
        var userStockNotificationSetting = JsonSerializer.Deserialize<UserStockNotificationSetting>(value);
        if (userStockNotificationSetting==null)
        {
            return new UserStockNotificationSetting()
            {
                StockNotificationSettings = new List<StockNotificationSetting>()
            };
        }
        return userStockNotificationSetting;
        
    }

    public UserLatestNotificationInfo GetUserLatestNotification(string user)
    {
        var value = _cacheService.Read($"UserLatestNotification_{user}");
        if (value==null)
        {
            return new UserLatestNotificationInfo()
            {
                User = user,
                LatestNotificationInfos = new Dictionary<string, DateTime>()
            };
        }
        return JsonSerializer.Deserialize<UserLatestNotificationInfo>(value);
    }

    public async Task<long> GetChatId(string user)
    {
        if (!_cacheService.Exist("UserChatIdDict"))
        {
            var userChatIdMap = await _userRepository.GetUserChatIdDict();
                _cacheService.Write("UserChatIdDict", JsonSerializer.Serialize(userChatIdMap));
        }
        var value = _cacheService.Read("UserChatIdDict");
        var userChatIdDict = JsonSerializer.Deserialize<Dictionary<string, long>>(value);

        return userChatIdDict.GetValueOrDefault(user);
    }

    public void UpdateUserLatestNotification(UserLatestNotificationInfo userLatestNotification)
    {
        _cacheService.Write($"UserLatestNotification_{userLatestNotification.User}", JsonSerializer.Serialize(userLatestNotification));

    }


}
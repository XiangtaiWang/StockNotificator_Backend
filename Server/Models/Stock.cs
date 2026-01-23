namespace Server.Models;

public class Stock
{
    public string Code { get; set; }
    public string Name { get; set; }
    
    //最近成交價
    public string Price { get; set; }
    //最高委買
    public string AskPrice { get; set; }
    //最高委賣
    public string BidPrice { get; set; }
}
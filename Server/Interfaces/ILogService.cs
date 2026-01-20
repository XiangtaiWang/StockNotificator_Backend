using Server.Models;
using Server.Services;

namespace Server.Interfaces;

public interface ILogService
{
    public Task Write(LogModel content);
}
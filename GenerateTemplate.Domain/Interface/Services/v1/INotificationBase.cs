﻿using GenerateTemplate.Domain.Entity;

namespace GenerateTemplate.Domain.Interface.Services.v1;

public interface INotificationBase
{
    IEnumerable<Notification> GetNotifications();
    bool HasNotifications();
    Task NotifyAsync(string key, string message);
    Task NotifyAsync(string key, string[] message);
    Task NotifyAsync(string key, List<string> message);
}

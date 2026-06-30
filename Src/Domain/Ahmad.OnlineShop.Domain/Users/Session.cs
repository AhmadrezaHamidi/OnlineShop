using AhmadBase.Doamin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.User;

public class Session : TEntity<long>
{
    public long UserId { get; private set; }
    public Guid SessionId { get; private set; }
    public bool IsActive { get; private set; }

    private Session() { }

    private Session(long userId, Guid sessionId)
    {
        Id = DateTime.UtcNow.Ticks; // بدون دسترسی به DB در Domain — شناسه یکتای زمان‌محور
        UserId = userId;
        SessionId = sessionId;
        IsActive = true;
    }

    public static Session New(long userId, Guid sessionId)
    {
        return new Session(userId, sessionId);
    }

    public void Deactivate()
    {
        IsActive = false;
        ModificationTime = DateTimeOffset.UtcNow;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DateTimeShared
{
  public interface IDateTimeProvider : niwrA.CommandManager.IDateTimeProvider
  {
    DateTime GetUtcDateTime();
    DateTime GetSessionUtcDateTime();
  }

  public class DateTimeProvider : IDateTimeProvider
  {
    private DateTime? _sessionDateTime;
    /// <summary>
    /// Will return the current UTC datetime
    /// </summary>
    /// <returns></returns>
    public DateTime GetUtcDateTime()
    {
      return DateTime.UtcNow;
    }
    /// <summary>
    /// Will return the same UTC datetime for the duration of the session
    /// </summary>
    /// <returns></returns>
    public DateTime GetSessionUtcDateTime()
    {
      if (_sessionDateTime == null)
      {
        _sessionDateTime = GetUtcDateTime();
      }
      return _sessionDateTime.Value;
    }

    public DateTime GetSessionDateTime()
    {
      return GetSessionUtcDateTime();
    }

    public DateTime GetServerDateTime()
    {
      return GetUtcDateTime();
    }
  }
}

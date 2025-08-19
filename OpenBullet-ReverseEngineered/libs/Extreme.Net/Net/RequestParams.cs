// Decompiled with JetBrains decompiler
// Type: Extreme.Net.RequestParams
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Extreme.Net;

public class RequestParams : List<KeyValuePair<string, string>>
{
  public object this[string paramName]
  {
    set
    {
      switch (paramName)
      {
        case null:
          throw new ArgumentNullException(nameof (paramName));
        case "":
          throw ExceptionHelper.EmptyString(nameof (paramName));
        default:
          string str = value == null ? string.Empty : value.ToString();
          this.Add(new KeyValuePair<string, string>(paramName, str));
          break;
      }
    }
  }
}

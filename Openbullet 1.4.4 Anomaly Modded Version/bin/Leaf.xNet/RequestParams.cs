// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.RequestParams
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Leaf.xNet;

public class RequestParams : List<KeyValuePair<string, string>>
{
  public readonly bool ValuesUnescaped;
  public readonly bool KeysUnescaped;

  public string Query
  {
    get
    {
      return Http.ToQueryString((IEnumerable<KeyValuePair<string, string>>) this, this.ValuesUnescaped, this.KeysUnescaped);
    }
  }

  public RequestParams(bool valuesUnescaped = false, bool keysUnescaped = false)
  {
    this.ValuesUnescaped = valuesUnescaped;
    this.KeysUnescaped = keysUnescaped;
  }

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
          string str = value?.ToString() ?? string.Empty;
          this.Add(new KeyValuePair<string, string>(paramName, str));
          break;
      }
    }
  }
}

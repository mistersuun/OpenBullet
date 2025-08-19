// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.TokenizerService
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public abstract class TokenizerService
{
  public abstract void Initialize(
    object state,
    TextReader sourceReader,
    SourceUnit sourceUnit,
    SourceLocation initialLocation);

  public abstract object CurrentState { get; }

  public abstract SourceLocation CurrentPosition { get; }

  public abstract TokenInfo ReadToken();

  public abstract bool IsRestartable { get; }

  public abstract ErrorSink ErrorSink { get; set; }

  public virtual bool SkipToken() => this.ReadToken().Category != TokenCategory.EndOfStream;

  public IEnumerable<TokenInfo> ReadTokens(int characterCount)
  {
    List<TokenInfo> tokenInfoList = new List<TokenInfo>();
    int index = this.CurrentPosition.Index;
    while (this.CurrentPosition.Index - index < characterCount)
    {
      TokenInfo tokenInfo = this.ReadToken();
      if (tokenInfo.Category != TokenCategory.EndOfStream)
        tokenInfoList.Add(tokenInfo);
      else
        break;
    }
    return (IEnumerable<TokenInfo>) tokenInfoList;
  }

  public bool SkipTokens(int countOfChars)
  {
    bool flag = false;
    int index = this.CurrentPosition.Index;
    do
      ;
    while (this.CurrentPosition.Index - index < countOfChars && (flag = this.SkipToken()));
    return flag;
  }
}

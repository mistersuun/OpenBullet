// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.QueryValueFromTextEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core;

public class QueryValueFromTextEventArgs : EventArgs
{
  private string m_text;
  private object m_value;
  private bool m_hasParsingError;

  public QueryValueFromTextEventArgs(string text, object value)
  {
    this.m_text = text;
    this.m_value = value;
  }

  public string Text => this.m_text;

  public object Value
  {
    get => this.m_value;
    set => this.m_value = value;
  }

  public bool HasParsingError
  {
    get => this.m_hasParsingError;
    set => this.m_hasParsingError = value;
  }
}

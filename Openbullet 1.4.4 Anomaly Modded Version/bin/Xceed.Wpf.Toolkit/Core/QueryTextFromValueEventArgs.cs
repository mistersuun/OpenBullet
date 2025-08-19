// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.QueryTextFromValueEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core;

public class QueryTextFromValueEventArgs : EventArgs
{
  private object m_value;
  private string m_text;

  public QueryTextFromValueEventArgs(object value, string text)
  {
    this.m_value = value;
    this.m_text = text;
  }

  public object Value => this.m_value;

  public string Text
  {
    get => this.m_text;
    set => this.m_text = value;
  }
}

// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Input;

public class InputValidationErrorEventArgs : EventArgs
{
  private Exception exception;
  private bool _throwException;

  public InputValidationErrorEventArgs(Exception e) => this.Exception = e;

  public Exception Exception
  {
    get => this.exception;
    private set => this.exception = value;
  }

  public bool ThrowException
  {
    get => this._throwException;
    set => this._throwException = value;
  }
}

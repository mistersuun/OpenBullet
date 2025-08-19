// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.ContainerTracking`1
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Diagnostics;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class ContainerTracking<T>
{
  private T _container;
  private ContainerTracking<T> _next;
  private ContainerTracking<T> _previous;

  internal ContainerTracking(T container) => this._container = container;

  internal T Container => this._container;

  internal ContainerTracking<T> Next => this._next;

  internal ContainerTracking<T> Previous => this._previous;

  internal void StartTracking(ref ContainerTracking<T> root)
  {
    if (root != null)
      root._previous = this;
    this._next = root;
    root = this;
  }

  internal void StopTracking(ref ContainerTracking<T> root)
  {
    if (this._previous != null)
      this._previous._next = this._next;
    if (this._next != null)
      this._next._previous = this._previous;
    if (root == this)
      root = this._next;
    this._previous = (ContainerTracking<T>) null;
    this._next = (ContainerTracking<T>) null;
  }

  [Conditional("DEBUG")]
  internal void Debug_AssertIsInList(ContainerTracking<T> root)
  {
  }

  [Conditional("DEBUG")]
  internal void Debug_AssertNotInList(ContainerTracking<T> root)
  {
  }
}

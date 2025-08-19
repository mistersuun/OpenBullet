// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Zoombox.ZoomboxCursors
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Zoombox;

public class ZoomboxCursors
{
  private static readonly Cursor _zoom = Cursors.Arrow;
  private static readonly Cursor _zoomRelative = Cursors.Arrow;

  static ZoomboxCursors()
  {
    try
    {
      new EnvironmentPermission(PermissionState.Unrestricted).Demand();
      ZoomboxCursors._zoom = new Cursor(ResourceHelper.LoadResourceStream(Assembly.GetExecutingAssembly(), "Zoombox/Resources/Zoom.cur"));
      ZoomboxCursors._zoomRelative = new Cursor(ResourceHelper.LoadResourceStream(Assembly.GetExecutingAssembly(), "Zoombox/Resources/ZoomRelative.cur"));
    }
    catch (SecurityException ex)
    {
    }
  }

  public static Cursor Zoom => ZoomboxCursors._zoom;

  public static Cursor ZoomRelative => ZoomboxCursors._zoomRelative;
}

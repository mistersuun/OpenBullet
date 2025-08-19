// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Shell.SystemParameters2
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Standard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Shell;

public class SystemParameters2 : INotifyPropertyChanged
{
  [ThreadStatic]
  private static SystemParameters2 _threadLocalSingleton;
  private MessageWindow _messageHwnd;
  private bool _isGlassEnabled;
  private Color _glassColor;
  private SolidColorBrush _glassColorBrush;
  private Thickness _windowResizeBorderThickness;
  private Thickness _windowNonClientFrameThickness;
  private double _captionHeight;
  private Size _smallIconSize;
  private string _uxThemeName;
  private string _uxThemeColor;
  private bool _isHighContrast;
  private CornerRadius _windowCornerRadius;
  private Rect _captionButtonLocation;
  private readonly Dictionary<WM, List<SystemParameters2._SystemMetricUpdate>> _UpdateTable;

  private void _InitializeIsGlassEnabled()
  {
    this.IsGlassEnabled = Standard.NativeMethods.DwmIsCompositionEnabled();
  }

  private void _UpdateIsGlassEnabled(IntPtr wParam, IntPtr lParam)
  {
    this._InitializeIsGlassEnabled();
  }

  private void _InitializeGlassColor()
  {
    uint pcrColorization;
    bool pfOpaqueBlend;
    Standard.NativeMethods.DwmGetColorizationColor(out pcrColorization, out pfOpaqueBlend);
    this.WindowGlassColor = Utility.ColorFromArgbDword(pcrColorization | (pfOpaqueBlend ? 4278190080U /*0xFF000000*/ : 0U));
    SolidColorBrush solidColorBrush = new SolidColorBrush(this.WindowGlassColor);
    solidColorBrush.Freeze();
    this.WindowGlassBrush = solidColorBrush;
  }

  private void _UpdateGlassColor(IntPtr wParam, IntPtr lParam)
  {
    bool flag = lParam != IntPtr.Zero;
    this.WindowGlassColor = Utility.ColorFromArgbDword((uint) (int) wParam.ToInt64() | (flag ? 4278190080U /*0xFF000000*/ : 0U));
    SolidColorBrush solidColorBrush = new SolidColorBrush(this.WindowGlassColor);
    solidColorBrush.Freeze();
    this.WindowGlassBrush = solidColorBrush;
  }

  private void _InitializeCaptionHeight()
  {
    this.WindowCaptionHeight = DpiHelper.DevicePixelsToLogical(new Point(0.0, (double) Standard.NativeMethods.GetSystemMetrics(SM.CYCAPTION))).Y;
  }

  private void _UpdateCaptionHeight(IntPtr wParam, IntPtr lParam)
  {
    this._InitializeCaptionHeight();
  }

  private void _InitializeWindowResizeBorderThickness()
  {
    Size logical = DpiHelper.DeviceSizeToLogical(new Size((double) Standard.NativeMethods.GetSystemMetrics(SM.CXFRAME), (double) Standard.NativeMethods.GetSystemMetrics(SM.CYFRAME)));
    this.WindowResizeBorderThickness = new Thickness(logical.Width, logical.Height, logical.Width, logical.Height);
  }

  private void _UpdateWindowResizeBorderThickness(IntPtr wParam, IntPtr lParam)
  {
    this._InitializeWindowResizeBorderThickness();
  }

  private void _InitializeWindowNonClientFrameThickness()
  {
    Size logical = DpiHelper.DeviceSizeToLogical(new Size((double) Standard.NativeMethods.GetSystemMetrics(SM.CXFRAME), (double) Standard.NativeMethods.GetSystemMetrics(SM.CYFRAME)));
    double y = DpiHelper.DevicePixelsToLogical(new Point(0.0, (double) Standard.NativeMethods.GetSystemMetrics(SM.CYCAPTION))).Y;
    this.WindowNonClientFrameThickness = new Thickness(logical.Width, logical.Height + y, logical.Width, logical.Height);
  }

  private void _UpdateWindowNonClientFrameThickness(IntPtr wParam, IntPtr lParam)
  {
    this._InitializeWindowNonClientFrameThickness();
  }

  private void _InitializeSmallIconSize()
  {
    this.SmallIconSize = new Size((double) Standard.NativeMethods.GetSystemMetrics(SM.CXSMICON), (double) Standard.NativeMethods.GetSystemMetrics(SM.CYSMICON));
  }

  private void _UpdateSmallIconSize(IntPtr wParam, IntPtr lParam)
  {
    this._InitializeSmallIconSize();
  }

  private void _LegacyInitializeCaptionButtonLocation()
  {
    int systemMetrics1 = Standard.NativeMethods.GetSystemMetrics(SM.CXSIZE);
    int systemMetrics2 = Standard.NativeMethods.GetSystemMetrics(SM.CYSIZE);
    int num = Standard.NativeMethods.GetSystemMetrics(SM.CXFRAME) + Standard.NativeMethods.GetSystemMetrics(SM.CXEDGE);
    int offsetY = Standard.NativeMethods.GetSystemMetrics(SM.CYFRAME) + Standard.NativeMethods.GetSystemMetrics(SM.CYEDGE);
    Rect rect = new Rect(0.0, 0.0, (double) (systemMetrics1 * 3), (double) systemMetrics2);
    rect.Offset((double) -num - rect.Width, (double) offsetY);
    this.WindowCaptionButtonsLocation = rect;
  }

  private void _InitializeCaptionButtonLocation()
  {
    if (!Utility.IsOSVistaOrNewer || !Standard.NativeMethods.IsThemeActive())
    {
      this._LegacyInitializeCaptionButtonLocation();
    }
    else
    {
      TITLEBARINFOEX structure = new TITLEBARINFOEX()
      {
        cbSize = Marshal.SizeOf(typeof (TITLEBARINFOEX))
      };
      IntPtr hglobal = Marshal.AllocHGlobal(structure.cbSize);
      try
      {
        Marshal.StructureToPtr((object) structure, hglobal, false);
        Standard.NativeMethods.ShowWindow(this._messageHwnd.Handle, SW.SHOW);
        Standard.NativeMethods.SendMessage(this._messageHwnd.Handle, WM.GETTITLEBARINFOEX, IntPtr.Zero, hglobal);
        structure = (TITLEBARINFOEX) Marshal.PtrToStructure(hglobal, typeof (TITLEBARINFOEX));
      }
      finally
      {
        Standard.NativeMethods.ShowWindow(this._messageHwnd.Handle, SW.HIDE);
        Utility.SafeFreeHGlobal(ref hglobal);
      }
      RECT rect = RECT.Union(structure.rgrect_CloseButton, structure.rgrect_MinimizeButton);
      RECT windowRect = Standard.NativeMethods.GetWindowRect(this._messageHwnd.Handle);
      this.WindowCaptionButtonsLocation = DpiHelper.DeviceRectToLogical(new Rect((double) (rect.Left - windowRect.Width - windowRect.Left), (double) (rect.Top - windowRect.Top), (double) rect.Width, (double) rect.Height));
    }
  }

  private void _UpdateCaptionButtonLocation(IntPtr wParam, IntPtr lParam)
  {
    this._InitializeCaptionButtonLocation();
  }

  private void _InitializeHighContrast()
  {
    this.HighContrast = (Standard.NativeMethods.SystemParameterInfo_GetHIGHCONTRAST().dwFlags & HCF.HIGHCONTRASTON) != 0;
  }

  private void _UpdateHighContrast(IntPtr wParam, IntPtr lParam) => this._InitializeHighContrast();

  private void _InitializeThemeInfo()
  {
    if (!Standard.NativeMethods.IsThemeActive())
    {
      this.UxThemeName = "Classic";
      this.UxThemeColor = "";
    }
    else
    {
      string themeFileName;
      string color;
      Standard.NativeMethods.GetCurrentThemeName(out themeFileName, out color, out string _);
      this.UxThemeName = Path.GetFileNameWithoutExtension(themeFileName);
      this.UxThemeColor = color;
    }
  }

  private void _UpdateThemeInfo(IntPtr wParam, IntPtr lParam) => this._InitializeThemeInfo();

  private void _InitializeWindowCornerRadius()
  {
    CornerRadius cornerRadius = new CornerRadius();
    switch (this.UxThemeName.ToUpperInvariant())
    {
      case "LUNA":
        cornerRadius = new CornerRadius(6.0, 6.0, 0.0, 0.0);
        break;
      case "AERO":
        cornerRadius = !Standard.NativeMethods.DwmIsCompositionEnabled() ? new CornerRadius(6.0, 6.0, 0.0, 0.0) : new CornerRadius(8.0);
        break;
      default:
        cornerRadius = new CornerRadius(0.0);
        break;
    }
    this.WindowCornerRadius = cornerRadius;
  }

  private void _UpdateWindowCornerRadius(IntPtr wParam, IntPtr lParam)
  {
    this._InitializeWindowCornerRadius();
  }

  private SystemParameters2()
  {
    this._messageHwnd = new MessageWindow((CS) 0, WS.TILEDWINDOW | WS.DISABLED, WS_EX.None, new Rect(-16000.0, -16000.0, 100.0, 100.0), "", new WndProc(this._WndProc));
    this._messageHwnd.Dispatcher.ShutdownStarted += (EventHandler) ((sender, e) => Utility.SafeDispose<MessageWindow>(ref this._messageHwnd));
    this._InitializeIsGlassEnabled();
    this._InitializeGlassColor();
    this._InitializeCaptionHeight();
    this._InitializeWindowNonClientFrameThickness();
    this._InitializeWindowResizeBorderThickness();
    this._InitializeCaptionButtonLocation();
    this._InitializeSmallIconSize();
    this._InitializeHighContrast();
    this._InitializeThemeInfo();
    this._InitializeWindowCornerRadius();
    this._UpdateTable = new Dictionary<WM, List<SystemParameters2._SystemMetricUpdate>>()
    {
      {
        WM.THEMECHANGED,
        new List<SystemParameters2._SystemMetricUpdate>()
        {
          new SystemParameters2._SystemMetricUpdate(this._UpdateThemeInfo),
          new SystemParameters2._SystemMetricUpdate(this._UpdateHighContrast),
          new SystemParameters2._SystemMetricUpdate(this._UpdateWindowCornerRadius),
          new SystemParameters2._SystemMetricUpdate(this._UpdateCaptionButtonLocation)
        }
      },
      {
        WM.WININICHANGE,
        new List<SystemParameters2._SystemMetricUpdate>()
        {
          new SystemParameters2._SystemMetricUpdate(this._UpdateCaptionHeight),
          new SystemParameters2._SystemMetricUpdate(this._UpdateWindowResizeBorderThickness),
          new SystemParameters2._SystemMetricUpdate(this._UpdateSmallIconSize),
          new SystemParameters2._SystemMetricUpdate(this._UpdateHighContrast),
          new SystemParameters2._SystemMetricUpdate(this._UpdateWindowNonClientFrameThickness),
          new SystemParameters2._SystemMetricUpdate(this._UpdateCaptionButtonLocation)
        }
      },
      {
        WM.DWMNCRENDERINGCHANGED,
        new List<SystemParameters2._SystemMetricUpdate>()
        {
          new SystemParameters2._SystemMetricUpdate(this._UpdateIsGlassEnabled)
        }
      },
      {
        WM.DWMCOMPOSITIONCHANGED,
        new List<SystemParameters2._SystemMetricUpdate>()
        {
          new SystemParameters2._SystemMetricUpdate(this._UpdateIsGlassEnabled)
        }
      },
      {
        WM.DWMCOLORIZATIONCOLORCHANGED,
        new List<SystemParameters2._SystemMetricUpdate>()
        {
          new SystemParameters2._SystemMetricUpdate(this._UpdateGlassColor)
        }
      }
    };
  }

  public static SystemParameters2 Current
  {
    get
    {
      if (SystemParameters2._threadLocalSingleton == null)
        SystemParameters2._threadLocalSingleton = new SystemParameters2();
      return SystemParameters2._threadLocalSingleton;
    }
  }

  private IntPtr _WndProc(IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam)
  {
    List<SystemParameters2._SystemMetricUpdate> systemMetricUpdateList;
    if (this._UpdateTable != null && this._UpdateTable.TryGetValue(msg, out systemMetricUpdateList))
    {
      foreach (SystemParameters2._SystemMetricUpdate systemMetricUpdate in systemMetricUpdateList)
        systemMetricUpdate(wParam, lParam);
    }
    return Standard.NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
  }

  public bool IsGlassEnabled
  {
    get => Standard.NativeMethods.DwmIsCompositionEnabled();
    private set
    {
      if (value == this._isGlassEnabled)
        return;
      this._isGlassEnabled = value;
      this._NotifyPropertyChanged(nameof (IsGlassEnabled));
    }
  }

  public Color WindowGlassColor
  {
    get => this._glassColor;
    private set
    {
      if (!(value != this._glassColor))
        return;
      this._glassColor = value;
      this._NotifyPropertyChanged(nameof (WindowGlassColor));
    }
  }

  public SolidColorBrush WindowGlassBrush
  {
    get => this._glassColorBrush;
    private set
    {
      if (this._glassColorBrush != null && !(value.Color != this._glassColorBrush.Color))
        return;
      this._glassColorBrush = value;
      this._NotifyPropertyChanged(nameof (WindowGlassBrush));
    }
  }

  public Thickness WindowResizeBorderThickness
  {
    get => this._windowResizeBorderThickness;
    private set
    {
      if (!(value != this._windowResizeBorderThickness))
        return;
      this._windowResizeBorderThickness = value;
      this._NotifyPropertyChanged(nameof (WindowResizeBorderThickness));
    }
  }

  public Thickness WindowNonClientFrameThickness
  {
    get => this._windowNonClientFrameThickness;
    private set
    {
      if (!(value != this._windowNonClientFrameThickness))
        return;
      this._windowNonClientFrameThickness = value;
      this._NotifyPropertyChanged(nameof (WindowNonClientFrameThickness));
    }
  }

  public double WindowCaptionHeight
  {
    get => this._captionHeight;
    private set
    {
      if (value == this._captionHeight)
        return;
      this._captionHeight = value;
      this._NotifyPropertyChanged(nameof (WindowCaptionHeight));
    }
  }

  public Size SmallIconSize
  {
    get => new Size(this._smallIconSize.Width, this._smallIconSize.Height);
    private set
    {
      if (!(value != this._smallIconSize))
        return;
      this._smallIconSize = value;
      this._NotifyPropertyChanged(nameof (SmallIconSize));
    }
  }

  public string UxThemeName
  {
    get => this._uxThemeName;
    private set
    {
      if (!(value != this._uxThemeName))
        return;
      this._uxThemeName = value;
      this._NotifyPropertyChanged(nameof (UxThemeName));
    }
  }

  public string UxThemeColor
  {
    get => this._uxThemeColor;
    private set
    {
      if (!(value != this._uxThemeColor))
        return;
      this._uxThemeColor = value;
      this._NotifyPropertyChanged(nameof (UxThemeColor));
    }
  }

  public bool HighContrast
  {
    get => this._isHighContrast;
    private set
    {
      if (value == this._isHighContrast)
        return;
      this._isHighContrast = value;
      this._NotifyPropertyChanged(nameof (HighContrast));
    }
  }

  public CornerRadius WindowCornerRadius
  {
    get => this._windowCornerRadius;
    private set
    {
      if (!(value != this._windowCornerRadius))
        return;
      this._windowCornerRadius = value;
      this._NotifyPropertyChanged(nameof (WindowCornerRadius));
    }
  }

  public Rect WindowCaptionButtonsLocation
  {
    get => this._captionButtonLocation;
    private set
    {
      if (!(value != this._captionButtonLocation))
        return;
      this._captionButtonLocation = value;
      this._NotifyPropertyChanged(nameof (WindowCaptionButtonsLocation));
    }
  }

  private void _NotifyPropertyChanged(string propertyName)
  {
    PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
    if (propertyChanged == null)
      return;
    propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
  }

  public event PropertyChangedEventHandler PropertyChanged;

  private delegate void _SystemMetricUpdate(IntPtr wParam, IntPtr lParam);
}

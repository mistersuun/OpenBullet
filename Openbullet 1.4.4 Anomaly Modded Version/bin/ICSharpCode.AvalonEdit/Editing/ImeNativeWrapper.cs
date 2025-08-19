// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.ImeNativeWrapper
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal static class ImeNativeWrapper
{
  private const int CPS_CANCEL = 4;
  private const int NI_COMPOSITIONSTR = 21;
  private const int GCS_COMPSTR = 8;
  public const int WM_IME_COMPOSITION = 271;
  public const int WM_IME_SETCONTEXT = 641;
  public const int WM_INPUTLANGCHANGE = 81;
  [ThreadStatic]
  private static bool textFrameworkThreadMgrInitialized;
  [ThreadStatic]
  private static ITfThreadMgr textFrameworkThreadMgr;
  private static readonly Rect EMPTY_RECT = new Rect(0.0, 0.0, 0.0, 0.0);

  [DllImport("imm32.dll")]
  public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

  [DllImport("imm32.dll")]
  internal static extern IntPtr ImmGetContext(IntPtr hWnd);

  [DllImport("imm32.dll")]
  internal static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

  [DllImport("imm32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

  [DllImport("imm32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue = 0);

  [DllImport("imm32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool ImmSetCompositionWindow(
    IntPtr hIMC,
    ref ImeNativeWrapper.CompositionForm form);

  [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool ImmSetCompositionFont(IntPtr hIMC, ref ImeNativeWrapper.LOGFONT font);

  [DllImport("imm32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool ImmGetCompositionFont(IntPtr hIMC, out ImeNativeWrapper.LOGFONT font);

  [DllImport("msctf.dll")]
  private static extern int TF_CreateThreadMgr(out ITfThreadMgr threadMgr);

  public static ITfThreadMgr GetTextFrameworkThreadManager()
  {
    if (!ImeNativeWrapper.textFrameworkThreadMgrInitialized)
    {
      ImeNativeWrapper.textFrameworkThreadMgrInitialized = true;
      ImeNativeWrapper.TF_CreateThreadMgr(out ImeNativeWrapper.textFrameworkThreadMgr);
    }
    return ImeNativeWrapper.textFrameworkThreadMgr;
  }

  public static bool NotifyIme(IntPtr hIMC) => ImeNativeWrapper.ImmNotifyIME(hIMC, 21, 4);

  public static bool SetCompositionWindow(HwndSource source, IntPtr hIMC, TextArea textArea)
  {
    if (textArea == null)
      throw new ArgumentNullException(nameof (textArea));
    Rect bounds = textArea.TextView.GetBounds(source);
    Rect characterBounds = textArea.TextView.GetCharacterBounds(textArea.Caret.Position, source);
    return ImeNativeWrapper.ImmSetCompositionWindow(hIMC, ref new ImeNativeWrapper.CompositionForm()
    {
      dwStyle = 32 /*0x20*/,
      ptCurrentPos = {
        x = (int) Math.Max(characterBounds.Left, bounds.Left),
        y = (int) Math.Max(characterBounds.Top, bounds.Top)
      },
      rcArea = {
        left = (int) bounds.Left,
        top = (int) bounds.Top,
        right = (int) bounds.Right,
        bottom = (int) bounds.Bottom
      }
    });
  }

  public static bool SetCompositionFont(HwndSource source, IntPtr hIMC, TextArea textArea)
  {
    if (textArea == null)
      throw new ArgumentNullException(nameof (textArea));
    ImeNativeWrapper.LOGFONT font = new ImeNativeWrapper.LOGFONT();
    Rect characterBounds = textArea.TextView.GetCharacterBounds(textArea.Caret.Position, source);
    font.lfFaceName = textArea.FontFamily.Source;
    font.lfHeight = (int) characterBounds.Height;
    return ImeNativeWrapper.ImmSetCompositionFont(hIMC, ref font);
  }

  private static Rect GetBounds(this TextView textView, HwndSource source)
  {
    if (source.RootVisual == null || !source.RootVisual.IsAncestorOf((DependencyObject) textView))
      return ImeNativeWrapper.EMPTY_RECT;
    Rect rect = new Rect(0.0, 0.0, textView.ActualWidth, textView.ActualHeight);
    return textView.TransformToAncestor(source.RootVisual).TransformBounds(rect).TransformToDevice(source.RootVisual);
  }

  private static Rect GetCharacterBounds(
    this TextView textView,
    TextViewPosition pos,
    HwndSource source)
  {
    VisualLine visualLine = textView.GetVisualLine(pos.Line);
    if (visualLine == null || source.RootVisual == null || !source.RootVisual.IsAncestorOf((DependencyObject) textView))
      return ImeNativeWrapper.EMPTY_RECT;
    TextLine textLine = visualLine.GetTextLine(pos.VisualColumn, pos.IsAtEndOfLine);
    Rect rect;
    if (pos.VisualColumn < visualLine.VisualLengthWithEndOfLineMarker)
    {
      rect = textLine.GetTextBounds(pos.VisualColumn, 1).First<TextBounds>().Rectangle;
      rect.Offset(0.0, visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineTop));
    }
    else
      rect = new Rect(visualLine.GetVisualPosition(pos.VisualColumn, VisualYPosition.TextTop), new Size(textView.WideSpaceWidth, textView.DefaultLineHeight));
    rect.Offset(-textView.ScrollOffset);
    return textView.TransformToAncestor(source.RootVisual).TransformBounds(rect).TransformToDevice(source.RootVisual);
  }

  private struct CompositionForm
  {
    public int dwStyle;
    public ImeNativeWrapper.POINT ptCurrentPos;
    public ImeNativeWrapper.RECT rcArea;
  }

  private struct POINT
  {
    public int x;
    public int y;
  }

  private struct RECT
  {
    public int left;
    public int top;
    public int right;
    public int bottom;
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  private struct LOGFONT
  {
    public int lfHeight;
    public int lfWidth;
    public int lfEscapement;
    public int lfOrientation;
    public int lfWeight;
    public byte lfItalic;
    public byte lfUnderline;
    public byte lfStrikeOut;
    public byte lfCharSet;
    public byte lfOutPrecision;
    public byte lfClipPrecision;
    public byte lfQuality;
    public byte lfPitchAndFamily;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32 /*0x20*/)]
    public string lfFaceName;
  }
}

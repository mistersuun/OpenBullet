// Decompiled with JetBrains decompiler
// Type: Tesseract.ResultIterator
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections.Generic;
using System.Text;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public sealed class ResultIterator : PageIterator
{
  private Dictionary<int, FontInfo> _fontInfoCache = new Dictionary<int, FontInfo>();

  internal ResultIterator(Page page, IntPtr handle)
    : base(page, handle)
  {
  }

  public float GetConfidence(PageIteratorLevel level)
  {
    this.VerifyNotDisposed();
    return this.handle.Handle == IntPtr.Zero ? 0.0f : TessApi.Native.ResultIteratorGetConfidence(this.handle, level);
  }

  public string GetText(PageIteratorLevel level)
  {
    this.VerifyNotDisposed();
    return this.handle.Handle == IntPtr.Zero ? string.Empty : TessApi.ResultIteratorGetUTF8Text(this.handle, level);
  }

  public FontAttributes GetWordFontAttributes()
  {
    this.VerifyNotDisposed();
    if (this.handle.Handle == IntPtr.Zero)
      return (FontAttributes) null;
    bool isBold;
    bool isItalic;
    bool isUnderlined;
    bool isMonospace;
    bool isSerif;
    bool isSmallCaps;
    int pointSize;
    int fontId;
    IntPtr handle = TessApi.Native.ResultIteratorWordFontAttributes(this.handle, out isBold, out isItalic, out isUnderlined, out isMonospace, out isSerif, out isSmallCaps, out pointSize, out fontId);
    if (handle == IntPtr.Zero)
      return (FontAttributes) null;
    FontInfo fontInfo;
    if (!this._fontInfoCache.TryGetValue(fontId, out fontInfo))
    {
      fontInfo = new FontInfo(MarshalHelper.PtrToString(handle, Encoding.UTF8), fontId, isItalic, isBold, isMonospace, isSerif);
      this._fontInfoCache.Add(fontId, fontInfo);
    }
    return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);
  }

  public string GetWordRecognitionLanguage()
  {
    this.VerifyNotDisposed();
    return this.handle.Handle == IntPtr.Zero ? (string) null : TessApi.ResultIteratorWordRecognitionLanguage(this.handle);
  }

  public bool GetWordIsFromDictionary()
  {
    this.VerifyNotDisposed();
    return !(this.handle.Handle == IntPtr.Zero) && TessApi.Native.ResultIteratorWordIsFromDictionary(this.handle);
  }

  public bool GetWordIsNumeric()
  {
    this.VerifyNotDisposed();
    return !(this.handle.Handle == IntPtr.Zero) && TessApi.Native.ResultIteratorWordIsNumeric(this.handle);
  }

  public bool GetSymbolIsSuperscript()
  {
    this.VerifyNotDisposed();
    return !(this.handle.Handle == IntPtr.Zero) && TessApi.Native.ResultIteratorSymbolIsSuperscript(this.handle);
  }

  public bool GetSymbolIsSubscript()
  {
    this.VerifyNotDisposed();
    return !(this.handle.Handle == IntPtr.Zero) && TessApi.Native.ResultIteratorSymbolIsSubscript(this.handle);
  }

  public bool GetSymbolIsDropcap()
  {
    this.VerifyNotDisposed();
    return !(this.handle.Handle == IntPtr.Zero) && TessApi.Native.ResultIteratorSymbolIsDropcap(this.handle);
  }

  public ChoiceIterator GetChoiceIterator()
  {
    IntPtr choiceIterator = TessApi.Native.ResultIteratorGetChoiceIterator(this.handle);
    return choiceIterator == IntPtr.Zero ? (ChoiceIterator) null : new ChoiceIterator(choiceIterator);
  }
}

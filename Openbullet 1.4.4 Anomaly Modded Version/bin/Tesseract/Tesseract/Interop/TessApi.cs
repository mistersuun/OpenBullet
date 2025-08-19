// Decompiled with JetBrains decompiler
// Type: Tesseract.Interop.TessApi
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using InteropDotNet;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Tesseract.Internal;

#nullable disable
namespace Tesseract.Interop;

internal static class TessApi
{
  public const string xhtmlBeginTag = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\n    \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\n <head>\n  <title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\" />\n  <meta name='ocr-system' content='tesseract' />\n  <meta name='ocr-capabilities' content='ocr_page ocr_carea ocr_par ocr_line ocrx_word'/>\n</head>\n<body>\n";
  public const string xhtmlEndTag = " </body>\n</html>\n";
  public const string htmlBeginTag = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\n<html>\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\" />\n<meta name='ocr-system' content='tesseract'/>\n</head>\n<body>\n";
  public const string htmlEndTag = "</body>\n</html>\n";
  private static ITessApiSignatures native;

  public static ITessApiSignatures Native
  {
    get
    {
      if (TessApi.native == null)
        TessApi.Initialize();
      return TessApi.native;
    }
  }

  public static string BaseAPIGetHOCRText(HandleRef handle, int pageNum)
  {
    IntPtr hocrTextInternal = TessApi.Native.BaseAPIGetHOCRTextInternal(handle, pageNum);
    if (!(hocrTextInternal != IntPtr.Zero))
      return (string) null;
    string str = MarshalHelper.PtrToString(hocrTextInternal, Encoding.UTF8);
    TessApi.Native.DeleteText(hocrTextInternal);
    return $"<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\n<html>\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\" />\n<meta name='ocr-system' content='tesseract'/>\n</head>\n<body>\n{str}</body>\n</html>\n";
  }

  public static string BaseAPIGetHOCRText2(HandleRef handle, int pageNum)
  {
    IntPtr hocrTextInternal = TessApi.Native.BaseAPIGetHOCRTextInternal(handle, pageNum);
    if (!(hocrTextInternal != IntPtr.Zero))
      return (string) null;
    string str = MarshalHelper.PtrToString(hocrTextInternal, Encoding.UTF8);
    TessApi.Native.DeleteText(hocrTextInternal);
    return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\n    \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\n <head>\n  <title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\" />\n  <meta name='ocr-system' content='tesseract' />\n  <meta name='ocr-capabilities' content='ocr_page ocr_carea ocr_par ocr_line ocrx_word'/>\n</head>\n<body>\n{str} </body>\n</html>\n";
  }

  public static string BaseApiGetStringVariable(HandleRef handle, string name)
  {
    IntPtr variableInternal = TessApi.Native.BaseApiGetStringVariableInternal(handle, name);
    return variableInternal != IntPtr.Zero ? MarshalHelper.PtrToString(variableInternal, Encoding.UTF8) : (string) null;
  }

  public static string BaseAPIGetUTF8Text(HandleRef handle)
  {
    IntPtr utF8TextInternal = TessApi.Native.BaseAPIGetUTF8TextInternal(handle);
    if (!(utF8TextInternal != IntPtr.Zero))
      return (string) null;
    string utF8Text = MarshalHelper.PtrToString(utF8TextInternal, Encoding.UTF8);
    TessApi.Native.DeleteText(utF8TextInternal);
    return utF8Text;
  }

  public static int BaseApiInit(
    HandleRef handle,
    string datapath,
    string language,
    int mode,
    IEnumerable<string> configFiles,
    IDictionary<string, object> initialValues,
    bool setOnlyNonDebugParams)
  {
    Guard.Require(nameof (handle), handle.Handle != IntPtr.Zero, "Handle for BaseApi, created through BaseApiCreate is required.");
    Guard.RequireNotNullOrEmpty(nameof (language), language);
    Guard.RequireNotNull(nameof (configFiles), (object) configFiles);
    Guard.RequireNotNull(nameof (initialValues), (object) initialValues);
    string[] array = new List<string>(configFiles).ToArray();
    string[] vars_vec = new string[initialValues.Count];
    string[] vars_values = new string[initialValues.Count];
    int index = 0;
    foreach (KeyValuePair<string, object> initialValue in (IEnumerable<KeyValuePair<string, object>>) initialValues)
    {
      Guard.Require(nameof (initialValues), !string.IsNullOrEmpty(initialValue.Key), "Variable must have a name.");
      Guard.Require(nameof (initialValues), (initialValue.Value != null ? 1 : 0) != 0, "Variable '{0}': The type '{1}' is not supported.", (object) initialValue.Key, (object) initialValue.Value.GetType());
      vars_vec[index] = initialValue.Key;
      string result;
      if (!TessConvert.TryToString(initialValue.Value, out result))
        throw new ArgumentException($"Variable '{initialValue.Key}': The type '{initialValue.Value.GetType()}' is not supported.", nameof (initialValues));
      vars_values[index] = result;
      ++index;
    }
    return TessApi.Native.BaseApiInit(handle, datapath, language, mode, array, array.Length, vars_vec, vars_values, new UIntPtr((uint) vars_vec.Length), setOnlyNonDebugParams);
  }

  public static int BaseApiSetDebugVariable(HandleRef handle, string name, string value)
  {
    IntPtr num = IntPtr.Zero;
    try
    {
      num = MarshalHelper.StringToPtr(value, Encoding.UTF8);
      return TessApi.Native.BaseApiSetDebugVariable(handle, name, num);
    }
    finally
    {
      if (num != IntPtr.Zero)
        Marshal.FreeHGlobal(num);
    }
  }

  public static int BaseApiSetVariable(HandleRef handle, string name, string value)
  {
    IntPtr num = IntPtr.Zero;
    try
    {
      num = MarshalHelper.StringToPtr(value, Encoding.UTF8);
      return TessApi.Native.BaseApiSetVariable(handle, name, num);
    }
    finally
    {
      if (num != IntPtr.Zero)
        Marshal.FreeHGlobal(num);
    }
  }

  public static void Initialize()
  {
    if (TessApi.native != null)
      return;
    LeptonicaApi.Initialize();
    TessApi.native = InteropRuntimeImplementer.CreateInstance<ITessApiSignatures>();
  }

  public static string ResultIteratorWordRecognitionLanguage(HandleRef handle)
  {
    IntPtr handle1 = TessApi.Native.ResultIteratorWordRecognitionLanguageInternal(handle);
    return !(handle1 != IntPtr.Zero) ? (string) null : MarshalHelper.PtrToString(handle1, Encoding.UTF8);
  }

  public static string ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level)
  {
    IntPtr utF8TextInternal = TessApi.Native.ResultIteratorGetUTF8TextInternal(handle, level);
    if (!(utF8TextInternal != IntPtr.Zero))
      return (string) null;
    string utF8Text = MarshalHelper.PtrToString(utF8TextInternal, Encoding.UTF8);
    TessApi.Native.DeleteText(utF8TextInternal);
    return utF8Text;
  }

  internal static string ChoiceIteratorGetUTF8Text(HandleRef choiceIteratorHandle)
  {
    Guard.Require(nameof (choiceIteratorHandle), choiceIteratorHandle.Handle != IntPtr.Zero, "ChoiceIterator Handle cannot be a null IntPtr and is required");
    return MarshalHelper.PtrToString(TessApi.Native.ChoiceIteratorGetUTF8TextInternal(choiceIteratorHandle), Encoding.UTF8);
  }
}

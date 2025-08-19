// Decompiled with JetBrains decompiler
// Type: Tesseract.Interop.ITessApiSignatures
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using InteropDotNet;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Tesseract.Interop;

public interface ITessApiSignatures
{
  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetComponentImages")]
  IntPtr BaseAPIGetComponentImages(
    HandleRef handle,
    PageIteratorLevel level,
    int text_only,
    IntPtr pixa,
    IntPtr blockids);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAnalyseLayout")]
  IntPtr BaseAPIAnalyseLayout(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClear")]
  void BaseAPIClear(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPICreate")]
  IntPtr BaseApiCreate();

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDelete")]
  void BaseApiDelete(HandleRef ptr);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDetectOrientationScript")]
  int TessBaseAPIDetectOrientationScript(
    HandleRef handle,
    out int orient_deg,
    out float orient_conf,
    out IntPtr script_name,
    out float script_conf);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetBoolVariable")]
  int BaseApiGetBoolVariable(HandleRef handle, string name, out int value);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDoubleVariable")]
  int BaseApiGetDoubleVariable(HandleRef handle, string name, out double value);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetHOCRText")]
  IntPtr BaseAPIGetHOCRTextInternal(HandleRef handle, int pageNum);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIntVariable")]
  int BaseApiGetIntVariable(HandleRef handle, string name, out int value);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIterator")]
  IntPtr BaseApiGetIterator(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPageSegMode")]
  PageSegMode BaseAPIGetPageSegMode(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetStringVariable")]
  IntPtr BaseApiGetStringVariableInternal(HandleRef handle, string name);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetThresholdedImage")]
  IntPtr BaseAPIGetThresholdedImage(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputName")]
  void BaseAPISetInputName(HandleRef handle, string name);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDatapath")]
  string BaseAPIGetDatapath(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetOutputName")]
  void BaseAPISetOutputName(HandleRef handle, string name);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUTF8Text")]
  IntPtr BaseAPIGetUTF8TextInternal(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit4")]
  int BaseApiInit(
    HandleRef handle,
    string datapath,
    string language,
    int mode,
    string[] configs,
    int configs_size,
    string[] vars_vec,
    string[] vars_values,
    UIntPtr vars_vec_size,
    bool set_only_non_debug_params);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIMeanTextConf")]
  int BaseAPIMeanTextConf(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRecognize")]
  int BaseApiRecognize(HandleRef handle, HandleRef monitor);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetDebugVariable")]
  int BaseApiSetDebugVariable(HandleRef handle, string name, IntPtr valPtr);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage2")]
  void BaseApiSetImage(HandleRef handle, HandleRef pixHandle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputName")]
  void BaseApiSetInputName(HandleRef handle, string value);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetPageSegMode")]
  void BaseAPISetPageSegMode(HandleRef handle, PageSegMode mode);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetRectangle")]
  void BaseApiSetRectangle(HandleRef handle, int left, int top, int width, int height);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetVariable")]
  int BaseApiSetVariable(HandleRef handle, string name, IntPtr valPtr);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteBlockList")]
  void DeleteBlockList(IntPtr arr);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteIntArray")]
  void DeleteIntArray(IntPtr arr);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteText")]
  void DeleteText(IntPtr textPtr);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteTextArray")]
  void DeleteTextArray(IntPtr arr);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessVersion")]
  string GetVersion();

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBaseline")]
  int PageIteratorBaseline(
    HandleRef handle,
    PageIteratorLevel level,
    out int x1,
    out int y1,
    out int x2,
    out int y2);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBegin")]
  void PageIteratorBegin(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBlockType")]
  PolyBlockType PageIteratorBlockType(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBoundingBox")]
  int PageIteratorBoundingBox(
    HandleRef handle,
    PageIteratorLevel level,
    out int left,
    out int top,
    out int right,
    out int bottom);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorCopy")]
  IntPtr PageIteratorCopy(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
  void PageIteratorDelete(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetBinaryImage")]
  IntPtr PageIteratorGetBinaryImage(HandleRef handle, PageIteratorLevel level);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetImage")]
  IntPtr PageIteratorGetImage(
    HandleRef handle,
    PageIteratorLevel level,
    int padding,
    HandleRef originalImage,
    out int left,
    out int top);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
  int PageIteratorIsAtBeginningOf(HandleRef handle, PageIteratorLevel level);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
  int PageIteratorIsAtFinalElement(
    HandleRef handle,
    PageIteratorLevel level,
    PageIteratorLevel element);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorNext")]
  int PageIteratorNext(HandleRef handle, PageIteratorLevel level);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorOrientation")]
  void PageIteratorOrientation(
    HandleRef handle,
    out Orientation orientation,
    out WritingDirection writing_direction,
    out TextLineOrder textLineOrder,
    out float deskew_angle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorCopy")]
  IntPtr ResultIteratorCopy(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorDelete")]
  void ResultIteratorDelete(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorConfidence")]
  float ResultIteratorGetConfidence(HandleRef handle, PageIteratorLevel level);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordFontAttributes")]
  IntPtr ResultIteratorWordFontAttributes(
    HandleRef handle,
    out bool isBold,
    out bool isItalic,
    out bool isUnderlined,
    out bool isMonospace,
    out bool isSerif,
    out bool isSmallCaps,
    out int pointSize,
    out int fontId);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsFromDictionary")]
  bool ResultIteratorWordIsFromDictionary(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsNumeric")]
  bool ResultIteratorWordIsNumeric(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordRecognitionLanguage")]
  IntPtr ResultIteratorWordRecognitionLanguageInternal(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSuperscript")]
  bool ResultIteratorSymbolIsSuperscript(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSubscript")]
  bool ResultIteratorSymbolIsSubscript(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsDropcap")]
  bool ResultIteratorSymbolIsDropcap(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIterator")]
  IntPtr ResultIteratorGetPageIterator(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text")]
  IntPtr ResultIteratorGetUTF8TextInternal(HandleRef handle, PageIteratorLevel level);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetChoiceIterator")]
  IntPtr ResultIteratorGetChoiceIterator(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorDelete")]
  void ChoiceIteratorDelete(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorNext")]
  int ChoiceIteratorNext(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorGetUTF8Text")]
  IntPtr ChoiceIteratorGetUTF8TextInternal(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorConfidence")]
  float ChoiceIteratorGetConfidence(HandleRef handle);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIPrintVariablesToFile")]
  int BaseApiPrintVariablesToFile(HandleRef handle, string filename);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessTextRendererCreate")]
  IntPtr TextRendererCreate(string outputbase);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate")]
  IntPtr HOcrRendererCreate(string outputbase);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate2")]
  IntPtr HOcrRendererCreate2(string outputbase, int font_info);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPDFRendererCreate")]
  IntPtr PDFRendererCreate(string outputbase, IntPtr datadir);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPDFRendererCreateTextonly")]
  IntPtr PDFRendererCreateTextonly(string outputbase, IntPtr datadir, int textonly);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessUnlvRendererCreate")]
  IntPtr UnlvRendererCreate(string outputbase);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBoxTextRendererCreate")]
  IntPtr BoxTextRendererCreate(string outputbase);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteResultRenderer")]
  void DeleteResultRenderer(HandleRef renderer);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererInsert")]
  void ResultRendererInsert(HandleRef renderer, HandleRef next);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererNext")]
  IntPtr ResultRendererNext(HandleRef renderer);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererBeginDocument")]
  int ResultRendererBeginDocument(HandleRef renderer, IntPtr titlePtr);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererAddImage")]
  int ResultRendererAddImage(HandleRef renderer, HandleRef api);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererEndDocument")]
  int ResultRendererEndDocument(HandleRef renderer);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererExtention")]
  IntPtr ResultRendererExtention(HandleRef renderer);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererTitle")]
  IntPtr ResultRendererTitle(HandleRef renderer);

  [RuntimeDllImport("libtesseract3052", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererImageNum")]
  int ResultRendererImageNum(HandleRef renderer);
}

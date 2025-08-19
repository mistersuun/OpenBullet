// Decompiled with JetBrains decompiler
// Type: Tesseract.Interop.ILeptonicaApiSignatures
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using InteropDotNet;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Tesseract.Interop;

public interface ILeptonicaApiSignatures
{
  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaReadMultipageTiff")]
  IntPtr pixaReadMultipageTiff(string filename);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaCreate")]
  IntPtr pixaCreate(int n);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaAddPix")]
  int pixaAddPix(HandleRef pixa, HandleRef pix, PixArrayAccessType copyflag);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaGetPix")]
  IntPtr pixaGetPix(HandleRef pixa, int index, PixArrayAccessType accesstype);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaRemovePix")]
  int pixaRemovePix(HandleRef pixa, int index);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaClear")]
  int pixaClear(HandleRef pixa);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaGetCount")]
  int pixaGetCount(HandleRef pixa);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixaDestroy")]
  void pixaDestroy(ref IntPtr pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCreate")]
  IntPtr pixCreate(int width, int height, int depth);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixClone")]
  IntPtr pixClone(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroy")]
  void pixDestroy(ref IntPtr pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixEqual")]
  int pixEqual(HandleRef pix1, HandleRef pix2, out int same);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWidth")]
  int pixGetWidth(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetHeight")]
  int pixGetHeight(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetDepth")]
  int pixGetDepth(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetXRes")]
  int pixGetXRes(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetYRes")]
  int pixGetYRes(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetResolution")]
  int pixGetResolution(HandleRef pix, out int xres, out int yres);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetWpl")]
  int pixGetWpl(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetXRes")]
  int pixSetXRes(HandleRef pix, int xres);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetYRes")]
  int pixSetYRes(HandleRef pix, int yres);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetResolution")]
  int pixSetResolution(HandleRef pix, int xres, int yres);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixScaleResolution")]
  int pixScaleResolution(HandleRef pix, float xscale, float yscale);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetData")]
  IntPtr pixGetData(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetInputFormat")]
  ImageFormat pixGetInputFormat(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetInputFormat")]
  int pixSetInputFormat(HandleRef pix, ImageFormat inputFormat);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixEndianByteSwap")]
  int pixEndianByteSwap(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRead")]
  IntPtr pixRead(string filename);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixReadMemTiff")]
  unsafe IntPtr pixReadMemTiff(byte* data, int length, int page);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixWrite")]
  int pixWrite(string filename, HandleRef handle, ImageFormat format);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDisplayWrite")]
  int pixDisplayWrite(HandleRef pixs, int reduction);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDisplayMultiple")]
  int pixDisplayMultiple(string filepattern);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixGetColormap")]
  IntPtr pixGetColormap(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSetColormap")]
  int pixSetColormap(HandleRef pix, HandleRef pixCmap);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDestroyColormap")]
  int pixDestroyColormap(HandleRef pix);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixConvertRGBToGray")]
  IntPtr pixConvertRGBToGray(HandleRef pix, float rwt, float gwt, float bwt);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixDeskewGeneral")]
  IntPtr pixDeskewGeneral(
    HandleRef pix,
    int redSweep,
    float sweepRange,
    float sweepDelta,
    int redSearch,
    int thresh,
    out float pAngle,
    out float pConf);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixFindSkew")]
  int pixFindSkew(HandleRef pixs, out float pangle, out float pconf);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotate")]
  IntPtr pixRotate(
    HandleRef pixs,
    float angle,
    RotationMethod type,
    RotationFill fillColor,
    int width,
    int heigh);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotateOrth")]
  IntPtr pixRotateOrth(HandleRef pixs, int quads);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotateAMGray")]
  IntPtr pixRotateAMGray(HandleRef pixs, float angle, byte grayval);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixRotate90")]
  IntPtr pixRotate90(HandleRef pixs, int direction);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCloseGray")]
  IntPtr pixCloseGray(HandleRef pixs, int hsize, int vsize);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixErodeGray")]
  IntPtr pixErodeGray(HandleRef pixs, int hsize, int vsize);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixAddGray")]
  IntPtr pixAddGray(HandleRef pixd, HandleRef pixs1, HandleRef pixs2);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixOpenGray")]
  IntPtr pixOpenGray(HandleRef pixs, int hsize, int vsize);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixCombineMasked")]
  int pixCombineMasked(HandleRef pixd, HandleRef pixs, HandleRef pixm);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixThresholdToValue")]
  IntPtr pixThresholdToValue(HandleRef pixd, HandleRef pixs, int threshval, int setval);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixThresholdToBinary")]
  IntPtr pixThresholdToBinary(HandleRef pixs, int thresh);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixInvert")]
  IntPtr pixInvert(HandleRef pixd, HandleRef pixs);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixOtsuAdaptiveThreshold")]
  int pixOtsuAdaptiveThreshold(
    HandleRef pix,
    int sx,
    int sy,
    int smoothx,
    int smoothy,
    float scorefract,
    out IntPtr ppixth,
    out IntPtr ppixd);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSauvolaBinarize")]
  int pixSauvolaBinarize(
    HandleRef pix,
    int whsize,
    float factor,
    int addborder,
    out IntPtr ppixm,
    out IntPtr ppixsd,
    out IntPtr ppixth,
    out IntPtr ppixd);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixSauvolaBinarizeTiled")]
  int pixSauvolaBinarizeTiled(
    HandleRef pix,
    int whsize,
    float factor,
    int nx,
    int ny,
    out IntPtr ppixth,
    out IntPtr ppixd);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixScale")]
  IntPtr pixScale(HandleRef pixs, float scalex, float scaley);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreate")]
  IntPtr pixcmapCreate(int depth);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreateRandom")]
  IntPtr pixcmapCreateRandom(int depth, int hasBlack, int hasWhite);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCreateLinear")]
  IntPtr pixcmapCreateLinear(int depth, int levels);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCopy")]
  IntPtr pixcmapCopy(HandleRef cmaps);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapDestroy")]
  void pixcmapDestroy(ref IntPtr cmap);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetCount")]
  int pixcmapGetCount(HandleRef cmap);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetFreeCount")]
  int pixcmapGetFreeCount(HandleRef cmap);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetDepth")]
  int pixcmapGetDepth(HandleRef cmap);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetMinDepth")]
  int pixcmapGetMinDepth(HandleRef cmap, out int minDepth);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapClear")]
  int pixcmapClear(HandleRef cmap);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddColor")]
  int pixcmapAddColor(HandleRef cmap, int redValue, int greenValue, int blueValue);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddNewColor")]
  int pixcmapAddNewColor(
    HandleRef cmap,
    int redValue,
    int greenValue,
    int blueValue,
    out int colorIndex);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddNearestColor")]
  int pixcmapAddNearestColor(
    HandleRef cmap,
    int redValue,
    int greenValue,
    int blueValue,
    out int colorIndex);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapUsableColor")]
  int pixcmapUsableColor(
    HandleRef cmap,
    int redValue,
    int greenValue,
    int blueValue,
    out int usable);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapAddBlackOrWhite")]
  int pixcmapAddBlackOrWhite(HandleRef cmap, int color, out int index);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapSetBlackAndWhite")]
  int pixcmapSetBlackAndWhite(HandleRef cmap, int setBlack, int setWhite);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetColor")]
  int pixcmapGetColor(
    HandleRef cmap,
    int index,
    out int redValue,
    out int blueValue,
    out int greenValue);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetColor32")]
  int pixcmapGetColor32(HandleRef cmap, int index, out int color);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapResetColor")]
  int pixcmapResetColor(HandleRef cmap, int index, int redValue, int blueValue, int greenValue);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetIndex")]
  int pixcmapGetIndex(HandleRef cmap, int redValue, int blueValue, int greenValue, out int index);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapHasColor")]
  int pixcmapHasColor(HandleRef cmap, int color);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCountGrayColors")]
  int pixcmapCountGrayColors(HandleRef cmap, out int ngray);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapCountGrayColors")]
  int pixcmapGetRankIntensity(HandleRef cmap, float rankVal, out int index);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetNearestIndex")]
  int pixcmapGetNearestIndex(HandleRef cmap, int rVal, int bVal, int gVal, out int index);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGetNearestGrayIndex")]
  int pixcmapGetNearestGrayIndex(HandleRef cmap, int val, out int index);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGrayToColor")]
  IntPtr pixcmapGrayToColor(int color);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapColorToGray")]
  IntPtr pixcmapColorToGray(HandleRef cmaps, float redWeight, float greenWeight, float blueWeight);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapColorToGray")]
  int pixcmapToArrays(HandleRef cmap, out IntPtr redMap, out IntPtr blueMap, out IntPtr greenMap);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapToRGBTable")]
  int pixcmapToRGBTable(HandleRef cmap, out IntPtr colorTable, out int colorCount);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapSerializeToMemory")]
  int pixcmapSerializeToMemory(
    HandleRef cmap,
    out int components,
    out int colorCount,
    out IntPtr colorData,
    out int colorDataLength);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapDeserializeFromMemory")]
  IntPtr pixcmapDeserializeFromMemory(HandleRef colorData, int colorCount, int colorDataLength);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapGammaTRC")]
  int pixcmapGammaTRC(HandleRef cmap, float gamma, int minVal, int maxVal);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapContrastTRC")]
  int pixcmapContrastTRC(HandleRef cmap, float factor);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pixcmapShiftIntensity")]
  int pixcmapShiftIntensity(HandleRef cmap, float fraction);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaGetCount")]
  int boxaGetCount(HandleRef boxa);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaGetBox")]
  IntPtr boxaGetBox(HandleRef boxa, int index, PixArrayAccessType accesstype);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxGetGeometry")]
  int boxGetGeometry(HandleRef box, out int px, out int py, out int pw, out int ph);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxDestroy")]
  void boxDestroy(ref IntPtr box);

  [RuntimeDllImport("liblept1753", CallingConvention = CallingConvention.Cdecl, EntryPoint = "boxaDestroy")]
  void boxaDestroy(ref IntPtr box);
}

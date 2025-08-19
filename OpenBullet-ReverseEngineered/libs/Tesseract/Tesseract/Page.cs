// Decompiled with JetBrains decompiler
// Type: Tesseract.Page
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Tesseract.Internal;
using Tesseract.Interop;

#nullable disable
namespace Tesseract;

public sealed class Page : DisposableBase
{
  private static readonly TraceSource trace = new TraceSource("Tesseract");
  private bool runRecognitionPhase;
  private Rect regionOfInterest;

  public TesseractEngine Engine { get; private set; }

  public Pix Image { get; private set; }

  public string ImageName { get; private set; }

  public PageSegMode PageSegmentMode { get; private set; }

  internal Page(
    TesseractEngine engine,
    Pix image,
    string imageName,
    Rect regionOfInterest,
    PageSegMode pageSegmentMode)
  {
    this.Engine = engine;
    this.Image = image;
    this.ImageName = imageName;
    this.RegionOfInterest = regionOfInterest;
    this.PageSegmentMode = pageSegmentMode;
  }

  public Rect RegionOfInterest
  {
    get => this.regionOfInterest;
    set
    {
      if (value.X1 < 0 || value.Y1 < 0 || value.X2 > this.Image.Width || value.Y2 > this.Image.Height)
        throw new ArgumentException("The region of interest to be processed must be within the image bounds.", nameof (value));
      if (!(this.regionOfInterest != value))
        return;
      this.regionOfInterest = value;
      TessApi.Native.BaseApiSetRectangle(this.Engine.Handle, this.regionOfInterest.X1, this.regionOfInterest.Y1, this.regionOfInterest.Width, this.regionOfInterest.Height);
      this.runRecognitionPhase = false;
    }
  }

  public Pix GetThresholdedImage()
  {
    this.Recognize();
    IntPtr thresholdedImage = TessApi.Native.BaseAPIGetThresholdedImage(this.Engine.Handle);
    return !(thresholdedImage == IntPtr.Zero) ? Pix.Create(thresholdedImage) : throw new TesseractException("Failed to get thresholded image.");
  }

  public PageIterator AnalyseLayout()
  {
    Guard.Verify(this.PageSegmentMode != 0, "Cannot analyse image layout when using OSD only page segmentation, please use DetectBestOrientation instead.");
    return new PageIterator(this, TessApi.Native.BaseAPIAnalyseLayout(this.Engine.Handle));
  }

  public ResultIterator GetIterator()
  {
    this.Recognize();
    return new ResultIterator(this, TessApi.Native.BaseApiGetIterator(this.Engine.Handle));
  }

  public string GetText()
  {
    this.Recognize();
    return TessApi.BaseAPIGetUTF8Text(this.Engine.Handle);
  }

  public string GetHOCRText(int pageNum, bool useXHtml = false)
  {
    Guard.Require(nameof (pageNum), pageNum >= 0, "Page number must be greater than or equal to zero (0).");
    this.Recognize();
    return useXHtml ? TessApi.BaseAPIGetHOCRText2(this.Engine.Handle, pageNum) : TessApi.BaseAPIGetHOCRText(this.Engine.Handle, pageNum);
  }

  public float GetMeanConfidence()
  {
    this.Recognize();
    return (float) TessApi.Native.BaseAPIMeanTextConf(this.Engine.Handle) / 100f;
  }

  public List<Rectangle> GetSegmentedRegions(PageIteratorLevel pageIteratorLevel)
  {
    IntPtr componentImages = TessApi.Native.BaseAPIGetComponentImages(this.Engine.Handle, pageIteratorLevel, 1, IntPtr.Zero, IntPtr.Zero);
    int count = LeptonicaApi.Native.boxaGetCount(new HandleRef((object) this, componentImages));
    List<Rectangle> segmentedRegions = new List<Rectangle>();
    for (int index = 0; index < count; ++index)
    {
      IntPtr box = LeptonicaApi.Native.boxaGetBox(new HandleRef((object) this, componentImages), index, PixArrayAccessType.Clone);
      if (!(box == IntPtr.Zero))
      {
        int px;
        int py;
        int pw;
        int ph;
        LeptonicaApi.Native.boxGetGeometry(new HandleRef((object) this, box), out px, out py, out pw, out ph);
        segmentedRegions.Add(new Rectangle(px, py, pw, ph));
        LeptonicaApi.Native.boxDestroy(ref box);
      }
    }
    LeptonicaApi.Native.boxaDestroy(ref componentImages);
    return segmentedRegions;
  }

  [Obsolete("Use DetectBestOrientation(int orientationDegrees, float confidence) that returns orientation in degrees instead.")]
  public void DetectBestOrientation(out Orientation orientation, out float confidence)
  {
    int orientation1;
    float confidence1;
    this.DetectBestOrientation(out orientation1, out confidence1);
    int num = orientation1 % 360;
    if (num < 0)
      num += 360;
    orientation = num > 315 || num <= 45 ? Orientation.PageUp : (num <= 45 || num > 135 ? (num <= 135 || num > 225 ? Orientation.PageLeft : Orientation.PageDown) : Orientation.PageRight);
    confidence = confidence1;
  }

  public void DetectBestOrientation(out int orientation, out float confidence)
  {
    this.DetectBestOrientationAndScript(out orientation, out confidence, out string _, out float _);
  }

  public void DetectBestOrientationAndScript(
    out int orientation,
    out float confidence,
    out string scriptName,
    out float scriptConfidence)
  {
    int orient_deg;
    float orient_conf;
    IntPtr script_name;
    float script_conf;
    if (TessApi.Native.TessBaseAPIDetectOrientationScript(this.Engine.Handle, out orient_deg, out orient_conf, out script_name, out script_conf) == 0)
      throw new TesseractException("Failed to detect image orientation.");
    orientation = orient_deg;
    confidence = orient_conf;
    scriptName = !(script_name != IntPtr.Zero) ? (string) null : MarshalHelper.PtrToString(script_name, Encoding.ASCII);
    scriptConfidence = script_conf;
  }

  internal void Recognize()
  {
    Guard.Verify(this.PageSegmentMode != 0, "Cannot OCR image when using OSD only page segmentation, please use DetectBestOrientation instead.");
    if (this.runRecognitionPhase)
      return;
    if (TessApi.Native.BaseApiRecognize(this.Engine.Handle, new HandleRef((object) this, IntPtr.Zero)) != 0)
      throw new InvalidOperationException("Recognition of image failed.");
    this.runRecognitionPhase = true;
    bool flag;
    if (!(this.Engine.TryGetBoolVariable("tessedit_write_images", out flag) & flag))
      return;
    using (Pix thresholdedImage = this.GetThresholdedImage())
    {
      string filename = Path.Combine(Environment.CurrentDirectory, "tessinput.tif");
      try
      {
        thresholdedImage.Save(filename, new ImageFormat?(ImageFormat.TiffG4));
      }
      catch (Exception ex)
      {
      }
    }
  }

  protected override void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    TessApi.Native.BaseAPIClear(this.Engine.Handle);
  }
}

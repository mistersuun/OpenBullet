// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XProcessingInstructionWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Xml.Linq;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XProcessingInstructionWrapper(XProcessingInstruction processingInstruction) : 
  XObjectWrapper((XObject) processingInstruction)
{
  private XProcessingInstruction ProcessingInstruction => (XProcessingInstruction) this.WrappedNode;

  public override string LocalName => this.ProcessingInstruction.Target;

  public override string Value
  {
    get => this.ProcessingInstruction.Data;
    set => this.ProcessingInstruction.Data = value;
  }
}

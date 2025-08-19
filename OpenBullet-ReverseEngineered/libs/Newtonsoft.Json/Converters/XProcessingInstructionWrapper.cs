// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XProcessingInstructionWrapper
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

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

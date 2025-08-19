// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.BinderMappingInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Dynamic;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class BinderMappingInfo
{
  public DynamicMetaObjectBinder Binder;
  public IList<ParameterMappingInfo> MappingInfo;

  public BinderMappingInfo(DynamicMetaObjectBinder binder, IList<ParameterMappingInfo> mappingInfo)
  {
    this.Binder = binder;
    this.MappingInfo = mappingInfo;
  }

  public BinderMappingInfo(
    DynamicMetaObjectBinder binder,
    params ParameterMappingInfo[] mappingInfos)
    : this(binder, (IList<ParameterMappingInfo>) mappingInfos)
  {
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append((object) this.Binder);
    stringBuilder.Append(" ");
    string str = "";
    foreach (ParameterMappingInfo parameterMappingInfo in (IEnumerable<ParameterMappingInfo>) this.MappingInfo)
    {
      stringBuilder.Append(str);
      stringBuilder.Append((object) parameterMappingInfo);
      str = ", ";
    }
    return stringBuilder.ToString();
  }
}

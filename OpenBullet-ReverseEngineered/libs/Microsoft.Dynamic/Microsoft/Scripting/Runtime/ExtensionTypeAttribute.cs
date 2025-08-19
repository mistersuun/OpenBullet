// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ExtensionTypeAttribute
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Runtime;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
public sealed class ExtensionTypeAttribute : Attribute
{
  private readonly Type _extensionType;

  public ExtensionTypeAttribute(Type extends, Type extensionType)
  {
    if (extends == (Type) null)
      throw new ArgumentNullException(nameof (extends));
    if (extensionType != (Type) null && !extensionType.IsPublic && !extensionType.IsNestedPublic)
      throw Error.ExtensionMustBePublic((object) extensionType.FullName);
    this.Extends = extends;
    this._extensionType = extensionType;
  }

  public Type ExtensionType => this._extensionType;

  public Type Extends { get; }
}

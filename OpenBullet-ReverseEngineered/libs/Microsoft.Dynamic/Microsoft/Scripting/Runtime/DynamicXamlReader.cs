// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DynamicXamlReader
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xaml;
using System.Xaml.Schema;
using System.Xml;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class DynamicXamlReader
{
  public static object LoadComponent(
    object scope,
    DynamicOperations operations,
    Stream stream,
    XamlSchemaContext schemaContext)
  {
    return DynamicXamlReader.LoadComponent(scope, operations, new XamlXmlReader(stream, schemaContext ?? new XamlSchemaContext()));
  }

  public static object LoadComponent(
    object scope,
    DynamicOperations operations,
    string filename,
    XamlSchemaContext schemaContext)
  {
    using (StreamReader streamReader = new StreamReader(filename))
      return DynamicXamlReader.LoadComponent(scope, operations, new XamlXmlReader((TextReader) streamReader, schemaContext ?? new XamlSchemaContext()));
  }

  public static object LoadComponent(
    object scope,
    DynamicOperations operations,
    XmlReader reader,
    XamlSchemaContext schemaContext)
  {
    return DynamicXamlReader.LoadComponent(scope, operations, new XamlXmlReader(reader, schemaContext ?? new XamlSchemaContext()));
  }

  public static object LoadComponent(
    object scope,
    DynamicOperations operations,
    TextReader reader,
    XamlSchemaContext schemaContext)
  {
    return DynamicXamlReader.LoadComponent(scope, operations, new XamlXmlReader(reader, schemaContext ?? new XamlSchemaContext()));
  }

  public static object LoadComponent(
    object scope,
    DynamicOperations operations,
    XamlXmlReader reader)
  {
    DynamicXamlReader.DynamicWriter dynamicWriter = new DynamicXamlReader.DynamicWriter(scope, operations, reader.SchemaContext, new XamlObjectWriterSettings()
    {
      RootObjectInstance = scope
    });
    while (reader.Read())
      dynamicWriter.WriteNode((XamlReader) reader);
    foreach (string name1 in dynamicWriter.Names)
    {
      object name2 = dynamicWriter.RootNameScope.FindName(name1);
      if (name2 != null)
        operations.SetMember(scope, name1, name2);
    }
    return dynamicWriter.Result;
  }

  private class DynamicWriter : XamlObjectWriter
  {
    private readonly object _scope;
    private readonly DynamicOperations _operations;
    private readonly Stack<bool> _nameStack = new Stack<bool>();
    private System.Collections.Generic.HashSet<string> _names = new System.Collections.Generic.HashSet<string>();
    private static MethodInfo Dummy = new Action<object, object>(DynamicXamlReader.DynamicWriter.Adder).Method;

    public DynamicWriter(
      object scope,
      DynamicOperations operations,
      XamlSchemaContext context,
      XamlObjectWriterSettings settings)
      : base(context, settings)
    {
      this._scope = scope;
      this._operations = operations;
    }

    public IEnumerable<string> Names => (IEnumerable<string>) this._names;

    public static void Adder(object inst, object args) => throw new InvalidOperationException();

    public override void WriteValue(object value)
    {
      if (this._nameStack.Peek() && value is string str)
        this._names.Add(str);
      base.WriteValue(value);
    }

    public override void WriteEndMember()
    {
      this._nameStack.Pop();
      base.WriteEndMember();
    }

    public override void WriteStartMember(XamlMember property)
    {
      if (property.Name == "Name" && property.Type.UnderlyingType == typeof (string))
        this._nameStack.Push(true);
      else
        this._nameStack.Push(false);
      if (property.UnderlyingMember != (MemberInfo) null && property.UnderlyingMember.MemberType == MemberTypes.Event)
        base.WriteStartMember((XamlMember) new DynamicXamlReader.DynamicWriter.DynamicEventMember(this, (EventInfo) property.UnderlyingMember, this.SchemaContext));
      else
        base.WriteStartMember(property);
    }

    private class DynamicEventMember(
      DynamicXamlReader.DynamicWriter writer,
      EventInfo eventInfo,
      XamlSchemaContext ctx) : XamlMember(eventInfo.Name, DynamicXamlReader.DynamicWriter.Dummy, ctx, (XamlMemberInvoker) new DynamicXamlReader.DynamicWriter.DynamicEventInvoker(eventInfo, writer))
    {
    }

    private class DynamicEventInvoker : XamlMemberInvoker
    {
      private readonly DynamicXamlReader.DynamicWriter _writer;
      private readonly EventInfo _info;

      public DynamicEventInvoker(EventInfo info, DynamicXamlReader.DynamicWriter writer)
      {
        this._writer = writer;
        this._info = info;
      }

      public override void SetValue(object instance, object value)
      {
        object member = this._writer._operations.GetMember(this._writer._scope, (string) value);
        this._info.AddEventHandler(instance, (Delegate) this._writer._operations.ConvertTo(member, this._info.EventHandlerType));
      }
    }
  }
}

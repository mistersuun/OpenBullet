// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonDocumentationProvider
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime;

internal class PythonDocumentationProvider : DocumentationProvider
{
  private readonly PythonContext _context;

  public PythonDocumentationProvider(PythonContext context) => this._context = context;

  public override ICollection<MemberDoc> GetMembers(object value)
  {
    List<MemberDoc> res = new List<MemberDoc>();
    switch (value)
    {
      case PythonModule pythonModule:
        foreach (KeyValuePair<object, object> member in pythonModule.__dict__)
          PythonDocumentationProvider.AddMember(res, member, false);
        return (ICollection<MemberDoc>) res;
      case NamespaceTracker namespaceTracker:
        using (IEnumerator<KeyValuePair<string, object>> enumerator = namespaceTracker.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<string, object> current = enumerator.Current;
            PythonDocumentationProvider.AddMember(res, new KeyValuePair<object, object>((object) current.Key, Importer.MemberTrackerToPython(this._context.SharedClsContext, current.Value)), false);
          }
          goto label_53;
        }
      case OldInstance oldInstance:
        foreach (KeyValuePair<object, object> member in oldInstance.Dictionary)
          PythonDocumentationProvider.AddMember(res, member, false);
        this.AddOldClassMembers(res, oldInstance._class);
        goto label_53;
      case PythonType pythonType:
        using (IEnumerator<PythonType> enumerator = pythonType.ResolutionOrder.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            foreach (KeyValuePair<object, object> member in enumerator.Current.GetMemberDictionary(this._context.SharedContext))
              PythonDocumentationProvider.AddMember(res, member, true);
          }
          break;
        }
      case OldClass oc:
        this.AddOldClassMembers(res, oc);
        break;
      default:
        using (IEnumerator<KeyValuePair<object, object>> enumerator = DynamicHelpers.GetPythonType(value).GetMemberDictionary(this._context.SharedContext).GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<object, object> current = enumerator.Current;
            PythonDocumentationProvider.AddMember(res, current, true);
          }
          break;
        }
    }
    if (value is IPythonObject pythonObject && pythonObject.Dict != null)
    {
      foreach (KeyValuePair<object, object> member in pythonObject.Dict)
        PythonDocumentationProvider.AddMember(res, member, false);
    }
label_53:
    return (ICollection<MemberDoc>) res.ToArray();
  }

  private void AddOldClassMembers(List<MemberDoc> res, OldClass oc)
  {
    foreach (KeyValuePair<object, object> member in oc._dict)
      PythonDocumentationProvider.AddMember(res, member, true);
    foreach (OldClass baseClass in oc.BaseClasses)
      this.AddOldClassMembers(res, baseClass);
  }

  private static void AddMember(
    List<MemberDoc> res,
    KeyValuePair<object, object> member,
    bool fromClass)
  {
    if (!(member.Key is string key))
      return;
    res.Add(PythonDocumentationProvider.MakeMemberDoc(key, member.Value, fromClass));
  }

  private static MemberDoc MakeMemberDoc(string name, object value, bool fromClass)
  {
    MemberKind kind = MemberKind.None;
    switch (value)
    {
      case BuiltinFunction _:
        kind = MemberKind.Function;
        break;
      case NamespaceTracker _:
        kind = MemberKind.Namespace;
        break;
      case PythonFunction _:
        kind = !fromClass ? MemberKind.Function : MemberKind.Method;
        break;
      case BuiltinMethodDescriptor _:
      case Method _:
        kind = MemberKind.Method;
        break;
      case PythonType _:
        PythonType pythonType = value as PythonType;
        kind = !pythonType.IsSystemType || !pythonType.UnderlyingSystemType.IsEnum() ? MemberKind.Class : MemberKind.Enum;
        break;
      default:
        if ((object) (value as Delegate) != null)
        {
          kind = MemberKind.Delegate;
          break;
        }
        switch (value)
        {
          case ReflectedProperty _:
          case ReflectedExtensionProperty _:
            kind = MemberKind.Property;
            break;
          case ReflectedEvent _:
            kind = MemberKind.Event;
            break;
          case ReflectedField _:
            kind = MemberKind.Field;
            break;
          case null:
            switch (value)
            {
              case PythonType _:
              case OldClass _:
                kind = MemberKind.Class;
                break;
              case IPythonObject _:
              case OldInstance _:
                kind = MemberKind.Instance;
                break;
            }
            break;
          default:
            if (value.GetType().IsEnum())
            {
              kind = MemberKind.EnumMember;
              break;
            }
            goto case null;
        }
        break;
    }
    return new MemberDoc(name, kind);
  }

  public override ICollection<OverloadDoc> GetOverloads(object value)
  {
    switch (value)
    {
      case BuiltinFunction bf:
        return PythonDocumentationProvider.GetBuiltinFunctionOverloads(bf);
      case BuiltinMethodDescriptor methodDescriptor:
        return PythonDocumentationProvider.GetBuiltinFunctionOverloads(methodDescriptor.Template);
      case PythonFunction pf:
        return (ICollection<OverloadDoc>) new OverloadDoc[1]
        {
          new OverloadDoc(pf.__name__, pf.__doc__ as string, PythonDocumentationProvider.GetParameterDocs(pf))
        };
      case Method method:
        return this.GetOverloads(method.__func__);
      default:
        Delegate @delegate = value as Delegate;
        if ((object) @delegate == null)
          return (ICollection<OverloadDoc>) new OverloadDoc[0];
        return (ICollection<OverloadDoc>) new OverloadDoc[1]
        {
          DocBuilder.GetOverloadDoc((MethodBase) @delegate.GetType().GetMethod("Invoke"), @delegate.GetType().Name, 0, false)
        };
    }
  }

  private static ICollection<ParameterDoc> GetParameterDocs(PythonFunction pf)
  {
    ParameterDoc[] parameterDocs = new ParameterDoc[pf.ArgNames.Length];
    for (int index = 0; index < parameterDocs.Length; ++index)
    {
      ParameterFlags paramFlags = ParameterFlags.None;
      if (index == pf.ExpandDictPosition)
        paramFlags |= ParameterFlags.ParamsDict;
      else if (index == pf.ExpandListPosition)
        paramFlags |= ParameterFlags.ParamsArray;
      parameterDocs[index] = new ParameterDoc(pf.ArgNames[index], paramFlags);
    }
    return (ICollection<ParameterDoc>) parameterDocs;
  }

  private static ICollection<OverloadDoc> GetBuiltinFunctionOverloads(BuiltinFunction bf)
  {
    OverloadDoc[] functionOverloads = new OverloadDoc[bf.Targets.Count];
    for (int index = 0; index < bf.Targets.Count; ++index)
      functionOverloads[index] = PythonDocumentationProvider.GetOverloadDoc(bf.__name__, bf.Targets[index]);
    return (ICollection<OverloadDoc>) functionOverloads;
  }

  private static OverloadDoc GetOverloadDoc(string name, MethodBase method)
  {
    return DocBuilder.GetOverloadDoc(method, name, 0);
  }
}

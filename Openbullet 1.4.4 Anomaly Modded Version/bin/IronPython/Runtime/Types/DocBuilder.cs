// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.DocBuilder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.XPath;

#nullable disable
namespace IronPython.Runtime.Types;

internal static class DocBuilder
{
  private static readonly object _CachedDocLockObject = new object();
  private static readonly List<Assembly> _AssembliesWithoutXmlDoc = new List<Assembly>();
  private static XPathDocument _CachedDoc;
  private static string _CachedDocName;
  private const string _frameworkReferencePath = "Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.0";

  internal static string GetDefaultDocumentation(string methodName)
  {
    switch (methodName)
    {
      case "__abs__":
        return "x.__abs__() <==> abs(x)";
      case "__add__":
        return "x.__add__(y) <==> x+y";
      case "__call__":
        return "x.__call__(...) <==> x(...)";
      case "__cmp__":
        return "x.__cmp__(y) <==> cmp(x,y)";
      case "__delitem__":
        return "x.__delitem__(y) <==> del x[y]";
      case "__div__":
        return "x.__div__(y) <==> x/y";
      case "__eq__":
        return "x.__eq__(y) <==> x==y";
      case "__floordiv__":
        return "x.__floordiv__(y) <==> x//y";
      case "__getitem__":
        return "x.__getitem__(y) <==> x[y]";
      case "__gt__":
        return "x.__gt__(y) <==> x>y";
      case "__hash__":
        return "x.__hash__() <==> hash(x)";
      case "__init__":
        return "x.__init__(...) initializes x; see x.__class__.__doc__ for signature";
      case "__len__":
        return "x.__len__() <==> len(x)";
      case "__lshift__":
        return "x.__rshift__(y) <==> x<<y";
      case "__lt__":
        return "x.__lt__(y) <==> x<y";
      case "__mod__":
        return "x.__mod__(y) <==> x%y";
      case "__mul__":
        return "x.__mul__(y) <==> x*y";
      case "__neg__":
        return "x.__neg__() <==> -x";
      case "__pow__":
        return "x.__pow__(y[, z]) <==> pow(x, y[, z])";
      case "__reduce__":
      case "__reduce_ex__":
        return "helper for pickle";
      case "__rshift__":
        return "x.__rshift__(y) <==> x>>y";
      case "__setitem__":
        return "x.__setitem__(i, y) <==> x[i]=";
      case "__str__":
        return "x.__str__() <==> str(x)";
      case "__sub__":
        return "x.__sub__(y) <==> x-y";
      case "__truediv__":
        return "x.__truediv__(y) <==> x/y";
      default:
        return (string) null;
    }
  }

  private static string DocOneInfoForProperty(
    Type declaringType,
    string propertyName,
    MethodInfo getter,
    MethodInfo setter,
    IEnumerable<DocumentationAttribute> attrs)
  {
    if (!attrs.Any<DocumentationAttribute>())
    {
      StringBuilder stringBuilder = new StringBuilder();
      string summary = (string) null;
      string returns = (string) null;
      DocBuilder.GetXmlDocForProperty(declaringType, propertyName, out summary, out returns);
      if (summary != null)
      {
        stringBuilder.AppendLine(summary);
        stringBuilder.AppendLine();
      }
      if (getter != (MethodInfo) null)
      {
        stringBuilder.Append("Get: ");
        stringBuilder.AppendLine(DocBuilder.CreateAutoDoc((MethodBase) getter, propertyName, 0));
      }
      if (setter != (MethodInfo) null)
      {
        stringBuilder.Append("Set: ");
        stringBuilder.Append(DocBuilder.CreateAutoDoc((MethodBase) setter, propertyName, 1));
        stringBuilder.AppendLine(" = value");
      }
      return stringBuilder.ToString();
    }
    StringBuilder stringBuilder1 = new StringBuilder();
    foreach (DocumentationAttribute attr in attrs)
    {
      stringBuilder1.Append(attr.Documentation);
      stringBuilder1.Append(Environment.NewLine);
    }
    return stringBuilder1.ToString();
  }

  public static string DocOneInfo(ExtensionPropertyInfo info)
  {
    return DocBuilder.DocOneInfoForProperty(info.DeclaringType, info.Name, info.Getter, info.Setter, Enumerable.Empty<DocumentationAttribute>());
  }

  public static string DocOneInfo(PropertyInfo info)
  {
    IEnumerable<DocumentationAttribute> customAttributes = CustomAttributeExtensions.GetCustomAttributes<DocumentationAttribute>((MemberInfo) info);
    return DocBuilder.DocOneInfoForProperty(info.DeclaringType, info.Name, info.GetGetMethod(), info.GetSetMethod(), customAttributes);
  }

  public static string DocOneInfo(FieldInfo info)
  {
    IEnumerable<DocumentationAttribute> customAttributes = CustomAttributeExtensions.GetCustomAttributes<DocumentationAttribute>((MemberInfo) info);
    return DocBuilder.DocOneInfoForProperty(info.DeclaringType, info.Name, (MethodInfo) null, (MethodInfo) null, customAttributes);
  }

  public static string DocOneInfo(MethodBase info, string name)
  {
    return DocBuilder.DocOneInfo(info, name, true);
  }

  public static string DocOneInfo(MethodBase info, string name, bool includeSelf)
  {
    DocumentationAttribute documentationAttribute = CustomAttributeExtensions.GetCustomAttributes<DocumentationAttribute>((MemberInfo) info).SingleOrDefault<DocumentationAttribute>();
    return documentationAttribute != null ? documentationAttribute.Documentation : DocBuilder.GetDefaultDocumentation(name) ?? DocBuilder.CreateAutoDoc(info, name, 0, includeSelf);
  }

  public static string CreateAutoDoc(MethodBase info)
  {
    return DocBuilder.CreateAutoDoc(info, (string) null, 0);
  }

  public static string CreateAutoDoc(EventInfo info)
  {
    string summary = (string) null;
    DocBuilder.GetXmlDoc(info, out summary, out string _);
    return summary;
  }

  public static string CreateAutoDoc(Type t)
  {
    string summary = (string) null;
    DocBuilder.GetXmlDoc(t, out summary);
    if (t.IsEnum)
    {
      string[] names = Enum.GetNames(t);
      Array values = Enum.GetValues(t);
      for (int index = 0; index < names.Length; ++index)
        names[index] = $"{names[index]} ({Convert.ChangeType(values.GetValue(index), Enum.GetUnderlyingType(t), (IFormatProvider) null).ToString()})";
      Array.Sort<string>(names);
      summary = $"{summary}{Environment.NewLine}{Environment.NewLine}enum {(t.IsDefined(typeof (FlagsAttribute), false) ? "(flags) " : "")}{DocBuilder.GetPythonTypeName(t)}, values: {string.Join(", ", names)}";
    }
    return summary;
  }

  public static OverloadDoc GetOverloadDoc(MethodBase info, string name, int endParamSkip)
  {
    return DocBuilder.GetOverloadDoc(info, name, endParamSkip, true);
  }

  public static OverloadDoc GetOverloadDoc(
    MethodBase info,
    string name,
    int endParamSkip,
    bool includeSelf)
  {
    string summary = (string) null;
    string returns = (string) null;
    List<KeyValuePair<string, string>> parameters1 = (List<KeyValuePair<string, string>>) null;
    DocBuilder.GetXmlDoc(info, out summary, out returns, out parameters1);
    StringBuilder stringBuilder1 = new StringBuilder();
    int num = 0;
    MethodInfo methodInfo = info as MethodInfo;
    if (methodInfo != (MethodInfo) null)
    {
      if (methodInfo.ReturnType != typeof (void))
      {
        stringBuilder1.Append(DocBuilder.GetPythonTypeName(methodInfo.ReturnType));
        ++num;
        try
        {
          object[] customAttributes = methodInfo.ReturnParameter.GetCustomAttributes(typeof (SequenceTypeInfoAttribute), true);
          if (((IEnumerable<object>) customAttributes).Any<object>())
          {
            stringBuilder1.Append(" (of ");
            SequenceTypeInfoAttribute typeInfoAttribute = (SequenceTypeInfoAttribute) ((IEnumerable<object>) customAttributes).First<object>();
            for (int index = 0; index < typeInfoAttribute.Types.Count; ++index)
            {
              if (index != 0)
                stringBuilder1.Append(", ");
              stringBuilder1.Append(DocBuilder.GetPythonTypeName(typeInfoAttribute.Types[index]));
            }
            stringBuilder1.Append(")");
          }
        }
        catch (IndexOutOfRangeException ex)
        {
        }
        try
        {
          object[] customAttributes = methodInfo.ReturnParameter.GetCustomAttributes(typeof (DictionaryTypeInfoAttribute), true);
          if (((IEnumerable<object>) customAttributes).Any<object>())
          {
            DictionaryTypeInfoAttribute typeInfoAttribute = (DictionaryTypeInfoAttribute) ((IEnumerable<object>) customAttributes).First<object>();
            stringBuilder1.Append($" (of {DocBuilder.GetPythonTypeName(typeInfoAttribute.KeyType)} to {DocBuilder.GetPythonTypeName(typeInfoAttribute.ValueType)})");
          }
        }
        catch (IndexOutOfRangeException ex)
        {
        }
      }
      if (name == null)
      {
        int length = methodInfo.Name.IndexOf('#');
        name = length != -1 ? methodInfo.Name.Substring(0, length) : methodInfo.Name;
      }
    }
    else if (name == null)
      name = "__new__";
    if (methodInfo != (MethodInfo) null && methodInfo.IsGenericMethod)
    {
      Type[] genericArguments = methodInfo.GetGenericArguments();
      bool genericParameters = methodInfo.ContainsGenericParameters;
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder2.Append(name);
      stringBuilder2.Append("[");
      if (genericArguments.Length > 1)
        stringBuilder2.Append("(");
      bool flag = false;
      foreach (Type type in genericArguments)
      {
        if (flag)
          stringBuilder2.Append(", ");
        if (genericParameters)
          stringBuilder2.Append(type.Name);
        else
          stringBuilder2.Append(DocBuilder.GetPythonTypeName(type));
        flag = true;
      }
      if (genericArguments.Length > 1)
        stringBuilder2.Append(")");
      stringBuilder2.Append("]");
      name = stringBuilder2.ToString();
    }
    List<ParameterDoc> parameters2 = new List<ParameterDoc>();
    if (methodInfo == (MethodInfo) null)
    {
      if (name == "__new__")
        parameters2.Add(new ParameterDoc("cls", "type"));
    }
    else if (!methodInfo.IsStatic & includeSelf)
      parameters2.Add(new ParameterDoc("self", DocBuilder.GetPythonTypeName(methodInfo.DeclaringType)));
    ParameterInfo[] parameters3 = info.GetParameters();
    for (int index = 0; index < parameters3.Length - endParamSkip; ++index)
    {
      ParameterInfo parameterInfo = parameters3[index];
      if (index != 0 || !(parameterInfo.ParameterType == typeof (CodeContext)))
      {
        if ((parameterInfo.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out || parameterInfo.ParameterType.IsByRef)
        {
          if (num == 1)
            stringBuilder1.Insert(0, "(");
          if (num != 0)
            stringBuilder1.Append(", ");
          ++num;
          stringBuilder1.Append(DocBuilder.GetPythonTypeName(parameterInfo.ParameterType));
          if ((parameterInfo.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out)
            continue;
        }
        ParameterFlags paramFlags = ParameterFlags.None;
        if (parameterInfo.IsDefined(typeof (ParamArrayAttribute), false))
          paramFlags |= ParameterFlags.ParamsArray;
        else if (parameterInfo.IsDefined(typeof (ParamDictionaryAttribute), false))
          paramFlags |= ParameterFlags.ParamsDict;
        string documentation = (string) null;
        if (parameters1 != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in parameters1)
          {
            if (keyValuePair.Key == parameterInfo.Name)
            {
              documentation = keyValuePair.Value;
              break;
            }
          }
        }
        parameters2.Add(new ParameterDoc(parameterInfo.Name ?? "", parameterInfo.ParameterType.IsGenericParameter ? parameterInfo.ParameterType.Name : DocBuilder.GetPythonTypeName(parameterInfo.ParameterType), documentation, paramFlags));
      }
    }
    if (num > 1)
      stringBuilder1.Append(')');
    ParameterDoc returnParameter = new ParameterDoc(string.Empty, stringBuilder1.ToString(), returns);
    return new OverloadDoc(name, summary, (ICollection<ParameterDoc>) parameters2, returnParameter);
  }

  internal static string CreateAutoDoc(MethodBase info, string name, int endParamSkip)
  {
    return DocBuilder.CreateAutoDoc(info, name, endParamSkip, true);
  }

  internal static string CreateAutoDoc(
    MethodBase info,
    string name,
    int endParamSkip,
    bool includeSelf)
  {
    int lineWidth;
    try
    {
      lineWidth = Console.WindowWidth - 30;
    }
    catch
    {
      lineWidth = 80 /*0x50*/;
    }
    OverloadDoc overloadDoc = DocBuilder.GetOverloadDoc(info, name, endParamSkip, includeSelf);
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(overloadDoc.Name);
    stringBuilder.Append("(");
    string str = "";
    foreach (ParameterDoc parameter in (IEnumerable<ParameterDoc>) overloadDoc.Parameters)
    {
      stringBuilder.Append(str);
      if ((parameter.Flags & ParameterFlags.ParamsArray) != ParameterFlags.None)
        stringBuilder.Append('*');
      else if ((parameter.Flags & ParameterFlags.ParamsDict) != ParameterFlags.None)
        stringBuilder.Append("**");
      stringBuilder.Append(parameter.Name);
      if (!string.IsNullOrEmpty(parameter.TypeName))
      {
        stringBuilder.Append(": ");
        stringBuilder.Append(parameter.TypeName);
      }
      str = ", ";
    }
    stringBuilder.Append(")");
    if (!string.IsNullOrEmpty(overloadDoc.ReturnParameter.TypeName))
    {
      stringBuilder.Append(" -> ");
      stringBuilder.AppendLine(overloadDoc.ReturnParameter.TypeName);
    }
    if (!string.IsNullOrEmpty(overloadDoc.Documentation))
    {
      stringBuilder.AppendLine();
      stringBuilder.AppendLine(StringUtils.SplitWords(overloadDoc.Documentation, true, lineWidth));
    }
    bool flag = false;
    foreach (ParameterDoc parameter in (IEnumerable<ParameterDoc>) overloadDoc.Parameters)
    {
      if (!string.IsNullOrEmpty(parameter.Documentation))
      {
        if (!flag)
        {
          stringBuilder.AppendLine();
          flag = true;
        }
        stringBuilder.Append("    ");
        stringBuilder.Append(parameter.Name);
        stringBuilder.Append(": ");
        stringBuilder.AppendLine(StringUtils.SplitWords(parameter.Documentation, false, lineWidth));
      }
    }
    if (!string.IsNullOrEmpty(overloadDoc.ReturnParameter.Documentation))
    {
      stringBuilder.Append("    Returns: ");
      stringBuilder.AppendLine(StringUtils.SplitWords(overloadDoc.ReturnParameter.Documentation, false, lineWidth));
    }
    return stringBuilder.ToString();
  }

  private static string GetPythonTypeName(Type type)
  {
    if (type.IsByRef)
      type = type.GetElementType();
    return DynamicHelpers.GetPythonTypeFromType(type).Name;
  }

  private static string GetXmlName(Type type)
  {
    StringBuilder res = new StringBuilder();
    res.Append("T:");
    DocBuilder.AppendTypeFormat(type, res);
    return res.ToString();
  }

  private static string GetXmlName(EventInfo field)
  {
    StringBuilder res = new StringBuilder();
    res.Append("E:");
    DocBuilder.AppendTypeFormat(field.DeclaringType, res);
    res.Append('.');
    res.Append(field.Name);
    return res.ToString();
  }

  private static string GetXmlNameForProperty(Type declaringType, string propertyName)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("P:");
    if (!string.IsNullOrEmpty(declaringType.Namespace))
    {
      stringBuilder.Append(declaringType.Namespace);
      stringBuilder.Append('.');
    }
    stringBuilder.Append(declaringType.Name);
    stringBuilder.Append('.');
    stringBuilder.Append(propertyName);
    return stringBuilder.ToString();
  }

  private static string GetXmlName(MethodBase info)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("M:");
    if (!string.IsNullOrEmpty(info.DeclaringType.Namespace))
    {
      stringBuilder.Append(info.DeclaringType.Namespace);
      stringBuilder.Append('.');
    }
    stringBuilder.Append(info.DeclaringType.Name);
    stringBuilder.Append('.');
    stringBuilder.Append(info.Name);
    ParameterInfo[] parameters = info.GetParameters();
    if (parameters.Length != 0)
    {
      stringBuilder.Append('(');
      for (int index = 0; index < parameters.Length; ++index)
      {
        Type parameterType = parameters[index].ParameterType;
        if (index != 0)
          stringBuilder.Append(',');
        StringBuilder res = stringBuilder;
        ParameterInfo pi = parameters[index];
        DocBuilder.AppendTypeFormat(parameterType, res, pi);
      }
      stringBuilder.Append(')');
    }
    return stringBuilder.ToString();
  }

  private static void AppendTypeFormat(Type curType, StringBuilder res, ParameterInfo pi = null)
  {
    if (curType.IsGenericType)
      curType = curType.GetGenericTypeDefinition();
    if (curType.IsGenericParameter)
    {
      res.Append('`');
      res.Append(curType.GenericParameterPosition);
    }
    else if (curType.ContainsGenericParameters)
    {
      if (!string.IsNullOrEmpty(curType.Namespace))
      {
        res.Append(curType.Namespace);
        res.Append('.');
      }
      res.Append(curType.Name.Substring(0, curType.Name.Length - 2));
      res.Append('{');
      Type[] genericArguments = curType.GetGenericArguments();
      for (int index = 0; index < genericArguments.Length; ++index)
      {
        if (index != 0)
          res.Append(',');
        if (genericArguments[index].IsGenericParameter)
        {
          res.Append('`');
          res.Append(genericArguments[index].GenericParameterPosition);
        }
        else
          DocBuilder.AppendTypeFormat(genericArguments[index], res);
      }
      res.Append('}');
    }
    else if (pi != null)
    {
      if ((pi.IsOut || pi.ParameterType.IsByRef) && curType.FullName.EndsWith("&"))
        res.Append(curType.FullName.Replace("&", "@"));
      else
        res.Append(curType.FullName);
    }
    else
      res.Append(curType.FullName);
  }

  private static string GetXmlDocLocation(Assembly assem)
  {
    if (DocBuilder._AssembliesWithoutXmlDoc.Contains(assem))
      return (string) null;
    string xmlDocLocation = (string) null;
    try
    {
      xmlDocLocation = assem.Location;
    }
    catch (SecurityException ex)
    {
    }
    catch (NotSupportedException ex)
    {
    }
    if (!string.IsNullOrEmpty(xmlDocLocation))
      return xmlDocLocation;
    DocBuilder._AssembliesWithoutXmlDoc.Add(assem);
    return (string) null;
  }

  private static XPathDocument GetXPathDocument(Assembly asm)
  {
    CultureInfo currentCulture = CultureInfo.CurrentCulture;
    string xmlDocLocation = DocBuilder.GetXmlDocLocation(asm);
    if (xmlDocLocation == null)
      return (XPathDocument) null;
    string directoryName = Path.GetDirectoryName(xmlDocLocation);
    string path2 = Path.GetFileNameWithoutExtension(xmlDocLocation) + ".xml";
    string str1 = Path.Combine(Path.Combine(directoryName, currentCulture.Name), path2);
    bool flag = false;
    if (!File.Exists(str1))
    {
      int length = currentCulture.Name.IndexOf('-');
      if (length != -1)
        str1 = Path.Combine(Path.Combine(directoryName, currentCulture.Name.Substring(0, length)), path2);
      if (!File.Exists(str1))
      {
        str1 = Path.Combine(directoryName, path2);
        if (!File.Exists(str1))
        {
          str1 = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.Is64BitProcess ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles), "Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.0"), path2);
          flag = true;
          if (!File.Exists(str1))
          {
            DocBuilder._AssembliesWithoutXmlDoc.Add(asm);
            return (XPathDocument) null;
          }
        }
      }
    }
    XPathDocument xpathDocument;
    lock (DocBuilder._CachedDocLockObject)
    {
      if (DocBuilder._CachedDocName == str1)
      {
        xpathDocument = DocBuilder._CachedDoc;
      }
      else
      {
        xpathDocument = new XPathDocument(str1);
        if (flag)
        {
          XPathNavigator xpathNavigator = xpathDocument.CreateNavigator().SelectSingleNode("/doc/@redirect");
          if (xpathNavigator != null)
          {
            string str2 = xpathNavigator.Value.Replace("%PROGRAMFILESDIR%", Environment.GetFolderPath(Environment.Is64BitProcess ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles) + Path.DirectorySeparatorChar.ToString());
            if (File.Exists(str2))
              xpathDocument = new XPathDocument(str2);
          }
        }
      }
      DocBuilder._CachedDoc = xpathDocument;
      DocBuilder._CachedDocName = str1;
    }
    return xpathDocument;
  }

  private static void GetXmlDoc(
    MethodBase info,
    out string summary,
    out string returns,
    out List<KeyValuePair<string, string>> parameters)
  {
    summary = (string) null;
    returns = (string) null;
    parameters = (List<KeyValuePair<string, string>>) null;
    XPathDocument xpathDocument = DocBuilder.GetXPathDocument(info.DeclaringType.Assembly);
    if (xpathDocument == null)
      return;
    XPathNodeIterator iter = xpathDocument.CreateNavigator().Select($"/doc/members/member[@name='{DocBuilder.GetXmlName(info)}']/*");
    while (iter.MoveNext())
    {
      switch (iter.Current.Name)
      {
        case nameof (summary):
          summary = DocBuilder.XmlToString(iter);
          continue;
        case nameof (returns):
          returns = DocBuilder.XmlToString(iter);
          continue;
        case "param":
          string key = (string) null;
          string str = DocBuilder.XmlToString(iter);
          if (iter.Current.MoveToFirstAttribute())
            key = iter.Current.Value;
          if (key != null)
          {
            if (parameters == null)
              parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>(key, str.Trim()));
            continue;
          }
          continue;
        default:
          continue;
      }
    }
  }

  private static void GetXmlDoc(Type type, out string summary)
  {
    summary = (string) null;
    XPathDocument xpathDocument = DocBuilder.GetXPathDocument(type.Assembly);
    if (xpathDocument == null)
      return;
    XPathNodeIterator iter = xpathDocument.CreateNavigator().Select($"/doc/members/member[@name='{DocBuilder.GetXmlName(type)}']/*");
    while (iter.MoveNext())
    {
      if (iter.Current.Name == nameof (summary))
        summary = DocBuilder.XmlToString(iter);
    }
  }

  private static void GetXmlDocForProperty(
    Type declaringType,
    string propertyName,
    out string summary,
    out string returns)
  {
    summary = (string) null;
    returns = (string) null;
    XPathDocument xpathDocument = DocBuilder.GetXPathDocument(declaringType.Assembly);
    if (xpathDocument == null)
      return;
    XPathNodeIterator iter = xpathDocument.CreateNavigator().Select($"/doc/members/member[@name='{DocBuilder.GetXmlNameForProperty(declaringType, propertyName)}']/*");
    while (iter.MoveNext())
    {
      switch (iter.Current.Name)
      {
        case nameof (summary):
          summary = DocBuilder.XmlToString(iter);
          continue;
        case nameof (returns):
          returns = DocBuilder.XmlToString(iter);
          continue;
        default:
          continue;
      }
    }
  }

  private static void GetXmlDoc(EventInfo info, out string summary, out string returns)
  {
    summary = (string) null;
    returns = (string) null;
    XPathDocument xpathDocument = DocBuilder.GetXPathDocument(info.DeclaringType.Assembly);
    if (xpathDocument == null)
      return;
    XPathNodeIterator iter = xpathDocument.CreateNavigator().Select($"/doc/members/member[@name='{DocBuilder.GetXmlName(info)}']/*");
    while (iter.MoveNext())
    {
      if (iter.Current.Name == nameof (summary))
        summary = DocBuilder.XmlToString(iter) + Environment.NewLine;
    }
  }

  private static string XmlToString(XPathNodeIterator iter)
  {
    XmlReader xmlReader = iter.Current.ReadSubtree();
    StringBuilder stringBuilder = new StringBuilder();
    if (xmlReader.Read())
    {
      do
      {
        switch (xmlReader.NodeType)
        {
          case XmlNodeType.Element:
            switch (xmlReader.Name)
            {
              case "see":
                if (xmlReader.MoveToFirstAttribute() && xmlReader.ReadAttributeValue())
                {
                  int num = xmlReader.Value.IndexOf('`');
                  if (num != -1)
                  {
                    stringBuilder.Append(xmlReader.Value, 2, num - 2);
                    break;
                  }
                  stringBuilder.Append(xmlReader.Value, 2, xmlReader.Value.Length - 2);
                  break;
                }
                break;
              case "paramref":
                if (xmlReader.MoveToAttribute("name"))
                {
                  stringBuilder.Append(xmlReader.Value);
                  break;
                }
                break;
            }
            break;
          case XmlNodeType.Text:
            stringBuilder.Append(xmlReader.ReadContentAsString());
            continue;
        }
      }
      while (xmlReader.Read());
    }
    return stringBuilder.ToString().Trim();
  }
}

// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonExpat
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

#nullable disable
namespace IronPython.Modules;

public static class PythonExpat
{
  public const int XML_PARAM_ENTITY_PARSING_NEVER = 0;
  public const int XML_PARAM_ENTITY_PARSING_UNLESS_STANDALONE = 1;
  public const int XML_PARAM_ENTITY_PARSING_ALWAYS = 2;
  private static readonly object _errorsKey = new object();

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    PythonType pythonType = context.EnsureModuleException((object) "pyexpaterror", dict, "ExpatError", "xml.parsers.expat");
    dict[(object) "error"] = (object) pythonType;
    dict[(object) "ExpatError"] = (object) pythonType;
    context.GetOrCreateModuleState<PythonDictionary>(PythonExpat._errorsKey, (Func<PythonDictionary>) (() =>
    {
      dict.Add((object) "errors", (object) new PythonExpat.PyExpatErrors());
      return dict;
    }));
  }

  public static string ErrorString(int errno)
  {
    return typeof (PythonExpat.XmlErrors).IsEnumDefined((object) errno) ? PythonExpat.PyExpatErrors.GetFieldDescription((PythonExpat.XmlErrors) errno) : (string) null;
  }

  public static object ParserCreate(
    CodeContext context,
    [ParamDictionary] IDictionary<object, object> kwArgsø,
    params object[] argsø)
  {
    int num = argsø.Length + kwArgsø.Count;
    if (num > 3)
      throw PythonOps.TypeError($"ParserCreate() takes at most 3 arguments ({num} given)");
    object encoding = argsø.Length != 0 ? argsø[0] : (object) null;
    object namespace_separator = argsø.Length > 1 ? argsø[1] : (object) null;
    object intern = argsø.Length > 2 ? argsø[0] : (object) new PythonDictionary();
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) kwArgsø)
    {
      object key = keyValuePair.Key;
      if (key != null && key is string str)
      {
        switch (str)
        {
          case "encoding":
            if (argsø.Length != 0)
              throw PythonOps.TypeError("Argument given by name('encoding') and position(1)");
            encoding = keyValuePair.Value;
            continue;
          case "namespace_separator":
            if (argsø.Length > 1)
              throw PythonOps.TypeError("Argument given by name('namespace_separator') and position(2)");
            namespace_separator = keyValuePair.Value;
            continue;
          case "intern":
            intern = keyValuePair.Value;
            continue;
        }
      }
      throw PythonOps.TypeError($"'{keyValuePair.Key}' is an invalid keyword argument for this function");
    }
    return PythonExpat.ParserCreate(encoding, namespace_separator, intern);
  }

  public static object ParserCreate(object encoding, object namespace_separator, object intern)
  {
    string str1;
    if (encoding != null)
      str1 = encoding is string str2 ? str2 : throw PythonOps.TypeError("ParserCreate() argument 1 must be string or None, not " + PythonTypeOps.GetName(encoding));
    else
      str1 = (string) null;
    string encoding1 = str1;
    string str3;
    if (namespace_separator != null)
      str3 = namespace_separator is string str4 ? str4 : throw PythonOps.TypeError("ParserCreate() argument 2 must be string or None, not " + PythonTypeOps.GetName(namespace_separator));
    else
      str3 = (string) null;
    string namespace_separator1 = str3;
    return PythonExpat.ParserCreateImpl(encoding1, namespace_separator1, intern);
  }

  private static object ParserCreateImpl(
    string encoding,
    string namespace_separator,
    object intern)
  {
    if (namespace_separator != null && namespace_separator.Length > 1)
      throw PythonOps.ValueError("namespace_separator must be at most one character, omitted, or None");
    return (object) new PythonExpat.xmlparser(encoding, namespace_separator, intern);
  }

  public static object XMLParserType => (object) typeof (PythonExpat.xmlparser);

  internal enum XmlErrors
  {
    [Description("out of memory")] XML_ERROR_NO_MEMORY = 1,
    [Description("syntax error")] XML_ERROR_SYNTAX = 2,
    [Description("no element found")] XML_ERROR_NO_ELEMENTS = 3,
    [Description("not well-formed (invalid token)")] XML_ERROR_INVALID_TOKEN = 4,
    [Description("unclosed token")] XML_ERROR_UNCLOSED_TOKEN = 5,
    [Description("partial character")] XML_ERROR_PARTIAL_CHAR = 6,
    [Description("mismatched tag")] XML_ERROR_TAG_MISMATCH = 7,
    [Description("duplicate attribute")] XML_ERROR_DUPLICATE_ATTRIBUTE = 8,
    [Description("junk after document element")] XML_ERROR_JUNK_AFTER_DOC_ELEMENT = 9,
    [Description("illegal parameter entity reference")] XML_ERROR_PARAM_ENTITY_REF = 10, // 0x0000000A
    [Description("undefined entity")] XML_ERROR_UNDEFINED_ENTITY = 11, // 0x0000000B
    [Description("recursive entity reference")] XML_ERROR_RECURSIVE_ENTITY_REF = 12, // 0x0000000C
    [Description("asynchronous entity")] XML_ERROR_ASYNC_ENTITY = 13, // 0x0000000D
    [Description("reference to invalid character number")] XML_ERROR_BAD_CHAR_REF = 14, // 0x0000000E
    [Description("reference to binary entity")] XML_ERROR_BINARY_ENTITY_REF = 15, // 0x0000000F
    [Description("reference to external entity in attribute")] XML_ERROR_ATTRIBUTE_EXTERNAL_ENTITY_REF = 16, // 0x00000010
    [Description("XML or text declaration not at start of entity")] XML_ERROR_MISPLACED_XML_PI = 17, // 0x00000011
    [Description("unknown encoding")] XML_ERROR_UNKNOWN_ENCODING = 18, // 0x00000012
    [Description("entity declared in parameter entity")] XML_ERROR_ENTITY_DECLARED_IN_PE = 19, // 0x00000013
    [Description("requested feature requires XML_DTD support in Expat")] XML_ERROR_FEATURE_REQUIRES_XML_DTD = 20, // 0x00000014
    [Description("cannot change setting once parsing has begun")] XML_ERROR_CANT_CHANGE_FEATURE_ONCE_PARSING = 21, // 0x00000015
    [Description("unbound prefix")] XML_ERROR_UNBOUND_PREFIX = 22, // 0x00000016
    [Description("must not undeclare prefix")] XML_ERROR_UNDECLARING_PREFIX = 23, // 0x00000017
    [Description("incomplete markup in parameter entity")] XML_ERROR_INCOMPLETE_PE = 24, // 0x00000018
    [Description("XML declaration not well-formed")] XML_ERROR_XML_DECL = 25, // 0x00000019
    [Description("text declaration not well-formed")] XML_ERROR_TEXT_DECL = 26, // 0x0000001A
    [Description("illegal character(s) in public id")] XML_ERROR_PUBLICID = 27, // 0x0000001B
    [Description("parser suspended")] XML_ERROR_SUSPENDED = 28, // 0x0000001C
    [Description("parser not suspended")] XML_ERROR_NOT_SUSPENDED = 29, // 0x0000001D
    [Description("parsing aborted")] XML_ERROR_ABORTED = 30, // 0x0000001E
    [Description("parsing finished")] XML_ERROR_FINISHED = 31, // 0x0000001F
    [Description("cannot suspend in external parameter entity")] XML_ERROR_SUSPEND_PE = 32, // 0x00000020
    [Description("encoding specified in XML declaration is incorrect")] XML_ERROR_INCORRECT_ENCODING = 33, // 0x00000021
    [Description("unclosed CDATA section")] XML_ERROR_UNCLOSED_CDATA_SECTION = 34, // 0x00000022
    [Description("error in processing external entity reference")] XML_ERROR_EXTERNAL_ENTITY_HANDLING = 35, // 0x00000023
    [Description("document is not standalone")] XML_ERROR_NOT_STANDALONE = 36, // 0x00000024
    [Description("unexpected parser state - please send a bug report")] XML_ERROR_UNEXPECTED_STATE = 37, // 0x00000025
  }

  [PythonHidden(new PlatformID[] {})]
  public class PyExpatErrors : IPythonMembersList, IMembersList
  {
    [SpecialName]
    public object GetCustomMember(CodeContext context, string name)
    {
      FieldInfo field = typeof (PythonExpat.XmlErrors).GetField(name);
      if (field != (FieldInfo) null)
      {
        DescriptionAttribute[] customAttributes = (DescriptionAttribute[]) field.GetCustomAttributes(typeof (DescriptionAttribute), false);
        if (customAttributes != null && customAttributes.Length != 0)
          return (object) customAttributes[0].Description;
      }
      return (object) OperationFailed.Value;
    }

    IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
    {
      throw new NotImplementedException();
    }

    IList<string> IMembersList.GetMemberNames() => throw new NotImplementedException();

    internal static string GetFieldDescription(PythonExpat.XmlErrors value)
    {
      DescriptionAttribute[] customAttributes = (DescriptionAttribute[]) typeof (PythonExpat.XmlErrors).GetField(value.ToString()).GetCustomAttributes(typeof (DescriptionAttribute), false);
      return customAttributes != null && customAttributes.Length != 0 ? customAttributes[0].Description : value.ToString();
    }
  }

  [PythonType]
  public class xmlparser
  {
    private readonly StringBuilder text_buffer = new StringBuilder();
    private int _buffer_size = 8192 /*0x2000*/;
    private bool _use_foreign_dtd = true;
    private bool _parsing_done;
    private readonly string namespace_separator;
    private XmlReader xmlReader;
    private readonly Stack<string> ns_stack = new Stack<string>();
    private MemoryStream buffer = new MemoryStream();

    public object buffer_size
    {
      get => (object) this._buffer_size;
      set
      {
        if (!(value is int num))
          throw PythonOps.TypeError("buffer_size must be an integer");
        if (num <= 0)
          throw PythonOps.ValueError("buffer_size must be greater than zero");
        this.FlushBuffer();
        this._buffer_size = num;
      }
    }

    public bool buffer_text { get; set; }

    public int buffer_used { get; }

    public bool namespace_prefixes { get; set; }

    public bool ordered_attributes { get; set; }

    public bool returns_unicode { get; set; } = true;

    public bool specified_attributes { get; set; }

    public Action<string, string, string, string, bool> AttlistDeclHandler { get; set; }

    public Action<string> CharacterDataHandler { get; set; }

    public Action<string> CommentHandler { get; set; }

    public Action<string> DefaultHandler { get; set; }

    public Action<string> DefaultHandlerExpand { get; set; }

    public Action<string, object> ElementDeclHandler { get; set; }

    public Action EndCdataSectionHandler { get; set; }

    public Action EndDoctypeDeclHandler { get; set; }

    public Action<string> EndElementHandler { get; set; }

    public Action<string> EndNamespaceDeclHandler { get; set; }

    public Action<string, bool, string, string, string, string, string> EntityDeclHandler { get; set; }

    public Action<string, string, string, string> ExternalEntityRefHandler { get; set; }

    public Action NotStandaloneHandler { get; set; }

    public Action<string, string, string, string> NotationDeclHandler { get; set; }

    public Action<string, object> ProcessingInstructionHandler { get; set; }

    public Action<string, object> SkippedEntityHandler { get; set; }

    public Action StartCdataSectionHandler { get; set; }

    public PythonExpat.xmlparser.StartDoctypeDeclHandlerDelegate StartDoctypeDeclHandler { get; set; }

    public Action<string, object> StartElementHandler { get; set; }

    public Action<string, string> StartNamespaceDeclHandler { get; set; }

    public Action<string, string, string, string, string> UnparsedEntityDeclHandler { get; set; }

    public PythonExpat.xmlparser.XmlDeclHandlerDelegate XmlDeclHandler { get; set; }

    public object intern { get; set; }

    public xmlparser(string encoding, string namespace_separator, object intern)
    {
      this.intern = intern;
      this.namespace_separator = namespace_separator;
    }

    public bool SetParamEntityParsing(int flag) => throw new NotImplementedException();

    public void UseForeignDTD(bool use_foreign_dtd = true)
    {
      this._use_foreign_dtd = use_foreign_dtd;
    }

    public int CurrentColumnNumber
    {
      get => !(this.xmlReader is IXmlLineInfo xmlReader) ? 0 : xmlReader.LinePosition;
    }

    public int CurrentLineNumber
    {
      get => !(this.xmlReader is IXmlLineInfo xmlReader) ? 0 : xmlReader.LineNumber;
    }

    public long CurrentByteIndex { get; private set; }

    private void parse(CodeContext context)
    {
      try
      {
        while (this.xmlReader.Read())
        {
          switch (this.xmlReader.NodeType)
          {
            case XmlNodeType.Element:
              this.handleElement();
              continue;
            case XmlNodeType.Text:
              this.BufferText(this.xmlReader.Value);
              continue;
            case XmlNodeType.CDATA:
              this.handleCDATA();
              continue;
            case XmlNodeType.ProcessingInstruction:
              this.handleProcessingInstruction();
              continue;
            case XmlNodeType.Comment:
              this.handleComment();
              continue;
            case XmlNodeType.DocumentType:
              this.handleDocumentType();
              continue;
            case XmlNodeType.Whitespace:
            case XmlNodeType.SignificantWhitespace:
              if (this.xmlReader.Depth > 0)
              {
                this.BufferText(this.xmlReader.Value);
                continue;
              }
              continue;
            case XmlNodeType.EndElement:
              this.handleEndElement();
              continue;
            case XmlNodeType.XmlDeclaration:
              this.handleXmlDeclaration();
              continue;
            default:
              continue;
          }
        }
      }
      catch (XmlException ex)
      {
        throw PythonExpat.xmlparser.Error(context, ex);
      }
      this.FlushBuffer();
    }

    private string qname()
    {
      if (this.namespace_separator == null)
        return this.xmlReader.Name;
      if (string.IsNullOrEmpty(this.xmlReader.NamespaceURI))
        return this.xmlReader.LocalName;
      string str = this.xmlReader.NamespaceURI + this.namespace_separator + this.xmlReader.LocalName;
      return this.namespace_prefixes && !string.IsNullOrEmpty(this.xmlReader.Prefix) ? str + this.namespace_separator + this.xmlReader.Prefix : str;
    }

    private void handleElement()
    {
      string str1 = this.qname();
      this.ns_stack.Push((string) null);
      IronPython.Runtime.List list = (IronPython.Runtime.List) null;
      PythonDictionary pythonDictionary = (PythonDictionary) null;
      if (this.ordered_attributes)
        list = new IronPython.Runtime.List();
      else
        pythonDictionary = new PythonDictionary();
      while (this.xmlReader.MoveToNextAttribute())
      {
        if (this.namespace_separator != null && (this.xmlReader.Prefix == "xmlns" || this.xmlReader.Prefix == string.Empty && this.xmlReader.LocalName == "xmlns"))
        {
          string str2 = this.xmlReader.Prefix == string.Empty ? string.Empty : this.xmlReader.LocalName;
          string str3 = this.xmlReader.Value;
          this.ns_stack.Push(str2);
          Action<string, string> namespaceDeclHandler = this.StartNamespaceDeclHandler;
          if (namespaceDeclHandler != null)
          {
            this.FlushBuffer();
            namespaceDeclHandler(str2 == string.Empty ? (string) null : str2, str3);
          }
        }
        else
        {
          string key = this.qname();
          string str4 = this.xmlReader.Value;
          if (this.ordered_attributes)
          {
            list.append((object) key);
            list.append((object) str4);
          }
          else
            pythonDictionary[(object) key] = (object) str4;
        }
      }
      this.xmlReader.MoveToElement();
      Action<string, object> startElementHandler = this.StartElementHandler;
      if (startElementHandler != null)
      {
        this.FlushBuffer();
        startElementHandler(str1, this.ordered_attributes ? (object) list : (object) pythonDictionary);
      }
      if (!this.xmlReader.IsEmptyElement)
        return;
      this.handleEndElement();
    }

    private void handleEndElement()
    {
      string str1 = this.qname();
      Action<string> endElementHandler = this.EndElementHandler;
      if (endElementHandler != null)
      {
        this.FlushBuffer();
        endElementHandler(str1);
      }
      while (true)
      {
        string str2;
        Action<string> namespaceDeclHandler;
        do
        {
          str2 = this.ns_stack.Pop();
          if (str2 != null)
            namespaceDeclHandler = this.EndNamespaceDeclHandler;
          else
            goto label_5;
        }
        while (namespaceDeclHandler == null);
        this.FlushBuffer();
        namespaceDeclHandler(str2 == string.Empty ? (string) null : str2);
      }
label_5:;
    }

    private void handleComment()
    {
      Action<string> commentHandler = this.CommentHandler;
      if (commentHandler == null)
        return;
      this.FlushBuffer();
      commentHandler(this.xmlReader.Value);
    }

    private void handleCDATA()
    {
      Action cdataSectionHandler1 = this.StartCdataSectionHandler;
      if (cdataSectionHandler1 != null)
      {
        this.FlushBuffer();
        cdataSectionHandler1();
      }
      this.BufferText(this.xmlReader.Value);
      Action cdataSectionHandler2 = this.EndCdataSectionHandler;
      if (cdataSectionHandler2 == null)
        return;
      this.FlushBuffer();
      cdataSectionHandler2();
    }

    private void BufferText(string text)
    {
      if (this.buffer_text)
      {
        this.text_buffer.Append(text);
        while (this.text_buffer.Length >= this._buffer_size)
        {
          text = this.text_buffer.Remove(0, this._buffer_size).ToString();
          Action<string> characterDataHandler = this.CharacterDataHandler;
          if (characterDataHandler != null)
            characterDataHandler(text);
        }
      }
      else if (text.Contains("\n"))
      {
        string[] strArray = text.Split('\n');
        int index;
        for (index = 0; index < strArray.Length - 1; ++index)
        {
          string str = strArray[index];
          if (str.Length != 0)
          {
            Action<string> characterDataHandler = this.CharacterDataHandler;
            if (characterDataHandler != null)
              characterDataHandler(str);
          }
          Action<string> characterDataHandler1 = this.CharacterDataHandler;
          if (characterDataHandler1 != null)
            characterDataHandler1("\n");
        }
        Action<string> characterDataHandler2 = this.CharacterDataHandler;
        if (characterDataHandler2 == null)
          return;
        characterDataHandler2(strArray[index]);
      }
      else
      {
        Action<string> characterDataHandler = this.CharacterDataHandler;
        if (characterDataHandler == null)
          return;
        characterDataHandler(text);
      }
    }

    private void FlushBuffer()
    {
      if (!this.buffer_text || this.text_buffer.Length <= 0)
        return;
      string str = this.text_buffer.ToString();
      Action<string> characterDataHandler = this.CharacterDataHandler;
      if (characterDataHandler != null)
        characterDataHandler(str);
      this.text_buffer.Clear();
    }

    private void handleProcessingInstruction()
    {
      Action<string, object> instructionHandler = this.ProcessingInstructionHandler;
      if (instructionHandler == null)
        return;
      this.FlushBuffer();
      instructionHandler(this.qname(), (object) this.xmlReader.Value);
    }

    private void handleDocumentType()
    {
      string name = this.xmlReader.Name;
      string systemId = (string) null;
      string publicId = (string) null;
      bool has_internal_subset = !string.IsNullOrEmpty(this.xmlReader.Value);
      while (this.xmlReader.MoveToNextAttribute())
      {
        if (this.xmlReader.Name == "SYSTEM")
          systemId = this.xmlReader.Value;
        else if (this.xmlReader.Name == "PUBLIC")
          publicId = this.xmlReader.Value;
      }
      PythonExpat.xmlparser.StartDoctypeDeclHandlerDelegate doctypeDeclHandler1 = this.StartDoctypeDeclHandler;
      if (doctypeDeclHandler1 != null)
        doctypeDeclHandler1(name, systemId, publicId, has_internal_subset);
      Action doctypeDeclHandler2 = this.EndDoctypeDeclHandler;
      if (doctypeDeclHandler2 == null)
        return;
      doctypeDeclHandler2();
    }

    private void handleXmlDeclaration()
    {
      string version = (string) null;
      string encoding = (string) null;
      int standalone = -1;
      while (this.xmlReader.MoveToNextAttribute())
      {
        if (this.xmlReader.Name == "version")
          version = this.xmlReader.Value;
        else if (this.xmlReader.Name == "encoding")
          encoding = this.xmlReader.Value;
        else if (this.xmlReader.Name == "standalone")
          standalone = this.xmlReader.Value == "yes" ? 1 : 0;
      }
      PythonExpat.xmlparser.XmlDeclHandlerDelegate xmlDeclHandler = this.XmlDeclHandler;
      if (xmlDeclHandler == null)
        return;
      xmlDeclHandler(version, encoding, standalone);
    }

    public void Parse(CodeContext context, [BytesConversion] IList<byte> data, bool isfinal = false)
    {
      this.CheckParsingDone(context);
      if (data.Any<byte>())
      {
        byte[] buffer;
        switch (data)
        {
          case Bytes bytes:
            buffer = bytes.GetUnsafeByteArray();
            break;
          case byte[] numArray:
            buffer = numArray;
            break;
          default:
            buffer = data.ToArray<byte>();
            break;
        }
        this.buffer.Write(buffer, 0, buffer.Length);
      }
      if (!isfinal)
        return;
      this.buffer.Seek(0L, SeekOrigin.Begin);
      this.xmlReader = XmlReader.Create((Stream) this.buffer, new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Parse,
        XmlResolver = (XmlResolver) null
      });
      this.parse(context);
      this._parsing_done = true;
      this.xmlReader.Dispose();
      this.buffer.Dispose();
    }

    public void ParseFile(CodeContext context, object file)
    {
      this.CheckParsingDone(context);
      object ret;
      if (!PythonOps.TryGetBoundAttr(context, file, "read", out ret))
        throw PythonOps.TypeError("argument must have 'read' attribute");
      if (!(PythonOps.CallWithContext(context, ret) is string s))
        throw PythonOps.TypeError("read() did not return a string object");
      using (StringReader input = new StringReader(s))
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Parse,
          XmlResolver = (XmlResolver) null
        };
        this.xmlReader = XmlReader.Create((TextReader) input, settings);
        this.parse(context);
        this._parsing_done = true;
        this.xmlReader.Dispose();
      }
    }

    private void CheckParsingDone(CodeContext context)
    {
      if (this._parsing_done)
        throw PythonExpat.xmlparser.Error(context, PythonExpat.XmlErrors.XML_ERROR_FINISHED);
    }

    private static Exception Error(CodeContext context, XmlException e)
    {
      int code;
      string str = PythonExpat.xmlparser.FormatExpatMsg(e, out code);
      PythonExceptions.BaseException pythonThrowable = PythonExceptions.CreatePythonThrowable((PythonType) context.LanguageContext.GetModuleState((object) "pyexpaterror"), (object) str);
      pythonThrowable.SetMemberAfter("lineno", (object) e.LineNumber);
      pythonThrowable.SetMemberAfter("offset", (object) e.LinePosition);
      pythonThrowable.SetMemberAfter("code", (object) code);
      return pythonThrowable.GetClrException();
    }

    private static Exception Error(CodeContext context, PythonExpat.XmlErrors code)
    {
      string fieldDescription = PythonExpat.PyExpatErrors.GetFieldDescription(code);
      PythonExceptions.BaseException pythonThrowable = PythonExceptions.CreatePythonThrowable((PythonType) context.LanguageContext.GetModuleState((object) "pyexpaterror"), (object) fieldDescription);
      pythonThrowable.SetMemberAfter(nameof (code), (object) (int) code);
      return pythonThrowable.GetClrException();
    }

    private static string FormatExpatMsg(XmlException e, out int code)
    {
      string str = e.Message;
      code = 0;
      if (e.Message.StartsWith("Syntax for an XML declaration"))
      {
        str = $"XML declaration not well-formed: line {e.LineNumber}, column {e.LinePosition}";
        code = 25;
      }
      return str;
    }

    public delegate void StartDoctypeDeclHandlerDelegate(
      string doctypeName,
      string systemId,
      string publicId,
      bool has_internal_subset);

    public delegate void XmlDeclHandlerDelegate(string version, string encoding, int standalone);
  }
}

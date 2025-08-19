// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Xml;
using System.Xml.Schema;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

public static class HighlightingLoader
{
  public static XshdSyntaxDefinition LoadXshd(XmlReader reader)
  {
    return HighlightingLoader.LoadXshd(reader, false);
  }

  internal static XshdSyntaxDefinition LoadXshd(XmlReader reader, bool skipValidation)
  {
    if (reader == null)
      throw new ArgumentNullException(nameof (reader));
    try
    {
      int content = (int) reader.MoveToContent();
      return reader.NamespaceURI == "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008" ? V2Loader.LoadDefinition(reader, skipValidation) : V1Loader.LoadDefinition(reader, skipValidation);
    }
    catch (XmlSchemaException ex)
    {
      throw HighlightingLoader.WrapException((Exception) ex, ex.LineNumber, ex.LinePosition);
    }
    catch (XmlException ex)
    {
      throw HighlightingLoader.WrapException((Exception) ex, ex.LineNumber, ex.LinePosition);
    }
  }

  private static Exception WrapException(Exception ex, int lineNumber, int linePosition)
  {
    return (Exception) new HighlightingDefinitionInvalidException(HighlightingLoader.FormatExceptionMessage(ex.Message, lineNumber, linePosition), ex);
  }

  internal static string FormatExceptionMessage(string message, int lineNumber, int linePosition)
  {
    if (lineNumber <= 0)
      return message;
    return $"Error at position (line {(object) lineNumber}, column {(object) linePosition}):\n{message}";
  }

  internal static XmlReader GetValidatingReader(
    XmlReader input,
    bool ignoreWhitespace,
    XmlSchemaSet schemaSet)
  {
    XmlReaderSettings settings = new XmlReaderSettings();
    settings.CloseInput = true;
    settings.IgnoreComments = true;
    settings.IgnoreWhitespace = ignoreWhitespace;
    if (schemaSet != null)
    {
      settings.Schemas = schemaSet;
      settings.ValidationType = ValidationType.Schema;
    }
    return XmlReader.Create(input, settings);
  }

  internal static XmlSchemaSet LoadSchemaSet(XmlReader schemaInput)
  {
    XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
    xmlSchemaSet.Add((string) null, schemaInput);
    xmlSchemaSet.ValidationEventHandler += (ValidationEventHandler) ((sender, args) =>
    {
      throw new HighlightingDefinitionInvalidException(args.Message);
    });
    return xmlSchemaSet;
  }

  public static IHighlightingDefinition Load(
    XshdSyntaxDefinition syntaxDefinition,
    IHighlightingDefinitionReferenceResolver resolver)
  {
    return syntaxDefinition != null ? (IHighlightingDefinition) new XmlHighlightingDefinition(syntaxDefinition, resolver) : throw new ArgumentNullException(nameof (syntaxDefinition));
  }

  public static IHighlightingDefinition Load(
    XmlReader reader,
    IHighlightingDefinitionReferenceResolver resolver)
  {
    return HighlightingLoader.Load(HighlightingLoader.LoadXshd(reader), resolver);
  }
}

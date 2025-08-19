// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Folding.XmlFoldingStrategy
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

#nullable disable
namespace ICSharpCode.AvalonEdit.Folding;

public class XmlFoldingStrategy
{
  public bool ShowAttributesWhenFolded { get; set; }

  public void UpdateFoldings(FoldingManager manager, TextDocument document)
  {
    int firstErrorOffset;
    IEnumerable<NewFolding> newFoldings = this.CreateNewFoldings(document, out firstErrorOffset);
    manager.UpdateFoldings(newFoldings, firstErrorOffset);
  }

  public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
  {
    try
    {
      return this.CreateNewFoldings(document, (XmlReader) new XmlTextReader(document.CreateReader())
      {
        XmlResolver = (XmlResolver) null
      }, out firstErrorOffset);
    }
    catch (XmlException ex)
    {
      firstErrorOffset = 0;
      return Enumerable.Empty<NewFolding>();
    }
  }

  public IEnumerable<NewFolding> CreateNewFoldings(
    TextDocument document,
    XmlReader reader,
    out int firstErrorOffset)
  {
    Stack<XmlFoldStart> xmlFoldStartStack = new Stack<XmlFoldStart>();
    List<NewFolding> foldMarkers = new List<NewFolding>();
    try
    {
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:
            if (!reader.IsEmptyElement)
            {
              XmlFoldStart elementFoldStart = this.CreateElementFoldStart(document, reader);
              xmlFoldStartStack.Push(elementFoldStart);
              continue;
            }
            continue;
          case XmlNodeType.Comment:
            XmlFoldingStrategy.CreateCommentFold(document, foldMarkers, reader);
            continue;
          case XmlNodeType.EndElement:
            XmlFoldStart foldStart = xmlFoldStartStack.Pop();
            XmlFoldingStrategy.CreateElementFold(document, foldMarkers, reader, foldStart);
            continue;
          default:
            continue;
        }
      }
      firstErrorOffset = -1;
    }
    catch (XmlException ex)
    {
      firstErrorOffset = ex.LineNumber < 1 || ex.LineNumber > document.LineCount ? 0 : document.GetOffset(ex.LineNumber, ex.LinePosition);
    }
    foldMarkers.Sort((Comparison<NewFolding>) ((a, b) => a.StartOffset.CompareTo(b.StartOffset)));
    return (IEnumerable<NewFolding>) foldMarkers;
  }

  private static int GetOffset(TextDocument document, XmlReader reader)
  {
    if (reader is IXmlLineInfo xmlLineInfo && xmlLineInfo.HasLineInfo())
      return document.GetOffset(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
    throw new ArgumentException("XmlReader does not have positioning information.");
  }

  private static void CreateCommentFold(
    TextDocument document,
    List<NewFolding> foldMarkers,
    XmlReader reader)
  {
    string str1 = reader.Value;
    if (str1 == null)
      return;
    int length = str1.IndexOf('\n');
    if (length < 0)
      return;
    int start = XmlFoldingStrategy.GetOffset(document, reader) - 4;
    int end = start + str1.Length + 7;
    string str2 = $"<!--{str1.Substring(0, length).TrimEnd('\r')}-->";
    foldMarkers.Add(new NewFolding(start, end)
    {
      Name = str2
    });
  }

  private XmlFoldStart CreateElementFoldStart(TextDocument document, XmlReader reader)
  {
    XmlFoldStart elementFoldStart = new XmlFoldStart();
    IXmlLineInfo xmlLineInfo = (IXmlLineInfo) reader;
    elementFoldStart.StartLine = xmlLineInfo.LineNumber;
    elementFoldStart.StartOffset = document.GetOffset(elementFoldStart.StartLine, xmlLineInfo.LinePosition - 1);
    if (this.ShowAttributesWhenFolded && reader.HasAttributes)
      elementFoldStart.Name = $"<{reader.Name} {XmlFoldingStrategy.GetAttributeFoldText(reader)}>";
    else
      elementFoldStart.Name = $"<{reader.Name}>";
    return elementFoldStart;
  }

  private static void CreateElementFold(
    TextDocument document,
    List<NewFolding> foldMarkers,
    XmlReader reader,
    XmlFoldStart foldStart)
  {
    IXmlLineInfo xmlLineInfo = (IXmlLineInfo) reader;
    int lineNumber = xmlLineInfo.LineNumber;
    if (lineNumber <= foldStart.StartLine)
      return;
    int column = xmlLineInfo.LinePosition + reader.Name.Length + 1;
    foldStart.EndOffset = document.GetOffset(lineNumber, column);
    foldMarkers.Add((NewFolding) foldStart);
  }

  private static string GetAttributeFoldText(XmlReader reader)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int i = 0; i < reader.AttributeCount; ++i)
    {
      reader.MoveToAttribute(i);
      stringBuilder.Append(reader.Name);
      stringBuilder.Append("=");
      stringBuilder.Append(reader.QuoteChar.ToString());
      stringBuilder.Append(XmlFoldingStrategy.XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
      stringBuilder.Append(reader.QuoteChar.ToString());
      if (i < reader.AttributeCount - 1)
        stringBuilder.Append(" ");
    }
    return stringBuilder.ToString();
  }

  private static string XmlEncodeAttributeValue(string attributeValue, char quoteChar)
  {
    StringBuilder stringBuilder = new StringBuilder(attributeValue);
    stringBuilder.Replace("&", "&amp;");
    stringBuilder.Replace("<", "&lt;");
    stringBuilder.Replace(">", "&gt;");
    if (quoteChar == '"')
      stringBuilder.Replace("\"", "&quot;");
    else
      stringBuilder.Replace("'", "&apos;");
    return stringBuilder.ToString();
  }
}

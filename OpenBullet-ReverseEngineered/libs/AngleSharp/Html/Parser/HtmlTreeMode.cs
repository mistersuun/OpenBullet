// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlTreeMode
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Html.Parser;

internal enum HtmlTreeMode : byte
{
  Initial,
  BeforeHtml,
  BeforeHead,
  InHead,
  InHeadNoScript,
  AfterHead,
  InBody,
  Text,
  InTable,
  InCaption,
  InColumnGroup,
  InTableBody,
  InRow,
  InCell,
  InSelect,
  InSelectInTable,
  InTemplate,
  AfterBody,
  InFrameset,
  AfterFrameset,
  AfterAfterBody,
  AfterAfterFrameset,
}

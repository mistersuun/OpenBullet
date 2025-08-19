// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.Tokens.HtmlDoctypeToken
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Html.Parser.Tokens;

public sealed class HtmlDoctypeToken : HtmlToken
{
  private bool _quirks;
  private string _publicIdentifier;
  private string _systemIdentifier;

  public HtmlDoctypeToken(bool quirksForced, TextPosition position)
    : base(HtmlTokenType.Doctype, position)
  {
    this._publicIdentifier = (string) null;
    this._systemIdentifier = (string) null;
    this._quirks = quirksForced;
  }

  public bool IsQuirksForced
  {
    get => this._quirks;
    set => this._quirks = value;
  }

  public bool IsPublicIdentifierMissing => this._publicIdentifier == null;

  public bool IsSystemIdentifierMissing => this._systemIdentifier == null;

  public string PublicIdentifier
  {
    get => this._publicIdentifier ?? string.Empty;
    set => this._publicIdentifier = value;
  }

  public string SystemIdentifier
  {
    get => this._systemIdentifier ?? string.Empty;
    set => this._systemIdentifier = value;
  }

  public bool IsLimitedQuirks
  {
    get
    {
      string publicIdentifier = this.PublicIdentifier;
      string systemIdentifier = this.SystemIdentifier;
      return publicIdentifier.StartsWith("-//W3C//DTD XHTML 1.0 Frameset//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD XHTML 1.0 Transitional//", StringComparison.OrdinalIgnoreCase) || systemIdentifier.StartsWith("-//W3C//DTD HTML 4.01 Frameset//", StringComparison.OrdinalIgnoreCase) || systemIdentifier.StartsWith("-//W3C//DTD HTML 4.01 Transitional//", StringComparison.OrdinalIgnoreCase);
    }
  }

  public bool IsFullQuirks
  {
    get
    {
      string publicIdentifier = this.PublicIdentifier;
      if (this.IsQuirksForced || !this.Name.Is("html") || publicIdentifier.StartsWith("+//Silmaril//dtd html Pro v0r11 19970101//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//AdvaSoft Ltd//DTD HTML 3.0 asWedit + extensions//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//AS//DTD HTML 3.0 asWedit + extensions//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 2.0 Level 1//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 2.0 Level 2//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 2.0 Strict Level 1//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 2.0 Strict Level 2//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 2.0 Strict//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 2.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 2.1E//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 3.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 3.2 Final//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 3.2//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML 3//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Level 0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Level 1//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Level 2//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Level 3//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Strict Level 0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Strict Level 1//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Strict Level 2//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Strict Level 3//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML Strict//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//IETF//DTD HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Metrius//DTD Metrius Presentational//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Microsoft//DTD Internet Explorer 2.0 HTML Strict//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Microsoft//DTD Internet Explorer 2.0 HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Microsoft//DTD Internet Explorer 2.0 Tables//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Microsoft//DTD Internet Explorer 3.0 HTML Strict//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Microsoft//DTD Internet Explorer 3.0 HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Microsoft//DTD Internet Explorer 3.0 Tables//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Netscape Comm. Corp.//DTD HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Netscape Comm. Corp.//DTD Strict HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//O'Reilly and Associates//DTD HTML 2.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//O'Reilly and Associates//DTD HTML Extended 1.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//O'Reilly and Associates//DTD HTML Extended Relaxed 1.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//SoftQuad Software//DTD HoTMetaL PRO 6.0::19990601::extensions to HTML 4.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//SoftQuad//DTD HoTMetaL PRO 4.0::19971010::extensions to HTML 4.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Spyglass//DTD HTML 2.0 Extended//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//SQ//DTD HTML 2.0 HoTMetaL + extensions//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Sun Microsystems Corp.//DTD HotJava HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//Sun Microsystems Corp.//DTD HotJava Strict HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML 3 1995-03-24//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML 3.2 Draft//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML 3.2 Final//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML 3.2//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML 3.2S Draft//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML 4.0 Frameset//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML 4.0 Transitional//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML Experimental 19960712//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD HTML Experimental 970421//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3C//DTD W3 HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//W3O//DTD W3 HTML 3.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.Isi("-//W3O//DTD W3 HTML Strict 3.0//EN//") || publicIdentifier.StartsWith("-//WebTechs//DTD Mozilla HTML 2.0//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.StartsWith("-//WebTechs//DTD Mozilla HTML//", StringComparison.OrdinalIgnoreCase) || publicIdentifier.Isi("-/W3C/DTD HTML 4.0 Transitional/EN") || publicIdentifier.Isi("HTML") || this.SystemIdentifier.Equals("http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd", StringComparison.OrdinalIgnoreCase) || this.IsSystemIdentifierMissing && publicIdentifier.StartsWith("-//W3C//DTD HTML 4.01 Frameset//", StringComparison.OrdinalIgnoreCase))
        return true;
      return this.IsSystemIdentifierMissing && publicIdentifier.StartsWith("-//W3C//DTD HTML 4.01 Transitional//", StringComparison.OrdinalIgnoreCase);
    }
  }

  public bool IsValid
  {
    get
    {
      if (!this.Name.Is("html"))
        return false;
      if (!this.IsPublicIdentifierMissing)
      {
        string publicIdentifier = this.PublicIdentifier;
        if (publicIdentifier.Is("-//W3C//DTD HTML 4.0//EN"))
          return this.IsSystemIdentifierMissing || this.SystemIdentifier.Is("http://www.w3.org/TR/REC-html40/strict.dtd");
        if (publicIdentifier.Is("-//W3C//DTD HTML 4.01//EN"))
          return this.IsSystemIdentifierMissing || this.SystemIdentifier.Is("http://www.w3.org/TR/html4/strict.dtd");
        if (publicIdentifier.Is("-//W3C//DTD XHTML 1.0 Strict//EN"))
          return this.SystemIdentifier.Is("http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd");
        if (publicIdentifier.Is("-//W3C//DTD XHTML 1.1//EN"))
          return this.SystemIdentifier.Is("http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd");
      }
      return this.IsSystemIdentifierMissing || this.SystemIdentifier.Is("about:legacy-compat");
    }
  }
}

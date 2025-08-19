// Decompiled with JetBrains decompiler
// Type: LiteDB.JsonTokenizer
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.IO;
using System.Text;

#nullable disable
namespace LiteDB;

internal class JsonTokenizer
{
  private char _current;
  private TextReader _reader;

  public bool EOF { get; private set; }

  public long Position { get; private set; }

  public JsonTokenizer(TextReader reader)
  {
    this._reader = reader;
    this.Position = 0L;
    int num = (int) this.Read();
  }

  private char Read()
  {
    if (this.EOF)
      return char.MinValue;
    int num = this._reader.Read();
    ++this.Position;
    if (num == -1)
    {
      this._current = char.MinValue;
      this.EOF = true;
    }
    this._current = (char) num;
    return this._current;
  }

  public JsonToken ReadToken()
  {
    this.EatWhitespace();
    if (this.EOF)
      return new JsonToken()
      {
        TokenType = JsonTokenType.EOF
      };
    JsonToken jsonToken;
    switch (this._current)
    {
      case '"':
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.String,
          Token = this.ReadString()
        };
        break;
      case ',':
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.Comma,
          Token = ","
        };
        int num1 = (int) this.Read();
        break;
      case '-':
      case '0':
      case '1':
      case '2':
      case '3':
      case '4':
      case '5':
      case '6':
      case '7':
      case '8':
      case '9':
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.Number,
          Token = this.ReadNumber()
        };
        break;
      case ':':
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.Colon,
          Token = ":"
        };
        int num2 = (int) this.Read();
        break;
      case '[':
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.BeginArray,
          Token = "["
        };
        int num3 = (int) this.Read();
        break;
      case ']':
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.EndArray,
          Token = "]"
        };
        int num4 = (int) this.Read();
        break;
      case '{':
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.BeginDoc,
          Token = "{"
        };
        int num5 = (int) this.Read();
        break;
      case '}':
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.EndDoc,
          Token = "}"
        };
        int num6 = (int) this.Read();
        break;
      default:
        jsonToken = new JsonToken()
        {
          TokenType = JsonTokenType.Word,
          Token = this.ReadWord()
        };
        break;
    }
    return jsonToken;
  }

  private void EatWhitespace()
  {
    while (char.IsWhiteSpace(this._current) && !this.EOF)
    {
      int num = (int) this.Read();
    }
  }

  private string ReadWord()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(this._current);
    int num1 = (int) this.Read();
    while (!this.EOF && (char.IsLetterOrDigit(this._current) || this._current == '_' || this._current == '$'))
    {
      stringBuilder.Append(this._current);
      int num2 = (int) this.Read();
    }
    return stringBuilder.ToString();
  }

  private string ReadNumber()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(this._current);
    int num1 = (int) this.Read();
    while (!this.EOF && (char.IsDigit(this._current) || this._current == '+' || this._current == '-' || this._current == '.' || this._current == 'e' || this._current == 'E'))
    {
      stringBuilder.Append(this._current);
      int num2 = (int) this.Read();
    }
    return stringBuilder.ToString();
  }

  private string ReadString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    int num1 = (int) this.Read();
    while (this._current != '"' && !this.EOF)
    {
      if (this._current == '\\')
      {
        int num2 = (int) this.Read();
        switch (this._current)
        {
          case '"':
            stringBuilder.Append('"');
            break;
          case '/':
            stringBuilder.Append('/');
            break;
          case '\\':
            stringBuilder.Append('\\');
            break;
          case 'b':
            stringBuilder.Append('\b');
            break;
          case 'f':
            stringBuilder.Append('\f');
            break;
          case 'n':
            stringBuilder.Append('\n');
            break;
          case 'r':
            stringBuilder.Append('\r');
            break;
          case 't':
            stringBuilder.Append('\t');
            break;
          case 'u':
            uint unicode = this.ParseUnicode(this.Read(), this.Read(), this.Read(), this.Read());
            stringBuilder.Append((char) unicode);
            break;
        }
      }
      else
        stringBuilder.Append(this._current);
      int num3 = (int) this.Read();
    }
    int num4 = (int) this.Read();
    return stringBuilder.ToString();
  }

  private uint ParseUnicode(char c1, char c2, char c3, char c4)
  {
    int singleChar1 = (int) this.ParseSingleChar(c1, 4096U /*0x1000*/);
    uint singleChar2 = this.ParseSingleChar(c2, 256U /*0x0100*/);
    uint singleChar3 = this.ParseSingleChar(c3, 16U /*0x10*/);
    uint singleChar4 = this.ParseSingleChar(c4, 1U);
    int num = (int) singleChar2;
    return (uint) (singleChar1 + num) + singleChar3 + singleChar4;
  }

  private uint ParseSingleChar(char c1, uint multiplier)
  {
    uint singleChar = 0;
    if (c1 >= '0' && c1 <= '9')
      singleChar = ((uint) c1 - 48U /*0x30*/) * multiplier;
    else if (c1 >= 'A' && c1 <= 'F')
      singleChar = (uint) ((int) c1 - 65 + 10) * multiplier;
    else if (c1 >= 'a' && c1 <= 'f')
      singleChar = (uint) ((int) c1 - 97 + 10) * multiplier;
    return singleChar;
  }
}

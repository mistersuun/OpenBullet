// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.SuperConsole
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public sealed class SuperConsole : BasicConsole
{
  private StringBuilder _input = new StringBuilder();
  private int _current;
  private int _autoIndentSize;
  private int _rendered;
  private SuperConsole.History _history = new SuperConsole.History();
  private SuperConsole.SuperConsoleOptions _options = new SuperConsole.SuperConsoleOptions();
  private SuperConsole.Cursor _cursor;
  private CommandLine _commandLine;
  private const int TabSize = 4;

  public SuperConsole(CommandLine commandLine, bool colorful)
    : base(colorful)
  {
    ContractUtils.RequiresNotNull((object) commandLine, nameof (commandLine));
    this._commandLine = commandLine;
  }

  private bool GetOptions()
  {
    this._options.Clear();
    int length1;
    for (length1 = this._input.Length; length1 > 0; --length1)
    {
      char c = this._input[length1 - 1];
      if (!char.IsLetterOrDigit(c) && c != '.' && c != '_')
        break;
    }
    string name = this._input.ToString(length1, this._input.Length - length1);
    if (name.Trim().Length <= 0)
      return false;
    int length2 = name.LastIndexOf('.');
    string code;
    string str1;
    string str2;
    if (length2 < 0)
    {
      code = string.Empty;
      str1 = name;
      str2 = this._input.ToString(0, length1);
    }
    else
    {
      code = name.Substring(0, length2);
      str1 = name.Substring(length2 + 1);
      str2 = this._input.ToString(0, length1 + length2 + 1);
    }
    try
    {
      IList<string> stringList = !string.IsNullOrEmpty(code) ? this._commandLine.GetMemberNames(code) : this._commandLine.GetGlobals(name);
      this._options.Root = str2;
      foreach (string line in (IEnumerable<string>) stringList)
      {
        if (line.StartsWith(str1, StringComparison.CurrentCultureIgnoreCase))
          this._options.Add(line);
      }
    }
    catch
    {
      this._options.Clear();
    }
    return true;
  }

  private void SetInput(string line)
  {
    this._input.Length = 0;
    this._input.Append(line);
    this._current = this._input.Length;
    this.Render();
  }

  private void Initialize()
  {
    this._cursor.Anchor();
    this._input.Length = 0;
    this._current = 0;
    this._rendered = 0;
  }

  private bool BackspaceAutoIndentation()
  {
    if (this._input.Length == 0 || this._input.Length > this._autoIndentSize)
      return false;
    for (int index = 0; index < this._input.Length; ++index)
    {
      if (this._input[index] != ' ')
        return false;
    }
    int startIndex = this._input.Length - 4;
    int length = this._input.Length - startIndex;
    this._input.Remove(startIndex, length);
    this._current -= length;
    this.Render();
    return true;
  }

  private void OnBackspace()
  {
    if (this.BackspaceAutoIndentation() || this._input.Length <= 0 || this._current <= 0)
      return;
    this._input.Remove(this._current - 1, 1);
    --this._current;
    this.Render();
  }

  private void OnDelete()
  {
    if (this._input.Length <= 0 || this._current >= this._input.Length)
      return;
    this._input.Remove(this._current, 1);
    this.Render();
  }

  private void Insert(ConsoleKeyInfo key)
  {
    this.Insert(key.Key != ConsoleKey.F6 ? key.KeyChar : this.FinalLineText[0]);
  }

  private void Insert(char c)
  {
    if (this._current == this._input.Length)
    {
      if (char.IsControl(c))
      {
        string str = SuperConsole.MapCharacter(c);
        ++this._current;
        this._input.Append(c);
        this.Output.Write(str);
        this._rendered += str.Length;
      }
      else
      {
        ++this._current;
        this._input.Append(c);
        this.Output.Write(c);
        ++this._rendered;
      }
    }
    else
    {
      this._input.Insert(this._current, c);
      ++this._current;
      this.Render();
    }
  }

  private static string MapCharacter(char c)
  {
    if (c == '\r')
      return "\r\n";
    return c <= '\u001A' ? "^" + ((char) ((int) c + 65 - 1)).ToString() : "^?";
  }

  private static int GetCharacterSize(char c)
  {
    return char.IsControl(c) ? SuperConsole.MapCharacter(c).Length : 1;
  }

  private void Render()
  {
    this._cursor.Reset();
    StringBuilder stringBuilder = new StringBuilder();
    int index1 = -1;
    for (int index2 = 0; index2 < this._input.Length; ++index2)
    {
      if (index2 == this._current)
        index1 = stringBuilder.Length;
      char c = this._input[index2];
      if (char.IsControl(c))
        stringBuilder.Append(SuperConsole.MapCharacter(c));
      else
        stringBuilder.Append(c);
    }
    if (this._current == this._input.Length)
      index1 = stringBuilder.Length;
    string str = stringBuilder.ToString();
    this.Output.Write(str);
    if (str.Length < this._rendered)
      this.Output.Write(new string(' ', this._rendered - str.Length));
    this._rendered = str.Length;
    this._cursor.Place(index1);
  }

  private void MoveLeft(ConsoleModifiers keyModifiers)
  {
    if ((keyModifiers & ConsoleModifiers.Control) != (ConsoleModifiers) 0)
    {
      if (this._input.Length <= 0 || this._current == 0)
        return;
      bool flag = SuperConsole.IsSeperator(this._input[this._current - 1]);
      while (this._current > 0 && this._current - 1 < this._input.Length)
      {
        this.MoveLeft();
        if (SuperConsole.IsSeperator(this._input[this._current]) != flag)
        {
          if (!flag)
          {
            this.MoveRight();
            break;
          }
          flag = false;
        }
      }
    }
    else
      this.MoveLeft();
  }

  private static bool IsSeperator(char ch) => !char.IsLetter(ch);

  private void MoveRight(ConsoleModifiers keyModifiers)
  {
    if ((keyModifiers & ConsoleModifiers.Control) != (ConsoleModifiers) 0)
    {
      if (this._input.Length == 0 || this._current >= this._input.Length)
        return;
      bool flag = SuperConsole.IsSeperator(this._input[this._current]);
      while (this._current < this._input.Length)
      {
        this.MoveRight();
        if (this._current == this._input.Length)
          break;
        if (SuperConsole.IsSeperator(this._input[this._current]) != flag)
        {
          if (flag)
            break;
          flag = true;
        }
      }
    }
    else
      this.MoveRight();
  }

  private void MoveRight()
  {
    if (this._current >= this._input.Length)
      return;
    int c = (int) this._input[this._current];
    ++this._current;
    SuperConsole.Cursor.Move(SuperConsole.GetCharacterSize((char) c));
  }

  private void MoveLeft()
  {
    if (this._current <= 0 || this._current - 1 >= this._input.Length)
      return;
    --this._current;
    SuperConsole.Cursor.Move(-SuperConsole.GetCharacterSize(this._input[this._current]));
  }

  private void InsertTab()
  {
    for (int index = 4 - this._current % 4; index > 0; --index)
      this.Insert(' ');
  }

  private void MoveHome()
  {
    this._current = 0;
    this._cursor.Reset();
  }

  private void MoveEnd()
  {
    this._current = this._input.Length;
    this._cursor.Place(this._rendered);
  }

  public override string ReadLine(int autoIndentSize)
  {
    this.Initialize();
    this._autoIndentSize = autoIndentSize;
    for (int index = 0; index < this._autoIndentSize; ++index)
      this.Insert(' ');
    bool inputChanged = false;
    bool flag = false;
    while (true)
    {
      ConsoleKeyInfo key1;
      ConsoleKey key2;
      do
      {
        key1 = Console.ReadKey(true);
        key2 = key1.Key;
        if (key2 <= ConsoleKey.Enter)
        {
          if (key2 != ConsoleKey.Backspace)
          {
            if (key2 != ConsoleKey.Tab)
            {
              if (key2 == ConsoleKey.Enter)
                goto label_13;
              break;
            }
            goto label_14;
          }
          goto label_11;
        }
        if (key2 != ConsoleKey.Escape)
        {
          switch (key2 - 35)
          {
            case (ConsoleKey) 0:
              goto label_23;
            case (ConsoleKey) 1:
              goto label_22;
            case (ConsoleKey) 2:
              goto label_20;
            case (ConsoleKey) 3:
              goto label_17;
            case (ConsoleKey) 4:
              goto label_19;
            case (ConsoleKey) 5:
              goto label_18;
            case (ConsoleKey) 6:
            case (ConsoleKey) 7:
            case ConsoleKey.Backspace:
            case ConsoleKey.Tab:
            case (ConsoleKey) 10:
              goto label_24;
            case (ConsoleKey) 11:
              goto label_12;
            default:
              continue;
          }
        }
        else
          goto label_21;
      }
      while ((uint) (key2 - 91) <= 1U);
      goto label_24;
label_11:
      this.OnBackspace();
      inputChanged = flag = true;
      continue;
label_12:
      this.OnDelete();
      inputChanged = flag = true;
      continue;
label_14:
      bool prefix = false;
      if (flag)
      {
        prefix = this.GetOptions();
        flag = false;
      }
      this.DisplayNextOption(key1, prefix);
      inputChanged = true;
      continue;
label_17:
      this.SetInput(this._history.Previous());
      flag = true;
      inputChanged = false;
      continue;
label_18:
      this.SetInput(this._history.Next());
      flag = true;
      inputChanged = false;
      continue;
label_19:
      this.MoveRight(key1.Modifiers);
      flag = true;
      continue;
label_20:
      this.MoveLeft(key1.Modifiers);
      flag = true;
      continue;
label_21:
      this.SetInput(string.Empty);
      inputChanged = flag = true;
      continue;
label_22:
      this.MoveHome();
      flag = true;
      continue;
label_23:
      this.MoveEnd();
      flag = true;
      continue;
label_24:
      if (key1.KeyChar != '\r')
      {
        if (key1.KeyChar != '\b')
        {
          this.Insert(key1);
          inputChanged = flag = true;
        }
        else
          goto label_11;
      }
      else
        break;
    }
label_13:
    return this.OnEnter(inputChanged);
  }

  private void DisplayNextOption(ConsoleKeyInfo key, bool prefix)
  {
    if (this._options.Count > 0)
      this.SetInput(this._options.Root + ((key.Modifiers & ConsoleModifiers.Shift) != (ConsoleModifiers) 0 ? this._options.Previous() : this._options.Next()));
    else if (prefix)
      Console.Beep();
    else
      this.InsertTab();
  }

  private string OnEnter(bool inputChanged)
  {
    this.Output.Write("\n");
    string line = this._input.ToString();
    if (line == this.FinalLineText)
      return (string) null;
    if (line.Length > 0)
      this._history.Add(line, inputChanged);
    return line;
  }

  private string FinalLineText
  {
    get => Environment.OSVersion.Platform == PlatformID.Unix ? "\u0004" : "\u001A";
  }

  private class History
  {
    protected List<string> _list = new List<string>();
    private int _current;
    private bool _increment;

    public string Current
    {
      get
      {
        return this._current < 0 || this._current >= this._list.Count ? string.Empty : this._list[this._current];
      }
    }

    public void Add(string line, bool setCurrentAsLast)
    {
      if (line == null || line.Length <= 0)
        return;
      int count = this._list.Count;
      this._list.Add(line);
      if (setCurrentAsLast || this._current == count)
        this._current = this._list.Count;
      else
        ++this._current;
      this._increment = false;
    }

    public string Previous()
    {
      if (this._current > 0)
      {
        --this._current;
        this._increment = true;
      }
      return this.Current;
    }

    public string Next()
    {
      if (this._current + 1 < this._list.Count)
      {
        if (this._increment)
          ++this._current;
        this._increment = true;
      }
      return this.Current;
    }
  }

  private class SuperConsoleOptions
  {
    private List<string> _list = new List<string>();
    private int _current;

    public int Count => this._list.Count;

    private string Current
    {
      get
      {
        return this._current < 0 || this._current >= this._list.Count ? string.Empty : this._list[this._current];
      }
    }

    public void Clear()
    {
      this._list.Clear();
      this._current = -1;
    }

    public void Add(string line)
    {
      if (line == null || line.Length <= 0)
        return;
      this._list.Add(line);
    }

    public string Previous()
    {
      if (this._list.Count > 0)
        this._current = (this._current - 1 + this._list.Count) % this._list.Count;
      return this.Current;
    }

    public string Next()
    {
      if (this._list.Count > 0)
        this._current = (this._current + 1) % this._list.Count;
      return this.Current;
    }

    public string Root { get; set; }
  }

  private struct Cursor
  {
    private int _anchorTop;
    private int _anchorLeft;

    public void Anchor()
    {
      this._anchorTop = Console.CursorTop;
      this._anchorLeft = Console.CursorLeft;
    }

    public void Reset()
    {
      Console.CursorTop = this._anchorTop;
      Console.CursorLeft = this._anchorLeft;
    }

    public void Place(int index)
    {
      Console.CursorLeft = (this._anchorLeft + index) % Console.BufferWidth;
      int num = this._anchorTop + (this._anchorLeft + index) / Console.BufferWidth;
      if (num >= Console.BufferHeight)
      {
        this._anchorTop -= num - Console.BufferHeight + 1;
        num = Console.BufferHeight - 1;
      }
      Console.CursorTop = num;
    }

    public static void Move(int delta)
    {
      int num = Console.CursorTop * Console.BufferWidth + Console.CursorLeft + delta;
      Console.CursorLeft = num % Console.BufferWidth;
      Console.CursorTop = num / Console.BufferWidth;
    }
  }
}

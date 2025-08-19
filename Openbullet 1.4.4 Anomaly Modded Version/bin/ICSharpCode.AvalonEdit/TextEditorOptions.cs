// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.TextEditorOptions
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.ComponentModel;
using System.Reflection;

#nullable disable
namespace ICSharpCode.AvalonEdit;

[Serializable]
public class TextEditorOptions : INotifyPropertyChanged
{
  private bool showSpaces;
  private bool showTabs;
  private bool showEndOfLine;
  private bool showBoxForControlCharacters = true;
  private bool enableHyperlinks = true;
  private bool enableEmailHyperlinks = true;
  private bool requireControlModifierForHyperlinkClick = true;
  private int indentationSize = 4;
  private bool convertTabsToSpaces;
  private bool cutCopyWholeLine = true;
  private bool allowScrollBelowDocument;
  private double wordWrapIndentation;
  private bool inheritWordWrapIndentation = true;
  private bool enableRectangularSelection = true;
  private bool enableTextDragDrop = true;
  private bool enableVirtualSpace;
  private bool enableImeSupport = true;
  private bool showColumnRuler;
  private int columnRulerPosition = 80 /*0x50*/;
  private bool highlightCurrentLine;
  private bool hideCursorWhileTyping = true;
  private bool allowToggleOverstrikeMode;

  public TextEditorOptions()
  {
  }

  public TextEditorOptions(TextEditorOptions options)
  {
    foreach (FieldInfo field in typeof (TextEditorOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
      if (!field.IsNotSerialized)
        field.SetValue((object) this, field.GetValue((object) options));
    }
  }

  [field: NonSerialized]
  public event PropertyChangedEventHandler PropertyChanged;

  protected void OnPropertyChanged(string propertyName)
  {
    this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
  }

  protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
  {
    if (this.PropertyChanged == null)
      return;
    this.PropertyChanged((object) this, e);
  }

  [DefaultValue(false)]
  public virtual bool ShowSpaces
  {
    get => this.showSpaces;
    set
    {
      if (this.showSpaces == value)
        return;
      this.showSpaces = value;
      this.OnPropertyChanged(nameof (ShowSpaces));
    }
  }

  [DefaultValue(false)]
  public virtual bool ShowTabs
  {
    get => this.showTabs;
    set
    {
      if (this.showTabs == value)
        return;
      this.showTabs = value;
      this.OnPropertyChanged(nameof (ShowTabs));
    }
  }

  [DefaultValue(false)]
  public virtual bool ShowEndOfLine
  {
    get => this.showEndOfLine;
    set
    {
      if (this.showEndOfLine == value)
        return;
      this.showEndOfLine = value;
      this.OnPropertyChanged(nameof (ShowEndOfLine));
    }
  }

  [DefaultValue(true)]
  public virtual bool ShowBoxForControlCharacters
  {
    get => this.showBoxForControlCharacters;
    set
    {
      if (this.showBoxForControlCharacters == value)
        return;
      this.showBoxForControlCharacters = value;
      this.OnPropertyChanged(nameof (ShowBoxForControlCharacters));
    }
  }

  [DefaultValue(true)]
  public virtual bool EnableHyperlinks
  {
    get => this.enableHyperlinks;
    set
    {
      if (this.enableHyperlinks == value)
        return;
      this.enableHyperlinks = value;
      this.OnPropertyChanged(nameof (EnableHyperlinks));
    }
  }

  [DefaultValue(true)]
  public virtual bool EnableEmailHyperlinks
  {
    get => this.enableEmailHyperlinks;
    set
    {
      if (this.enableEmailHyperlinks == value)
        return;
      this.enableEmailHyperlinks = value;
      this.OnPropertyChanged("EnableEMailHyperlinks");
    }
  }

  [DefaultValue(true)]
  public virtual bool RequireControlModifierForHyperlinkClick
  {
    get => this.requireControlModifierForHyperlinkClick;
    set
    {
      if (this.requireControlModifierForHyperlinkClick == value)
        return;
      this.requireControlModifierForHyperlinkClick = value;
      this.OnPropertyChanged(nameof (RequireControlModifierForHyperlinkClick));
    }
  }

  [DefaultValue(4)]
  public virtual int IndentationSize
  {
    get => this.indentationSize;
    set
    {
      if (value < 1)
        throw new ArgumentOutOfRangeException(nameof (value), (object) value, "value must be positive");
      if (value > 1000)
        throw new ArgumentOutOfRangeException(nameof (value), (object) value, "indentation size is too large");
      if (this.indentationSize == value)
        return;
      this.indentationSize = value;
      this.OnPropertyChanged(nameof (IndentationSize));
      this.OnPropertyChanged("IndentationString");
    }
  }

  [DefaultValue(false)]
  public virtual bool ConvertTabsToSpaces
  {
    get => this.convertTabsToSpaces;
    set
    {
      if (this.convertTabsToSpaces == value)
        return;
      this.convertTabsToSpaces = value;
      this.OnPropertyChanged(nameof (ConvertTabsToSpaces));
      this.OnPropertyChanged("IndentationString");
    }
  }

  [Browsable(false)]
  public string IndentationString => this.GetIndentationString(1);

  public virtual string GetIndentationString(int column)
  {
    if (column < 1)
      throw new ArgumentOutOfRangeException(nameof (column), (object) column, "Value must be at least 1.");
    int indentationSize = this.IndentationSize;
    return this.ConvertTabsToSpaces ? new string(' ', indentationSize - (column - 1) % indentationSize) : "\t";
  }

  [DefaultValue(true)]
  public virtual bool CutCopyWholeLine
  {
    get => this.cutCopyWholeLine;
    set
    {
      if (this.cutCopyWholeLine == value)
        return;
      this.cutCopyWholeLine = value;
      this.OnPropertyChanged(nameof (CutCopyWholeLine));
    }
  }

  [DefaultValue(false)]
  public virtual bool AllowScrollBelowDocument
  {
    get => this.allowScrollBelowDocument;
    set
    {
      if (this.allowScrollBelowDocument == value)
        return;
      this.allowScrollBelowDocument = value;
      this.OnPropertyChanged(nameof (AllowScrollBelowDocument));
    }
  }

  [DefaultValue(0.0)]
  public virtual double WordWrapIndentation
  {
    get => this.wordWrapIndentation;
    set
    {
      if (double.IsNaN(value) || double.IsInfinity(value))
        throw new ArgumentOutOfRangeException(nameof (value), (object) value, "value must not be NaN/infinity");
      if (value == this.wordWrapIndentation)
        return;
      this.wordWrapIndentation = value;
      this.OnPropertyChanged(nameof (WordWrapIndentation));
    }
  }

  [DefaultValue(true)]
  public virtual bool InheritWordWrapIndentation
  {
    get => this.inheritWordWrapIndentation;
    set
    {
      if (value == this.inheritWordWrapIndentation)
        return;
      this.inheritWordWrapIndentation = value;
      this.OnPropertyChanged(nameof (InheritWordWrapIndentation));
    }
  }

  [DefaultValue(true)]
  public bool EnableRectangularSelection
  {
    get => this.enableRectangularSelection;
    set
    {
      if (this.enableRectangularSelection == value)
        return;
      this.enableRectangularSelection = value;
      this.OnPropertyChanged(nameof (EnableRectangularSelection));
    }
  }

  [DefaultValue(true)]
  public bool EnableTextDragDrop
  {
    get => this.enableTextDragDrop;
    set
    {
      if (this.enableTextDragDrop == value)
        return;
      this.enableTextDragDrop = value;
      this.OnPropertyChanged(nameof (EnableTextDragDrop));
    }
  }

  [DefaultValue(false)]
  public virtual bool EnableVirtualSpace
  {
    get => this.enableVirtualSpace;
    set
    {
      if (this.enableVirtualSpace == value)
        return;
      this.enableVirtualSpace = value;
      this.OnPropertyChanged(nameof (EnableVirtualSpace));
    }
  }

  [DefaultValue(true)]
  public virtual bool EnableImeSupport
  {
    get => this.enableImeSupport;
    set
    {
      if (this.enableImeSupport == value)
        return;
      this.enableImeSupport = value;
      this.OnPropertyChanged(nameof (EnableImeSupport));
    }
  }

  [DefaultValue(false)]
  public virtual bool ShowColumnRuler
  {
    get => this.showColumnRuler;
    set
    {
      if (this.showColumnRuler == value)
        return;
      this.showColumnRuler = value;
      this.OnPropertyChanged(nameof (ShowColumnRuler));
    }
  }

  [DefaultValue(80 /*0x50*/)]
  public virtual int ColumnRulerPosition
  {
    get => this.columnRulerPosition;
    set
    {
      if (this.columnRulerPosition == value)
        return;
      this.columnRulerPosition = value;
      this.OnPropertyChanged(nameof (ColumnRulerPosition));
    }
  }

  [DefaultValue(false)]
  public virtual bool HighlightCurrentLine
  {
    get => this.highlightCurrentLine;
    set
    {
      if (this.highlightCurrentLine == value)
        return;
      this.highlightCurrentLine = value;
      this.OnPropertyChanged(nameof (HighlightCurrentLine));
    }
  }

  [DefaultValue(true)]
  public bool HideCursorWhileTyping
  {
    get => this.hideCursorWhileTyping;
    set
    {
      if (this.hideCursorWhileTyping == value)
        return;
      this.hideCursorWhileTyping = value;
      this.OnPropertyChanged(nameof (HideCursorWhileTyping));
    }
  }

  [DefaultValue(false)]
  public bool AllowToggleOverstrikeMode
  {
    get => this.allowToggleOverstrikeMode;
    set
    {
      if (this.allowToggleOverstrikeMode == value)
        return;
      this.allowToggleOverstrikeMode = value;
      this.OnPropertyChanged(nameof (AllowToggleOverstrikeMode));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.EditingCommandHandler
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal static class EditingCommandHandler
{
  private const string LineSelectedType = "MSDEVLineSelect";
  private static readonly List<CommandBinding> CommandBindings = new List<CommandBinding>();
  private static readonly List<InputBinding> InputBindings = new List<InputBinding>();

  public static TextAreaInputHandler Create(TextArea textArea)
  {
    TextAreaInputHandler areaInputHandler = new TextAreaInputHandler(textArea);
    areaInputHandler.CommandBindings.AddRange<CommandBinding>((IEnumerable<CommandBinding>) EditingCommandHandler.CommandBindings);
    areaInputHandler.InputBindings.AddRange<InputBinding>((IEnumerable<InputBinding>) EditingCommandHandler.InputBindings);
    return areaInputHandler;
  }

  private static void AddBinding(
    ICommand command,
    ModifierKeys modifiers,
    Key key,
    ExecutedRoutedEventHandler handler)
  {
    EditingCommandHandler.CommandBindings.Add(new CommandBinding(command, handler));
    EditingCommandHandler.InputBindings.Add((InputBinding) TextAreaDefaultInputHandler.CreateFrozenKeyBinding(command, modifiers, key));
  }

  static EditingCommandHandler()
  {
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Delete, EditingCommandHandler.OnDelete(CaretMovementType.None), new CanExecuteRoutedEventHandler(EditingCommandHandler.CanDelete)));
    EditingCommandHandler.AddBinding((ICommand) EditingCommands.Delete, ModifierKeys.None, Key.Delete, EditingCommandHandler.OnDelete(CaretMovementType.CharRight));
    EditingCommandHandler.AddBinding((ICommand) EditingCommands.DeleteNextWord, ModifierKeys.Control, Key.Delete, EditingCommandHandler.OnDelete(CaretMovementType.WordRight));
    EditingCommandHandler.AddBinding((ICommand) EditingCommands.Backspace, ModifierKeys.None, Key.Back, EditingCommandHandler.OnDelete(CaretMovementType.Backspace));
    EditingCommandHandler.InputBindings.Add((InputBinding) TextAreaDefaultInputHandler.CreateFrozenKeyBinding((ICommand) EditingCommands.Backspace, ModifierKeys.Shift, Key.Back));
    EditingCommandHandler.AddBinding((ICommand) EditingCommands.DeletePreviousWord, ModifierKeys.Control, Key.Back, EditingCommandHandler.OnDelete(CaretMovementType.WordLeft));
    EditingCommandHandler.AddBinding((ICommand) EditingCommands.EnterParagraphBreak, ModifierKeys.None, Key.Return, new ExecutedRoutedEventHandler(EditingCommandHandler.OnEnter));
    EditingCommandHandler.AddBinding((ICommand) EditingCommands.EnterLineBreak, ModifierKeys.Shift, Key.Return, new ExecutedRoutedEventHandler(EditingCommandHandler.OnEnter));
    EditingCommandHandler.AddBinding((ICommand) EditingCommands.TabForward, ModifierKeys.None, Key.Tab, new ExecutedRoutedEventHandler(EditingCommandHandler.OnTab));
    EditingCommandHandler.AddBinding((ICommand) EditingCommands.TabBackward, ModifierKeys.Shift, Key.Tab, new ExecutedRoutedEventHandler(EditingCommandHandler.OnShiftTab));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Copy, new ExecutedRoutedEventHandler(EditingCommandHandler.OnCopy), new CanExecuteRoutedEventHandler(EditingCommandHandler.CanCutOrCopy)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Cut, new ExecutedRoutedEventHandler(EditingCommandHandler.OnCut), new CanExecuteRoutedEventHandler(EditingCommandHandler.CanCutOrCopy)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Paste, new ExecutedRoutedEventHandler(EditingCommandHandler.OnPaste), new CanExecuteRoutedEventHandler(EditingCommandHandler.CanPaste)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.ToggleOverstrike, new ExecutedRoutedEventHandler(EditingCommandHandler.OnToggleOverstrike)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.DeleteLine, new ExecutedRoutedEventHandler(EditingCommandHandler.OnDeleteLine)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.RemoveLeadingWhitespace, new ExecutedRoutedEventHandler(EditingCommandHandler.OnRemoveLeadingWhitespace)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.RemoveTrailingWhitespace, new ExecutedRoutedEventHandler(EditingCommandHandler.OnRemoveTrailingWhitespace)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.ConvertToUppercase, new ExecutedRoutedEventHandler(EditingCommandHandler.OnConvertToUpperCase)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.ConvertToLowercase, new ExecutedRoutedEventHandler(EditingCommandHandler.OnConvertToLowerCase)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.ConvertToTitleCase, new ExecutedRoutedEventHandler(EditingCommandHandler.OnConvertToTitleCase)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.InvertCase, new ExecutedRoutedEventHandler(EditingCommandHandler.OnInvertCase)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.ConvertTabsToSpaces, new ExecutedRoutedEventHandler(EditingCommandHandler.OnConvertTabsToSpaces)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.ConvertSpacesToTabs, new ExecutedRoutedEventHandler(EditingCommandHandler.OnConvertSpacesToTabs)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.ConvertLeadingTabsToSpaces, new ExecutedRoutedEventHandler(EditingCommandHandler.OnConvertLeadingTabsToSpaces)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.ConvertLeadingSpacesToTabs, new ExecutedRoutedEventHandler(EditingCommandHandler.OnConvertLeadingSpacesToTabs)));
    EditingCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) AvalonEditCommands.IndentSelection, new ExecutedRoutedEventHandler(EditingCommandHandler.OnIndentSelection)));
    TextAreaDefaultInputHandler.WorkaroundWPFMemoryLeak(EditingCommandHandler.InputBindings);
  }

  private static TextArea GetTextArea(object target) => target as TextArea;

  private static void TransformSelectedLines(
    Action<TextArea, DocumentLine> transformLine,
    object target,
    ExecutedRoutedEventArgs args,
    EditingCommandHandler.DefaultSegmentType defaultSegmentType)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    using (textArea.Document.RunUpdate())
    {
      DocumentLine documentLine1;
      DocumentLine documentLine2;
      if (textArea.Selection.IsEmpty)
      {
        switch (defaultSegmentType)
        {
          case EditingCommandHandler.DefaultSegmentType.WholeDocument:
            documentLine2 = textArea.Document.Lines.First<DocumentLine>();
            documentLine1 = textArea.Document.Lines.Last<DocumentLine>();
            break;
          case EditingCommandHandler.DefaultSegmentType.CurrentLine:
            documentLine2 = documentLine1 = textArea.Document.GetLineByNumber(textArea.Caret.Line);
            break;
          default:
            documentLine2 = documentLine1 = (DocumentLine) null;
            break;
        }
      }
      else
      {
        ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
        documentLine2 = textArea.Document.GetLineByOffset(surroundingSegment.Offset);
        documentLine1 = textArea.Document.GetLineByOffset(surroundingSegment.EndOffset);
        if (documentLine2 != documentLine1 && documentLine1.Offset == surroundingSegment.EndOffset)
          documentLine1 = documentLine1.PreviousLine;
      }
      if (documentLine2 != null)
      {
        transformLine(textArea, documentLine2);
        while (documentLine2 != documentLine1)
        {
          documentLine2 = documentLine2.NextLine;
          transformLine(textArea, documentLine2);
        }
      }
    }
    textArea.Caret.BringCaretToView();
    args.Handled = true;
  }

  private static void TransformSelectedSegments(
    Action<TextArea, ISegment> transformSegment,
    object target,
    ExecutedRoutedEventArgs args,
    EditingCommandHandler.DefaultSegmentType defaultSegmentType)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    using (textArea.Document.RunUpdate())
    {
      IEnumerable<ISegment> source;
      if (textArea.Selection.IsEmpty)
      {
        switch (defaultSegmentType)
        {
          case EditingCommandHandler.DefaultSegmentType.WholeDocument:
            source = textArea.Document.Lines.Cast<ISegment>();
            break;
          case EditingCommandHandler.DefaultSegmentType.CurrentLine:
            source = (IEnumerable<ISegment>) new ISegment[1]
            {
              (ISegment) textArea.Document.GetLineByNumber(textArea.Caret.Line)
            };
            break;
          default:
            source = (IEnumerable<ISegment>) null;
            break;
        }
      }
      else
        source = textArea.Selection.Segments.Cast<ISegment>();
      if (source != null)
      {
        foreach (ISegment segment1 in source.Reverse<ISegment>())
        {
          foreach (ISegment segment2 in ((IEnumerable<ISegment>) textArea.GetDeletableSegments(segment1)).Reverse<ISegment>())
            transformSegment(textArea, segment2);
        }
      }
    }
    textArea.Caret.BringCaretToView();
    args.Handled = true;
  }

  private static void OnEnter(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || !textArea.IsKeyboardFocused)
      return;
    textArea.PerformTextInput("\n");
    args.Handled = true;
  }

  private static void OnTab(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    using (textArea.Document.RunUpdate())
    {
      if (textArea.Selection.IsMultiline)
      {
        ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
        DocumentLine lineByOffset = textArea.Document.GetLineByOffset(surroundingSegment.Offset);
        DocumentLine documentLine1 = textArea.Document.GetLineByOffset(surroundingSegment.EndOffset);
        if (lineByOffset != documentLine1 && documentLine1.Offset == surroundingSegment.EndOffset)
          documentLine1 = documentLine1.PreviousLine;
        DocumentLine documentLine2 = lineByOffset;
        while (true)
        {
          int offset = documentLine2.Offset;
          if (textArea.ReadOnlySectionProvider.CanInsert(offset))
            textArea.Document.Replace(offset, 0, textArea.Options.IndentationString, OffsetChangeMappingType.KeepAnchorBeforeInsertion);
          if (documentLine2 != documentLine1)
            documentLine2 = documentLine2.NextLine;
          else
            break;
        }
      }
      else
      {
        string indentationString = textArea.Options.GetIndentationString(textArea.Caret.Column);
        textArea.ReplaceSelectionWithText(indentationString);
      }
    }
    textArea.Caret.BringCaretToView();
    args.Handled = true;
  }

  private static void OnShiftTab(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.TransformSelectedLines((Action<TextArea, DocumentLine>) ((textArea, line) =>
    {
      int offset = line.Offset;
      ISegment indentationSegment = TextUtilities.GetSingleIndentationSegment((ITextSource) textArea.Document, offset, textArea.Options.IndentationSize);
      if (indentationSegment.Length <= 0)
        return;
      ISegment segment = ((IEnumerable<ISegment>) textArea.GetDeletableSegments(indentationSegment)).FirstOrDefault<ISegment>();
      if (segment == null || segment.Length <= 0)
        return;
      textArea.Document.Remove(segment.Offset, segment.Length);
    }), target, args, EditingCommandHandler.DefaultSegmentType.CurrentLine);
  }

  private static ExecutedRoutedEventHandler OnDelete(CaretMovementType caretMovement)
  {
    return (ExecutedRoutedEventHandler) ((target, args) =>
    {
      TextArea textArea = EditingCommandHandler.GetTextArea(target);
      if (textArea == null || textArea.Document == null)
        return;
      if (textArea.Selection.IsEmpty)
      {
        TextViewPosition position = textArea.Caret.Position;
        bool enableVirtualSpace = textArea.Options.EnableVirtualSpace;
        if (caretMovement == CaretMovementType.CharRight)
          enableVirtualSpace = false;
        double desiredXpos = textArea.Caret.DesiredXPos;
        TextViewPosition end = CaretNavigationCommandHandler.GetNewCaretPosition(textArea.TextView, position, caretMovement, enableVirtualSpace, ref desiredXpos);
        if (end.Line < 1 || end.Column < 1)
          end = new TextViewPosition(Math.Max(end.Line, 1), Math.Max(end.Column, 1));
        new SimpleSelection(textArea, position, end).ReplaceSelectionWithText(string.Empty);
      }
      else
        textArea.RemoveSelectedText();
      textArea.Caret.BringCaretToView();
      args.Handled = true;
    });
  }

  private static void CanDelete(object target, CanExecuteRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    args.CanExecute = !textArea.Selection.IsEmpty;
    args.Handled = true;
  }

  private static void CanCutOrCopy(object target, CanExecuteRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    args.CanExecute = textArea.Options.CutCopyWholeLine || !textArea.Selection.IsEmpty;
    args.Handled = true;
  }

  private static void OnCopy(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine)
    {
      DocumentLine lineByNumber = textArea.Document.GetLineByNumber(textArea.Caret.Line);
      EditingCommandHandler.CopyWholeLine(textArea, lineByNumber);
    }
    else
      EditingCommandHandler.CopySelectedText(textArea);
    args.Handled = true;
  }

  private static void OnCut(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine)
    {
      DocumentLine lineByNumber = textArea.Document.GetLineByNumber(textArea.Caret.Line);
      if (EditingCommandHandler.CopyWholeLine(textArea, lineByNumber))
      {
        ISegment[] deletableSegments = textArea.GetDeletableSegments((ISegment) new SimpleSegment(lineByNumber.Offset, lineByNumber.TotalLength));
        for (int index = deletableSegments.Length - 1; index >= 0; --index)
          textArea.Document.Remove(deletableSegments[index]);
      }
    }
    else if (EditingCommandHandler.CopySelectedText(textArea))
      textArea.RemoveSelectedText();
    textArea.Caret.BringCaretToView();
    args.Handled = true;
  }

  private static bool CopySelectedText(TextArea textArea)
  {
    DataObject dataObject = textArea.Selection.CreateDataObject(textArea);
    DataObjectCopyingEventArgs e = new DataObjectCopyingEventArgs((IDataObject) dataObject, false);
    textArea.RaiseEvent((RoutedEventArgs) e);
    if (e.CommandCancelled)
      return false;
    try
    {
      Clipboard.SetDataObject((object) dataObject, true);
    }
    catch (ExternalException ex)
    {
    }
    string text = TextUtilities.NormalizeNewLines(textArea.Selection.GetText(), Environment.NewLine);
    textArea.OnTextCopied(new TextEventArgs(text));
    return true;
  }

  public static bool ConfirmDataFormat(TextArea textArea, DataObject dataObject, string format)
  {
    DataObjectSettingDataEventArgs e = new DataObjectSettingDataEventArgs((IDataObject) dataObject, format);
    textArea.RaiseEvent((RoutedEventArgs) e);
    return !e.CommandCancelled;
  }

  private static bool CopyWholeLine(TextArea textArea, DocumentLine line)
  {
    ISegment segment = (ISegment) new SimpleSegment(line.Offset, line.TotalLength);
    string str = TextUtilities.NormalizeNewLines(textArea.Document.GetText(segment), Environment.NewLine);
    DataObject dataObject = new DataObject();
    if (EditingCommandHandler.ConfirmDataFormat(textArea, dataObject, DataFormats.UnicodeText))
      dataObject.SetText(str);
    if (EditingCommandHandler.ConfirmDataFormat(textArea, dataObject, DataFormats.Html))
    {
      IHighlighter service = textArea.GetService(typeof (IHighlighter)) as IHighlighter;
      HtmlClipboard.SetHtml(dataObject, HtmlClipboard.CreateHtmlFragment((IDocument) textArea.Document, service, segment, new HtmlOptions(textArea.Options)));
    }
    if (EditingCommandHandler.ConfirmDataFormat(textArea, dataObject, "MSDEVLineSelect"))
    {
      MemoryStream data = new MemoryStream(1);
      data.WriteByte((byte) 1);
      dataObject.SetData("MSDEVLineSelect", (object) data, false);
    }
    DataObjectCopyingEventArgs e = new DataObjectCopyingEventArgs((IDataObject) dataObject, false);
    textArea.RaiseEvent((RoutedEventArgs) e);
    if (e.CommandCancelled)
      return false;
    try
    {
      Clipboard.SetDataObject((object) dataObject, true);
    }
    catch (ExternalException ex)
    {
      return false;
    }
    textArea.OnTextCopied(new TextEventArgs(str));
    return true;
  }

  private static void CanPaste(object target, CanExecuteRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    args.CanExecute = textArea.ReadOnlySectionProvider.CanInsert(textArea.Caret.Offset) && Clipboard.ContainsText();
    args.Handled = true;
  }

  private static void OnPaste(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null)
      return;
    if (textArea.Document == null)
      return;
    IDataObject dataObject1;
    try
    {
      dataObject1 = Clipboard.GetDataObject();
    }
    catch (ExternalException ex)
    {
      return;
    }
    if (dataObject1 == null)
      return;
    DataObjectPastingEventArgs pastingEventArgs = new DataObjectPastingEventArgs(dataObject1, false, DataFormats.UnicodeText);
    textArea.RaiseEvent((RoutedEventArgs) pastingEventArgs);
    if (pastingEventArgs.CommandCancelled)
      return;
    string textToPaste = EditingCommandHandler.GetTextToPaste(pastingEventArgs, textArea);
    if (!string.IsNullOrEmpty(textToPaste))
    {
      IDataObject dataObject2 = pastingEventArgs.DataObject;
      bool flag = textArea.Options.CutCopyWholeLine && dataObject2.GetDataPresent("MSDEVLineSelect");
      bool dataPresent = dataObject2.GetDataPresent("AvalonEditRectangularSelection");
      if (flag)
      {
        DocumentLine lineByNumber = textArea.Document.GetLineByNumber(textArea.Caret.Line);
        if (textArea.ReadOnlySectionProvider.CanInsert(lineByNumber.Offset))
          textArea.Document.Insert(lineByNumber.Offset, textToPaste);
      }
      else if (dataPresent && textArea.Selection.IsEmpty && !(textArea.Selection is RectangleSelection))
      {
        if (!RectangleSelection.PerformRectangularPaste(textArea, textArea.Caret.Position, textToPaste, false))
          textArea.ReplaceSelectionWithText(textToPaste);
      }
      else
        textArea.ReplaceSelectionWithText(textToPaste);
    }
    textArea.Caret.BringCaretToView();
    args.Handled = true;
  }

  internal static string GetTextToPaste(
    DataObjectPastingEventArgs pastingEventArgs,
    TextArea textArea)
  {
    IDataObject dataObject = pastingEventArgs.DataObject;
    if (dataObject == null)
      return (string) null;
    try
    {
      string data;
      if (pastingEventArgs.FormatToApply != null && dataObject.GetDataPresent(pastingEventArgs.FormatToApply))
        data = (string) dataObject.GetData(pastingEventArgs.FormatToApply);
      else if (pastingEventArgs.FormatToApply != DataFormats.UnicodeText && dataObject.GetDataPresent(DataFormats.UnicodeText))
      {
        data = (string) dataObject.GetData(DataFormats.UnicodeText);
      }
      else
      {
        if (!(pastingEventArgs.FormatToApply != DataFormats.Text) || !dataObject.GetDataPresent(DataFormats.Text))
          return (string) null;
        data = (string) dataObject.GetData(DataFormats.Text);
      }
      string lineFromDocument = TextUtilities.GetNewLineFromDocument((IDocument) textArea.Document, textArea.Caret.Line);
      string str = TextUtilities.NormalizeNewLines(data, lineFromDocument);
      return textArea.Options.ConvertTabsToSpaces ? str.Replace("\t", new string(' ', textArea.Options.IndentationSize)) : str;
    }
    catch (OutOfMemoryException ex)
    {
      return (string) null;
    }
  }

  private static void OnToggleOverstrike(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || !textArea.Options.AllowToggleOverstrikeMode)
      return;
    textArea.OverstrikeMode = !textArea.OverstrikeMode;
  }

  private static void OnDeleteLine(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    int number1;
    int number2;
    if (textArea.Selection.Length == 0)
    {
      number2 = number1 = textArea.Caret.Line;
    }
    else
    {
      number2 = Math.Min(textArea.Selection.StartPosition.Line, textArea.Selection.EndPosition.Line);
      number1 = Math.Max(textArea.Selection.StartPosition.Line, textArea.Selection.EndPosition.Line);
    }
    DocumentLine lineByNumber1 = textArea.Document.GetLineByNumber(number2);
    DocumentLine lineByNumber2 = textArea.Document.GetLineByNumber(number1);
    textArea.Selection = Selection.Create(textArea, lineByNumber1.Offset, lineByNumber2.Offset + lineByNumber2.TotalLength);
    textArea.RemoveSelectedText();
    args.Handled = true;
  }

  private static void OnRemoveLeadingWhitespace(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.TransformSelectedLines((Action<TextArea, DocumentLine>) ((textArea, line) => textArea.Document.Remove(TextUtilities.GetLeadingWhitespace(textArea.Document, line))), target, args, EditingCommandHandler.DefaultSegmentType.WholeDocument);
  }

  private static void OnRemoveTrailingWhitespace(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.TransformSelectedLines((Action<TextArea, DocumentLine>) ((textArea, line) => textArea.Document.Remove(TextUtilities.GetTrailingWhitespace(textArea.Document, line))), target, args, EditingCommandHandler.DefaultSegmentType.WholeDocument);
  }

  private static void OnConvertTabsToSpaces(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.TransformSelectedSegments(new Action<TextArea, ISegment>(EditingCommandHandler.ConvertTabsToSpaces), target, args, EditingCommandHandler.DefaultSegmentType.WholeDocument);
  }

  private static void OnConvertLeadingTabsToSpaces(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.TransformSelectedLines((Action<TextArea, DocumentLine>) ((textArea, line) => EditingCommandHandler.ConvertTabsToSpaces(textArea, TextUtilities.GetLeadingWhitespace(textArea.Document, line))), target, args, EditingCommandHandler.DefaultSegmentType.WholeDocument);
  }

  private static void ConvertTabsToSpaces(TextArea textArea, ISegment segment)
  {
    TextDocument document = textArea.Document;
    int endOffset = segment.EndOffset;
    string text = new string(' ', textArea.Options.IndentationSize);
    for (int offset = segment.Offset; offset < endOffset; ++offset)
    {
      if (document.GetCharAt(offset) == '\t')
      {
        document.Replace(offset, 1, text, OffsetChangeMappingType.CharacterReplace);
        endOffset += text.Length - 1;
      }
    }
  }

  private static void OnConvertSpacesToTabs(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.TransformSelectedSegments(new Action<TextArea, ISegment>(EditingCommandHandler.ConvertSpacesToTabs), target, args, EditingCommandHandler.DefaultSegmentType.WholeDocument);
  }

  private static void OnConvertLeadingSpacesToTabs(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.TransformSelectedLines((Action<TextArea, DocumentLine>) ((textArea, line) => EditingCommandHandler.ConvertSpacesToTabs(textArea, TextUtilities.GetLeadingWhitespace(textArea.Document, line))), target, args, EditingCommandHandler.DefaultSegmentType.WholeDocument);
  }

  private static void ConvertSpacesToTabs(TextArea textArea, ISegment segment)
  {
    TextDocument document = textArea.Document;
    int endOffset = segment.EndOffset;
    int indentationSize = textArea.Options.IndentationSize;
    int num = 0;
    for (int offset = segment.Offset; offset < endOffset; ++offset)
    {
      if (document.GetCharAt(offset) == ' ')
      {
        ++num;
        if (num == indentationSize)
        {
          document.Replace(offset - (indentationSize - 1), indentationSize, "\t", OffsetChangeMappingType.CharacterReplace);
          num = 0;
          offset -= indentationSize - 1;
          endOffset -= indentationSize - 1;
        }
      }
      else
        num = 0;
    }
  }

  private static void ConvertCase(
    Func<string, string> transformText,
    object target,
    ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.TransformSelectedSegments((Action<TextArea, ISegment>) ((textArea, segment) =>
    {
      string text = transformText(textArea.Document.GetText(segment));
      textArea.Document.Replace(segment.Offset, segment.Length, text, OffsetChangeMappingType.CharacterReplace);
    }), target, args, EditingCommandHandler.DefaultSegmentType.WholeDocument);
  }

  private static void OnConvertToUpperCase(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.ConvertCase(new Func<string, string>(CultureInfo.CurrentCulture.TextInfo.ToUpper), target, args);
  }

  private static void OnConvertToLowerCase(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.ConvertCase(new Func<string, string>(CultureInfo.CurrentCulture.TextInfo.ToLower), target, args);
  }

  private static void OnConvertToTitleCase(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.ConvertCase(new Func<string, string>(CultureInfo.CurrentCulture.TextInfo.ToTitleCase), target, args);
  }

  private static void OnInvertCase(object target, ExecutedRoutedEventArgs args)
  {
    EditingCommandHandler.ConvertCase(new Func<string, string>(EditingCommandHandler.InvertCase), target, args);
  }

  private static string InvertCase(string text)
  {
    CultureInfo currentCulture = CultureInfo.CurrentCulture;
    char[] charArray = text.ToCharArray();
    for (int index = 0; index < charArray.Length; ++index)
    {
      char c = charArray[index];
      charArray[index] = char.IsUpper(c) ? char.ToLower(c, currentCulture) : char.ToUpper(c, currentCulture);
    }
    return new string(charArray);
  }

  private static void OnIndentSelection(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = EditingCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null || textArea.IndentationStrategy == null)
      return;
    using (textArea.Document.RunUpdate())
    {
      int beginLine;
      int endLine;
      if (textArea.Selection.IsEmpty)
      {
        beginLine = 1;
        endLine = textArea.Document.LineCount;
      }
      else
      {
        beginLine = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset).LineNumber;
        endLine = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.EndOffset).LineNumber;
      }
      textArea.IndentationStrategy.IndentLines(textArea.Document, beginLine, endLine);
    }
    textArea.Caret.BringCaretToView();
    args.Handled = true;
  }

  private enum DefaultSegmentType
  {
    None,
    WholeDocument,
    CurrentLine,
  }
}

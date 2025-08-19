// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.CaretNavigationCommandHandler
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal static class CaretNavigationCommandHandler
{
  private static readonly List<CommandBinding> CommandBindings = new List<CommandBinding>();
  private static readonly List<InputBinding> InputBindings = new List<InputBinding>();

  public static TextAreaInputHandler Create(TextArea textArea)
  {
    TextAreaInputHandler areaInputHandler = new TextAreaInputHandler(textArea);
    areaInputHandler.CommandBindings.AddRange<CommandBinding>((IEnumerable<CommandBinding>) CaretNavigationCommandHandler.CommandBindings);
    areaInputHandler.InputBindings.AddRange<InputBinding>((IEnumerable<InputBinding>) CaretNavigationCommandHandler.InputBindings);
    return areaInputHandler;
  }

  private static void AddBinding(
    ICommand command,
    ModifierKeys modifiers,
    Key key,
    ExecutedRoutedEventHandler handler)
  {
    CaretNavigationCommandHandler.CommandBindings.Add(new CommandBinding(command, handler));
    CaretNavigationCommandHandler.InputBindings.Add((InputBinding) TextAreaDefaultInputHandler.CreateFrozenKeyBinding(command, modifiers, key));
  }

  static CaretNavigationCommandHandler()
  {
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveLeftByCharacter, ModifierKeys.None, Key.Left, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.CharLeft));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectLeftByCharacter, ModifierKeys.Shift, Key.Left, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.CharLeft));
    CaretNavigationCommandHandler.AddBinding((ICommand) RectangleSelection.BoxSelectLeftByCharacter, ModifierKeys.Alt | ModifierKeys.Shift, Key.Left, CaretNavigationCommandHandler.OnMoveCaretBoxSelection(CaretMovementType.CharLeft));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveRightByCharacter, ModifierKeys.None, Key.Right, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.CharRight));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectRightByCharacter, ModifierKeys.Shift, Key.Right, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.CharRight));
    CaretNavigationCommandHandler.AddBinding((ICommand) RectangleSelection.BoxSelectRightByCharacter, ModifierKeys.Alt | ModifierKeys.Shift, Key.Right, CaretNavigationCommandHandler.OnMoveCaretBoxSelection(CaretMovementType.CharRight));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveLeftByWord, ModifierKeys.Control, Key.Left, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.WordLeft));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectLeftByWord, ModifierKeys.Control | ModifierKeys.Shift, Key.Left, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.WordLeft));
    CaretNavigationCommandHandler.AddBinding((ICommand) RectangleSelection.BoxSelectLeftByWord, ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift, Key.Left, CaretNavigationCommandHandler.OnMoveCaretBoxSelection(CaretMovementType.WordLeft));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveRightByWord, ModifierKeys.Control, Key.Right, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.WordRight));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectRightByWord, ModifierKeys.Control | ModifierKeys.Shift, Key.Right, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.WordRight));
    CaretNavigationCommandHandler.AddBinding((ICommand) RectangleSelection.BoxSelectRightByWord, ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift, Key.Right, CaretNavigationCommandHandler.OnMoveCaretBoxSelection(CaretMovementType.WordRight));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveUpByLine, ModifierKeys.None, Key.Up, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.LineUp));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectUpByLine, ModifierKeys.Shift, Key.Up, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.LineUp));
    CaretNavigationCommandHandler.AddBinding((ICommand) RectangleSelection.BoxSelectUpByLine, ModifierKeys.Alt | ModifierKeys.Shift, Key.Up, CaretNavigationCommandHandler.OnMoveCaretBoxSelection(CaretMovementType.LineUp));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveDownByLine, ModifierKeys.None, Key.Down, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.LineDown));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectDownByLine, ModifierKeys.Shift, Key.Down, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.LineDown));
    CaretNavigationCommandHandler.AddBinding((ICommand) RectangleSelection.BoxSelectDownByLine, ModifierKeys.Alt | ModifierKeys.Shift, Key.Down, CaretNavigationCommandHandler.OnMoveCaretBoxSelection(CaretMovementType.LineDown));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveDownByPage, ModifierKeys.None, Key.Next, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.PageDown));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectDownByPage, ModifierKeys.Shift, Key.Next, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.PageDown));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveUpByPage, ModifierKeys.None, Key.Prior, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.PageUp));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectUpByPage, ModifierKeys.Shift, Key.Prior, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.PageUp));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveToLineStart, ModifierKeys.None, Key.Home, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.LineStart));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectToLineStart, ModifierKeys.Shift, Key.Home, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.LineStart));
    CaretNavigationCommandHandler.AddBinding((ICommand) RectangleSelection.BoxSelectToLineStart, ModifierKeys.Alt | ModifierKeys.Shift, Key.Home, CaretNavigationCommandHandler.OnMoveCaretBoxSelection(CaretMovementType.LineStart));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveToLineEnd, ModifierKeys.None, Key.End, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.LineEnd));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectToLineEnd, ModifierKeys.Shift, Key.End, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.LineEnd));
    CaretNavigationCommandHandler.AddBinding((ICommand) RectangleSelection.BoxSelectToLineEnd, ModifierKeys.Alt | ModifierKeys.Shift, Key.End, CaretNavigationCommandHandler.OnMoveCaretBoxSelection(CaretMovementType.LineEnd));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveToDocumentStart, ModifierKeys.Control, Key.Home, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.DocumentStart));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectToDocumentStart, ModifierKeys.Control | ModifierKeys.Shift, Key.Home, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.DocumentStart));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.MoveToDocumentEnd, ModifierKeys.Control, Key.End, CaretNavigationCommandHandler.OnMoveCaret(CaretMovementType.DocumentEnd));
    CaretNavigationCommandHandler.AddBinding((ICommand) EditingCommands.SelectToDocumentEnd, ModifierKeys.Control | ModifierKeys.Shift, Key.End, CaretNavigationCommandHandler.OnMoveCaretExtendSelection(CaretMovementType.DocumentEnd));
    CaretNavigationCommandHandler.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.SelectAll, new ExecutedRoutedEventHandler(CaretNavigationCommandHandler.OnSelectAll)));
    TextAreaDefaultInputHandler.WorkaroundWPFMemoryLeak(CaretNavigationCommandHandler.InputBindings);
  }

  private static void OnSelectAll(object target, ExecutedRoutedEventArgs args)
  {
    TextArea textArea = CaretNavigationCommandHandler.GetTextArea(target);
    if (textArea == null || textArea.Document == null)
      return;
    args.Handled = true;
    textArea.Caret.Offset = textArea.Document.TextLength;
    textArea.Selection = Selection.Create(textArea, 0, textArea.Document.TextLength);
  }

  private static TextArea GetTextArea(object target) => target as TextArea;

  private static ExecutedRoutedEventHandler OnMoveCaret(CaretMovementType direction)
  {
    return (ExecutedRoutedEventHandler) ((target, args) =>
    {
      TextArea textArea = CaretNavigationCommandHandler.GetTextArea(target);
      if (textArea == null || textArea.Document == null)
        return;
      args.Handled = true;
      textArea.ClearSelection();
      CaretNavigationCommandHandler.MoveCaret(textArea, direction);
      textArea.Caret.BringCaretToView();
    });
  }

  private static ExecutedRoutedEventHandler OnMoveCaretExtendSelection(CaretMovementType direction)
  {
    return (ExecutedRoutedEventHandler) ((target, args) =>
    {
      TextArea textArea = CaretNavigationCommandHandler.GetTextArea(target);
      if (textArea == null || textArea.Document == null)
        return;
      args.Handled = true;
      TextViewPosition position = textArea.Caret.Position;
      CaretNavigationCommandHandler.MoveCaret(textArea, direction);
      textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
      textArea.Caret.BringCaretToView();
    });
  }

  private static ExecutedRoutedEventHandler OnMoveCaretBoxSelection(CaretMovementType direction)
  {
    return (ExecutedRoutedEventHandler) ((target, args) =>
    {
      TextArea textArea = CaretNavigationCommandHandler.GetTextArea(target);
      if (textArea == null || textArea.Document == null)
        return;
      args.Handled = true;
      if (textArea.Options.EnableRectangularSelection && !(textArea.Selection is RectangleSelection))
        textArea.Selection = !textArea.Selection.IsEmpty ? (Selection) new RectangleSelection(textArea, textArea.Selection.StartPosition, textArea.Caret.Position) : (Selection) new RectangleSelection(textArea, textArea.Caret.Position, textArea.Caret.Position);
      TextViewPosition position = textArea.Caret.Position;
      CaretNavigationCommandHandler.MoveCaret(textArea, direction);
      textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
      textArea.Caret.BringCaretToView();
    });
  }

  internal static void MoveCaret(TextArea textArea, CaretMovementType direction)
  {
    double desiredXpos = textArea.Caret.DesiredXPos;
    textArea.Caret.Position = CaretNavigationCommandHandler.GetNewCaretPosition(textArea.TextView, textArea.Caret.Position, direction, textArea.Selection.EnableVirtualSpace, ref desiredXpos);
    textArea.Caret.DesiredXPos = desiredXpos;
  }

  internal static TextViewPosition GetNewCaretPosition(
    TextView textView,
    TextViewPosition caretPosition,
    CaretMovementType direction,
    bool enableVirtualSpace,
    ref double desiredXPos)
  {
    switch (direction)
    {
      case CaretMovementType.None:
        return caretPosition;
      case CaretMovementType.DocumentStart:
        desiredXPos = double.NaN;
        return new TextViewPosition(0, 0);
      case CaretMovementType.DocumentEnd:
        desiredXPos = double.NaN;
        return new TextViewPosition(textView.Document.GetLocation(textView.Document.TextLength));
      default:
        DocumentLine lineByNumber = textView.Document.GetLineByNumber(caretPosition.Line);
        VisualLine constructVisualLine = textView.GetOrConstructVisualLine(lineByNumber);
        TextLine textLine = constructVisualLine.GetTextLine(caretPosition.VisualColumn, caretPosition.IsAtEndOfLine);
        switch (direction)
        {
          case CaretMovementType.CharLeft:
            desiredXPos = double.NaN;
            return caretPosition.VisualColumn == 0 && enableVirtualSpace ? caretPosition : CaretNavigationCommandHandler.GetPrevCaretPosition(textView, caretPosition, constructVisualLine, CaretPositioningMode.Normal, enableVirtualSpace);
          case CaretMovementType.CharRight:
            desiredXPos = double.NaN;
            return CaretNavigationCommandHandler.GetNextCaretPosition(textView, caretPosition, constructVisualLine, CaretPositioningMode.Normal, enableVirtualSpace);
          case CaretMovementType.Backspace:
            desiredXPos = double.NaN;
            return CaretNavigationCommandHandler.GetPrevCaretPosition(textView, caretPosition, constructVisualLine, CaretPositioningMode.EveryCodepoint, enableVirtualSpace);
          case CaretMovementType.WordLeft:
            desiredXPos = double.NaN;
            return CaretNavigationCommandHandler.GetPrevCaretPosition(textView, caretPosition, constructVisualLine, CaretPositioningMode.WordStart, enableVirtualSpace);
          case CaretMovementType.WordRight:
            desiredXPos = double.NaN;
            return CaretNavigationCommandHandler.GetNextCaretPosition(textView, caretPosition, constructVisualLine, CaretPositioningMode.WordStart, enableVirtualSpace);
          case CaretMovementType.LineUp:
          case CaretMovementType.LineDown:
          case CaretMovementType.PageUp:
          case CaretMovementType.PageDown:
            return CaretNavigationCommandHandler.GetUpDownCaretPosition(textView, caretPosition, direction, constructVisualLine, textLine, enableVirtualSpace, ref desiredXPos);
          case CaretMovementType.LineStart:
            desiredXPos = double.NaN;
            return CaretNavigationCommandHandler.GetStartOfLineCaretPosition(caretPosition.VisualColumn, constructVisualLine, textLine, enableVirtualSpace);
          case CaretMovementType.LineEnd:
            desiredXPos = double.NaN;
            return CaretNavigationCommandHandler.GetEndOfLineCaretPosition(constructVisualLine, textLine);
          default:
            throw new NotSupportedException(direction.ToString());
        }
    }
  }

  private static TextViewPosition GetStartOfLineCaretPosition(
    int oldVC,
    VisualLine visualLine,
    TextLine textLine,
    bool enableVirtualSpace)
  {
    int visualColumn = visualLine.GetTextLineVisualStartColumn(textLine);
    if (visualColumn == 0)
      visualColumn = visualLine.GetNextCaretPosition(visualColumn - 1, LogicalDirection.Forward, CaretPositioningMode.WordStart, enableVirtualSpace);
    if (visualColumn < 0)
      throw ThrowUtil.NoValidCaretPosition();
    if (visualColumn == oldVC)
      visualColumn = 0;
    return visualLine.GetTextViewPosition(visualColumn);
  }

  private static TextViewPosition GetEndOfLineCaretPosition(
    VisualLine visualLine,
    TextLine textLine)
  {
    int visualColumn = visualLine.GetTextLineVisualStartColumn(textLine) + textLine.Length - textLine.TrailingWhitespaceLength;
    return visualLine.GetTextViewPosition(visualColumn) with
    {
      IsAtEndOfLine = true
    };
  }

  private static TextViewPosition GetNextCaretPosition(
    TextView textView,
    TextViewPosition caretPosition,
    VisualLine visualLine,
    CaretPositioningMode mode,
    bool enableVirtualSpace)
  {
    int nextCaretPosition1 = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, LogicalDirection.Forward, mode, enableVirtualSpace);
    if (nextCaretPosition1 >= 0)
      return visualLine.GetTextViewPosition(nextCaretPosition1);
    DocumentLine nextLine = visualLine.LastDocumentLine.NextLine;
    if (nextLine == null)
      return new TextViewPosition(textView.Document.GetLocation(textView.Document.TextLength));
    VisualLine constructVisualLine = textView.GetOrConstructVisualLine(nextLine);
    int nextCaretPosition2 = constructVisualLine.GetNextCaretPosition(-1, LogicalDirection.Forward, mode, enableVirtualSpace);
    return nextCaretPosition2 >= 0 ? constructVisualLine.GetTextViewPosition(nextCaretPosition2) : throw ThrowUtil.NoValidCaretPosition();
  }

  private static TextViewPosition GetPrevCaretPosition(
    TextView textView,
    TextViewPosition caretPosition,
    VisualLine visualLine,
    CaretPositioningMode mode,
    bool enableVirtualSpace)
  {
    int nextCaretPosition1 = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, LogicalDirection.Backward, mode, enableVirtualSpace);
    if (nextCaretPosition1 >= 0)
      return visualLine.GetTextViewPosition(nextCaretPosition1);
    DocumentLine previousLine = visualLine.FirstDocumentLine.PreviousLine;
    if (previousLine == null)
      return new TextViewPosition(0, 0);
    VisualLine constructVisualLine = textView.GetOrConstructVisualLine(previousLine);
    int nextCaretPosition2 = constructVisualLine.GetNextCaretPosition(constructVisualLine.VisualLength + 1, LogicalDirection.Backward, mode, enableVirtualSpace);
    return nextCaretPosition2 >= 0 ? constructVisualLine.GetTextViewPosition(nextCaretPosition2) : throw ThrowUtil.NoValidCaretPosition();
  }

  private static TextViewPosition GetUpDownCaretPosition(
    TextView textView,
    TextViewPosition caretPosition,
    CaretMovementType direction,
    VisualLine visualLine,
    TextLine textLine,
    bool enableVirtualSpace,
    ref double xPos)
  {
    if (double.IsNaN(xPos))
      xPos = visualLine.GetTextLineVisualXPosition(textLine, caretPosition.VisualColumn);
    VisualLine visualLine1 = visualLine;
    int num = visualLine.TextLines.IndexOf(textLine);
    TextLine textLine1;
    switch (direction)
    {
      case CaretMovementType.LineUp:
        int number1 = visualLine.FirstDocumentLine.LineNumber - 1;
        if (num > 0)
        {
          textLine1 = visualLine.TextLines[num - 1];
          break;
        }
        if (number1 >= 1)
        {
          DocumentLine lineByNumber = textView.Document.GetLineByNumber(number1);
          visualLine1 = textView.GetOrConstructVisualLine(lineByNumber);
          textLine1 = visualLine1.TextLines[visualLine1.TextLines.Count - 1];
          break;
        }
        textLine1 = (TextLine) null;
        break;
      case CaretMovementType.LineDown:
        int number2 = visualLine.LastDocumentLine.LineNumber + 1;
        if (num < visualLine.TextLines.Count - 1)
        {
          textLine1 = visualLine.TextLines[num + 1];
          break;
        }
        if (number2 <= textView.Document.LineCount)
        {
          DocumentLine lineByNumber = textView.Document.GetLineByNumber(number2);
          visualLine1 = textView.GetOrConstructVisualLine(lineByNumber);
          textLine1 = visualLine1.TextLines[0];
          break;
        }
        textLine1 = (TextLine) null;
        break;
      case CaretMovementType.PageUp:
      case CaretMovementType.PageDown:
        double lineVisualYposition1 = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineMiddle);
        double visualTop = direction != CaretMovementType.PageUp ? lineVisualYposition1 + textView.RenderSize.Height : lineVisualYposition1 - textView.RenderSize.Height;
        DocumentLine documentLineByVisualTop = textView.GetDocumentLineByVisualTop(visualTop);
        visualLine1 = textView.GetOrConstructVisualLine(documentLineByVisualTop);
        textLine1 = visualLine1.GetTextLineByVisualYPosition(visualTop);
        break;
      default:
        throw new NotSupportedException(direction.ToString());
    }
    if (textLine1 == null)
      return caretPosition;
    double lineVisualYposition2 = visualLine1.GetTextLineVisualYPosition(textLine1, VisualYPosition.LineMiddle);
    int visualColumn = visualLine1.GetVisualColumn(new Point(xPos, lineVisualYposition2), enableVirtualSpace);
    int visualStartColumn = visualLine1.GetTextLineVisualStartColumn(textLine1);
    if (visualColumn >= visualStartColumn + textLine1.Length && visualColumn <= visualLine1.VisualLength)
      visualColumn = visualStartColumn + textLine1.Length - 1;
    return visualLine1.GetTextViewPosition(visualColumn);
  }
}

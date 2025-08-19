// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlParseError
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Html.Parser;

public enum HtmlParseError : byte
{
  [DomDescription("Unexpected end of the given file.")] EOF = 0,
  [DomDescription("NULL character replaced by repl. character.")] Null = 1,
  [DomDescription("Bogus comment detected.")] BogusComment = 26, // 0x1A
  [DomDescription("Ambiguous open tag.")] AmbiguousOpenTag = 27, // 0x1B
  [DomDescription("The tag has been closed unexpectedly.")] TagClosedWrong = 28, // 0x1C
  [DomDescription("The closing slash has been misplaced.")] ClosingSlashMisplaced = 29, // 0x1D
  [DomDescription("Undefined markup declaration found.")] UndefinedMarkupDeclaration = 30, // 0x1E
  [DomDescription("Comment ended with an exclamation mark.")] CommentEndedWithEM = 31, // 0x1F
  [DomDescription("Comment ended with a dash.")] CommentEndedWithDash = 32, // 0x20
  [DomDescription("Comment ended with an unexpected character.")] CommentEndedUnexpected = 33, // 0x21
  [DomDescription("The given tag cannot be self-closed.")] TagCannotBeSelfClosed = 34, // 0x22
  [DomDescription("End tags can never be self-closed.")] EndTagCannotBeSelfClosed = 35, // 0x23
  [DomDescription("End tags cannot carry attributes.")] EndTagCannotHaveAttributes = 36, // 0x24
  [DomDescription("No caption tag has been found within the local scope.")] CaptionNotInScope = 37, // 0x25
  [DomDescription("No select tag has been found within the local scope.")] SelectNotInScope = 38, // 0x26
  [DomDescription("No table row has been found within the local scope.")] TableRowNotInScope = 39, // 0x27
  [DomDescription("No table has been found within the local scope.")] TableNotInScope = 40, // 0x28
  [DomDescription("No paragraph has been found within the local scope.")] ParagraphNotInScope = 41, // 0x29
  [DomDescription("No body has been found within the local scope.")] BodyNotInScope = 42, // 0x2A
  [DomDescription("No block element has been found within the local scope.")] BlockNotInScope = 43, // 0x2B
  [DomDescription("No table cell has been found within the local scope.")] TableCellNotInScope = 44, // 0x2C
  [DomDescription("No table section has been found within the local scope.")] TableSectionNotInScope = 45, // 0x2D
  [DomDescription("No object element has been found within the local scope.")] ObjectNotInScope = 46, // 0x2E
  [DomDescription("No heading element has been found within the local scope.")] HeadingNotInScope = 47, // 0x2F
  [DomDescription("No list item has been found within the local scope.")] ListItemNotInScope = 48, // 0x30
  [DomDescription("No form has been found within the local scope.")] FormNotInScope = 49, // 0x31
  [DomDescription("No button has been found within the local scope.")] ButtonInScope = 50, // 0x32
  [DomDescription("No nobr element has been found within the local scope.")] NobrInScope = 51, // 0x33
  [DomDescription("No element has been found within the local scope.")] ElementNotInScope = 52, // 0x34
  [DomDescription("Character reference found no numbers.")] CharacterReferenceWrongNumber = 53, // 0x35
  [DomDescription("Character reference found no semicolon.")] CharacterReferenceSemicolonMissing = 54, // 0x36
  [DomDescription("Character reference within an invalid range.")] CharacterReferenceInvalidRange = 55, // 0x37
  [DomDescription("Character reference is an invalid number.")] CharacterReferenceInvalidNumber = 56, // 0x38
  [DomDescription("Character reference is an invalid code.")] CharacterReferenceInvalidCode = 57, // 0x39
  [DomDescription("Character reference is not terminated by a semicolon.")] CharacterReferenceNotTerminated = 58, // 0x3A
  [DomDescription("Character reference in attribute contains an invalid character (=).")] CharacterReferenceAttributeEqualsFound = 59, // 0x3B
  [DomDescription("The specified item has not been found.")] ItemNotFound = 60, // 0x3C
  [DomDescription("The encoding operation (either encoded or decoding) failed.")] EncodingError = 61, // 0x3D
  [DomDescription("Doctype unexpected character after the name detected.")] DoctypeUnexpectedAfterName = 64, // 0x40
  [DomDescription("Invalid character in the public identifier detected.")] DoctypePublicInvalid = 65, // 0x41
  [DomDescription("Invalid character in the doctype detected.")] DoctypeInvalidCharacter = 66, // 0x42
  [DomDescription("Invalid character in the system identifier detected.")] DoctypeSystemInvalid = 67, // 0x43
  [DomDescription("The doctype tag is misplaced and ignored.")] DoctypeTagInappropriate = 68, // 0x44
  [DomDescription("The given doctype tag is invalid.")] DoctypeInvalid = 69, // 0x45
  [DomDescription("Doctype encountered unexpected character.")] DoctypeUnexpected = 70, // 0x46
  [DomDescription("The doctype tag is missing.")] DoctypeMissing = 71, // 0x47
  [DomDescription("The given public identifier for the notation declaration is invalid.")] NotationPublicInvalid = 72, // 0x48
  [DomDescription("The given system identifier for the notation declaration is invalid.")] NotationSystemInvalid = 73, // 0x49
  [DomDescription("The type declaration is missing a valid definition.")] TypeDeclarationUndefined = 74, // 0x4A
  [DomDescription("A required quantifier is missing in the provided expression.")] QuantifierMissing = 75, // 0x4B
  [DomDescription("The double quotation marks have been misplaced.")] DoubleQuotationMarkUnexpected = 80, // 0x50
  [DomDescription("The single quotation marks have been misplaced.")] SingleQuotationMarkUnexpected = 81, // 0x51
  [DomDescription("The attribute's name contains an invalid character.")] AttributeNameInvalid = 96, // 0x60
  [DomDescription("The attribute's value contains an invalid character.")] AttributeValueInvalid = 97, // 0x61
  [DomDescription("The beginning of a new attribute has been expected.")] AttributeNameExpected = 98, // 0x62
  [DomDescription("The attribute has already been added.")] AttributeDuplicateOmitted = 99, // 0x63
  [DomDescription("The given tag must be placed in head tag.")] TagMustBeInHead = 112, // 0x70
  [DomDescription("The given tag is not appropriate for the current position.")] TagInappropriate = 113, // 0x71
  [DomDescription("The given tag cannot end at the current position.")] TagCannotEndHere = 114, // 0x72
  [DomDescription("The given tag cannot start at the current position.")] TagCannotStartHere = 115, // 0x73
  [DomDescription("The given form cannot be placed at the current position.")] FormInappropriate = 116, // 0x74
  [DomDescription("The given input cannot be placed at the current position.")] InputUnexpected = 117, // 0x75
  [DomDescription("The closing tag and the currently open tag do not match.")] TagClosingMismatch = 118, // 0x76
  [DomDescription("The given end tag does not match the current node.")] TagDoesNotMatchCurrentNode = 119, // 0x77
  [DomDescription("This position does not support a linebreak (LF, FF).")] LineBreakUnexpected = 120, // 0x78
  [DomDescription("The head tag can only be placed once inside the html tag.")] HeadTagMisplaced = 128, // 0x80
  [DomDescription("The html tag can only be placed once as the root element.")] HtmlTagMisplaced = 129, // 0x81
  [DomDescription("The body tag can only be placed once inside the html tag.")] BodyTagMisplaced = 130, // 0x82
  [DomDescription("The image tag has been named image instead of img.")] ImageTagNamedWrong = 131, // 0x83
  [DomDescription("Tables cannot be nested.")] TableNesting = 132, // 0x84
  [DomDescription("An illegal element has been detected in a table.")] IllegalElementInTableDetected = 133, // 0x85
  [DomDescription("Select elements cannot be nested.")] SelectNesting = 134, // 0x86
  [DomDescription("An illegal element has been detected in a select.")] IllegalElementInSelectDetected = 135, // 0x87
  [DomDescription("The frameset element has been misplaced.")] FramesetMisplaced = 136, // 0x88
  [DomDescription("Headings cannot be nested.")] HeadingNested = 137, // 0x89
  [DomDescription("Anchor elements cannot be nested.")] AnchorNested = 138, // 0x8A
  [DomDescription("The given token cannot be inserted here.")] TokenNotPossible = 144, // 0x90
  [DomDescription("The current node is not the root element.")] CurrentNodeIsNotRoot = 145, // 0x91
  [DomDescription("The current node is the root element.")] CurrentNodeIsRoot = 146, // 0x92
  [DomDescription("This tag is invalid in fragment mode.")] TagInvalidInFragmentMode = 147, // 0x93
  [DomDescription("There is already an open form.")] FormAlreadyOpen = 148, // 0x94
  [DomDescription("The form has been closed wrong.")] FormClosedWrong = 149, // 0x95
  [DomDescription("The body has been closed wrong.")] BodyClosedWrong = 150, // 0x96
  [DomDescription("An expected formatting element has not been found.")] FormattingElementNotFound = 151, // 0x97
}

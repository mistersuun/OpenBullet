// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.DomError
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("DOMError")]
public enum DomError : byte
{
  [DomDescription("The index is not in the allowed range."), DomName("INDEX_SIZE_ERR")] IndexSizeError = 1,
  [DomDescription("The size of the string is invalid."), DomName("DOMSTRING_SIZE_ERR"), DomHistorical] DomStringSize = 2,
  [DomDescription("The operation would yield an incorrect node tree."), DomName("HIERARCHY_REQUEST_ERR")] HierarchyRequest = 3,
  [DomDescription("The object is in the wrong document."), DomName("WRONG_DOCUMENT_ERR")] WrongDocument = 4,
  [DomDescription("Invalid character detected."), DomName("INVALID_CHARACTER_ERR")] InvalidCharacter = 5,
  [DomDescription("The data is allowed for this object."), DomName("NO_DATA_ALLOWED_ERR"), DomHistorical] NoDataAllowed = 6,
  [DomDescription("The object can not be modified."), DomName("NO_MODIFICATION_ALLOWED_ERR")] NoModificationAllowed = 7,
  [DomDescription("The object can not be found here."), DomName("NOT_FOUND_ERR")] NotFound = 8,
  [DomDescription("The operation is not supported."), DomName("NOT_SUPPORTED_ERR")] NotSupported = 9,
  [DomDescription("The element is already in-use."), DomName("INUSE_ATTRIBUTE_ERR"), DomHistorical] InUse = 10, // 0x0A
  [DomDescription("The object is in an invalid state."), DomName("INVALID_STATE_ERR")] InvalidState = 11, // 0x0B
  [DomDescription("The string did not match the expected pattern."), DomName("SYNTAX_ERR")] Syntax = 12, // 0x0C
  [DomDescription("The object can not be modified in this way."), DomName("INVALID_MODIFICATION_ERR")] InvalidModification = 13, // 0x0D
  [DomDescription("The operation is not allowed by namespaces in XML."), DomName("NAMESPACE_ERR")] Namespace = 14, // 0x0E
  [DomDescription("The object does not support the operation or argument."), DomName("INVALID_ACCESS_ERR")] InvalidAccess = 15, // 0x0F
  [DomDescription("The validation failed."), DomName("VALIDATION_ERR")] Validation = 15, // 0x0F
  [DomDescription("The provided argument type is invalid."), DomName("TYPE_MISMATCH_ERR"), DomHistorical] TypeMismatch = 17, // 0x11
  [DomDescription("The operation is insecure."), DomName("SECURITY_ERR")] Security = 18, // 0x12
  [DomDescription("A network error occurred."), DomName("NETWORK_ERR")] Network = 19, // 0x13
  [DomDescription("The operation was aborted."), DomName("ABORT_ERR")] Abort = 20, // 0x14
  [DomDescription("The given URL does not match another URL."), DomName("URL_MISMATCH_ERR")] UrlMismatch = 21, // 0x15
  [DomDescription("The quota has been exceeded."), DomName("QUOTA_EXCEEDED_ERR")] QuotaExceeded = 22, // 0x16
  [DomDescription("The operation timed out."), DomName("TIMEOUT_ERR")] Timeout = 23, // 0x17
  [DomDescription("The supplied node is incorrect or has an incorrect ancestor for this operation."), DomName("INVALID_NODE_TYPE_ERR")] InvalidNodeType = 24, // 0x18
  [DomDescription("The object can not be cloned."), DomName("DATA_CLONE_ERR")] DataClone = 25, // 0x19
}

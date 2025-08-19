// Decompiled with JetBrains decompiler
// Type: IronPython.Resources
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal Resources()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (IronPython.Resources.resourceMan == null)
        IronPython.Resources.resourceMan = new ResourceManager("IronPython.Resources", typeof (IronPython.Resources).Assembly);
      return IronPython.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => IronPython.Resources.resourceCulture;
    set => IronPython.Resources.resourceCulture = value;
  }

  internal static string CantFindMember
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (CantFindMember), IronPython.Resources.resourceCulture);
  }

  internal static string DefaultRequired
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (DefaultRequired), IronPython.Resources.resourceCulture);
  }

  internal static string DuplicateArgumentInFuncDef
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (DuplicateArgumentInFuncDef), IronPython.Resources.resourceCulture);
    }
  }

  internal static string DuplicateKeywordArg
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (DuplicateKeywordArg), IronPython.Resources.resourceCulture);
    }
  }

  internal static string EofInString
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (EofInString), IronPython.Resources.resourceCulture);
  }

  internal static string EofInTripleQuotedString
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (EofInTripleQuotedString), IronPython.Resources.resourceCulture);
    }
  }

  internal static string EolInSingleQuotedString
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (EolInSingleQuotedString), IronPython.Resources.resourceCulture);
    }
  }

  internal static string ExpectedIndentation
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (ExpectedIndentation), IronPython.Resources.resourceCulture);
    }
  }

  internal static string ExpectedName
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (ExpectedName), IronPython.Resources.resourceCulture);
  }

  internal static string ExpectingIdentifier
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (ExpectingIdentifier), IronPython.Resources.resourceCulture);
    }
  }

  internal static string InconsistentWhitespace
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (InconsistentWhitespace), IronPython.Resources.resourceCulture);
    }
  }

  internal static string IndentationMismatch
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (IndentationMismatch), IronPython.Resources.resourceCulture);
    }
  }

  internal static string InvalidArgumentValue
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (InvalidArgumentValue), IronPython.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperation_MakeGenericOnNonGeneric
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (InvalidOperation_MakeGenericOnNonGeneric), IronPython.Resources.resourceCulture);
    }
  }

  internal static string InvalidParameters
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (InvalidParameters), IronPython.Resources.resourceCulture);
    }
  }

  internal static string InvalidToken
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (InvalidToken), IronPython.Resources.resourceCulture);
  }

  internal static string InvalidSyntax
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (InvalidSyntax), IronPython.Resources.resourceCulture);
  }

  internal static string KeywordCreateUnavailable
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (KeywordCreateUnavailable), IronPython.Resources.resourceCulture);
    }
  }

  internal static string KeywordOutOfSequence
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (KeywordOutOfSequence), IronPython.Resources.resourceCulture);
    }
  }

  internal static string MemberDoesNotExist
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (MemberDoesNotExist), IronPython.Resources.resourceCulture);
    }
  }

  internal static string MisplacedFuture
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (MisplacedFuture), IronPython.Resources.resourceCulture);
  }

  internal static string MisplacedReturn
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (MisplacedReturn), IronPython.Resources.resourceCulture);
  }

  internal static string MisplacedYield
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (MisplacedYield), IronPython.Resources.resourceCulture);
  }

  internal static string NewLineInDoubleQuotedString
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (NewLineInDoubleQuotedString), IronPython.Resources.resourceCulture);
    }
  }

  internal static string NewLineInSingleQuotedString
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (NewLineInSingleQuotedString), IronPython.Resources.resourceCulture);
    }
  }

  internal static string NoFutureStar
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (NoFutureStar), IronPython.Resources.resourceCulture);
  }

  internal static string NonKeywordAfterKeywordArg
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (NonKeywordAfterKeywordArg), IronPython.Resources.resourceCulture);
    }
  }

  internal static string NotAChance
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (NotAChance), IronPython.Resources.resourceCulture);
  }

  internal static string NotImplemented
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (NotImplemented), IronPython.Resources.resourceCulture);
  }

  internal static string OneKeywordArgOnly
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (OneKeywordArgOnly), IronPython.Resources.resourceCulture);
    }
  }

  internal static string OneListArgOnly
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (OneListArgOnly), IronPython.Resources.resourceCulture);
  }

  internal static string PythonContextRequired
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (PythonContextRequired), IronPython.Resources.resourceCulture);
    }
  }

  internal static string Slot_CantDelete
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (Slot_CantDelete), IronPython.Resources.resourceCulture);
  }

  internal static string Slot_CantGet
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (Slot_CantGet), IronPython.Resources.resourceCulture);
  }

  internal static string Slot_CantSet
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (Slot_CantSet), IronPython.Resources.resourceCulture);
  }

  internal static string StaticAccessFromInstanceError
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (StaticAccessFromInstanceError), IronPython.Resources.resourceCulture);
    }
  }

  internal static string StaticAssignmentFromInstanceError
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (StaticAssignmentFromInstanceError), IronPython.Resources.resourceCulture);
    }
  }

  internal static string TokenHasNoValue
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (TokenHasNoValue), IronPython.Resources.resourceCulture);
  }

  internal static string TooManyVersions
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (TooManyVersions), IronPython.Resources.resourceCulture);
  }

  internal static string UnexpectedToken
  {
    get => IronPython.Resources.ResourceManager.GetString(nameof (UnexpectedToken), IronPython.Resources.resourceCulture);
  }

  internal static string UnknownFutureFeature
  {
    get
    {
      return IronPython.Resources.ResourceManager.GetString(nameof (UnknownFutureFeature), IronPython.Resources.resourceCulture);
    }
  }
}

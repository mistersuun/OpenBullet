// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockUtility
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using RuriLib.Functions.Conversions;
using RuriLib.LS;
using RuriLib.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BlockUtility : BlockBase
{
  private UtilityGroup group;
  private string variableName = "";
  private bool isCapture;
  private string inputString = "";
  private ListAction listAction = ListAction.Join;
  private string listName = "";
  private string separator = ",";
  private bool ascending = true;
  private bool numeric;
  private string secondListName = "";
  private string listItem = "";
  private string listIndex = "-1";
  private VarAction varAction;
  private string varName = "";
  private string splitSeparator = "";
  private Encoding conversionFrom;
  private Encoding conversionTo = Encoding.BASE64;
  private string filePath = "test.txt";
  private FileAction fileAction;

  public UtilityGroup Group
  {
    get => this.group;
    set
    {
      this.group = value;
      this.OnPropertyChanged(nameof (Group));
    }
  }

  public string VariableName
  {
    get => this.variableName;
    set
    {
      this.variableName = value;
      this.OnPropertyChanged(nameof (VariableName));
    }
  }

  public bool IsCapture
  {
    get => this.isCapture;
    set
    {
      this.isCapture = value;
      this.OnPropertyChanged(nameof (IsCapture));
    }
  }

  public string InputString
  {
    get => this.inputString;
    set
    {
      this.inputString = value;
      this.OnPropertyChanged(nameof (InputString));
    }
  }

  public ListAction ListAction
  {
    get => this.listAction;
    set
    {
      this.listAction = value;
      this.OnPropertyChanged(nameof (ListAction));
    }
  }

  public string ListName
  {
    get => this.listName;
    set
    {
      this.listName = value;
      this.OnPropertyChanged(nameof (ListName));
    }
  }

  public string Separator
  {
    get => this.separator;
    set
    {
      this.separator = value;
      this.OnPropertyChanged(nameof (Separator));
    }
  }

  public bool Ascending
  {
    get => this.ascending;
    set
    {
      this.ascending = value;
      this.OnPropertyChanged(nameof (Ascending));
    }
  }

  public bool Numeric
  {
    get => this.numeric;
    set
    {
      this.numeric = value;
      this.OnPropertyChanged(nameof (Numeric));
    }
  }

  public string SecondListName
  {
    get => this.secondListName;
    set
    {
      this.secondListName = value;
      this.OnPropertyChanged(nameof (SecondListName));
    }
  }

  public string ListItem
  {
    get => this.listItem;
    set
    {
      this.listItem = value;
      this.OnPropertyChanged(nameof (ListItem));
    }
  }

  public string ListIndex
  {
    get => this.listIndex;
    set
    {
      this.listIndex = value;
      this.OnPropertyChanged(nameof (ListIndex));
    }
  }

  public VarAction VarAction
  {
    get => this.varAction;
    set
    {
      this.varAction = value;
      this.OnPropertyChanged(nameof (VarAction));
    }
  }

  public string VarName
  {
    get => this.varName;
    set
    {
      this.varName = value;
      this.OnPropertyChanged(nameof (VarName));
    }
  }

  public string SplitSeparator
  {
    get => this.splitSeparator;
    set
    {
      this.splitSeparator = value;
      this.OnPropertyChanged(nameof (SplitSeparator));
    }
  }

  public Encoding ConversionFrom
  {
    get => this.conversionFrom;
    set
    {
      this.conversionFrom = value;
      this.OnPropertyChanged(nameof (ConversionFrom));
    }
  }

  public Encoding ConversionTo
  {
    get => this.conversionTo;
    set
    {
      this.conversionTo = value;
      this.OnPropertyChanged(nameof (ConversionTo));
    }
  }

  public string FilePath
  {
    get => this.filePath;
    set
    {
      this.filePath = value;
      this.OnPropertyChanged(nameof (FilePath));
    }
  }

  public FileAction FileAction
  {
    get => this.fileAction;
    set
    {
      this.fileAction = value;
      this.OnPropertyChanged(nameof (FileAction));
    }
  }

  public BlockUtility() => this.Label = "UTILITY";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    // ISSUE: reference to a compiler-generated field
    if (BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, UtilityGroup>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (UtilityGroup), typeof (BlockUtility)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.Group = BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__0.Target((CallSite) BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "Group", typeof (UtilityGroup)));
    switch (this.Group)
    {
      case UtilityGroup.List:
        this.ListName = LineParser.ParseLiteral(ref input, "List Name");
        // ISSUE: reference to a compiler-generated field
        if (BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, ListAction>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (ListAction), typeof (BlockUtility)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.ListAction = BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__1.Target((CallSite) BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__1, LineParser.ParseEnum(ref input, "List Action", typeof (ListAction)));
        switch (this.ListAction)
        {
          case ListAction.Join:
            this.Separator = LineParser.ParseLiteral(ref input, "Separator");
            break;
          case ListAction.Sort:
            while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
              LineParser.SetBool(ref input, (object) this);
            break;
          case ListAction.Concat:
          case ListAction.Zip:
          case ListAction.Map:
            this.SecondListName = LineParser.ParseLiteral(ref input, "Second List Name");
            break;
          case ListAction.Add:
            this.ListItem = LineParser.ParseLiteral(ref input, "Item");
            this.ListIndex = LineParser.ParseLiteral(ref input, "Index");
            break;
          case ListAction.Remove:
            this.ListIndex = LineParser.ParseLiteral(ref input, "Index");
            break;
        }
        break;
      case UtilityGroup.Variable:
        this.VarName = LineParser.ParseLiteral(ref input, "Var Name");
        // ISSUE: reference to a compiler-generated field
        if (BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, VarAction>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (VarAction), typeof (BlockUtility)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.VarAction = BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__2.Target((CallSite) BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__2, LineParser.ParseEnum(ref input, "Var Action", typeof (VarAction)));
        if (this.VarAction == VarAction.Split)
        {
          this.SplitSeparator = LineParser.ParseLiteral(ref input, "SplitSeparator");
          break;
        }
        break;
      case UtilityGroup.Conversion:
        // ISSUE: reference to a compiler-generated field
        if (BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, Encoding>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Encoding), typeof (BlockUtility)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.ConversionFrom = BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__3.Target((CallSite) BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__3, LineParser.ParseEnum(ref input, "Conversion From", typeof (Encoding)));
        // ISSUE: reference to a compiler-generated field
        if (BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, Encoding>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Encoding), typeof (BlockUtility)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.ConversionTo = BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__4.Target((CallSite) BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__4, LineParser.ParseEnum(ref input, "Conversion To", typeof (Encoding)));
        this.InputString = LineParser.ParseLiteral(ref input, "Input");
        break;
      case UtilityGroup.File:
        this.FilePath = LineParser.ParseLiteral(ref input, "File Name");
        // ISSUE: reference to a compiler-generated field
        if (BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, FileAction>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (FileAction), typeof (BlockUtility)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.FileAction = BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__5.Target((CallSite) BlockUtility.\u003C\u003Eo__77.\u003C\u003Ep__5, LineParser.ParseEnum(ref input, "File Action", typeof (FileAction)));
        switch (this.FileAction)
        {
          case FileAction.Write:
          case FileAction.WriteLines:
          case FileAction.Append:
          case FileAction.AppendLines:
            this.InputString = LineParser.ParseLiteral(ref input, "Input String");
            break;
        }
        break;
    }
    if (LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Arrow, false) == "")
      return (BlockBase) this;
    try
    {
      string token = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true);
      if (!(token.ToUpper() == "VAR"))
      {
        if (!(token.ToUpper() == "CAP"))
          goto label_33;
      }
      this.IsCapture = token.ToUpper() == "CAP";
    }
    catch
    {
      throw new ArgumentException("Invalid or missing variable type");
    }
label_33:
    try
    {
      this.VariableName = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Literal, true);
    }
    catch
    {
      throw new ArgumentException("Variable name not specified");
    }
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "UTILITY").Token((object) this.Group);
    switch (this.Group)
    {
      case UtilityGroup.List:
        blockWriter.Literal(this.ListName).Token((object) this.ListAction);
        switch (this.ListAction)
        {
          case ListAction.Join:
            blockWriter.Literal(this.Separator);
            break;
          case ListAction.Sort:
            blockWriter.Boolean(this.Ascending, "Ascending").Boolean(this.Numeric, "Numeric");
            break;
          case ListAction.Concat:
          case ListAction.Zip:
          case ListAction.Map:
            blockWriter.Literal(this.SecondListName);
            break;
          case ListAction.Add:
            blockWriter.Literal(this.ListItem).Literal(this.ListIndex);
            break;
          case ListAction.Remove:
            blockWriter.Literal(this.ListIndex);
            break;
        }
        break;
      case UtilityGroup.Variable:
        blockWriter.Literal(this.VarName).Token((object) this.VarAction);
        if (this.VarAction == VarAction.Split)
        {
          blockWriter.Literal(this.SplitSeparator);
          break;
        }
        break;
      case UtilityGroup.Conversion:
        blockWriter.Token((object) this.ConversionFrom).Token((object) this.ConversionTo).Literal(this.InputString);
        break;
      case UtilityGroup.File:
        blockWriter.Literal(this.FilePath).Token((object) this.FileAction);
        switch (this.FileAction)
        {
          case FileAction.Write:
          case FileAction.WriteLines:
          case FileAction.Append:
          case FileAction.AppendLines:
            blockWriter.Literal(this.InputString);
            break;
        }
        break;
    }
    if (!blockWriter.CheckDefault((object) this.VariableName, "VariableName"))
      blockWriter.Arrow().Token(this.IsCapture ? (object) "CAP" : (object) "VAR").Literal(this.VariableName);
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    try
    {
      switch (this.group)
      {
        case UtilityGroup.List:
          List<string> list1 = data.Variables.GetList(this.listName);
          List<string> list2 = data.Variables.GetList(this.secondListName);
          string str = BlockBase.ReplaceValues(this.listItem, data);
          int num1 = int.Parse(BlockBase.ReplaceValues(this.listIndex, data));
          switch (this.listAction)
          {
            case ListAction.Create:
              data.Variables.Set(new CVar(this.variableName, new List<string>(), this.isCapture));
              break;
            case ListAction.Length:
              data.Variables.Set(new CVar(this.variableName, list1.Count<string>().ToString(), this.isCapture));
              break;
            case ListAction.Join:
              data.Variables.Set(new CVar(this.variableName, string.Join(this.separator, (IEnumerable<string>) list1), this.isCapture));
              break;
            case ListAction.Sort:
              List<string> list3 = list1.Select<string, string>((Func<string, string>) (e => e)).ToList<string>();
              if (this.Numeric)
              {
                List<double> list4 = list3.Select<string, double>((Func<string, double>) (e => double.Parse(e, (IFormatProvider) CultureInfo.InvariantCulture))).ToList<double>();
                list4.Sort();
                list3 = list4.Select<double, string>((Func<double, string>) (e => e.ToString())).ToList<string>();
              }
              else
                list3.Sort();
              if (!this.Ascending)
                list3.Reverse();
              data.Variables.Set(new CVar(this.variableName, list3, this.isCapture));
              break;
            case ListAction.Concat:
              data.Variables.Set(new CVar(this.variableName, list1.Concat<string>((IEnumerable<string>) list2).ToList<string>(), this.isCapture));
              break;
            case ListAction.Zip:
              data.Variables.Set(new CVar(this.variableName, list1.Zip<string, string, string>((IEnumerable<string>) list2, (Func<string, string, string>) ((a, b) => a + b)).ToList<string>(), this.isCapture));
              break;
            case ListAction.Map:
              data.Variables.Set(new CVar(this.variableName, list1.Zip((IEnumerable<string>) list2, (k, v) => new
              {
                k = k,
                v = v
              }).ToDictionary(x => x.k, x => x.v), this.isCapture));
              break;
            case ListAction.Add:
              CVar cvar1 = data.Variables.Get(this.listName, CVar.VarType.List) ?? data.GlobalVariables.Get(this.listName, CVar.VarType.List);
              if (cvar1 != null)
              {
                // ISSUE: reference to a compiler-generated field
                if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__2 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target1 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__2.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p2 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__2;
                // ISSUE: reference to a compiler-generated field
                if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__1 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, int, object> target2 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__1.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, int, object>> p1 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__1;
                // ISSUE: reference to a compiler-generated field
                if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__0 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj1 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__0.Target((CallSite) BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__0, cvar1.Value);
                object obj2 = target2((CallSite) p1, obj1, 0);
                if (target1((CallSite) p2, obj2))
                  num1 = 0;
                else if (num1 < 0)
                {
                  // ISSUE: reference to a compiler-generated field
                  if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__5 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (int), typeof (BlockUtility)));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, object, int> target3 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__5.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, object, int>> p5 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__5;
                  // ISSUE: reference to a compiler-generated field
                  if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__4 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__4 = CallSite<Func<CallSite, int, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.AddAssign, typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, int, object, object> target4 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__4.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, int, object, object>> p4 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__4;
                  int num2 = num1;
                  // ISSUE: reference to a compiler-generated field
                  if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__3 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj3 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__3.Target((CallSite) BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__3, cvar1.Value);
                  object obj4 = target4((CallSite) p4, num2, obj3);
                  num1 = target3((CallSite) p5, obj4);
                }
                // ISSUE: reference to a compiler-generated field
                if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__6 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__6 = CallSite<Action<CallSite, object, int, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Insert", (IEnumerable<Type>) null, typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__6.Target((CallSite) BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__6, cvar1.Value, num1, str);
                break;
              }
              break;
            case ListAction.Remove:
              CVar cvar2 = data.Variables.Get(this.listName, CVar.VarType.List) ?? data.GlobalVariables.Get(this.listName, CVar.VarType.List);
              if (cvar2 != null)
              {
                // ISSUE: reference to a compiler-generated field
                if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__9 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target5 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__9.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p9 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__9;
                // ISSUE: reference to a compiler-generated field
                if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__8 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, int, object> target6 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__8.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, int, object>> p8 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__8;
                // ISSUE: reference to a compiler-generated field
                if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__7 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj5 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__7.Target((CallSite) BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__7, cvar2.Value);
                object obj6 = target6((CallSite) p8, obj5, 0);
                if (target5((CallSite) p9, obj6))
                  num1 = 0;
                else if (num1 < 0)
                {
                  // ISSUE: reference to a compiler-generated field
                  if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__12 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (int), typeof (BlockUtility)));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, object, int> target7 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__12.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, object, int>> p12 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__12;
                  // ISSUE: reference to a compiler-generated field
                  if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__11 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__11 = CallSite<Func<CallSite, int, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.AddAssign, typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, int, object, object> target8 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__11.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, int, object, object>> p11 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__11;
                  int num3 = num1;
                  // ISSUE: reference to a compiler-generated field
                  if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__10 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj7 = BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__10.Target((CallSite) BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__10, cvar2.Value);
                  object obj8 = target8((CallSite) p11, num3, obj7);
                  num1 = target7((CallSite) p12, obj8);
                }
                // ISSUE: reference to a compiler-generated field
                if (BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__13 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__13 = CallSite<Action<CallSite, object, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "RemoveAt", (IEnumerable<Type>) null, typeof (BlockUtility), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__13.Target((CallSite) BlockUtility.\u003C\u003Eo__79.\u003C\u003Ep__13, cvar2.Value, num1);
                break;
              }
              break;
            case ListAction.RemoveDuplicates:
              data.Variables.Set(new CVar(this.variableName, list1.Distinct<string>().ToList<string>(), this.isCapture));
              break;
            case ListAction.Random:
              data.Variables.Set(new CVar(this.variableName, list1[data.Random.Next(list1.Count)], this.isCapture));
              break;
          }
          data.Log(new LogEntry($"Executed action {this.listAction} on list {this.listName}", Colors.White));
          break;
        case UtilityGroup.Variable:
          if (this.varAction == VarAction.Split)
          {
            string single = data.Variables.GetSingle(this.varName);
            data.Variables.Set(new CVar(this.variableName, ((IEnumerable<string>) single.Split(new string[1]
            {
              BlockBase.ReplaceValues(this.splitSeparator, data)
            }, StringSplitOptions.None)).ToList<string>(), (this.isCapture ? 1 : 0) != 0));
          }
          data.Log(new LogEntry($"Executed action {this.varAction} on variable {this.varName}", Colors.White));
          break;
        case UtilityGroup.Conversion:
          byte[] input = BlockBase.ReplaceValues(this.inputString, data).ConvertFrom(this.conversionFrom);
          data.Variables.Set(new CVar(this.variableName, input.ConvertTo(this.conversionTo), this.isCapture));
          data.Log(new LogEntry($"Converted input from {this.conversionFrom} to {this.conversionTo}", Colors.White));
          break;
        case UtilityGroup.File:
          string path = BlockBase.ReplaceValues(this.filePath, data);
          string contents1 = BlockBase.ReplaceValues(this.inputString, data).Replace("\\r\\n", "\r\n").Replace("\\n", "\n");
          IEnumerable<string> contents2 = BlockBase.ReplaceValuesRecursive(this.inputString, data).Select<string, string>((Func<string, string>) (i => i.Replace("\\r\\n", "\r\n").Replace("\\n", "\n")));
          switch (this.fileAction)
          {
            case FileAction.Read:
              data.Variables.Set(new CVar(this.variableName, File.ReadAllText(path), this.isCapture));
              return;
            case FileAction.ReadLines:
              data.Variables.Set(new CVar(this.variableName, ((IEnumerable<string>) File.ReadAllLines(path)).ToList<string>(), this.isCapture));
              return;
            case FileAction.Write:
              File.WriteAllText(path, contents1);
              return;
            case FileAction.WriteLines:
              File.WriteAllLines(path, contents2);
              return;
            case FileAction.Append:
              File.AppendAllText(path, contents1);
              return;
            case FileAction.AppendLines:
              File.AppendAllLines(path, contents2);
              return;
            default:
              return;
          }
      }
    }
    catch (Exception ex)
    {
      data.Log(new LogEntry(ex.Message, Colors.Tomato));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.PythonWalkerNonRecursive
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

public class PythonWalkerNonRecursive : PythonWalker
{
  public override bool Walk(AndExpression node) => false;

  public override void PostWalk(AndExpression node)
  {
  }

  public override bool Walk(BackQuoteExpression node) => false;

  public override void PostWalk(BackQuoteExpression node)
  {
  }

  public override bool Walk(BinaryExpression node) => false;

  public override void PostWalk(BinaryExpression node)
  {
  }

  public override bool Walk(CallExpression node) => false;

  public override void PostWalk(CallExpression node)
  {
  }

  public override bool Walk(ConditionalExpression node) => false;

  public override void PostWalk(ConditionalExpression node)
  {
  }

  public override bool Walk(ConstantExpression node) => false;

  public override void PostWalk(ConstantExpression node)
  {
  }

  public override bool Walk(DictionaryComprehension node) => false;

  public override void PostWalk(DictionaryComprehension node)
  {
  }

  public override bool Walk(DictionaryExpression node) => false;

  public override void PostWalk(DictionaryExpression node)
  {
  }

  public override bool Walk(ErrorExpression node) => false;

  public override void PostWalk(ErrorExpression node)
  {
  }

  public override bool Walk(GeneratorExpression node) => false;

  public override void PostWalk(GeneratorExpression node)
  {
  }

  public override bool Walk(IndexExpression node) => false;

  public override void PostWalk(IndexExpression node)
  {
  }

  public override bool Walk(LambdaExpression node) => false;

  public override void PostWalk(LambdaExpression node)
  {
  }

  public override bool Walk(ListComprehension node) => false;

  public override void PostWalk(ListComprehension node)
  {
  }

  public override bool Walk(ListExpression node) => false;

  public override void PostWalk(ListExpression node)
  {
  }

  public override bool Walk(MemberExpression node) => false;

  public override void PostWalk(MemberExpression node)
  {
  }

  public override bool Walk(NameExpression node) => false;

  public override void PostWalk(NameExpression node)
  {
  }

  public override bool Walk(OrExpression node) => false;

  public override void PostWalk(OrExpression node)
  {
  }

  public override bool Walk(ParenthesisExpression node) => false;

  public override void PostWalk(ParenthesisExpression node)
  {
  }

  public override bool Walk(SetComprehension node) => false;

  public override void PostWalk(SetComprehension node)
  {
  }

  public override bool Walk(SetExpression node) => false;

  public override void PostWalk(SetExpression node)
  {
  }

  public override bool Walk(SliceExpression node) => false;

  public override void PostWalk(SliceExpression node)
  {
  }

  public override bool Walk(TupleExpression node) => false;

  public override void PostWalk(TupleExpression node)
  {
  }

  public override bool Walk(UnaryExpression node) => false;

  public override void PostWalk(UnaryExpression node)
  {
  }

  public override bool Walk(YieldExpression node) => false;

  public override void PostWalk(YieldExpression node)
  {
  }

  public override bool Walk(AssertStatement node) => false;

  public override void PostWalk(AssertStatement node)
  {
  }

  public override bool Walk(AssignmentStatement node) => false;

  public override void PostWalk(AssignmentStatement node)
  {
  }

  public override bool Walk(AugmentedAssignStatement node) => false;

  public override void PostWalk(AugmentedAssignStatement node)
  {
  }

  public override bool Walk(BreakStatement node) => false;

  public override void PostWalk(BreakStatement node)
  {
  }

  public override bool Walk(ClassDefinition node) => false;

  public override void PostWalk(ClassDefinition node)
  {
  }

  public override bool Walk(ContinueStatement node) => false;

  public override void PostWalk(ContinueStatement node)
  {
  }

  public override bool Walk(DelStatement node) => false;

  public override void PostWalk(DelStatement node)
  {
  }

  public override bool Walk(EmptyStatement node) => false;

  public override void PostWalk(EmptyStatement node)
  {
  }

  public override bool Walk(ExecStatement node) => false;

  public override void PostWalk(ExecStatement node)
  {
  }

  public override bool Walk(ExpressionStatement node) => false;

  public override void PostWalk(ExpressionStatement node)
  {
  }

  public override bool Walk(ForStatement node) => false;

  public override void PostWalk(ForStatement node)
  {
  }

  public override bool Walk(FromImportStatement node) => false;

  public override void PostWalk(FromImportStatement node)
  {
  }

  public override bool Walk(FunctionDefinition node) => false;

  public override void PostWalk(FunctionDefinition node)
  {
  }

  public override bool Walk(GlobalStatement node) => false;

  public override void PostWalk(GlobalStatement node)
  {
  }

  public override bool Walk(IfStatement node) => false;

  public override void PostWalk(IfStatement node)
  {
  }

  public override bool Walk(ImportStatement node) => false;

  public override void PostWalk(ImportStatement node)
  {
  }

  public override bool Walk(PrintStatement node) => false;

  public override void PostWalk(PrintStatement node)
  {
  }

  public override bool Walk(PythonAst node) => false;

  public override void PostWalk(PythonAst node)
  {
  }

  public override bool Walk(RaiseStatement node) => false;

  public override void PostWalk(RaiseStatement node)
  {
  }

  public override bool Walk(ReturnStatement node) => false;

  public override void PostWalk(ReturnStatement node)
  {
  }

  public override bool Walk(SuiteStatement node) => false;

  public override void PostWalk(SuiteStatement node)
  {
  }

  public override bool Walk(TryStatement node) => false;

  public override void PostWalk(TryStatement node)
  {
  }

  public override bool Walk(WhileStatement node) => false;

  public override void PostWalk(WhileStatement node)
  {
  }

  public override bool Walk(WithStatement node) => false;

  public override void PostWalk(WithStatement node)
  {
  }

  public override bool Walk(Arg node) => false;

  public override void PostWalk(Arg node)
  {
  }

  public override bool Walk(ComprehensionFor node) => false;

  public override void PostWalk(ComprehensionFor node)
  {
  }

  public override bool Walk(ComprehensionIf node) => false;

  public override void PostWalk(ComprehensionIf node)
  {
  }

  public override bool Walk(DottedName node) => false;

  public override void PostWalk(DottedName node)
  {
  }

  public override bool Walk(IfStatementTest node) => false;

  public override void PostWalk(IfStatementTest node)
  {
  }

  public override bool Walk(ModuleName node) => false;

  public override void PostWalk(ModuleName node)
  {
  }

  public override bool Walk(Parameter node) => false;

  public override void PostWalk(Parameter node)
  {
  }

  public override bool Walk(RelativeModuleName node) => false;

  public override void PostWalk(RelativeModuleName node)
  {
  }

  public override bool Walk(SublistParameter node) => false;

  public override void PostWalk(SublistParameter node)
  {
  }

  public override bool Walk(TryStatementHandler node) => false;

  public override void PostWalk(TryStatementHandler node)
  {
  }
}

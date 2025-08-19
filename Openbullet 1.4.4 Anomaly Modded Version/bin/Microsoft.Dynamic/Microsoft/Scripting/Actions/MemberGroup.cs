// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.MemberGroup
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class MemberGroup : IEnumerable<MemberTracker>, IEnumerable
{
  public static readonly MemberGroup EmptyGroup = new MemberGroup(MemberTracker.EmptyTrackers);
  private readonly MemberTracker[] _members;

  private MemberGroup(MemberTracker[] members, bool noChecks) => this._members = members;

  public MemberGroup(params MemberTracker[] members)
  {
    ContractUtils.RequiresNotNullItems<MemberTracker>((IList<MemberTracker>) members, nameof (members));
    this._members = members;
  }

  public MemberGroup(params MemberInfo[] members)
  {
    ContractUtils.RequiresNotNullItems<MemberInfo>((IList<MemberInfo>) members, nameof (members));
    MemberTracker[] memberTrackerArray = new MemberTracker[members.Length];
    for (int index = 0; index < memberTrackerArray.Length; ++index)
      memberTrackerArray[index] = MemberTracker.FromMemberInfo(members[index]);
    this._members = memberTrackerArray;
  }

  internal static MemberGroup CreateInternal(MemberTracker[] members)
  {
    return new MemberGroup(members, true);
  }

  public int Count => this._members.Length;

  public MemberTracker this[int index] => this._members[index];

  public IEnumerator<MemberTracker> GetEnumerator()
  {
    MemberTracker[] memberTrackerArray = this._members;
    for (int index = 0; index < memberTrackerArray.Length; ++index)
      yield return memberTrackerArray[index];
    memberTrackerArray = (MemberTracker[]) null;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    MemberTracker[] memberTrackerArray = this._members;
    for (int index = 0; index < memberTrackerArray.Length; ++index)
      yield return (object) memberTrackerArray[index];
    memberTrackerArray = (MemberTracker[]) null;
  }
}

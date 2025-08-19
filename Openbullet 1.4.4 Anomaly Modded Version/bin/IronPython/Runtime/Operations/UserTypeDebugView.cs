// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.UserTypeDebugView
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace IronPython.Runtime.Operations;

public class UserTypeDebugView
{
  private readonly IPythonObject _userObject;

  public UserTypeDebugView(IPythonObject userObject) => this._userObject = userObject;

  public PythonType __class__ => this._userObject.PythonType;

  [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
  internal List<ObjectDebugView> Members
  {
    get
    {
      List<ObjectDebugView> members = new List<ObjectDebugView>();
      if (this._userObject.Dict != null)
      {
        foreach (KeyValuePair<object, object> keyValuePair in this._userObject.Dict)
          members.Add(new ObjectDebugView(keyValuePair.Key, keyValuePair.Value));
      }
      object[] slots = this._userObject.GetSlots();
      if (slots != null)
      {
        IList<PythonType> resolutionOrder = this._userObject.PythonType.ResolutionOrder;
        List<string> stringList = new List<string>();
        for (int index = resolutionOrder.Count - 1; index >= 0; --index)
          stringList.AddRange((IEnumerable<string>) resolutionOrder[index].GetTypeSlots());
        for (int index = 0; index < slots.Length - 1; ++index)
        {
          if (slots[index] != Uninitialized.Instance)
            members.Add(new ObjectDebugView((object) stringList[index], slots[index]));
        }
      }
      return members;
    }
  }
}

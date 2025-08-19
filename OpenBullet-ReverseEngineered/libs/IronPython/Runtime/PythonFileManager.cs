// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonFileManager
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using Microsoft.Scripting.Utils;
using System.IO;

#nullable disable
namespace IronPython.Runtime;

internal class PythonFileManager
{
  private HybridMapping<object> mapping = new HybridMapping<object>(3);

  public int AddToStrongMapping(PythonFile pf, int pos) => this.mapping.StrongAdd((object) pf, pos);

  public int AddToStrongMapping(PythonFile pf) => this.mapping.StrongAdd((object) pf, -1);

  public int AddToStrongMapping(object o, int pos) => this.mapping.StrongAdd(o, pos);

  public int AddToStrongMapping(object o) => this.mapping.StrongAdd(o, -1);

  public void Remove(PythonFile pf) => this.mapping.RemoveOnObject((object) pf);

  public void RemovePythonFileOnId(int id) => this.mapping.RemoveOnId(id);

  public void Remove(object o) => this.mapping.RemoveOnObject(o);

  public void RemoveObjectOnId(int id) => this.mapping.RemoveOnId(id);

  public PythonFile GetFileFromId(PythonContext context, int id)
  {
    PythonFile pf;
    if (!this.TryGetFileFromId(context, id, out pf))
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
    return pf;
  }

  public bool TryGetFileFromId(PythonContext context, int id, out PythonFile pf)
  {
    switch (id)
    {
      case 0:
        pf = context.GetSystemStateValue("__stdin__") as PythonFile;
        break;
      case 1:
        pf = context.GetSystemStateValue("__stdout__") as PythonFile;
        break;
      case 2:
        pf = context.GetSystemStateValue("__stderr__") as PythonFile;
        break;
      default:
        pf = this.mapping.GetObjectFromId(id) as PythonFile;
        break;
    }
    return pf != null;
  }

  public bool TryGetObjectFromId(PythonContext context, int id, out object o)
  {
    o = this.mapping.GetObjectFromId(id);
    return o != null;
  }

  public object GetObjectFromId(int id)
  {
    return this.mapping.GetObjectFromId(id) ?? throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
  }

  public int GetIdFromFile(PythonFile pf) => this.mapping.GetIdFromObject((object) pf);

  public void CloseIfLast(int fd, PythonFile pf)
  {
    this.mapping.RemoveOnId(fd);
    if (-1 != this.mapping.GetIdFromObject((object) pf))
      return;
    pf.close();
  }

  public void CloseIfLast(int fd, Stream stream)
  {
    this.mapping.RemoveOnId(fd);
    if (-1 != this.mapping.GetIdFromObject((object) stream))
      return;
    stream.Dispose();
  }

  public int GetOrAssignIdForFile(PythonFile pf)
  {
    if (pf.IsConsole)
    {
      for (int id = 0; id < 3; ++id)
      {
        if (pf == this.GetFileFromId(pf.Context, id))
          return id;
      }
    }
    int orAssignIdForFile = this.mapping.GetIdFromObject((object) pf);
    if (orAssignIdForFile == -1)
      orAssignIdForFile = this.mapping.WeakAdd((object) pf);
    return orAssignIdForFile;
  }

  public int GetIdFromObject(object o) => this.mapping.GetIdFromObject(o);

  public int GetOrAssignIdForObject(object o)
  {
    int assignIdForObject = this.mapping.GetIdFromObject(o);
    if (assignIdForObject == -1)
      assignIdForObject = this.mapping.WeakAdd(o);
    return assignIdForObject;
  }

  public bool ValidateFdRange(int fd) => fd >= 0 && fd < 4096 /*0x1000*/;
}

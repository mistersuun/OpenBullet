// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.Submitters.Json.JsonStep
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html.Forms.Submitters.Json;

internal abstract class JsonStep
{
  public bool Append { get; set; }

  public JsonStep Next { get; set; }

  public static IEnumerable<JsonStep> Parse(string path)
  {
    List<JsonStep> jsonStepList = new List<JsonStep>();
    int num1 = 0;
    while (num1 < path.Length && path[num1] != '[')
      ++num1;
    if (num1 == 0)
      return JsonStep.FailedJsonSteps(path);
    jsonStepList.Add((JsonStep) new JsonStep.ObjectStep(path.Substring(0, num1)));
    while (num1 < path.Length)
    {
      if (num1 + 1 >= path.Length || path[num1] != '[')
        return JsonStep.FailedJsonSteps(path);
      if (path[num1 + 1] == ']')
      {
        jsonStepList[jsonStepList.Count - 1].Append = true;
        num1 += 2;
        if (num1 < path.Length)
          return JsonStep.FailedJsonSteps(path);
      }
      else if (path[num1 + 1].IsDigit())
      {
        int index;
        int startIndex;
        for (startIndex = index = num1 + 1; index < path.Length && path[index] != ']'; ++index)
        {
          if (!path[index].IsDigit())
            return JsonStep.FailedJsonSteps(path);
        }
        if (index == path.Length)
          return JsonStep.FailedJsonSteps(path);
        jsonStepList.Add((JsonStep) new JsonStep.ArrayStep(path.Substring(startIndex, index - startIndex).ToInteger(0)));
        num1 = index + 1;
      }
      else
      {
        int index;
        int startIndex = index = num1 + 1;
        while (index < path.Length && path[index] != ']')
          ++index;
        if (index == path.Length)
          return JsonStep.FailedJsonSteps(path);
        jsonStepList.Add((JsonStep) new JsonStep.ObjectStep(path.Substring(startIndex, index - startIndex)));
        num1 = index + 1;
      }
    }
    int num2 = jsonStepList.Count - 1;
    for (int index = 0; index < num2; ++index)
      jsonStepList[index].Next = jsonStepList[index + 1];
    return (IEnumerable<JsonStep>) jsonStepList;
  }

  private static IEnumerable<JsonStep> FailedJsonSteps(string original)
  {
    return (IEnumerable<JsonStep>) new JsonStep.ObjectStep[1]
    {
      new JsonStep.ObjectStep(original)
    };
  }

  protected abstract JsonElement CreateElement();

  protected abstract JsonElement SetValue(JsonElement context, JsonElement value);

  protected abstract JsonElement GetValue(JsonElement context);

  protected abstract JsonElement ConvertArray(JsonArray value);

  public JsonElement Run(JsonElement context, JsonElement value, bool file = false)
  {
    return this.Next == null ? this.JsonEncodeLastValue(context, value, file) : this.JsonEncodeValue(context, value, file);
  }

  private JsonElement JsonEncodeValue(JsonElement context, JsonElement value, bool file)
  {
    JsonElement jsonElement = this.GetValue(context);
    switch (jsonElement)
    {
      case null:
        JsonElement element = this.Next.CreateElement();
        return this.SetValue(context, element);
      case JsonObject _:
        return jsonElement;
      case JsonArray _:
        return this.SetValue(context, this.Next.ConvertArray((JsonArray) jsonElement));
      default:
        JsonObject jsonObject1 = new JsonObject();
        jsonObject1[string.Empty] = jsonElement;
        JsonObject jsonObject2 = jsonObject1;
        return this.SetValue(context, (JsonElement) jsonObject2);
    }
  }

  private JsonElement JsonEncodeLastValue(JsonElement context, JsonElement value, bool file)
  {
    JsonElement jsonElement = this.GetValue(context);
    switch (jsonElement)
    {
      case null:
        if (this.Append)
        {
          JsonArray jsonArray = new JsonArray();
          jsonArray.Push(value);
          value = (JsonElement) jsonArray;
        }
        this.SetValue(context, value);
        break;
      case JsonArray _:
        ((JsonArray) jsonElement).Push(value);
        break;
      case JsonObject _:
        if (!file)
          return new JsonStep.ObjectStep(string.Empty).JsonEncodeLastValue(jsonElement, value, true);
        goto default;
      default:
        JsonArray jsonArray1 = new JsonArray();
        jsonArray1.Push(jsonElement);
        jsonArray1.Push(value);
        this.SetValue(context, (JsonElement) jsonArray1);
        break;
    }
    return context;
  }

  private sealed class ObjectStep : JsonStep
  {
    public ObjectStep(string key) => this.Key = key;

    public string Key { get; private set; }

    protected override JsonElement GetValue(JsonElement context) => context[this.Key];

    protected override JsonElement SetValue(JsonElement context, JsonElement value)
    {
      context[this.Key] = value;
      return value;
    }

    protected override JsonElement CreateElement() => (JsonElement) new JsonObject();

    protected override JsonElement ConvertArray(JsonArray values)
    {
      JsonObject jsonObject = new JsonObject();
      for (int key = 0; key < values.Length; ++key)
      {
        JsonElement jsonElement = values[key];
        if (jsonElement != null)
          jsonObject[key.ToString()] = jsonElement;
      }
      return (JsonElement) jsonObject;
    }
  }

  private sealed class ArrayStep : JsonStep
  {
    public ArrayStep(int key) => this.Key = key;

    public int Key { get; private set; }

    protected override JsonElement GetValue(JsonElement context)
    {
      return context is JsonArray jsonArray ? jsonArray[this.Key] : context[this.Key.ToString()];
    }

    protected override JsonElement SetValue(JsonElement context, JsonElement value)
    {
      if (context is JsonArray jsonArray)
        jsonArray[this.Key] = value;
      else
        context[this.Key.ToString()] = value;
      return value;
    }

    protected override JsonElement CreateElement() => (JsonElement) new JsonArray();

    protected override JsonElement ConvertArray(JsonArray value) => (JsonElement) value;
  }
}

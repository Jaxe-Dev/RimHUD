using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using RimHUD.Engine;
using RimHUD.Extensions;
using Verse;

namespace RimHUD;

public abstract class ExternalWidgetDef : Def
{
  private static readonly Regex FastInvokeHandlerMethodNameRegex = new("FastInvoke_(.+)_indirect");

  public int apiVersion;

  public Type? defClass;

  public bool Initialized { get; private set; }

  public override void ResolveReferences()
  {
    try
    {
      if (apiVersion is 0)
      {
        apiVersion = 1;
        Report.Warning($"{GetLogDescription()} did not set the api version - defaulting to v1.");
      }

      base.ResolveReferences();

      switch (apiVersion)
      {
        case 1:
          InitializeV1();
          break;
        default:
          throw new Exception("Unsupported api version.");
      }

      Initialized = true;
    }
    catch (Exception exception) { throw exception.AddData($"{GetLogDescription()} using api v{apiVersion} threw an exception: ", true); }
  }

  protected virtual void InitializeV1()
  { }

  protected ExternalMethodHandler<T>? GetHandler<T>(bool isRequired, string methodName, params Type[] parameterTypes)
  {
    if (defClass is null)
    {
      if (!isRequired) { return null; }
      throw new Exception("Required defClass not found.");
    }

    var method = AccessTools.Method(defClass, methodName, parameterTypes);
    if (method is null) { return isRequired ? throw new Exception($"No method '{methodName}' in '{defClass.FullName}' with expected signature.") : null; }
    if (method.ReturnType != typeof(T)) { throw new Exception($"Method '{methodName}' has unexpected return type."); }

    var handler = MethodInvoker.GetHandler(method);
    if (handler is not null) { return new ExternalMethodHandler<T>(handler); }

    if (!isRequired) { return null; }
    throw new Exception($"Error getting handler for '{methodName}'");
  }

  private string GetLogDescription() => $"Widget '{this.GetDefNameOrLabel()}' from mod '{modContentPack?.Name ?? "<unknown>"}'";

  protected readonly struct ExternalMethodHandler<T>(FastInvokeHandler handler)
  {
    public T Invoke(params object[] parameters)
    {
      try { return (T)handler.Invoke(null, parameters); }
      catch (Exception exception) { throw exception.AddData($"Invoking method '{FastInvokeHandlerMethodNameRegex.Match(handler.Method.Name).Value.WithDefault("<unknown>")}' threw an exception: ", true); }
    }
  }
}

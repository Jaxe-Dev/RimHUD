using UnityEngine;

namespace RimHUD.Interface.Hud.Models
{
  public interface IModelSelector : IModelOwned
  {
    Color? Color { get; }
  }
}

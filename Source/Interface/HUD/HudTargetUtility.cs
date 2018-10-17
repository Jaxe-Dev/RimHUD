namespace RimHUD.Interface.HUD
{
    internal static class HudTargetUtility
    {
        public static bool HasTarget(this HudTarget self, HudTarget target)
        {
            var targettedInt = (int) target;
            return ((int) self & targettedInt) == targettedInt;
        }

        public static string GetId(this HudTarget self)
        {
            if (self == HudTarget.PlayerColonist) { return "C"; }
            if (self == HudTarget.PlayerAnimal) { return "N"; }
            if (self == HudTarget.OtherColonist) { return "c"; }
            if (self == HudTarget.OtherAnimal) { return "n"; }
            if (self == HudTarget.All) { return null; }

            throw new Mod.Exception("Invalid HUD target type");
        }

        public static HudTarget FromId(string value)
        {
            if (value == null) { return HudTarget.All; }

            var targets = HudTarget.Invalid;
            if (value.Contains(HudTarget.PlayerColonist.GetId())) { targets |= HudTarget.PlayerColonist; }
            if (value.Contains(HudTarget.PlayerAnimal.GetId())) { targets |= HudTarget.PlayerAnimal; }
            if (value.Contains(HudTarget.OtherColonist.GetId())) { targets |= HudTarget.OtherColonist; }
            if (value.Contains(HudTarget.OtherAnimal.GetId())) { targets |= HudTarget.OtherAnimal; }

            return targets;
        }

        public static string ToId(this HudTarget self)
        {
            if (self == HudTarget.All) { return null; }

            var value = "";
            if (self.HasTarget(HudTarget.PlayerColonist)) { value += HudTarget.PlayerColonist.GetId(); }
            if (self.HasTarget(HudTarget.PlayerAnimal)) { value += HudTarget.PlayerAnimal.GetId(); }
            if (self.HasTarget(HudTarget.OtherColonist)) { value += HudTarget.OtherColonist.GetId(); }
            if (self.HasTarget(HudTarget.OtherAnimal)) { value += HudTarget.OtherAnimal.GetId(); }

            return value.Length > 0 ? value : null;
        }
    }
}

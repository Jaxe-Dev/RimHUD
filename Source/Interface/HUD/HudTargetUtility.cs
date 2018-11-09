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
            if (self == HudTarget.PlayerHumanlike) { return "H"; }
            if (self == HudTarget.PlayerCreature) { return "C"; }
            if (self == HudTarget.OtherHumanlike) { return "h"; }
            if (self == HudTarget.OtherCreature) { return "c"; }
            if (self == HudTarget.All) { return null; }

            throw new Mod.Exception("Invalid HUD target type");
        }

        public static HudTarget FromId(string value)
        {
            if (value == null) { return HudTarget.All; }

            var targets = HudTarget.Invalid;
            if (value.Contains(HudTarget.PlayerHumanlike.GetId())) { targets |= HudTarget.PlayerHumanlike; }
            if (value.Contains(HudTarget.PlayerCreature.GetId())) { targets |= HudTarget.PlayerCreature; }
            if (value.Contains(HudTarget.OtherHumanlike.GetId())) { targets |= HudTarget.OtherHumanlike; }
            if (value.Contains(HudTarget.OtherCreature.GetId())) { targets |= HudTarget.OtherCreature; }

            return targets;
        }

        public static string ToId(this HudTarget self)
        {
            if (self == HudTarget.All) { return null; }

            var value = "";
            if (self.HasTarget(HudTarget.PlayerHumanlike)) { value += HudTarget.PlayerHumanlike.GetId(); }
            if (self.HasTarget(HudTarget.PlayerCreature)) { value += HudTarget.PlayerCreature.GetId(); }
            if (self.HasTarget(HudTarget.OtherHumanlike)) { value += HudTarget.OtherHumanlike.GetId(); }
            if (self.HasTarget(HudTarget.OtherCreature)) { value += HudTarget.OtherCreature.GetId(); }

            return value.Length > 0 ? value : null;
        }
    }
}

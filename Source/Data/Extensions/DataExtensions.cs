using System;
using Verse;

namespace RimHUD.Data.Extensions
{
    internal static class DataExtensions
    {
        public static int ComparePartial(this Version self, Version other)
        {
            if (other == null) { return 1; }

            if (self.Major > other.Major) { return 1; }
            if (self.Major < other.Major) { return -1; }

            if ((self.Minor == -1) || (other.Minor == -1)) { return 0; }
            if (self.Minor > other.Minor) { return 1; }
            if (self.Minor < other.Minor) { return -1; }

            if ((self.Build == -1) || (other.Build == -1)) { return 0; }
            if (self.Build > other.Build) { return 1; }
            if (self.Build < other.Build) { return -1; }

            if ((self.Revision == -1) || (other.Revision == -1)) { return 0; }
            if (self.Revision > other.Revision) { return 1; }
            if (self.Revision < other.Revision) { return -1; }

            return 0;
        }

        public static string GetName(this Pawn self) => self.Name?.ToStringFull.CapitalizeFirst() ?? self.LabelCap;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RimHUD.Extensions
{
  public static class BaseExtensions
  {
    public static int LastIndex(this IList self) => self.Count - 1;
    public static int Half(this int self) => self / 2;
    public static float Half(this float self) => self / 2f;

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> self) where T : class => self.Where(static item => item is not null)!;

    public static int ComparePartial(this Version self, Version? other)
    {
      if (other is null) { return 1; }

      if (self.Major > other.Major) { return 1; }
      if (self.Major < other.Major) { return -1; }

      if (self.Minor is -1 || other.Minor is -1) { return 0; }
      if (self.Minor > other.Minor) { return 1; }
      if (self.Minor < other.Minor) { return -1; }

      if (self.Build is -1 || other.Build is -1) { return 0; }
      if (self.Build > other.Build) { return 1; }
      if (self.Build < other.Build) { return -1; }

      if (self.Revision is -1 || other.Revision is -1) { return 0; }
      if (self.Revision > other.Revision) { return 1; }
      if (self.Revision < other.Revision) { return -1; }

      return 0;
    }
  }
}

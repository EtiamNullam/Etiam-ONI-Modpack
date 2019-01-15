using MaterialColor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaterialColor
{
    /// <summary>
    /// Filters text based on method and rules supplied.
    /// Designed for filtering building for coloring in HarmonyPatches.UpdateBuildingColor.
    /// </summary>
    public class TextFilter
    {
        /// <summary>
        /// Chooses strategy for filtering based on info supplied.
        /// </summary>
        public TextFilter(FilterInfo info)
        {
            this.Rules = info.Rules;

            if (info.Inclusive)
            {
                if (info.ExactMatch)
                {
                    this.Check = this.InclusiveExactCheck;
                }
                else
                {
                    this.Check = this.InclusiveContainsCheck;
                }
            }
            else
            {
                if (info.ExactMatch)
                {
                    this.Check = this.ExclusiveExactCheck;
                }
                else
                {
                    this.Check = this.ExclusiveContainsCheck;
                }
            }
        }

        /// <summary>
        /// Checks if value passes through the filter's ruleset.
        /// </summary>
        public Func<string, bool> Check { get; private set; }

        private readonly List<string> Rules;

        private bool InclusiveExactCheck(string value)
            => this.Rules.Any(rule => value.Equals(rule));

        private bool ExclusiveExactCheck(string value)
            => !this.Rules.Any(rule => value.Equals(rule));

        private bool InclusiveContainsCheck(string value)
            => this.Rules.Any(rule => value.Contains(rule));

        private bool ExclusiveContainsCheck(string value)
            => !this.Rules.Any(rule => value.Contains(rule));
    }
}

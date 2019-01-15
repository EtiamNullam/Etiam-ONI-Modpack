using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MaterialColor.Data
{
    /// <summary>
    /// Provides filtering method and rules. Used to find matching types to apply color to in TextFilter.
    /// </summary>
    public class FilterInfo
    {
        /// <param name="rules">Rules to check against</param>
        /// <param name="inclusive">True: check if there is rule for it, False: check if there is no rule for it</param>
        public FilterInfo(IEnumerable<string> rules = null)
        {
            this.Rules = new List<string>();

            if (rules != null)
            {
                this.Rules.AddRange(rules);
            }
        }

        public bool Inclusive { get; set; } = false;
        public bool ExactMatch { get; set; } = false;
        public List<string> Rules { get; private set; }
    }
}

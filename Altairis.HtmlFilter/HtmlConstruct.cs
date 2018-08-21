using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.HtmlFilter {
    public class HtmlConstruct {

        public HtmlConstruct() { }

        public HtmlConstruct(string rule) {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            if (string.IsNullOrWhiteSpace(rule)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(rule));

            var parts = rule.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            this.ElementName = parts[0];

            for (var i = 1; i < parts.Length; i++) {
                if (parts[i].StartsWith("#", StringComparison.Ordinal)) {
                    if (parts[i] == "#") continue;
                    this.AllowedUrlAttributes.Add(parts[i].Substring(1));
                }
                else {
                    this.AllowedAttributes.Add(parts[i]);
                }
            }

        }

        public string ElementName { get; set; }

        public ICollection<string> AllowedAttributes { get; set; } = new List<string>();

        public ICollection<string> AllowedUrlAttributes { get; set; } = new List<string>();

        public static IEnumerable<HtmlConstruct> Parse(string rules) {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            if (string.IsNullOrWhiteSpace(rules)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(rules));

            var elementDefinitions = rules.ToLower().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
            foreach (var item in elementDefinitions) {
                yield return new HtmlConstruct(item);
            }

        }
    }
}


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Altairis.HtmlFilter {


    public class SafeHtmlFilter {
        private readonly IEnumerable<HtmlConstruct> EnabledConstructs;

        private const string MinimalRules = "a title #href, b, i, p";
        private const string DefaultRules = "a title #href, abbr title, b, code, em, i, img alt title width height #src, ins, del, sup, sub, strong, ul, ol, li, dl, dt, dd, p";

        public SafeHtmlFilter() : this(DefaultRules) {
        }

        public SafeHtmlFilter(IEnumerable<HtmlConstruct> enabledConstructs) {
            this.EnabledConstructs = enabledConstructs ?? throw new ArgumentNullException(nameof(enabledConstructs));
        }

        public SafeHtmlFilter(string rules) {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            if (string.IsNullOrWhiteSpace(rules)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(rules));

            this.EnabledConstructs = HtmlConstruct.Parse(rules);
        }

        public string FilterHtml(string input) {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(input));

            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            doc.OptionOutputAsXml = true;

            foreach (var node in doc.DocumentNode.ChildNodes) {
                this.ProcessSingleNode(node);
            }

            return doc.DocumentNode.InnerHtml;
        }

        private void ProcessSingleNode(HtmlNode node) {
            // Process elements
            if (node.NodeType == HtmlNodeType.Element) {
                // Convert element name to lowercase
                node.Name = node.Name.ToLower();

                if (!this.EnabledConstructs.Any(x => x.ElementName.Equals(node.Name))) {
                    // Replace elements with unsupported names with harmless <span>
                    node.Name = "span";
                    node.Attributes.RemoveAll();
                }
                else {
                    // Process attributes
                    var rule = this.EnabledConstructs.Single(x => x.ElementName.Equals(node.Name));
                    if (!rule.AllowedAttributes.Any()) {
                        // No allowed attributes - remove all
                        node.Attributes.RemoveAll();
                    }
                    else {
                        // Process attributes
                        var removedAttributeNames = new List<string>();
                        foreach (var attr in node.Attributes) {
                            // Convert attribute name to lowercase
                            attr.Name = attr.Name.ToLower();

                            // Check generic allowed attributes
                            if (rule.AllowedAttributes.Contains(attr.Name)) {
                                continue;
                            }

                            // Check URL allowed attributes
                            if (rule.AllowedUrlAttributes.Contains(attr.Name) && Regex.IsMatch(attr.Value, @"^((http|https|ftp|mailto)\://|/|#)")) {
                                continue;
                            }

                            // Mark unsupported attribute for removal
                            removedAttributeNames.Add(attr.Name);
                        }

                        // Remove marked attributes
                        foreach (var attrName in removedAttributeNames) {
                            node.Attributes.Remove(attrName);
                        }
                    }
                }
            }

            // Process all children
            foreach (var child in node.ChildNodes) {
                this.ProcessSingleNode(child);
            }
        }

    }
}
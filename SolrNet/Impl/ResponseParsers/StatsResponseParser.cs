﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    public class StatsResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the stats results and uses recursion to get any facet results
        /// </summary>
        /// <param name="node"></param>
        /// <param name="selector">Start with 'stats_fields'</param>
        /// <returns></returns>
        public Dictionary<string, StatsResult> ParseStats(XmlNode node, string selector) {
            var d = new Dictionary<string, StatsResult>();
            var mainNode = node.SelectSingleNode(string.Format("lst[@name='{0}']", selector));
            foreach (XmlNode n in mainNode.ChildNodes) {
                var name = n.Attributes["name"].Value;
                d[name] = ParseStatsNode(n);
            }

            return d;
        }

        public IDictionary<string, Dictionary<string, StatsResult>> ParseFacetNode(XmlNode node) {
            var r = new Dictionary<string, Dictionary<string, StatsResult>>();
            foreach (XmlNode n in node.ChildNodes) {
                var facetName = n.Attributes["name"].Value;
                r[facetName] = ParseStats(n.ParentNode, facetName);
            }
            return r;
        }

        public StatsResult ParseStatsNode(XmlNode node) {
            var r = new StatsResult();
            foreach (XmlNode statNode in node.ChildNodes) {
                var name = statNode.Attributes["name"].Value;
                switch (name) {
                    case "min":
                        r.Min = Convert.ToDouble(statNode.InnerText);
                        break;
                    case "max":
                        r.Max = Convert.ToDouble(statNode.InnerText);
                        break;
                    case "sum":
                        r.Sum = Convert.ToDouble(statNode.InnerText);
                        break;
                    case "sumOfSquares":
                        r.SumOfSquares = Convert.ToDouble(statNode.InnerText);
                        break;
                    case "mean":
                        r.Mean = Convert.ToDouble(statNode.InnerText);
                        break;
                    case "stddev":
                        r.StdDev = Convert.ToDouble(statNode.InnerText);
                        break;
                    case "count":
                        r.Count = Convert.ToInt64(statNode.InnerText);
                        break;
                    case "missing":
                        r.Missing = Convert.ToInt64(statNode.InnerText);
                        break;
                    default:
                        r.FacetResults = ParseFacetNode(statNode);
                        break;
                }
            }
            return r;
        }
    }
}
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NimiTokiPona {
    public static class WordHelpers {
        //https://en.wikipedia.org/wiki/Levenshtein_distance
        public static int LevenshteinDistance(string s, string t) {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0) {
                return m;
            }
            if (m == 0) {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++) {
            }
            for (int j = 0; j <= m; d[0, j] = j++) {
            }
            for (int i = 1; i <= n; i++) {
                for (int j = 1; j <= m; j++) {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        public static float SearchDistance(string search, string name, string definition) {
            name = name.ToLowerInvariant();
            definition = definition.ToLowerInvariant();
            float distance = LevenshteinDistance(search, name);
            if (name.StartsWith(search)) {
                distance -= 3;
            } else if (name.Contains(search)) {
                distance -= 1;
            }
            string[] searchWords = search.Split(new char[]{' ', ',', ';', '-', '\n', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            string[] definitionWords = definition.Split(new char[]{' ', ',', ';', '-', '?', '!', '\n', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if (searchWords.All(s => definitionWords.Contains(s))) {
                distance = Math.Min(distance, 0.5f);
            } else if (searchWords.Any(s => definitionWords.Contains(s))) {
                distance = Math.Min(distance, 2f);
            }
            return distance;
        }

        public static float PlaceDistance(string search, string tokiPona, string english) {
            tokiPona = tokiPona.ToLowerInvariant();
            english = english.ToLowerInvariant();
            float distance = Math.Min(LevenshteinDistance(search, tokiPona), LevenshteinDistance(search, english));
            return distance;
        }
    }
}
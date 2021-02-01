namespace Codefusion.Jaskier.Common.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;

    using CCMEngine;

    public static class CyclomaticComplexityHelper
    {
        private static readonly string[] SupportedExtensions = new[] { ".cs", ".ts", ".js" };

        public static bool IsSupportedFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            var fileExtension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(fileExtension))
            {
                return false;
            }

            return SupportedExtensions.Any(e => e.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase));
        }

        public static CyclomaticComplexityMetric CalculateMetric(Stream stream, string filePath, bool disposeStream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var listener = new SortedListener(1000, new List<string>(), 0);

            try
            {
                var fileAnalyzer = new FileAnalyzer(new StreamReader(stream), listener, null, false, filePath);
                fileAnalyzer.Analyze();
            }
            finally
            {
                if (disposeStream)
                {
                    stream.Dispose();
                }
            }

            var ccmMetrics = listener.Metrics.Select(g => g.CCM).ToList();
            if (!ccmMetrics.Any())
            {
                return null;
            }

            return new CyclomaticComplexityMetric
            {
                CCMMax = ccmMetrics.Max(),
                CCMAvg = ccmMetrics.Average(),
                CCMMd = ccmMetrics.Median()
            };
        }
    }
}

//using System.Collections.Generic;
//using System.Linq;
//using Telerik.Reporting.Processing;

//namespace Telerik.Reporting.Processing
//{
//    public static class ReportProcessorExtensions
//    {
//        public static IEnumerable<object> GetSupportedExportFormats()
//        {
//            // Get the Telerik.Reporting.Processing assembly
//            var processingAssembly = typeof(ReportProcessor).Assembly;
//            // Get the internal RenderingExtensionManager type
//            var managerType = processingAssembly.GetType("Telerik.Reporting.Processing.RenderingExtensionManager");
//            if (managerType == null)
//                yield break;

//            // Get the static property 'RenderingExtensions'
//            var prop = managerType.GetProperty("RenderingExtensions", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
//            if (prop == null)
//                yield break;

//            // Get the value (IEnumerable<ExtensionInfo>)
//            var extensions = prop.GetValue(null) as System.Collections.IEnumerable;
//            if (extensions == null)
//                yield break;

//            foreach (var ext in extensions)
//            {
//                // Use reflection to get Name, Description, DefaultExtension
//                var name = ext.GetType().GetProperty("Name")?.GetValue(ext)?.ToString();
//                var desc = ext.GetType().GetProperty("Description")?.GetValue(ext)?.ToString();
//                var extn = ext.GetType().GetProperty("DefaultExtension")?.GetValue(ext)?.ToString();
//                yield return new { Name = name, Description = desc, Extension = extn };
//            }
//        }
//    }
//}
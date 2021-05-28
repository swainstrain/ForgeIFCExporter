using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using DesignAutomationFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExport
{
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Commands : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnStartup(Autodesk.Revit.ApplicationServices.ControlledApplication app)
        {
            DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;
            return ExternalDBApplicationResult.Succeeded;
        }

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            LogTrace("Design Automation Ready event triggered...");
            e.Succeeded = true;
            IFCExport(e.DesignAutomationData);
        }

        public static void IFCExport(DesignAutomationData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            Application rvtApp = data.RevitApp;
            if (rvtApp == null) throw new InvalidDataException(nameof(rvtApp));

            Document doc = data.RevitDoc;
            if (doc == null) throw new InvalidOperationException("Could not open document.");

            string modelPath = data.FilePath;
            if (String.IsNullOrWhiteSpace(modelPath)) throw new InvalidDataException(nameof(modelPath));

            /// The Code starts here

            IFCExportOptions options = new IFCExportOptions();

            //options.FileVersion = IFCVersion.IFC2x3;
            //options.SpaceBoundaryLevel = 0;
            //options.AddOption("ExportInternalRevitPropertySets", "false");
            //options.AddOption("ExportIFCCommonPropertySets", "false");
            //options.AddOption("ExportAnnotations ", "false");
            //options.AddOption("SpaceBoundaries ", "0");
            //options.AddOption("ExportRoomsInView", "false");
            //options.AddOption("Use2DRoomBoundaryForVolume ", "false");
            //options.AddOption("UseFamilyAndTypeNameForReference ", "false");
            //options.AddOption("Export2DElements", "false");
            //options.AddOption("ExportPartsAsBuildingElements", "false");
            //options.AddOption("ExportBoundingBox", "true");
            //options.AddOption("ExportSolidModelRep", "false");
            //options.AddOption("ExportSchedulesAsPsets", "false");
            //options.AddOption("ExportSpecificSchedules", "false");
            //options.AddOption("ExportLinkedFiles", "false");
            //options.AddOption("IncludeSiteElevation", "false");
            //options.AddOption("StoreIFCGUID", "false");
            //options.AddOption("VisibleElementsOfCurrentView", "true");
            //options.AddOption("UseActiveViewGeometry", "true");
            //options.AddOption("TessellationLevelOfDetail", "0");

            FilteredElementCollector coll = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D));

            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Export IFC");

                foreach (View3D view in coll)
                {

                    options.FilterViewId = view.Id;

                    doc.Export(Directory.GetCurrentDirectory(), "outputFile", options);
                    LogTrace("Saving file...");

                }

                transaction.Commit();
            }

            /// The Code ends here

            //string OUTPUT_FILE = "Result.rvt";
            //ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath(OUTPUT_FILE);
            //SaveAsOptions SAO = new SaveAsOptions();
            //SAO.OverwriteExistingFile = true;

            //doc.SaveAs(path, SAO);
        }

        public ExternalDBApplicationResult OnShutdown(Autodesk.Revit.ApplicationServices.ControlledApplication app)
        {
            return ExternalDBApplicationResult.Succeeded;
        }

        private static void LogTrace(string format, params object[] args) { System.Console.WriteLine(format, args); }


    }
}

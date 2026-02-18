using System;
using System.IO;
using System.Text;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace AlbeStudio.BIMTools
{
    [Transaction(TransactionMode.Manual)]
    public class ExtractRoomDataCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            // Collect all rooms in the document
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var rooms = collector.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToElements();

            if (rooms.Count == 0)
            {
                TaskDialog.Show("Info", "No rooms found in the current document.");
                return Result.Cancelled;
            }

            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Room Number,Room Name,Area (SQM),Volume (CUM)");

            int exportedCount = 0;

            foreach (Room room in rooms.Cast<Room>())
            {
                // Skip unplaced or unenclosed rooms
                if (room.Area > 0)
                {
                    string number = room.Number;
                    string name = room.Name;
                    
                    // Convert internal units (sq ft) to square meters
                    double areaSqm = UnitUtils.ConvertFromInternalUnits(room.Area, UnitTypeId.SquareMeters);
                    double volumeCum = UnitUtils.ConvertFromInternalUnits(room.Volume, UnitTypeId.CubicMeters);

                    csvContent.AppendLine($"{number},{name},{Math.Round(areaSqm, 2)},{Math.Round(volumeCum, 2)}");
                    exportedCount++;
                }
            }

            // Save file to Desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "BIM_RoomData_Export.csv");

            try
            {
                File.WriteAllText(filePath, csvContent.ToString());
                TaskDialog.Show("Success", $"Successfully exported {exportedCount} rooms to:\n{filePath}");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}

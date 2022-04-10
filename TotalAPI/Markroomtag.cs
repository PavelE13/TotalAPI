using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalAPI
{
    [Transaction(TransactionMode.Manual)]
    public class Markroomtag : IExternalCommand
    {
        public ElementId viewId;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listoflev = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction mark = new Transaction(doc, "Добавление помещений");
            mark.Start();
            ICollection<ElementId> rooms;
            foreach (Level level in listoflev)
            {
                rooms = doc.Create.NewRooms2(level);
            }

            FilteredElementCollector roomselect = new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Rooms);
            IList<ElementId> roomsid = roomselect.ToElementIds() as IList<ElementId>;

            foreach (ElementId roomid in roomsid)
            {
                UV roomTagLocation = new UV(0, 0);
                LinkElementId roomId = new LinkElementId(roomid);

                Element x = doc.GetElement(roomid);
                Room room = x as Room;
                string levname = room.Level.Name.Substring(7);
                room.Name = $"{levname}_{room.Number}";

                RoomTag roomTag = doc.Create.NewRoomTag(roomId, roomTagLocation, viewId);

                if (null == roomTag)
                {
                    throw new Exception("Create a new room tag failed.");
                }

                TaskDialog.Show("Revit", "Room tag created successfully.");
            }

            mark.Commit();

            return Result.Succeeded;
        }
    }
}

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPILibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingFurnitureCreate
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;              

        public List<FamilySymbol> FamilyTypes { get; } = new List<FamilySymbol>();

        public DelegateCommand SaveCommand { get; }

        public List<Level> Levels { get; } = new List<Level>();

        public Level SelectedLevel { get; set; }

        public FamilySymbol SelectedFamilyType { get; set; }

        public List<XYZ> Points { get; } = new List<XYZ>();

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;            
            FamilyTypes = CategoryFurnitureUtils.GetFurnitureTypes(commandData);
            Levels = LevelsUtils.GetLevels(commandData);
            Points = SelectionUtils.GetPoints(_commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var oLevel = (Level)doc.GetElement(SelectedLevel.Id);

            for (int i = 0; i < Points.Count; i++)
            {
                if (i == 0)
                    continue;
                FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedFamilyType, Points[i], oLevel);
            }
            RaiseCloseRequest();
        }

        public event EventHandler CloseRequest;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}

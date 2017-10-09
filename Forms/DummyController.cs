using System;
using System.Windows.Forms;

namespace Teleprompter.Forms {

    public partial class DummyController : Form {
        private bool nominalDirection;

        public DummyController() {
            nominalDirection = true;
            InitializeComponent();
        }

        private void btnUp_Click(object sender, EventArgs e) {
            Button b = sender as Button;
            switch (b.Name) {
                case "btnUp":
                    Classes.Controller.mainForm.ChangeMarkersFromController(-1);
                    break;

                case "btnDown":
                    Classes.Controller.mainForm.ChangeMarkersFromController(1);
                    break;

                case "btnLeft":
                    Classes.Controller.mainForm.AdjustFontSize(-1);
                    break;

                case "btnRight":
                    Classes.Controller.mainForm.AdjustFontSize(1);
                    break;

                case "btnMinus":
                    Classes.Controller.mainForm.UpdateSpeedFromController(nominalDirection ? -1 : 1);
                    break;

                case "btnHome":
                    Classes.Controller.mainForm.UpdateSpeedFromController(0);
                    break;

                case "btnPlus":
                    Classes.Controller.mainForm.UpdateSpeedFromController(nominalDirection ? 1 : -1);
                    break;

                case "btnOne":
                    Classes.Controller.mainForm.SetPercFromController(0);
                    break;

                case "btnTwo":
                    Classes.Controller.mainForm.SetPercFromController(100);
                    break;

                case "btnA":
                    nominalDirection = !nominalDirection;
                    Classes.Controller.mainForm.UpdateSpeedFromController(666);
                    break;

                case "btnB":
                    Classes.Controller.mainForm.TogglePlay();
                    break;
            }
        }

        private void DummyController_Load(object sender, EventArgs e) {
        }
    }
}
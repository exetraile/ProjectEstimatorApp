using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ProjectEstimatorApp.Styles;

namespace ProjectEstimatorApp.Views
{
    public partial class AddRoomForm : Form
    {
        public string RoomName => txtName.Text.Trim();
        public double WidthValue => ParseDimension(txtWidth.Text);
        public double HeightValue => ParseDimension(txtHeight.Text);

        private TextBox txtName;
        private TextBox txtWidth;
        private TextBox txtHeight;
        private Button btnOk;
        private Button btnCancel;

        public AddRoomForm()
        {
            InitializeForm();
            InitializeControls();
        }

        private void InitializeForm()
        {
            StyleHelper.Forms.ApplyDialogStyle(this);
            Text = "Add Room";
            ClientSize = new Size(360, 240);
        }

        private void InitializeControls()
        {
            txtName = StyleHelper.Inputs.TextBox("Room name");
            txtName.Location = new Point(20, 20);
            txtName.Width = 320;

            txtWidth = StyleHelper.Inputs.TextBox("Width (m)");
            txtWidth.Location = new Point(20, 70);
            txtWidth.Width = 150;

            txtHeight = StyleHelper.Inputs.TextBox("Height (m)");
            txtHeight.Location = new Point(190, 70);
            txtHeight.Width = 150;

            btnOk = StyleHelper.Buttons.Primary("OK", 100);
            btnOk.Location = new Point(140, 140);
            btnOk.DialogResult = DialogResult.OK;

            btnCancel = StyleHelper.Buttons.Secondary("Cancel", 100);
            btnCancel.Location = new Point(250, 140);
            btnCancel.DialogResult = DialogResult.Cancel;

            txtWidth.KeyPress += NumericInput_KeyPress;
            txtHeight.KeyPress += NumericInput_KeyPress;

            Controls.AddRange(new Control[] { txtName, txtWidth, txtHeight, btnOk, btnCancel });

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void NumericInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;

            if (e.KeyChar == '.' && ((sender as TextBox)?.Text.IndexOf('.') > -1))
                e.Handled = true;
        }

        private double ParseDimension(string text)
        {
            if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return Math.Round(result, 2);
            return 0;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(RoomName))
                {
                    MessageBox.Show("Please enter room name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }
                if (WidthValue <= 0 || HeightValue <= 0)
                {
                    MessageBox.Show("Width and height must be positive numbers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }
    }
}

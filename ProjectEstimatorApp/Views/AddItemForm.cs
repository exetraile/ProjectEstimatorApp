// Views/AddItemForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectEstimatorApp.Views
{
    public partial class AddItemForm : Form
    {
        public string ItemName => txtName.Text.Trim();
        public string Unit => txtUnit.Text.Trim();
        public decimal Quantity => decimal.Parse(txtQuantity.Text);
        public decimal Price => decimal.Parse(txtPrice.Text);

        private TextBox txtName;
        private TextBox txtUnit;
        private TextBox txtQuantity;
        private TextBox txtPrice;
        private Button btnOk;
        private Button btnCancel;

        public AddItemForm()
        {
            InitializeForm();
            InitializeControls();
            SetupLayout();
        }

        private void InitializeForm()
        {
            Text = "Add New Item";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(350, 220);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9);
        }

        private void InitializeControls()
        {
            txtName = new TextBox
            {
                Text = "Name",
                Top = 20,
                Left = 20,
                Width = 310,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtUnit = new TextBox
            {
                Text = "Unit",
                Top = 60,
                Left = 20,
                Width = 310,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtQuantity = new TextBox
            {
                Text = "1",
                Top = 100,
                Left = 20,
                Width = 310,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtPrice = new TextBox
            {
                Text = "0",
                Top = 140,
                Left = 20,
                Width = 310,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Top = 180,
                Left = 180,
                Width = 80,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Top = 180,
                Left = 270,
                Width = 80,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };

            Controls.AddRange(new Control[] { txtName, txtUnit, txtQuantity, txtPrice, btnOk, btnCancel });
        }

        private void SetupLayout()
        {
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}
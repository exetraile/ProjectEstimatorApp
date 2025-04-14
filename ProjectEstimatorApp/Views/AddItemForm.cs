// Views/AddItemForm.cs
using System;
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
            ClientSize = new System.Drawing.Size(300, 200);
        }

        private void InitializeControls()
        {
            txtName = new TextBox { Text = "Name", Top = 20, Left = 20, Width = 260 };
            txtUnit = new TextBox { Text = "Unit", Top = 50, Left = 20, Width = 260 };
            txtQuantity = new TextBox { Text = "1", Top = 80, Left = 20, Width = 260 };
            txtPrice = new TextBox { Text = "0", Top = 110, Left = 20, Width = 260 };

            txtName.Enter += (s, e) => { if (txtName.Text == "Name") txtName.Text = ""; };
            txtName.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtName.Text)) txtName.Text = "Name"; };

            txtUnit.Enter += (s, e) => { if (txtUnit.Text == "Unit") txtUnit.Text = ""; };
            txtUnit.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtUnit.Text)) txtUnit.Text = "Unit"; };

            txtQuantity.Enter += (s, e) => { if (txtQuantity.Text == "1") txtQuantity.Text = ""; };
            txtQuantity.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtQuantity.Text)) txtQuantity.Text = "1"; };

            txtPrice.Enter += (s, e) => { if (txtPrice.Text == "0") txtPrice.Text = ""; };
            txtPrice.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtPrice.Text)) txtPrice.Text = "0"; };

            btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Top = 150, Left = 120 };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Top = 150, Left = 200 };

            Controls.AddRange(new Control[] { txtName, txtUnit, txtQuantity, txtPrice, btnOk, btnCancel });
        }

        private void SetupLayout()
        {
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}
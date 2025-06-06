using System;
using System.Drawing;
using System.Windows.Forms;
using ProjectEstimatorApp.Styles;

namespace ProjectEstimatorApp.Views
{
    public partial class AddItemForm : Form
    {
        public string ItemName => txtName.Text.Trim();
        public string Unit => txtUnit.Text.Trim();
        public decimal Quantity => decimal.TryParse(txtQuantity.Text, out var q) ? q : 0;
        public decimal Price => decimal.TryParse(txtPrice.Text, out var p) ? p : 0;

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
        }

        private void InitializeForm()
        {
            StyleHelper.Forms.ApplyDialogStyle(this);
            Text = "Add New Item";
            ClientSize = new Size(360, 280);
        }

        private void InitializeControls()
        {
            txtName = StyleHelper.Inputs.TextBox("Name");
            txtName.Location = new Point(20, 20);
            txtName.Width = 320;

            txtUnit = StyleHelper.Inputs.TextBox("Unit");
            txtUnit.Location = new Point(20, 70);
            txtUnit.Width = 150;

            txtQuantity = StyleHelper.Inputs.TextBox("Quantity");
            txtQuantity.Location = new Point(190, 70);
            txtQuantity.Width = 150;

            txtPrice = StyleHelper.Inputs.TextBox("Price");
            txtPrice.Location = new Point(20, 120);
            txtPrice.Width = 320;

            btnOk = StyleHelper.Buttons.Primary("OK", 100);
            btnOk.Location = new Point(140, 190);
            btnOk.DialogResult = DialogResult.OK;

            btnCancel = StyleHelper.Buttons.Secondary("Cancel", 100);
            btnCancel.Location = new Point(250, 190);
            btnCancel.DialogResult = DialogResult.Cancel;

            txtQuantity.KeyPress += NumericInput_KeyPress;
            txtPrice.KeyPress += NumericInput_KeyPress;

            Controls.AddRange(new Control[] { txtName, txtUnit, txtQuantity, txtPrice, btnOk, btnCancel });

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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(ItemName))
                {
                    MessageBox.Show("Please enter item name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }
                if (Quantity <= 0 || Price < 0)
                {
                    MessageBox.Show("Quantity must be positive and price non-negative", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }
    }
}

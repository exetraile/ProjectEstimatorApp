// Views/AddRoomForm.cs
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ProjectEstimatorApp.Views
{
    public partial class AddRoomForm : Form
    {
        public string RoomName => txtName.Text.Trim();
        public double Width => ParseDimension(txtWidth.Text);
        public double Height => ParseDimension(txtHeight.Text);

        private TextBox txtName;
        private TextBox txtWidth;
        private TextBox txtHeight;
        private Button btnOk;
        private Button btnCancel;

        public AddRoomForm()
        {
            InitializeForm();
            InitializeControls();
            SetupLayout();
        }

        private void InitializeForm()
        {
            Text = "Add Room";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(350, 200);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9);
        }

        private void InitializeControls()
        {
            txtName = new TextBox
            {
                Text = "Room name",
                Top = 20,
                Left = 20,
                Width = 310,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtWidth = new TextBox
            {
                Text = "Width (m)",
                Top = 60,
                Left = 20,
                Width = 310,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtHeight = new TextBox
            {
                Text = "Height (m)",
                Top = 100,
                Left = 20,
                Width = 310,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtName.Enter += (s, e) => { if (txtName.Text == "Room name") txtName.Text = ""; };
            txtName.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtName.Text)) txtName.Text = "Room name"; };

            txtWidth.Enter += (s, e) => { if (txtWidth.Text == "Width (m)") txtWidth.Text = ""; };
            txtWidth.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtWidth.Text)) txtWidth.Text = "Width (m)"; };

            txtHeight.Enter += (s, e) => { if (txtHeight.Text == "Height (m)") txtHeight.Text = ""; };
            txtHeight.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtHeight.Text)) txtHeight.Text = "Height (m)"; };


            btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 180,
                Top = 140,
                Width = 80,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Left = 270,
                Top = 140,
                Width = 80,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };

            txtWidth.KeyPress += NumericInput_KeyPress;
            txtHeight.KeyPress += NumericInput_KeyPress;

            Controls.AddRange(new Control[] { txtName, txtWidth, txtHeight, btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void SetupLayout()
        {
            // Layout already set in InitializeControls
        }

        private void NumericInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private double ParseDimension(string text)
        {
            if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return Math.Round(result, 2);
            }
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

                if (Width <= 0 || Height <= 0)
                {
                    MessageBox.Show("Width and height must be positive numbers", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }
    }
}
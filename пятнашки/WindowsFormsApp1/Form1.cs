using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnStart.Enabled = false;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            btnStart.Enabled = txtName.Text.Length > 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length < 6)
            {
                MessageBox.Show("Имя должно содержать минимум 6 символов!", "Ошибка", MessageBoxButtons.OK);
                return;
            }

            GameForm gameForm = new GameForm(txtName.Text);
            this.Hide();
            gameForm.ShowDialog();
            this.Show();
        }
    }
}
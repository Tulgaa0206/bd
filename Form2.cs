using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace BD_visual_
{
    public partial class Form2 : Form
    {
        private Form1 _parentForm;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(Form1 parentForm) : this()
        {
            _parentForm = parentForm;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string connStr = "Host=103.50.205.42;Port=5432;Username=Worms;Password=Worms1231@;Database=VisualBD;Search Path=public";
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Button btn && btn.Name.StartsWith("seats"))
                    {
                        string seatId = btn.Name;
                        using (var cmd = new NpgsqlCommand(
                                   "SELECT status FROM public.seats WHERE seat_id = @seatId", conn))
                        {
                            cmd.Parameters.AddWithValue("seatId", seatId);
                            var status = cmd.ExecuteScalar() as string;
                            btn.Tag = status;

                            switch (status)
                            {
                                case "available":
                                    btn.BackColor = Color.LightGreen; btn.Enabled = true; break;
                                case "selected":
                                    btn.BackColor = Color.Yellow; btn.Enabled = true; break;
                                case "reserved":
                                    btn.BackColor = Color.Gray; btn.Enabled = false; break;
                                case "saved":
                                    btn.BackColor = Color.Red; btn.Enabled = false; break;
                            }

                            btn.Click += SeatButton_Click;
                        }
                    }
                }
            }
        }

        private void SeatButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var currentStatus = (string)btn.Tag;
            if (currentStatus == "available")
            {
                btn.Tag = "selected";
                btn.BackColor = Color.Yellow;
            }
            else if (currentStatus == "selected")
            {
                btn.Tag = "available";
                btn.BackColor = Color.LightGreen;
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager
                .ConnectionStrings["PostgresConnection"]
                .ConnectionString;

            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Button btn
                     && btn.Name.StartsWith("seat")
                     && (string)btn.Tag == "selected")
                    {
                        string seatId = btn.Name;
                        using (var cmd = new NpgsqlCommand(
                                   "UPDATE seats SET status = @status WHERE seat_id = @seatId", conn))
                        {
                            cmd.Parameters.AddWithValue("status", "saved");
                            cmd.Parameters.AddWithValue("seatId", seatId);
                            cmd.ExecuteNonQuery();
                        }
                        btn.Tag = "saved";
                        btn.BackColor = Color.Red;
                        btn.Enabled = false;
                    }
                }
            }

            MessageBox.Show(
                "Суудлууд амжилттай хадгалагдлаа.",
                "Амжилт",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_parentForm != null)
                _parentForm.Show();

            this.Close();
        }
    }
}

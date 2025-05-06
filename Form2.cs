using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Npgsql;
using BD_visual_.Properties;   // Resources.gray/green/red

namespace BD_visual_
{
    public partial class Form2 : Form
    {
        private readonly Form1 _parentForm;

        public Form2() => InitializeComponent();
        public Form2(Form1 parent) : this() => _parentForm = parent;

        private void Form2_Load(object sender, EventArgs e)
        {
            var connStr = ConfigurationManager
                .ConnectionStrings["PostgresConnection"]
                .ConnectionString;

            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();

                var seatButtons = this.Controls
                    .OfType<Button>()
                    .Where(b => b.Name.StartsWith("seat"))
                    .ToList();

                foreach (var btn in seatButtons)
                {
                    // numeric part as string
                    string seatKey = btn.Name.Substring(4);  // "42"

                    using (var cmd = new NpgsqlCommand(
                               "SELECT status FROM seat WHERE seat_id = @seatKey", conn))
                    {
                        // pass it as string, so it matches varchar
                        cmd.Parameters.AddWithValue("seatKey", seatKey);
                        var result = cmd.ExecuteScalar();
                        string status = (result as string) ?? "available";

                        btn.Tag = status;
                        switch (status)
                        {
                            case "available":
                                btn.BackgroundImage = Resources.gray;
                                btn.Enabled = true;
                                break;
                            case "reserved":
                            case "saved":
                                btn.BackgroundImage = Resources.red;
                                btn.Enabled = false;
                                break;
                        }

                        btn.Click += SeatButton_Click;
                    }
                }
            }
        }

        private void SeatButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var current = btn.Tag as string;
            if (current == "available")
            {
                btn.Tag = "selected";
                btn.BackgroundImage = Resources.green;
            }
            else if (current == "selected")
            {
                btn.Tag = "available";
                btn.BackgroundImage = Resources.gray;
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            var connStr = ConfigurationManager
                .ConnectionStrings["PostgresConnection"]
                .ConnectionString;

            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();

                var toSave = this.Controls
                                 .OfType<Button>()
                                 .Where(b => (b.Tag as string) == "selected");

                foreach (var btn in toSave)
                {
                    string seatKey = btn.Name.Substring(4);

                    using (var updateCmd = new NpgsqlCommand(
                               "UPDATE seat SET status = @status WHERE seat_id = @seatKey", conn))
                    {
                        updateCmd.Parameters.AddWithValue("status", "saved");
                        updateCmd.Parameters.AddWithValue("seatKey", seatKey);
                        updateCmd.ExecuteNonQuery();
                    }

                    btn.Tag = "saved";
                    btn.BackgroundImage = Resources.red;
                    btn.Enabled = false;
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
            _parentForm?.Show();
            Close();
        }
    }
}

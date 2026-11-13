using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.IO;

namespace teknikservis
{
    public partial class database_bilgileri : Form
    {
        public database_bilgileri()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox1.Text != "" && textBox2.Text != "")
            {
                XmlTextWriter yaz = new XmlTextWriter(Application.StartupPath + "\\settings.xml", System.Text.UTF8Encoding.UTF8);
                yaz.Formatting = Formatting.Indented;
                try
                {
                    yaz.WriteStartDocument();
                    yaz.WriteStartElement("setting");
                    yaz.WriteStartElement("DB");
                    yaz.WriteElementString("Provider", "SQLOLEDB.1");
                    yaz.WriteElementString("ServerName", textBox1.Text);
                    yaz.WriteElementString("DatabaseName", textBox2.Text);
                    yaz.WriteElementString("User Id", textBox3.Text);
                    yaz.WriteElementString("Password", Ayar.sifrele(textBox4.Text, textBox5.Text));
                    yaz.WriteElementString("Keys", textBox5.Text);
                    yaz.WriteEndElement();
                    yaz.WriteEndElement();
                    yaz.Close();
                    MessageBox.Show("ayarlar dosyaniz olustu,program yeniden basladiktan sonra kullanima hazir olacaktir.", "islem basarili", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata Mesaji:" + ex.Message.ToString(), "Hata Alindi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("tum alanlari doldurmaniz gerekir", "Uyari", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Restart();
            }
        }

        private void label(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Application.Exit();
        }
    }
}


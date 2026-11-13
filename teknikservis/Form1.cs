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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
          
        }
       

        public Form1(Label label4)
        {
            this.label4 = label4;
        }

        void database()
        {
            string sunucuadi = @".\SQLEXPRESS";
            string veritabaniAdi = "TeknikServis";
            string vtKullaniciAdi = "admin";
            string vtSifre = "12345";
            XDocument XDoc = XDocument.Load(Application.StartupPath + "\\settings.xml");
            XElement rootElement = XDoc.Root;
            foreach (XElement rehberimiz in rootElement.Elements())
            {
                sunucuadi = rehberimiz.Element("ServerName").Value;
                veritabaniAdi = rehberimiz.Element("DatabaseName").Value;
                vtKullaniciAdi = rehberimiz.Element("UserId").Value;
                vtSifre = Ayar.sifreCoz(rehberimiz.Element("Password").Value, rehberimiz.Element("Keys").Value);
            }
            Ayar.con = new SqlConnection("Data Source=" + sunucuadi + ";Initial Catalog=" + veritabaniAdi + ";Persist Security Info=True;User ID=" + vtKullaniciAdi + ";Password=" + vtSifre);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string dosya_dizi = Application.StartupPath + "\\settings.xml";
            if (File.Exists(dosya_dizi) == true)
            {
                //varsa yapilacak islemler
                //veritabani baglantisi yapilacak login islemleri yapilacak
                database();


            }
            else
            {              //yoksa yapilacak islemler
                           //database bilgileriformu acilacak
                database_bilgileri dt = new database_bilgileri();
                dt.Show();
                this.Hide();

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("Select * from Personel Where KullaniciAdi = @p1 And Sifre = @p2", Ayar.con);
                    cmd.Parameters.AddWithValue("@p1", textBox1.Text);
                    cmd.Parameters.AddWithValue("@p2", textBox2.Text);
                    Ayar.con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        FrmAnasayfa ans = new FrmAnasayfa();
                        
                        //   ans.labe
                        this.Hide();
                        ans.Show();
                        ans.label2.Text = dr["AD"].ToString() + " " + dr["SOYAD"].ToString(); // <--- Buraya yazıyorsun
                                                                                                       // label3.Text = "Giriş Başarılı";
                    }
                    else
                    {
                        label3.Text = "Kullanıcı adı veya şifre hatalı, kontrol edip tekrar deneyin";
                    }
                    Ayar.con.Close();
                }
                catch (Exception)
                {

                    label3.Text = "İşlem gerçekleştirilirken hata oluştu, daha sonra tekrar deneyin.";
                }
            }
            else
            {
                label3.Text = "Tüm alanları doldurmanız gerekir.";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
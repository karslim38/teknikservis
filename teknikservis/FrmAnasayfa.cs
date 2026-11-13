using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using teknikservis;

namespace teknikservis
{
    public partial class FrmAnasayfa : Form
    {
        public FrmAnasayfa()
        {
            InitializeComponent();
        }
        void markadoldur()
        {
            SqlDataAdapter adtr = new SqlDataAdapter("Select * From Marka Where  silindi=False", Ayar.con);
            Ayar.con.Open();
            DataTable table = new DataTable();
            adtr.Fill(table);
            LstMarkaİslemleriMarkalar.DisplayMember = "MarkAdi";
            LstMarkaİslemleriMarkalar.ValueMember = "Markaid";
            LstMarkaİslemleriMarkalar.DataSource = table;
            Ayar.con.Close();
        }

        private void FrmAnasayfa_Load(object sender, EventArgs e)
        {
            markaGetir(); // Form açıldığında markalar listelensin
        }

        // Markaları veritabanından çekip ListBox'a dolduran tek ve doğru fonksiyonumuz
        public void markaGetir()
        {
            try
            {
                if (Ayar.con.State == ConnectionState.Closed)
                {
                    Ayar.con.Open();
                }

                SqlDataAdapter da = new SqlDataAdapter("Select * from Marka where silindi = 0", Ayar.con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                LstMarkaİslemleriMarkalar.DataSource = dt;
                LstMarkaİslemleriMarkalar.DisplayMember = "MarkaAdi"; // Kullanıcının göreceği sütun adı
                LstMarkaİslemleriMarkalar.ValueMember = "MarkaID";   // Arkada tutulacak ID sütunu
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme hatası: " + ex.Message);
            }
            finally
            {
                if (Ayar.con.State == ConnectionState.Open)
                {
                    Ayar.con.Close();
                }
            }
        }
        public void cihazGetir()
        {
            try
            {
                if (Ayar.con.State == ConnectionState.Closed)
                {
                    Ayar.con.Open();
                }

                SqlDataAdapter da = new SqlDataAdapter("Select * from Cihazlar where silindi = 0", Ayar.con);
                DataTable dt = new DataTable();
                da.Fill(dt);

               LstMarkaİslemleriMarkalar.DataSource = dt;
                LstMarkaİslemleriMarkalar.DisplayMember = "CihazAdi"; // Veritabanındaki cihaz adı sütunu
                LstMarkaİslemleriMarkalar.ValueMember = "CihazID";     // Veritabanındaki ID sütunu
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cihaz listeleme hatası: " + ex.Message);
            }
            finally
            {
                if (Ayar.con.State == ConnectionState.Open)
                {
                    Ayar.con.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Panel açma kapama işlemlerin
            if (panel4.Visible == true)
            {
                panel4.Visible = false;
            }
            else
            {
                panel4.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Yeni marka ekleme butonu
            try
            {
                if (Ayar.con.State == ConnectionState.Closed)
                    Ayar.con.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO TblMarka (MarkaAdi, silindi) VALUES (@p1, 0)", Ayar.con);
                cmd.Parameters.AddWithValue("@p1", textBox2.Text);
                cmd.ExecuteNonQuery();
                Ayar.con.Close();

                MessageBox.Show("Marka başarıyla eklendi.");
                markaGetir();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekleme hatası: " + ex.Message);
                if (Ayar.con.State == ConnectionState.Open)
                    Ayar.con.Close();
            }
        }

        // SİL BUTONU (Sil butonuna çift tıklandığında açılan metot burası olmalı)
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (Ayar.con.State == ConnectionState.Closed)
                {
                    Ayar.con.Open();
                }

                // Kaydı tamamen silmek yerine 'silindi' sütununu 1 yaparak gizliyoruz
                SqlCommand komut = new SqlCommand("Update Marka set silindi = 1 where Markaid = @p1", Ayar.con);
                komut.Parameters.AddWithValue("@p1", textBox1.Text); // ID'nin yazılı olduğu textbox

                komut.ExecuteNonQuery();
                Ayar.con.Close();

                markaGetir(); // Listeyi yeniliyoruz

                // Kutuları temizliyoruz
                textBox1.Text = "";
                textBox2.Text = "";

                MessageBox.Show("Marka başarıyla silindi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme sırasında hata oluştu: " + ex.Message);
                if (Ayar.con.State == ConnectionState.Open)
                {
                    Ayar.con.Close();
                }
            }
        }
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Tasarımcının aradığı bu metot buraya eklendiği an hata kaybolacak.
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Listeden seçilen markanın adını ve ID'sini ilgili textbox'lara aktarıyoruz
                textBox1.Text = LstMarkaİslemleriMarkalar.SelectedValue.ToString();
                textBox2.Text = LstMarkaİslemleriMarkalar.Text;
            }
            catch
            {
                // İlk açılışta hata vermemesi için
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (Ayar.con.State == ConnectionState.Closed)
                {
                    Ayar.con.Open();
                }

                SqlCommand komut = new SqlCommand("Update Marka set MarkaAdi = @p1 where Markaid = @p2", Ayar.con);
                komut.Parameters.AddWithValue("@p1", textBox2.Text); // Güncel marka adı
                komut.Parameters.AddWithValue("@p2", textBox1.Text); // Seçili olan markanın ID'si

                komut.ExecuteNonQuery();
                Ayar.con.Close();

                markaGetir(); // Listeyi yeniliyoruz

                // Kutuları temizliyoruz
                textBox1.Text = "";
                textBox2.Text = "";

                MessageBox.Show("Marka başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme sırasında hata oluştu: " + ex.Message);
                if (Ayar.con.State == ConnectionState.Open)
                {
                    Ayar.con.Close();
                }
            }

        }
        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true; // Senin cihaz panelinin adı neyse
                                   // Diğer paneller...
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                try
                {
                    if (Ayar.con.State == ConnectionState.Closed)
                        Ayar.con.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO Cihazlar (CihazAdi, silindi) VALUES (@p1, 0)", Ayar.con);
                    cmd.Parameters.AddWithValue("@p1", textBox2.Text);
                    cmd.ExecuteNonQuery();
                    Ayar.con.Close();

                    MessageBox.Show("Cihaz başarıyla eklendi.");
                    cihazGetir(); // Listeyi güncelle
                    textBox2.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                    if (Ayar.con.State == ConnectionState.Open)
                        Ayar.con.Close();
                }
            }
            else
            {
                MessageBox.Show("Lütfen cihaz adı alanını boş bırakmayın.");
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // ID ve Ad kutularının isimlerini kendi formuna göre kontrol edebilirsin
                textBox1.Text = LstMarkaİslemleriMarkalar.SelectedValue.ToString();
                textBox2.Text = LstMarkaİslemleriMarkalar.Text;
            }
            catch
            {

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox2.Text != "")
            {
                try
                {
                    if (Ayar.con.State == ConnectionState.Closed)
                        Ayar.con.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE Cihazlar SET CihazAdi = @p1 WHERE CihazID = @p2", Ayar.con);
                    cmd.Parameters.AddWithValue("@p1", textBox2.Text);
                    cmd.Parameters.AddWithValue("@p2", textBox2.Text);
                    cmd.ExecuteNonQuery();
                    Ayar.con.Close();

                    MessageBox.Show("Cihaz başarıyla güncellendi.");
                    cihazGetir(); // Listeyi yenile
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Güncelleme hatası: " + ex.Message);
                    if (Ayar.con.State == ConnectionState.Open)
                        Ayar.con.Close();
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellenecek cihazı listeden seçin.");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                try
                {
                    if (Ayar.con.State == ConnectionState.Closed)
                        Ayar.con.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE Cihazlar SET silindi = 1 WHERE CihazID = @p1", Ayar.con);
                    cmd.Parameters.AddWithValue("@p1", textBox2.Text);
                    cmd.ExecuteNonQuery();
                    Ayar.con.Close();

                    MessageBox.Show("Cihaz başarıyla silindi.");
                    cihazGetir(); // Listeyi yenile
                    textBox2.Text = "";
                    textBox2.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme hatası: " + ex.Message);
                    if (Ayar.con.State == ConnectionState.Open)
                        Ayar.con.Close();
                }
            }
            else
            {
                MessageBox.Show("Lütfen silinecek cihazı seçin.");
            }
        }

        private void button9_Click_1(object sender, EventArgs e)

        {
            markadoldur();
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;

            button12.Enabled = false;
            button14.Enabled = false;
            button13.Enabled = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
        }
        private void LstMarkaİşlemleriMarkalar_DoubleClick(object sender, EventArgs e)
        {
            button12.Enabled = true;
            button14.Enabled = true;
            button13.Enabled = false;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("Update Marka set Silindi = 1 Where Markaid=@p1", Ayar.con);
            cmd.Parameters.AddWithValue("@p1", label3.Text);
            Ayar.con.Open();
            cmd.ExecuteNonQuery();
            Ayar.con.Close();
            textBox1.Text = "";
            label3.Text = "";
            button12.Enabled = false;
            button14.Enabled = false;
            markadoldur();
        }
        private void LstMarkaIslemleriMarkalar_DoubleClick(object sender, EventArgs e)
        {
            button12.Enabled = true;
            button14.Enabled = true;
            button13.Enabled = false;

            textBox2.Text = LstMarkaİslemleriMarkalar.Text;
            textBox1.Text = LstMarkaİslemleriMarkalar.SelectedValue.ToString();
        }

        private void button13_Click(object sender, EventArgs e)
        {
           
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Insert into Cihazlar (CihazAdi) values (@p1)", Ayar.con);
                cmd.Parameters.AddWithValue("@p1", TxtCihazİşlemleriCihazAdi.Text.ToUpper());
                Ayar.con.Open();
                cmd.ExecuteNonQuery();
                Ayar.con.Close();
                TxtCihazİşlemleriCihazAdi.Text = "";
                LblCihazİşlemleriCihazid.Text = "";
                cihazdoldur();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    }

}

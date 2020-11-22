using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoSQLInjectionWinforms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ActiveControl = textBox3;
            using (var conn = new SqliteConnection("Data Source=test.db"))
            {
                conn.Open();
                var commande = conn.CreateCommand();
                commande.CommandText = "DROP TABLE IF EXISTS PERSONNES; CREATE TABLE PERSONNES (nom VARCHAR(32), prenom VARCHAR(32), age INT)";
                commande.ExecuteNonQuery();

                commande = conn.CreateCommand();
                commande.CommandText = "DROP TABLE IF EXISTS CONTRATS; CREATE TABLE CONTRATS (entreprise VARCHAR(32), sujet VARCHAR(32), montant INT)";
                commande.ExecuteNonQuery();

                commande = conn.CreateCommand();
                commande.CommandText = "INSERT INTO PERSONNES (nom, prenom, age) VALUES ('Lagaffe', 'Gaston', 63)";
                commande.ExecuteNonQuery();

                commande = conn.CreateCommand();
                commande.CommandText = "INSERT INTO CONTRATS (entreprise, sujet, montant) VALUES ('Ministère', 'Contrat ultra-secret', 1000000)";
                commande.ExecuteNonQuery();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                using (var conn = new SqliteConnection("Data Source=test.db"))
                {
                    conn.Open();
                    var commande = conn.CreateCommand();
                    commande.CommandText = "SELECT nom, prenom, age FROM PERSONNES WHERE nom LIKE '%" + textBox3.Text + "%'";
                    using (var reader = commande.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listBox1.Items.Add($"{reader.GetString(0)} {reader.GetString(1)} ({reader.GetInt32(2)} ans)");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new SqliteConnection("Data Source=test.db"))
                {
                    conn.Open();
                    var commande = conn.CreateCommand();
                    commande.CommandText = "INSERT INTO PERSONNES (nom, prenom, age) VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "', " + numericUpDown1.Value.ToString() + ")";
                    commande.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBox3.Text = "' UNION SELECT name FROM sqlite_master WHERE type='table' --";
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBox3.Text = "' UNION SELECT name, name, 0 FROM sqlite_master WHERE type='table' --";
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBox3.Text = "' UNION SELECT name, name, 0 FROM pragma_table_info('CONTRATS') --";
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBox3.Text = "' UNION SELECT entreprise, sujet, montant FROM CONTRATS --";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBox1.Text = "', '', 0); DROP TABLE PERSONNES --";
        }
    }
}

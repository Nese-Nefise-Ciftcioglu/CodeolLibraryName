using System.Data.SQLite;
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
namespace LibraryName
{
    public partial class Form1 : Form
    {
        SQLiteConnection conn = new SQLiteConnection(@"data source= .\AutoCase Database.db");
        List<string> Library_list = new List<string>();
        public Form1()
        {
            InitializeComponent();
           
            Library_list.Add("Standard Library");
            Library_list.Add("Test1 lib");
            Library_list.Add("TestLib");
            Library_list.Add("New TestLib");

            comboBox1.Items.Add("All");
            comboBox1.Items.Add(Library_list[0]);
            comboBox1.Items.Add(Library_list[1]);
            comboBox1.Items.Add(Library_list[2]);
            comboBox1.Items.Add(Library_list[3]);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Get Hardware Names
            List<string> hardwareNameList = new List<string>();
            conn.Open();
            string query = "SELECT TableName FROM CaseHardware ";

            SQLiteCommand scmd = new SQLiteCommand(query, conn);
            SQLiteDataReader dr = scmd.ExecuteReader();
            while (dr.Read())
            {
                hardwareNameList.Add(dr["TableName"].ToString());
            }
            conn.Close();

   

            List<string> tableNames = new List<string>();
            foreach (string libraryName in Library_list)
            {
                string formattedLibraryName = libraryName.Replace(" ", String.Empty).Replace("&", "and");
                foreach (string hardwareName in hardwareNameList)
                {
                    string formattedHardwareName = hardwareName.Replace(" ", String.Empty).Replace("&", "and");
                    string tableName = formattedLibraryName + formattedHardwareName;
                    tableNames.Add(tableName);
                }
            }

            string selectedLibrary;
            if(comboBox1.SelectedItem == null)
            {
                selectedLibrary = "All";

            }
            else
            {
                selectedLibrary = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
            }


            DataTable dt = new DataTable();

            conn.Open();
            DataTable schema = conn.GetSchema("Tables");
            List<string> tableNamesInDatabase = new List<string>();
            foreach (DataRow row in schema.Rows)
            {
                tableNamesInDatabase.Add(row["TABLE_NAME"].ToString());
            }

            foreach (string libraryName in Library_list)
            {
                if (libraryName == selectedLibrary || selectedLibrary == "All")
                {
                    string formattedLibraryName = libraryName.Replace(" ", String.Empty).Replace("&", "and");
                    foreach (string hardwareName in hardwareNameList)
                    {
                        string formattedHardwareName = hardwareName.Replace(" ", String.Empty).Replace("&", "and");
                        string tableName = formattedLibraryName + formattedHardwareName;
                        if (!tableName.Contains("PriceList") && !tableName.Contains("InteriorComponents") && !tableName.Contains("TongueandGrooveExtrusions"))
                        {
                            if (tableNamesInDatabase.Contains(tableName))
                            {
                                string query1 = "SELECT t.SKU, t.Type, t.Library_Name, s.Unit, s.NetPrice_Old, s.NetPrice, s.[Description], s.[Discount], s.[Currency], s.[Min_Qty],t.Labour_Timing,t.Labour_Unit FROM " + formattedLibraryName + "CasePartsPriceList s JOIN " + tableName + " t ON s.Product = t.SKU";
                                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query1, conn);
                                adapter.Fill(dt);
                            }
                        }
                    }
                }
            }
            conn.Close();
            dataGridView1.DataSource = dt;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wikipedia___Yeni_maç_oluşturma
{
    public partial class Form1 : Form
    {
        OleDbConnection connection =
            new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=
C:\Users\R Fatih\Desktop\Wikipedia\2L24-25.xlsx; 
Extended Properties='Excel 12.0 xml;HDR=YES;'");
        public Form1()
        {
            InitializeComponent();
        }
        List<Team> teams = new List<Team>();
        List<Match> matches = new List<Match>();
        List<Bye> bye = new List<Bye>();
        private void button1_Click(object sender, EventArgs e)
        {

            string[] matches = File.ReadAllLines("1.txt");
           
            for (int i = 0; i < matches.Length; i++)
            {
                string home = matches[i].Split('-')[0];
                string away = matches[i].Split('-')[1].Split(',')[0];
                string tff = matches[i].Split(',')[1];
                string main =
                   "|" + home + "-" + away + " = \n" +
   "{{Kapanabilir futbol maçı kutusu\n" +
   "|tarih             = <!--{{Başlangıç tarihi|202?||}}-->\n" +
   "|zaman             = \n" +
   "|tur               = " + ((i/9)+1) + "\n" +
   "|takım1            = [[" + teams.Where(x=>x.ShortName==home).FirstOrDefault().FullName + "]]\n" +
   "|sonuç             = \n" +
   "|rapor             = [https://www.tff.org/Default.aspx?pageID=29&macID=" + tff + " Rapor]\n" +
   "|takım2            = [[" + teams.Where(x => x.ShortName == away).FirstOrDefault().FullName + "]]\n" +
   "|goller1           = \n" +
   "|goller2           = \n" +
   "|stadyum           = [[" + teams.Where(x => x.ShortName == home).FirstOrDefault().Stadium + "]]\n" +
   "|yer               = " + teams.Where(x => x.ShortName == home).FirstOrDefault().City + "\n" +
   "|seyirci           = \n" +
   "|hakem             = \n" +
   "|yardımcıhakemler  = \n" +
   "|dördüncühakem     = \n" +
   "|bg                = {{{2|B}}}\n" +
   "}}\n";
                if(i%9==0)
                    richTextBox1.AppendText($"<!-- {((i / 9) + 1)}. Hafta -->\n");
                richTextBox1.AppendText(main+"\n");
                this.matches.Add(new Match() { Home = home, Away = away,Week= ((i / 9) + 1) });
            }
            //bye.ForEach(x =>
            //{
            //    this.matches.Add(new Match { Home = x.Team, Away = "BAY", Week = x.Week });
            //});
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter("Select * from [Kırmızı$]", connection);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Team team = new Team();
                team.ShortName = dataTable.Rows[i][0].ToString();
                team.FullName = dataTable.Rows[i][2].ToString();
                team.Stadium = dataTable.Rows[i][3].ToString();
                team.City = dataTable.Rows[i][9].ToString();
                teams.Add(team);
            }
            //string[] byes = File.ReadAllLines("bay.txt");
            //byes = byes.Concat(byes).ToArray();
            //for (int i = 0; i < byes.Length; i++)
            //{
            //    string team = byes[i];
            //    bye.Add(new Bye() { Team = team, Week =(i+1) });
            //}

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string title = "| res1=";
            string title2 = "| res2=";
            for (int i = 0; i < teams.Count; i++)
            {
                richTextBox2.Clear();
                richTextBox3.Clear();
                richTextBox4.Clear();

                var matchesOfTeam=matches.Where(x=>x.Home==teams[i].ShortName||x.Away==teams[i].ShortName).OrderBy(x=>x.Week).ToList();
                richTextBox2.AppendText(title);
                richTextBox3.AppendText(title2);
                foreach (Match match in matchesOfTeam)
                {
                    
                    if (match.Home == teams[i].ShortName) //&& match.Away != "BAY"
					{
                        richTextBox2.AppendText(" E/");
                        richTextBox3.AppendText("  /");

                    }
                    else if (match.Away == teams[i].ShortName)//&& match.Home != "BAY"
					{
                        richTextBox2.AppendText(" D/");
                        richTextBox3.AppendText("  /");

                    }
                    else
                    {
                        richTextBox2.AppendText("  /");
                        richTextBox3.AppendText(" Y/");

                    }
                    if (match.Week == 1)
                        richTextBox4.AppendText("=== İlk devre ===\n");
                    if (match.Week == 20)
                        richTextBox4.AppendText("=== İkinci devre ===\n");

                    if (match.Away != "BAY")
                        richTextBox4.AppendText($"{{{{2024-25 2. Lig maçları|{match.Home}-{match.Away}|}}}}\n");
                    else
                        richTextBox4.AppendText($"{{{{BAY|{match.Week}}}}}\n");

                    File.WriteAllText("res1" + teams[i].ShortName+".txt", richTextBox2.Text);
                    File.WriteAllText("res2" + teams[i].ShortName+".txt", richTextBox3.Text);
                    File.WriteAllText("matches" + teams[i].ShortName+".txt", richTextBox4.Text);
                }




            }
        


        }

        private void button3_Click(object sender, EventArgs e)
        { int matchCount=0;
            var matches = this.matches.OrderBy(x => x.Week);
            foreach (Match item in matches)
            {

                if(matchCount%9==0)
                    richTextBox5.AppendText($"=== {item.Week}. Hafta=== \n<section begin={item.Week}/>\n");
                if (item.Away != "BAY")
                    richTextBox5.AppendText($"{{{{2024-25 2. Lig maçları|{item.Home}-{item.Away}|{(matchCount % 2 == 0 ? "eeeeee" : "")}}}}}\n");
                else
                    richTextBox5.AppendText($"{{{{BAY|{item.Week}|{item.Home}}}}}");
                if (matchCount % 9 == 8)
                    richTextBox5.AppendText($"<section end={item.Week}/>\n");
                matchCount++;

            }
        }
    }
}

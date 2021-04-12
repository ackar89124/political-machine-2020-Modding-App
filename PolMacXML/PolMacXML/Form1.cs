using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace PolMacXML
{
    public partial class Form1 : Form
    {
        public IdeologyTree tree = new IdeologyTree();
        public List<GridSeg> gridsegs = new List<GridSeg>();
        public int currentIde = 0;
        public int currentIdeIsu = 0;
        public int currentIsu = 0;

        public Form1()
        {
            LoadEmptyData();
            DataSet dataSet = new DataSet();
            InitializeComponent();
            if (File.Exists("Path.txt"))
            {
                string[] path = File.ReadAllLines("Path.txt");
                textBox2.Text = path[0];
            }
            comboBox2.Items.AddRange(dataSet.IdI);
            comboBox3.Items.AddRange(dataSet.IsI);
            UpdateTree();
            UpdateIdeologies(0);
            UpdateIdeologiesIssues(0, 0);
            UpdateIssues(0);
            UpdateButtons();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SerializeDataSet("myXmFile.xml");
            if(!File.Exists(textBox2.Text + "\\" + textBox1.Text + ".xml"))
            {
                SerializeDataSet(textBox2.Text + "\\" + textBox1.Text + ".xml");
            }
            else
            {
                if(!IsFileLocked(new FileInfo(textBox2.Text + "\\" + textBox1.Text + ".xml")))
                {
                    SerializeDataSet(textBox2.Text + "\\" + textBox1.Text + ".xml");
                }
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            //LoadEmptyData();
            currentIde = 0;
            currentIsu = 0;
            tree = new IdeologyTree();
            ReadXML(textBox2.Text + "\\" + textBox1.Text + ".xml");
            UpdateTree();
            if (tree.ideologies.Count == 0)
            {
                LoadEmptyDataD();
            }
            if (tree.issues.Count == 0)
            {
                LoadEmptyDataU();
            }
            UpdateIdeologies(0);
            UpdateIdeologiesIssues(0, 0);
            UpdateIssues(0);
            UpdateButtons();
            //tree.InternalName = tree.InternalName;
            //SerializeDataSet(textBox2.Text + textBox1.Text);
            //label1.Text = tree.ideologies[0].ScreenPlacement[0];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] path = { textBox2.Text };
            System.IO.File.WriteAllLines("Path.txt", path);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            currentIde--;
            UpdateIdeologies(currentIde);
            UpdateIdeologiesIssues(currentIde, 0);
            UpdateButtons();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            currentIde++;
            UpdateIdeologies(currentIde);
            UpdateIdeologiesIssues(currentIde, 0);
            UpdateButtons();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            currentIsu--;
            UpdateButtons();
            UpdateIssues(currentIsu);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            currentIsu++;
            UpdateButtons();
            UpdateIssues(currentIsu);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveIdeologies(currentIde);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            SaveIssues(currentIsu);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveTree();
        }

        private void SerializeDataSet(string filename)
        {
            XmlTextWriter textWriter = new XmlTextWriter(filename, null);

            // Opens the document  
            textWriter.WriteStartDocument();

            textWriter.WriteStartElement("IdeologyTree");
            WriteInitialData(textWriter);

            textWriter.WriteStartElement("Ideologies");
            foreach (Ideology i in tree.ideologies)
            {
                WriteIdeologieData(textWriter, i);
            }
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("Issues");
            foreach (Issue i in tree.issues)
            {
                WriteIssueData(textWriter, i);
            }
            textWriter.WriteEndElement();
            
            textWriter.WriteEndDocument();
            textWriter.Close();
        }

        private void WriteInitialData(XmlTextWriter textWriter)
        {
            textWriter.WriteStartElement("InternalName", "");
            textWriter.WriteString(tree.InternalName);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("DisplayName", "");
            textWriter.WriteString(tree.DisplayName);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("IconImage", "");
            textWriter.WriteString(tree.IconImage);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Description", "");
            textWriter.WriteString(tree.Description);
            textWriter.WriteEndElement();
        }

        private void WriteIdeologieData(XmlTextWriter textWriter, Ideology i)
        {
            textWriter.WriteStartElement("Ideology");
            textWriter.WriteStartElement("InternalName", "");
            textWriter.WriteString(i.InternalName);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("DisplayNameFor", "");
            textWriter.WriteString(i.DisplayNameFor);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("IconImage", "");
            textWriter.WriteString(i.IconImage);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("PointsCost", "");
            textWriter.WriteString(i.PointsCost);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("CostIncreasePerPurchasedIssue", "");
            textWriter.WriteString(i.CostIncreasePerPurchasedIssue);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("ScreenPlacement");
            textWriter.WriteStartElement("XPosition", "");
            textWriter.WriteString(i.ScreenPlacement[0]);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("YPosition", "");
            textWriter.WriteString(i.ScreenPlacement[1]);
            textWriter.WriteEndElement();
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("IdeologyColor", "");
            textWriter.WriteString(i.IdeologyColor);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Description", "");
            textWriter.WriteString(i.Description);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Prerequisites");
            if(i.Prerequisites != "")
            {
                textWriter.WriteStartElement("Prerequisite", "");
                textWriter.WriteString(i.Prerequisites);
                textWriter.WriteEndElement();
            }
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("IssueEffects");
            foreach(IssueT t in i.IssueTs)
            {
                textWriter.WriteStartElement("IssueEffect");
                textWriter.WriteStartElement("IssueTag", "");
                textWriter.WriteString(t.IssueTag);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("NationalImportanceChange", "");
                textWriter.WriteString(t.NationalImportanceChange);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("DemNationalImportanceChange", "");
                textWriter.WriteString(t.DemNationalImportanceChange);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("IndyNationalImportanceChange", "");
                textWriter.WriteString(t.IndyNationalImportanceChange);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("RepNationalImportanceChange", "");
                textWriter.WriteString(t.RepNationalImportanceChange);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("IssueStanceModifierPurchaser", "");
                textWriter.WriteString(t.IssueStanceModifierPurchaser);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("EnthusiasmIncreasePurchaser", "");
                textWriter.WriteString(t.EnthusiasmIncreasePurchaser);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("EnthusiasmIncreaseOpponent", "");
                textWriter.WriteString(t.EnthusiasmIncreaseOpponent);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("AdEnthusiasmScale", "");
                textWriter.WriteString(t.AdEnthusiasmScale);
                textWriter.WriteEndElement();
                textWriter.WriteEndElement();
            }
            textWriter.WriteEndElement();

            textWriter.WriteEndElement();
        }

        private void WriteIssueData(XmlTextWriter textWriter, Issue i)
        {
            textWriter.WriteStartElement("Issue");
            textWriter.WriteStartElement("Tag", "");
            textWriter.WriteString(i.Tag);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Display", "");
            textWriter.WriteString(i.Display);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Description", "");
            textWriter.WriteString(i.Description);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Default_Party_Position ", "");
            textWriter.WriteAttributeString("PartyID", null, "PARTY_LEFT");
            textWriter.WriteString(i.Left_Party_Position);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Default_Party_Position ", "");
            textWriter.WriteAttributeString("PartyID", null, "PARTY_RIGHT");
            textWriter.WriteString(i.Right_Party_Position);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Default_Party_Position ", "");
            textWriter.WriteAttributeString("PartyID", null, "PARTY_INDEPENDENT");
            textWriter.WriteString(i.Indy_Party_Position);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Default_Party_Importance ", "");
            textWriter.WriteAttributeString("PartyID", null, "PARTY_LEFT");
            textWriter.WriteString(i.Left_Party_Importance);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Default_Party_Importance ", "");
            textWriter.WriteAttributeString("PartyID", null, "PARTY_RIGHT");
            textWriter.WriteString(i.Right_Party_Importance);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("Default_Party_Importance ", "");
            textWriter.WriteAttributeString("PartyID", null, "PARTY_INDEPENDENT");
            textWriter.WriteString(i.Indy_Party_Importance);
            textWriter.WriteEndElement();
            textWriter.WriteEndElement();
        }

        private void ReadXML(string filename)
        {
            XmlTextReader textReader = new XmlTextReader(filename);
            string f = "";
            string d = "";
            string a = "";
            int p = 0;
            int t = 0;
            bool preIde = true;
            bool preIsu = false;
            if (File.Exists(filename) && !IsFileLocked(new FileInfo(filename)))
            {

                while (textReader.Read())
                {
                    switch (textReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            f = textReader.Name;
                            if (f == "Ideologies")
                            {
                                preIde = false;
                                preIsu = true;
                            }
                            else if (f == "Issues")
                            {
                                preIde = false;
                                preIsu = false;
                            }
                            else if (f == "Ideology")
                            {
                                if (tree.ideologies.Count <= p)
                                {
                                    tree.ideologies.Add(new Ideology());
                                }
                            }
                            else if (f == "IssueEffect")
                            {
                                if (tree.ideologies[p].IssueTs.Count <= p)
                                {
                                    tree.ideologies[p].IssueTs.Add(new IssueT());
                                }
                            }
                            else if (f == "Issue")
                            {
                                if (tree.issues.Count <= p)
                                {
                                    tree.issues.Add(new Issue());
                                }
                            }
                            if (textReader.GetAttribute("PartyID") != null)
                            {
                                a = textReader.GetAttribute("PartyID");
                            }
                            break;
                        case XmlNodeType.Text:
                            d = textReader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            if (f == textReader.Name)
                            {
                                if (preIde)
                                {
                                    a = "pre";
                                }
                                else if (preIsu)
                                {
                                    a = "pos";
                                }
                                DataTransfer(f, d, a, p, t);
                            }
                            if (textReader.Name == "Ideology" || textReader.Name == "Issue")
                            {
                                p++;
                                t = 0;
                            }
                            if (textReader.Name == "IssueEffect")
                            {
                                t++;
                            }
                            else if (textReader.Name == "Ideologies")
                            {
                                p = 0;
                            }
                            f = "";
                            d = "";
                            a = "";
                            break;
                    }
                }
                textReader.Close();
            }
            else
            {
                textBox1.Text = "File Not Found";
            }
        }

        private void DataTransfer(string f, string d, string a, int p, int t)
        {
            switch (f)
            {
                case "InternalName":
                    if (a == "pre")
                    {
                        tree.InternalName = d;
                    }
                    else
                    {
                        tree.ideologies[p].InternalName = d;
                    }
                    break;
                case "DisplayName":
                    tree.DisplayName = d;
                    break;
                case "IconImage":
                    if (a == "pre")
                    {
                        tree.IconImage = d;
                    }
                    else
                    {
                        tree.ideologies[p].IconImage = d;
                    }
                    break;
                case "Description":
                    if (a == "pre")
                    {
                        tree.Description = d;
                    }
                    else if(a == "pos")
                    {
                        tree.ideologies[p].Description = d;
                    }
                    else
                    {
                        tree.issues[p].Description = d;
                    }
                    break;
                case "DisplayNameFor":
                    tree.ideologies[p].DisplayNameFor = d;
                    break;
                case "PointsCost":
                    tree.ideologies[p].PointsCost = d;
                    break;
                case "CostIncreasePerPurchasedIssue":
                    tree.ideologies[p].CostIncreasePerPurchasedIssue = d;
                    break;
                case "XPosition":
                    tree.ideologies[p].ScreenPlacement[0] = d;
                    break;
                case "YPosition":
                    tree.ideologies[p].ScreenPlacement[1] = d;
                    break;
                case "IdeologyColor":
                    tree.ideologies[p].IdeologyColor = d;
                    break;
                case "Prerequisite":
                    tree.ideologies[p].Prerequisites = d;
                    break;
                case "IssueTag":
                    tree.ideologies[p].IssueTs[t].IssueTag = d;
                    break;
                case "NationalImportanceChange":
                    tree.ideologies[p].IssueTs[t].NationalImportanceChange = d;
                    break;
                case "DemNationalImportanceChange":
                    tree.ideologies[p].IssueTs[t].DemNationalImportanceChange = d;
                    break;
                case "IndyNationalImportanceChange":
                    tree.ideologies[p].IssueTs[t].IndyNationalImportanceChange = d;
                    break;
                case "RepNationalImportanceChange":
                    tree.ideologies[p].IssueTs[t].RepNationalImportanceChange = d;
                    break;
                case "IssueStanceModifierPurchaser":
                    tree.ideologies[p].IssueTs[t].IssueStanceModifierPurchaser = d;
                    break;
                case "EnthusiasmIncreasePurchaser":
                    tree.ideologies[p].IssueTs[t].EnthusiasmIncreasePurchaser = d;
                    break;
                case "EnthusiasmIncreaseOpponent":
                    tree.ideologies[p].IssueTs[t].EnthusiasmIncreaseOpponent = d;
                    break;
                case "AdEnthusiasmScale":
                    tree.ideologies[p].IssueTs[t].AdEnthusiasmScale = d;
                    break;
                case "Tag":
                    tree.issues[p].Tag = d;
                    break;
                case "Display":
                    tree.issues[p].Display = d;
                    break;
                case "Default_Party_Position":
                    if (a == "PARTY_LEFT")
                    {
                        tree.issues[p].Left_Party_Position = d;
                    }
                    else if (a == "PARTY_RIGHT")
                    {
                        tree.issues[p].Right_Party_Position = d;
                    }
                    else if (a == "PARTY_INDEPENDENT" || a == "PARTY_INDY")
                    {
                        tree.issues[p].Indy_Party_Position = d;
                    }
                    break;
                case "Default_Party_Importance":
                    if (a == "PARTY_LEFT")
                    {
                        tree.issues[p].Left_Party_Importance = d;
                    }
                    else if (a == "PARTY_RIGHT")
                    {
                        tree.issues[p].Right_Party_Importance = d;
                    }
                    else if (a == "PARTY_INDEPENDENT" || a == "PARTY_INDY")
                    {
                        tree.issues[p].Indy_Party_Importance = d;
                    }
                    break;
            }
        }

        private void LoadEmptyData()
        {
            LoadEmptyDataD();
            LoadEmptyDataU();
            LoadEmptyDataT();
        }
        private void LoadEmptyDataT()
        {
            tree.InternalName = "DEFAULT";
            tree.DisplayName = "Default";
            tree.IconImage = "IdeologyIcon_USAFlag";
            tree.Description = "Default Description for Party";
        }
        private void LoadEmptyDataU()
        {
            Issue isu = new Issue
            {
                Tag = "TAG",
                Display = "Issue",
                Description = "Issue Description",
                Left_Party_Position = "-1",
                Right_Party_Position = "-1",
                Indy_Party_Position = "-1",
                Left_Party_Importance = "1",
                Right_Party_Importance = "1",
                Indy_Party_Importance = "1"
            };
            tree.issues.Add(isu);
        }
        private void LoadEmptyDataD()
        {
            Ideology ide = new Ideology
            {
                InternalName = "IDEOLOGY",
                DisplayNameFor = "Ideology",
                IconImage = "IssueIcon_GovernmentBuilding",
                PointsCost = "1",
                CostIncreasePerPurchasedIssue = "1",
                ScreenPlacement = new string[] { "1", "1" },
                IdeologyColor = "Purple",
                Prerequisites = "",
                Description = ""
            };
            tree.ideologies.Add(ide);
            LoadEmptyDataDI(ide);
        }
        private void LoadEmptyDataDI(Ideology i)
        {
            IssueT idi = new IssueT
            {
                IssueTag = "TAG",
                NationalImportanceChange = "1",
                DemNationalImportanceChange = "1",
                IndyNationalImportanceChange = "1",
                RepNationalImportanceChange = "1",
                IssueStanceModifierPurchaser = "10",
                EnthusiasmIncreasePurchaser = "1",
                EnthusiasmIncreaseOpponent = "1",
                AdEnthusiasmScale = "1"
            };
            tree.ideologies.Find(x => x.InternalName == i.InternalName).IssueTs.Add(idi);
        }

        private void UpdateTree()
        {
            textBox3.Text = tree.InternalName + "";
            textBox4.Text = tree.DisplayName + "";
            //textBox5.Text = tree.IconImage + "";
            comboBox2.SelectedIndex = comboBox2.FindStringExact(tree.IconImage);
            textBox6.Text = tree.Description + "";
        }
        private void UpdateIdeologies(int i)
        {
            textBox23.Text = tree.ideologies[i].InternalName + "";
            textBox22.Text = tree.ideologies[i].DisplayNameFor + "";
            //textBox21.Text = tree.ideologies[i].IconImage + "";
            comboBox3.SelectedIndex = comboBox3.FindStringExact(tree.ideologies[i].IconImage);
            textBox20.Text = tree.ideologies[i].PointsCost + "";
            textBox19.Text = tree.ideologies[i].CostIncreasePerPurchasedIssue + "";
            textBox18.Text = tree.ideologies[i].ScreenPlacement[0] + "";
            textBox24.Text = tree.ideologies[i].ScreenPlacement[1] + "";
            //domainUpDown1 = tree.ideologies[i].IdeologyColor + "";
            comboBox1.SelectedIndex = comboBox1.FindStringExact(tree.ideologies[i].IdeologyColor);
            //comboBox1.BackColor = Color.FromName(tree.ideologies[i].IdeologyColor);
            textBox16.Text = tree.ideologies[i].Prerequisites + "";
            textBox5.Text = tree.ideologies[i].Description + "";
            //currentIdeIsu = 0;
        }
        private void UpdateIdeologiesIssues(int i, int s)
        {
            textBox7.Text = tree.ideologies[i].IssueTs[s].IssueTag + "";
            textBox8.Text = tree.ideologies[i].IssueTs[s].NationalImportanceChange + "";
            textBox9.Text = tree.ideologies[i].IssueTs[s].DemNationalImportanceChange + "";
            textBox10.Text = tree.ideologies[i].IssueTs[s].RepNationalImportanceChange + "";
            textBox11.Text = tree.ideologies[i].IssueTs[s].IndyNationalImportanceChange + "";
            textBox12.Text = tree.ideologies[i].IssueTs[s].IssueStanceModifierPurchaser + "";
            textBox13.Text = tree.ideologies[i].IssueTs[s].EnthusiasmIncreasePurchaser + "";
            textBox14.Text = tree.ideologies[i].IssueTs[s].EnthusiasmIncreaseOpponent + "";
            textBox15.Text = tree.ideologies[i].IssueTs[s].AdEnthusiasmScale + "";
        }
        private void UpdateIssues(int i)
        {
            textBox28.Text = tree.issues[i].Tag + "";
            textBox27.Text = tree.issues[i].Display + "";
            textBox25.Text = tree.issues[i].Description + "";
            textBox26.Text = tree.issues[i].Left_Party_Position + "";
            textBox31.Text = tree.issues[i].Right_Party_Position + "";
            textBox33.Text = tree.issues[i].Indy_Party_Position + "";
            textBox29.Text = tree.issues[i].Left_Party_Importance + "";
            textBox30.Text = tree.issues[i].Right_Party_Importance + "";
            textBox32.Text = tree.issues[i].Indy_Party_Importance + "";
        }
        private void SaveTree()
        {
            tree.InternalName = textBox3.Text;
            tree.DisplayName = textBox4.Text;
            //tree.IconImage = textBox5.Text;
            tree.IconImage = comboBox2.SelectedItem.ToString();
            tree.Description = textBox6.Text;
        }
        private void SaveIdeologies(int i)
        {
            tree.ideologies[i].InternalName = textBox23.Text;
            tree.ideologies[i].DisplayNameFor = textBox22.Text;
            //tree.ideologies[i].IconImage = textBox21.Text;
            tree.ideologies[i].IconImage = comboBox3.SelectedItem.ToString();
            tree.ideologies[i].PointsCost = textBox20.Text;
            tree.ideologies[i].CostIncreasePerPurchasedIssue = textBox19.Text;
            tree.ideologies[i].ScreenPlacement[0] = textBox18.Text;
            tree.ideologies[i].ScreenPlacement[1] = textBox24.Text;
            //domainUpDown1 = tree.ideologies[i].IdeologyColor + "";
            tree.ideologies[i].IdeologyColor = comboBox1.SelectedItem.ToString();
            tree.ideologies[i].Prerequisites = textBox16.Text;
            tree.ideologies[i].Description = textBox5.Text;
        }
        private void SaveIdeologiesIssues(int i, int s)
        {
            tree.ideologies[i].IssueTs[s].IssueTag = textBox7.Text;
            tree.ideologies[i].IssueTs[s].NationalImportanceChange = textBox8.Text;
            tree.ideologies[i].IssueTs[s].DemNationalImportanceChange = textBox9.Text;
            tree.ideologies[i].IssueTs[s].RepNationalImportanceChange = textBox10.Text;
            tree.ideologies[i].IssueTs[s].IndyNationalImportanceChange = textBox11.Text;
            tree.ideologies[i].IssueTs[s].IssueStanceModifierPurchaser = textBox12.Text;
            tree.ideologies[i].IssueTs[s].EnthusiasmIncreasePurchaser = textBox13.Text;
            tree.ideologies[i].IssueTs[s].EnthusiasmIncreaseOpponent = textBox14.Text;
            tree.ideologies[i].IssueTs[s].AdEnthusiasmScale = textBox15.Text;
        }
        private void SaveIssues(int i)
        {
           tree.issues[i].Tag = textBox28.Text;
           tree.issues[i].Display = textBox27.Text;
           tree.issues[i].Description = textBox25.Text;
           tree.issues[i].Left_Party_Position = textBox26.Text;
           tree.issues[i].Right_Party_Position = textBox31.Text;
           tree.issues[i].Indy_Party_Position = textBox33.Text;
           tree.issues[i].Left_Party_Importance = textBox29.Text;
           tree.issues[i].Right_Party_Importance = textBox30.Text;
           tree.issues[i].Indy_Party_Importance = textBox32.Text;
        }

        private void UpdateButtons()
        {
            if (currentIde <= 0)
            {
                currentIde = 0;
                button7.Enabled = false;
            }
            if (currentIde != tree.ideologies.Count - 1)
            {
                button8.Enabled = true;
            }
            if (currentIde >= tree.ideologies.Count - 1)
            {
                currentIde = tree.ideologies.Count - 1;
                button8.Enabled = false;
            }
            if (currentIde != 0)
            {
                button7.Enabled = true;
            }

            if (currentIsu <= 0)
            {
                currentIsu = 0;
                button11.Enabled = false;
            }
            if (currentIsu != tree.issues.Count - 1)
            {
                button10.Enabled = true;
            }
            if (currentIsu >= tree.issues.Count - 1)
            {
                currentIsu = tree.issues.Count - 1;
                button10.Enabled = false;
            }
            if (currentIsu != 0)
            {
                button11.Enabled = true;
            }

            if (currentIdeIsu <= 0)
            {
                currentIdeIsu = 0;
                button18.Enabled = false;
            }
            if (currentIdeIsu != tree.ideologies[currentIde].IssueTs.Count - 1)
            {
                button17.Enabled = true;
            }
            if (currentIdeIsu >= tree.ideologies[currentIde].IssueTs.Count - 1)
            {
                currentIdeIsu = tree.ideologies[currentIde].IssueTs.Count - 1;
                button17.Enabled = false;
            }
            if (currentIdeIsu != 0)
            {
                button18.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //NewIde
            tree.ideologies.Add(new Ideology());
            tree.ideologies[tree.ideologies.Count - 1].IssueTs.Add(new IssueT());
            UpdateIdeologies(currentIde);
            UpdateIdeologiesIssues(currentIde, 0);
            UpdateButtons();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //DeleteIde
            if (tree.ideologies.Count > 1)
            {
                tree.ideologies.RemoveAt(currentIde);
                if (currentIde > tree.ideologies.Count - 1)
                {
                    currentIde--;
                }
                UpdateIdeologies(currentIde);
                UpdateIdeologiesIssues(currentIde, 0);
                UpdateButtons();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //NewIsu
            tree.issues.Add(new Issue());
            UpdateIssues(currentIsu);
            UpdateButtons();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //DeleteIsu
            tree.issues.RemoveAt(currentIsu);
            if (currentIsu > tree.issues.Count - 1)
            {
                currentIsu--;
            }
            UpdateIssues(currentIsu);
            UpdateButtons();
        }

        // https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                Console.WriteLine("IOException");
                return true;
            }

            //file is not locked
            return false;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Form grid = new Form();
            grid.Size = new System.Drawing.Size(1000, 700);
            grid.Show();

            int z = 0;
            for (int x = 0; x < 9; x++)
            {
                if (x % 2 == 1) { z = 50; }
                else { z = 0; }
                for (int y = 0; y < 6; y++)
                {
                    bool found = false;
                    GridSeg seg = new GridSeg();
                    ComboBox combo = new ComboBox();
                    ComboBox comboC = new ComboBox();
                    ComboBox comboI = new ComboBox();
                    DataSet dataSet = new DataSet();
                    comboC.Items.AddRange(dataSet.Col);
                    comboI.Items.AddRange(dataSet.IsI2);
                    foreach (Ideology i in tree.ideologies)
                    {
                        combo.Items.Add(i.InternalName);
                        if (i.ScreenPlacement[0] == x.ToString() && i.ScreenPlacement[1] == y.ToString())
                        {
                            combo.SelectedIndex = combo.Items.Count - 1;
                            comboC.SelectedItem = i.IdeologyColor;
                            comboI.SelectedItem = i.IconImage.Remove(0,10);
                            found = true;
                        }
                    }
                    seg.cor = combo;
                    seg.col = comboC;
                    seg.img = comboI;
                    gridsegs.Add(seg);

                    combo.Items.Add("Empty");
                    combo.DropDownStyle = ComboBoxStyle.DropDownList;
                    if (!found) { combo.SelectedIndex = combo.Items.Count - 1; }
                    combo.Size = new System.Drawing.Size(90, 120);
                    combo.Name = x + " " + y;
                    combo.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                    combo.Location = new System.Drawing.Point(30 + (104 * x), 18 + z + (98 * y));
                    grid.Controls.Add(combo);

                    comboC.DropDownStyle = ComboBoxStyle.DropDownList;
                    if (!found) { comboC.SelectedIndex = comboC.Items.Count - 1; }
                    comboC.Size = new System.Drawing.Size(90, 120);
                    //comboC.Font = new Font("Microsoft Sans Serif", 7, FontStyle.Bold);
                    comboC.Location = new System.Drawing.Point(30 + (104 * x), 60 + z + (98 * y));
                    grid.Controls.Add(comboC);

                    comboI.DropDownStyle = ComboBoxStyle.DropDownList;
                    if (!found) { comboI.SelectedIndex = comboI.Items.Count - 1; }
                    comboI.Size = new System.Drawing.Size(90, 1200);
                    //comboC.Font = new Font("Microsoft Sans Serif", 7, FontStyle.Bold);
                    comboI.Location = new System.Drawing.Point(30 + (104 * x), 40 + z + (98 * y));
                    //comboI.RightToLeft = RightToLeft.Yes;
                    comboI.DropDownWidth = 130;
                    grid.Controls.Add(comboI);

                    if (z != 0 && y == 4) { y++; }
                }
            }

            PictureBox picture = new PictureBox();
            picture.Image = new Bitmap(PolMacXML.Properties.Resources._1590192912_PM_2020_Grid2);
            picture.SendToBack();
            picture.BackgroundImageLayout = ImageLayout.Stretch;
            picture.Size = new System.Drawing.Size(1000, 600);
            grid.Controls.Add(picture);

            Button button = new Button();
            button.Location = new Point(450, 620);
            button.Text = "Save Grid";
            button.Click += new EventHandler(Button_Click);
            grid.Controls.Add(button);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            foreach (GridSeg g in gridsegs)
            {
                if(g.cor.SelectedIndex != g.cor.Items.Count - 1)
                {
                    foreach(Ideology i in tree.ideologies)
                    {
                        if (i.InternalName == g.cor.SelectedItem.ToString())
                        {
                            i.ScreenPlacement = new string[] { g.cor.Name[0].ToString(), g.cor.Name[2].ToString() };
                            i.IdeologyColor = g.col.SelectedItem.ToString();
                            i.IconImage = "IssueIcon_" + g.img.SelectedItem.ToString();
                        }
                    }
                }
            }
            UpdateIdeologies(currentIde);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://discord.gg/9bvgeTpj6k");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            currentIdeIsu--;
            UpdateButtons();
            UpdateIdeologiesIssues(currentIde, currentIdeIsu);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            currentIdeIsu++;
            UpdateButtons();
            UpdateIdeologiesIssues(currentIde, currentIdeIsu);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            // Save Ids
            SaveIdeologiesIssues(currentIde, currentIdeIsu);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            // New Ids
            tree.ideologies[currentIde].IssueTs.Add(new IssueT());
            currentIdeIsu++;
            UpdateIdeologiesIssues(currentIde, currentIdeIsu);
            UpdateButtons();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // Delete Ids
            if (tree.ideologies[currentIde].IssueTs.Count > 1)
            {
                tree.ideologies[currentIde].IssueTs.RemoveAt(currentIdeIsu);
                if (currentIdeIsu > tree.ideologies[currentIde].IssueTs.Count - 1)
                {
                    currentIdeIsu--;
                }
                UpdateIdeologiesIssues(currentIde, currentIdeIsu);
                UpdateButtons();
            }
        }
    }

    public class IdeologyTree
    {
        public List<Ideology> ideologies = new List<Ideology>();
        public List<Issue> issues = new List<Issue>();
        public string InternalName = "";
        public string DisplayName = "";
        public string IconImage = "";
        public string Description = "";
        
    }

    public class Ideology
    {
        public string InternalName = "DEFAULT";
        public string DisplayNameFor = "";
        public string IconImage = "";
        public string PointsCost = "";
        public string CostIncreasePerPurchasedIssue = "";
        public string[] ScreenPlacement = { "1", "1" };
        public string IdeologyColor = "Yellow";
        public string Prerequisites = "";
        public string Description = "";
        public List<IssueT> IssueTs = new List<IssueT>();
        /*public List<IssueT> IssueTs = new List<IssueT>
        {
            new IssueT()
        };*/
    }
    public class IssueT
    {
        public string IssueTag = "TAG";
        public string NationalImportanceChange = "0";
        public string DemNationalImportanceChange = "0";
        public string IndyNationalImportanceChange = "0";
        public string RepNationalImportanceChange = "0";
        public string IssueStanceModifierPurchaser = "0";
        public string EnthusiasmIncreasePurchaser = "0";
        public string EnthusiasmIncreaseOpponent = "0";
        public string AdEnthusiasmScale = "0";
    }

    public class Issue
    {
        public string Tag = "";
        public string Display = "";
        public string Description = "";
        public string Left_Party_Position = "";
        public string Right_Party_Position = "";
        public string Indy_Party_Position = "";
        public string Left_Party_Importance = "";
        public string Right_Party_Importance = "";
        public string Indy_Party_Importance = "";

    }

    public class DataSet
    {
        public string[] IdI = { "IdeologyIcon_AI", "IdeologyIcon_Dem",
            "IdeologyIcon_Meteor", "IdeologyIcon_PeaceDove", "IdeologyIcon_People",
            "IdeologyIcon_Rep", "IdeologyIcon_Star", "IdeologyIcon_USAFlag" };
        public string[] IsI = { "IssueIcon_AI", "IssueIcon_Alien",
            "IssueIcon_AmericanFlag", "IssueIcon_Baby", "IssueIcon_BadmouthOpponents",
            "IssueIcon_BadVideoGame", "IssueIcon_BorderWall", "IssueIcon_Car",
            "IssueIcon_BringTroopsHome", "IssueIcon_CatsAndDogs", "IssueIcon_Christianity",
            "IssueIcon_Coffee", "IssueIcon_CommonCore", "IssueIcon_Communisim",
            "IssueIcon_CostOfDrugs", "IssueIcon_CrackedBell", "IssueIcon_CryptoCurrency",
            "IssueIcon_Diversity", "IssueIcon_Donkey", "IssueIcon_DontLookAtMe",
            "IssueIcon_EducationFunding", "IssueIcon_Elephant", "IssueIcon_Environment",
            "IssueIcon_Farming", "IssueIcon_EvilRobot", "IssueIcon_Fire", "IssueIcon_Frog",
            "IssueIcon_FrogHalo", "IssueIcon_FrogKing", "IssueIcon_GayMarriage",
            "IssueIcon_GovernmentBuilding", "IssueIcon_GreenJobs", "IssueIcon_GreenNewDeal",
            "IssueIcon_HappyFace", "IssueIcon_Gun", "IssueIcon_Hardhat", "IssueIcon_Illuminati",
            "IssueIcon_HealthcareHandout", "IssueIcon_IncomeInequality", "IssueIcon_Infrastructure",
            "IssueIcon_InvestInUSA", "IssueIcon_Justice", "IssueIcon_KillTheFish",
            "IssueIcon_LeaveEarth", "IssueIcon_Marijuana", "IssueIcon_Mayo", "IssueIcon_Meteor",
            "IssueIcon_Moon", "IssueIcon_MoneyHandout", "IssueIcon_MusicNote", "IssueIcon_NoSugar",
            "IssueIcon_NuclearEnergy", "IssueIcon_PaidFamilyLeave", "IssueIcon_PaveRoads",
            "IssueIcon_PeaceDove", "IssueIcon_PeaceSign", "IssueIcon_Pickaxe", "IssueIcon_Plane",
            "IssueIcon_Rainbow", "IssueIcon_Regulation", "IssueIcon_RenewableEnergy",
            "IssueIcon_SadFace", "IssueIcon_SchoolFunding", "IssueIcon_Shackles", "IssueIcon_Skulls",
            "IssueIcon_SmokeStacks", "IssueIcon_SolarEnergy", "IssueIcon_Syringe",
            "IssueIcon_TheConstitution", "IssueIcon_Torture", "IssueIcon_TraditionalMarriage",
            "IssueIcon_Transgender", "IssueIcon_Truckers", "IssueIcon_USAMap", "IssueIcon_Virus",
            "IssueIcon_War", "IssueIcon_Whip", "IssueIcon_WoodShip" };
        public string[] IsI2 = { "AI", "Alien",
            "AmericanFlag", "Baby", "BadmouthOpponents",
            "BadVideoGame", "BorderWall", "Car",
            "BringTroopsHome", "CatsAndDogs", "Christianity",
            "Coffee", "CommonCore", "Communism",
            "CostOfDrugs", "CrackedBell", "CryptoCurrency",
            "Diversity", "Donkey", "DontLookAtMe",
            "EducationFunding", "Elephant", "Environment",
            "Farming", "EvilRobot", "Fire", "Frog",
            "FrogHalo", "FrogKing", "GayMarriage",
            "GovernmentBuilding", "GreenJobs", "GreenNewDeal",
            "HappyFace", "Gun", "Hardhat", "Illuminati",
            "HealthcareHandout", "IncomeInequality", "Infrastructure",
            "InvestInUSA", "Justice", "KillTheFish",
            "LeaveEarth", "Marijuana", "Mayo", "Meteor",
            "Moon", "MoneyHandout", "MusicNote", "NoSugar",
            "NuclearEnergy", "PaidFamilyLeave", "PaveRoads",
            "PeaceDove", "PeaceSign", "Pickaxe", "Plane",
            "Rainbow", "Regulation", "RenewableEnergy",
            "SadFace", "SchoolFunding", "Shackles", "Skulls",
            "SmokeStacks", "SolarEnergy", "Syringe",
            "TheConstitution", "Torture", "TraditionalMarriage",
            "Transgender", "Truckers", "USAMap", "Virus",
            "War", "Whip", "WoodShip" };
        public string[] Col = { "Red", "Green", "Blue", "Purple", "Yellow" };
    }

    public class GridSeg
    {
        public ComboBox cor;
        public ComboBox col;
        public ComboBox img;
    }
}

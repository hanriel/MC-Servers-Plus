using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace MCSP
{
    public partial class Form1 : Form
    {
        XmlDocument xDoc = new XmlDocument();
        public Form1()
        {
            InitializeComponent();
            if (!File.Exists("servers.xml"))
            {
                XmlTextWriter textWritter = new XmlTextWriter("servers.xml", null);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("servers");
                textWritter.WriteEndElement();
                textWritter.Close();
            }
            xDoc.Load("servers.xml");
            for (int i = 0; i < 6; i++){ColLoad(i);}

            ItemsLoad();
            timer1.Enabled = true;
        }

        Inform Ping(string adr, ushort prt)
        {
            Inform inf = new Inform();
            MineStat ms = new MineStat(adr, prt);
            if (ms.IsServerUp())
            {
                inf.ver = ms.GetVersion();
                inf.current = ms.GetCurrentPlayers();
                inf.max = ms.GetMaximumPlayers();
                inf.ping = ms.GetPing();
                inf.online = true;
                return inf;
            }
            else
            {
                inf.online = false;
                return inf;
            }
        }
        public class Inform
        {
            public string ver, max, current, ping;
            public bool online;
        }

        void ColLoad(int i)
        {
            ColumnHeader colHead;
            colHead = new ColumnHeader();
            switch (i)
            {
                case 0:
                    colHead.Text = "IP";
                    break;
                case 1:
                    colHead.Text = "Port";
                    break;
                case 2:
                    colHead.Text = "Ver";
                    break;
                case 3:
                    colHead.Text = "Online";
                    break;
                case 4:
                    colHead.Text = "Status";
                    break;
                case 5:
                    colHead.Text = "Ping";
                    break;
            }
            listView1.Columns.Add(colHead);
        }

        void ItemsLoad()
        {
            listView1.Items.Clear();
            ListViewItem lvi;
            ListViewItem.ListViewSubItem lvsi;
            listView1.BeginUpdate();
            xDoc.Load("servers.xml");
            XmlNodeList elemList = xDoc.GetElementsByTagName("element");

            for (int i = 0; i < elemList.Count; i++)
            {
                lvi = new ListViewItem();
                lvi.Text = elemList[i].Attributes["ip"].Value;
                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = elemList[i].Attributes["port"].Value;
                lvi.SubItems.Add(lvsi);
                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = "Loading...";
                lvi.SubItems.Add(lvsi);
                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = "Loading...";
                lvi.SubItems.Add(lvsi);
                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = "Loading...";
                lvi.SubItems.Add(lvsi);
                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = "Loading...";
                lvi.SubItems.Add(lvsi);
                listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();
            listView1.View = View.Details;
        }

        private void timer1_Tick2(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            for (int s = 0; s < listView1.Items.Count; s++)
            {
                Application.DoEvents();
                Inform tmp = Ping(listView1.Items[s].SubItems[0].Text, ushort.Parse(listView1.Items[s].SubItems[1].Text));
                listView1.Items[s].SubItems[4].Text = tmp.online.ToString();
                listView1.Items[s].SubItems[3].Text = tmp.current + "/" + tmp.max;
                listView1.Items[s].SubItems[2].Text = tmp.ver;
                listView1.Items[s].SubItems[5].Text = tmp.ping;
            }
            timer1.Enabled = true;
        }
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                xDoc.Load("servers.xml");
                XmlNode element = xDoc.CreateElement("element");
                xDoc.DocumentElement.AppendChild(element);
                XmlAttribute attribute = xDoc.CreateAttribute("ip");
                XmlAttribute attribute2 = xDoc.CreateAttribute("port");
                attribute2.Value = numericUpDown1.Value.ToString();
                attribute.Value = textBox1.Text;
                element.Attributes.Append(attribute);
                element.Attributes.Append(attribute2);
                xDoc.Save("servers.xml");
                ItemsLoad();
            }
            else MessageBox.Show("Заполните поля!");

        }
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xDoc.Load("servers.xml");
            if (listView1.SelectedItems.Count > 0)
            {
                XmlNode xmlNode = xDoc.GetElementsByTagName("element")[listView1.Items.IndexOf(listView1.SelectedItems[0])];
                xmlNode.ParentNode.RemoveChild(xmlNode);
            }
            xDoc.Save("servers.xml");
            ItemsLoad();
        }
    }
}

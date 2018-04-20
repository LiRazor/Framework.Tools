using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Framework.Common.Interface;
using Framework.Common.FCache;
using System.Collections;
using System.Reflection;
using Framework.Common.Helper;
using Framework.Common.Model;

namespace WindowsFormsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool _start = true;
        LocalCache localCache = new LocalCache();


        delegate void SpeakGreetingDelegate(string name);

        private static void EnglishGreeting(string name)
        {
            MessageBox.Show("Hello," + name);
        }

        private static void ChineseGreeting(string name)
        {
            MessageBox.Show("早上好," + name);
        }

        private static void Greeting(string name, SpeakGreetingDelegate speakGreeting)
        {
            speakGreeting(name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpWebHelper httpWebHelper = new HttpWebHelper();
            ExceptionMsg exceptionMsg = new ExceptionMsg();
            try
            {
                string R_result = httpWebHelper.Get("http://baidu.com", Encoding.UTF8);
                MessageBox.Show(exceptionMsg.IP);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

            //localCache.Add("mykey", "123", 10);
            localCache.Add("mykey", "123");
            localCache.Set("11",DataBindingByList1());

            ThreadStart threadStart = new ThreadStart(refresh);
            Thread thread = new Thread(threadStart);
            thread.Start();

            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    SetText($"9999\r\n");
                }

            })
            {
                IsBackground = true
            }.Start();


            dynamic d = 1;
            int c = d + 1;
            //MessageBox.Show(c.ToString());

            //Greeting("Make", ChineseGreeting);


        }




        delegate void SetTextCallback(string text);
        public void SetText(string text)
        {
            if (this.textBox1.InvokeRequired & this.dataGridView1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                textBox1.AppendText(text);
                dataGridView1.DataSource = ListToDt(DataBindingByList2());
            }
        }

        //refresh就可以执行
        void refresh()
        {
            while (_start)
            {
                //if (DateTime.Now.Second > 28)
                //    _start = false;
                //在这里更新TextBox的内容
                string msg = localCache.Get("mykey") + "--" + DateTime.Now + "\r\n";
                SetText(msg);

                Thread.Sleep(2000);
            }
        }

        public DataTable ListToDt(IList list)
        {
            DataTable dt = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] properties = list[0].GetType().GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    dt.Columns.Add(info.Name, info.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList arrayList = new ArrayList();
                    foreach (PropertyInfo proInfo in properties)
                    {
                        arrayList.Add(proInfo.GetValue(list[i], null));
                    }
                    dt.LoadDataRow(arrayList.ToArray(), true);
                }
            }
            return dt;
        }


        public class PersonInfo
        {
            public string name { get; set; }
            public string id { get; set; }

            public PersonInfo(string name, string id)
            {
                this.name = name;
                this.id = id;
            }
        }

        /// <summary> 
        /// IList接口（包括一维数组，ArrayList等） 
        /// </summary> 
        /// <returns></returns> 
        private ArrayList DataBindingByList1()
        {
            ArrayList Al = new ArrayList();
            Al.Add(new PersonInfo("a", "-1"));
            Al.Add(new PersonInfo("b", "-2"));
            Al.Add(new PersonInfo("c", "-3"));
            return Al;
        }

        /// <summary> 
        /// IList接口（包括一维数组，ArrayList等） 
        /// </summary> 
        /// <returns></returns> 
        private ArrayList DataBindingByList2()
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new DictionaryEntry(i.ToString(), i.ToString() + "_List"));
            }
            return list;
        }


        /// <summary> 
        /// IListSource接口（DataTable、DataSet等） 
        /// </summary> 
        /// <returns></returns> 
        private DataTable DataBindingByDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("Name");
            DataColumn dc2 = new DataColumn("Value");

            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);

            for (int i = 1; i <= 10; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = i;
                dr[1] = i.ToString() + "_DataTable";
                dt.Rows.Add(dr);
            }

            return dt;
        }


        /// <summary> 
        /// IBindingListView接口（如BindingSource类） 
        /// </summary> 
        /// <returns></returns> 
        private BindingSource DataBindingByBindingSource()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0; i < 10; i++)
            {
                dic.Add(i.ToString(), i.ToString() + "_Dictionary");
            }
            return new BindingSource(dic, null);
        }

    }
}

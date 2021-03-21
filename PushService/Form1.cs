using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushService
{
    public partial class Form1 : Form
    {
        Toast backendThread;
        //0停止中 1正在运行
        int status = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hashtable properties = PropertiesUtil.Load("./cfg.txt");
            if (properties != null)
            {
                this.username.Text = (string)properties["username"];
                this.password.Text = (string)properties["password"];
            }
        }
        private Hashtable SaveProperties()
        {
            string file = @"./cfg.txt";
            Hashtable properties = new Hashtable
            {
                { "username", this.username.Text },
                { "password", this.password.Text }
            };
            PropertiesUtil.Save(file, properties);
            return properties;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveProperties();
            MessageBox.Show("保存成功！");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (status == 0)
            {
                this.button2.Text = "停止监听";
                status = 1;
                Hashtable properties = SaveProperties();
                backendThread = new Toast(properties);
                System.Drawing.Rectangle rec = Screen.GetWorkingArea(this);
                int h = rec.Height;
                int w = rec.Width;
                backendThread.Location = new Point(w - 500, h - 130);
                backendThread.Show();
            }else if (status == 1)
            {
                this.button2.Text = "启动监听";
                status = 0;
                backendThread.Close();
            }
            
        }
        public void BackendMethod()
        {
            while (true)
            {
                Console.WriteLine("running...");
                Thread.Sleep(1000);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

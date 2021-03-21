using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PushService
{
    public partial class Toast : Form
    {
        Thread thread;
        Hashtable properties;
        HttpListener listener;
        System.Timers.Timer timer = new System.Timers.Timer(5000);
        public Toast(Hashtable properties)
        {
            this.properties = properties;
            InitializeComponent();
        }

        private void Toast_Load(object sender, EventArgs e)
        {
            timer.Enabled = true;
            timer.AutoReset = false;
            thread = new Thread(startWebService);
            thread.IsBackground = true;
            thread.Start();
            showToast("PushService", "监听已启动", "待手机收到通知后，会自动显示");
        }
        //定时隐藏toast，支持打断操作
        private void touch()
        {
            timer.Stop();
            this.Show();
            timer.Elapsed += TimersTimer_Elapsed;
            timer.Start();
        }
        private void TimersTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.Hide();
            }));
            
        }
        private void showToast(string name,string title,string content)
        {
            this.label1.Text = title + " · "+ name;
            this.label2.Text = content;
            touch();
        }

        private void startWebService()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:1500/req/");
            listener.Start();
            
            while (true)
            {
                Console.WriteLine("\n\n等待客户连接中。。。。");
                //等待请求连接
                //没有请求则GetContext处于阻塞状态
                HttpListenerContext ctx = listener.GetContext();

                ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), ctx);
            }
        }

        void TaskProc(object o)
        {
            HttpListenerContext ctx = (HttpListenerContext)o;

            ctx.Response.StatusCode = 200;//设置返回给客服端http状态代码

            string name = parseByte(ctx.Request.QueryString["name"]);
            string title = parseByte(ctx.Request.QueryString["title"]);
            string content = parseByte(ctx.Request.QueryString["content"]);
            string username = ctx.Request.QueryString["username"];
            string password = ctx.Request.QueryString["password"];

            //进行处理

            //使用Writer输出http响应代码
            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream))
            {
                if (properties["username"].Equals(username) && properties["password"].Equals(password))
                {
                    writer.Write("{\"code\":0,\"msg\":\"successed\"}");
                    this.Invoke(new MethodInvoker(() =>
                    {
                        showToast(name, title, content);
                    }));
                }
                else
                {
                    writer.Write("{\"code\":400,\"msg\":\"error\"}");
                }
                
                writer.Close();
                ctx.Response.Close();
            }
        }
        private String parseByte(String ostr)
        {
            if (ostr.Length == 0)
            {
                return "";
            }
            byte[] buffer = Encoding.GetEncoding("GBK").GetBytes(ostr);
            return Encoding.GetEncoding("UTF-8").GetString(buffer);

        }
        private void Toast_FormClosed(object sender, FormClosedEventArgs e)
        {
            listener.Stop();
            thread.Abort();
            timer.Stop();
        }
    }
}

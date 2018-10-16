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
using System.Windows.Forms;
using WebSocketSharp;

namespace WindowsAppChatt
{
    public partial class Form1 : Form
    {
        private WebSocket client;
        private string host = "ws://127.0.0.1:8080/chat";
        private string userLogged;
        private bool conn = false;
        private string path_string = @"C:\Users\andre_figueira\Desktop\logChat.txt"; // @"C:\Users\andre_figueira\Desktop\logChat.txt"; 
        private StreamReader sr;
        private string queryStringLastMessage;
        private DateTime time;
        private bool init;

        public Form1()
        {
            InitializeComponent();
            
        }

       //LOGIN  AND CONNECT
        private void Button1_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(textBox2.Text) && !conn)
            {
                userLogged = textBox2.Text;
                var queryString = "?name="+ userLogged;

                if (!init)
                {
                    string[] readHistory = File.ReadAllLines(path_string);
                    foreach (var line in readHistory)
                    {
                        listBox1.Items.Add(line);
                        queryStringLastMessage = line;
                    }
                    init = true;

                }

                client = new WebSocket(host + queryString);

                //OnMessage
                client.OnMessage += (ss, ee) => Method1(ee);

                if (client.ReadyState == WebSocketState.Connecting)
                {
                    textBox2.Text = null;

                    

                    client.Connect();
                    conn = true;
                }
            }
        }

        //DISCONNECT
        private void Button2_Click(object sender, EventArgs e)
        {
            //Save the time when the disconnect button was pressed
            if (conn)
            {
                time = DateTime.Now;

                var tex = $"{time} : {userLogged} :> Has disconnected!"+Environment.NewLine;
                File.AppendAllText(path_string, tex);

                listBox1.Items.Add(tex);

                //client.Send("has disconnected!");
                //Thread.Sleep(100);
                client.Close();
                userLogged = "";
                conn = false;
            }
        }

        //SEND
        private void Button3_Click(object sender, EventArgs e)
        {
            if (conn)//client.ReadyState == WebSocketState.Open
            {
                var content = textBox1.Text;
                textBox1.Text = null;
                if (!string.IsNullOrEmpty(content))
                    client.Send(content);
            }
        }

        //This should be a class level declaration
        delegate void myDelegate(MessageEventArgs e);

        private void Method1(MessageEventArgs e)
        {
            //We need to handle CrossThred            
            if (listBox1.InvokeRequired)
            {
                //Through delegate firing the method "Method1"
                myDelegate md = new myDelegate(Method1);
                //Calling Invoke method to invoke the delegate and consequenty invoke Method1
                this.Invoke(md, new object[] { e });
            }
            //No need to handle CrossThread            
            else
            {
            //Save on the file the history chat

                //sw = new StreamWriter(path_string);

                var tex = e.Data + Environment.NewLine;
                File.AppendAllText(path_string, tex);

                listBox1.Items.Add(e.Data);
                
            }

        }
    }
}

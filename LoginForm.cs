﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gTunes
{
    public partial class LoginForm : Form
    {
        public Form1 parent = null;


        public LoginForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string uname = textBox1.Text;
            string pass = textBox2.Text;

            parent.USER_NAME = uname;
            parent.USER_PASS = pass;

            label3.Text = "Loading...";
            parent.loadGooglePlay(uname, pass);
            this.Close();
        }
    }
}

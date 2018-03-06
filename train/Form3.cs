using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace train
{
    public partial class Form3 : Form
    {
        private Form1 returnForm1 = null;
        AutoSizeFormClass asc = new AutoSizeFormClass();
        public Form3(Form1 F1)
        {
            InitializeComponent();
            // 接受Form1对象
            this.returnForm1 = F1;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            asc.controllInitializeSize(this);

            this.comboBox1.Text = "欢迎提示语";
        }

        private void Form3_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
        /// <summary>
        /// 试音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string ent1 = "intp65";
            string vcn1 = "xiaoyan";
            string ent2 = "intp65";
            string vcn2 = "henry";
            String text1 = textBox1.Text;
            String text2 = textBox2.Text;
            Speech speech = new Speech();
            speech.SpeechTest(text1, ent1, vcn1);
            speech.SpeechTest(text2, ent2, vcn2);
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            //恢复Form1
                this.returnForm1.Visible = true;
        }
        /// <summary>
        /// 获取语音播报内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_TextUpdate(object sender, EventArgs e)
        {
            string CombValue=comboBox1.Text;
            SpeechOperat SO = new SpeechOperat();
            List<SpeechModel> list = new List<SpeechModel>();
            list = SO.SelectSp(CombValue);
            if (list.Count == 0)
            {
                textBox1.Text = "";
                textBox2.Text = "";
            }
            else {
                for (int i = 0; i < list.Count; i++) {
                    textBox1.Text = list[i].speechContent;
                    textBox2.Text = list[i].speechEnContent;
                }
            }
        }
        /// <summary>
        /// 添加语音播报内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string type = comboBox1.Text;
            string content = textBox1.Text;
            string Encontent = textBox2.Text;
            SpeechOperat Spo = new SpeechOperat();
            Spo.InsertSpeech(type, content, Encontent);
        }
    }
}

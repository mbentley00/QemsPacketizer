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

namespace QemsPacketizer
{
    public partial class Form1 : Form
    {
        QuestionSet.SetType setType;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtComments.Text = @"far apart,far away,faraway,asg,after,all-star,all-stars,away from,before,day,days,final,finals,packetization,packetize,packet,playoff,superplayoff,tie,tb,tiebreaker";

            this.txtQuestionInput.Text = @"";
            this.txtLogoFile.Text = @"";
            //this.txtTemplateInput.Text = @"C:\Users\mbentley\AppData\Local\Temp\QEMS2\130960503249323562.csv";
            this.txtOutputDir.Text = Path.Combine(Path.GetTempPath(), "QEMS2");
            this.txtSetName.Text = "";
            this.opVHSL.Checked = true;
            this.setType = QuestionSet.SetType.VHSL;
            this.chkWriterNames.Checked = true;
        }

        private void cmdGeneratePackets_Click(object sender, EventArgs e)
        {
            txtTemplateInput.Text = txtTemplateInput.Text.Replace("\"", "");
            txtQuestionInput.Text = txtQuestionInput.Text.Replace("\"", "");

            if (string.IsNullOrWhiteSpace(txtOutputDir.Text))
            {
                MessageBox.Show("Must enter output directory");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQuestionInput.Text) || !File.Exists(txtQuestionInput.Text))
            {
                MessageBox.Show("Could not find input file");
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtLogoFile.Text) && (!File.Exists(txtLogoFile.Text) || Path.GetExtension(txtLogoFile.Text).ToLowerInvariant() != ".bmp"))
            {
                MessageBox.Show("Could not parse logo file.  Make sure it's a valid bmp.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTemplateInput.Text))
            {
                // Read the actual questions, generate a template file to replace
                QuestionSet questionSet = new QuestionSet(this.setType, this.txtQuestionInput.Text, Int32.Parse(txtPackets.Text));
                questionSet.LoadRealQuestions(txtQuestionInput.Text, this.setType);
                questionSet.CreatePackets(this.setType);
                string outputFile = questionSet.WriteOutput(this.txtOutputDir.Text, this.setType);
                questionSet.WriteCategoriesToCsv(this.txtOutputDir.Text, this.setType);

                //string outputFile = @"C:\Users\mbentley\AppData\Local\Temp\QEMS2\131424937605156008 - Copy.csv";

                Utilities.GenerateRtf(outputFile,
                    this.txtOutputDir.Text,
                    this.chkScoresheet.Checked,
                    this.chkDocx.Checked,
                    this.setType,
                    this.txtLogoFile.Text,
                    this.chkComments.Checked,
                    this.GetCommentFilters(),
                    this.chkWriterNames.Checked,
                    this.chkCategories.Checked,
                    this.txtSetName.Text,
                    this.cmbFont.Text);
            }
            else if (!File.Exists(txtTemplateInput.Text))
            {
                MessageBox.Show("Could not find template input file.");
                return;
            }
            else
            {
                // Load the template file, replace the placeholders with actual questions
                QuestionSet questionSet = new QuestionSet(this.setType, this.txtQuestionInput.Text, Int32.Parse(txtPackets.Text));
                questionSet.LoadRealQuestions(txtQuestionInput.Text, this.setType);
                questionSet.LoadTemplate(txtTemplateInput.Text, this.setType);
                string outputFile = questionSet.WriteOutput(this.txtOutputDir.Text, this.setType);
                questionSet.WriteCategoriesToCsv(this.txtOutputDir.Text, this.setType);

                Utilities.GenerateRtf(outputFile,
                    this.txtOutputDir.Text,
                    this.chkScoresheet.Checked,
                    this.chkDocx.Checked,
                    this.setType,
                    this.txtLogoFile.Text,
                    this.chkComments.Checked,
                    this.GetCommentFilters(),
                    this.chkWriterNames.Checked,
                    this.chkCategories.Checked,
                    this.txtSetName.Text,
                    this.cmbFont.Text);
            }
        }

        private List<string> GetCommentFilters()
        {
            return this.txtComments.Text.Split(',').ToList<string>();
        }

        private void cmdGenerateTemplates_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOutputDir.Text))
            {
                MessageBox.Show("Must enter output directory");
                return;
            }

            int tempTemplates;
            if (Int32.TryParse(this.txtTemplates.Text, out tempTemplates))
            {
                for (int i = 0; i < tempTemplates; i++)
                {
                    QuestionSet questionSet = new QuestionSet(this.setType, this.txtQuestionInput.Text, Int32.Parse(txtPackets.Text));
                    questionSet.CreatePackets(this.setType);
                    questionSet.WriteOutput(this.txtOutputDir.Text, this.setType);
                }

                lblOutput.Text = "Wrote templates to " + this.txtOutputDir.Text + " on " + DateTime.Now.ToShortTimeString();
            }
            else
            {
                MessageBox.Show("Error parsing number of templates");
            }
        }

        private void opNsc_CheckedChanged(object sender, EventArgs e)
        {
            this.setType = QuestionSet.SetType.NSC;
        }

        private void opNasat_CheckedChanged(object sender, EventArgs e)
        {
            this.setType = QuestionSet.SetType.Nasat;
        }

        private void opVHSL_CheckedChanged(object sender, EventArgs e)
        {
            this.setType = QuestionSet.SetType.VHSL;
        }

        private void cmdCategoryCsv_Click(object sender, EventArgs e)
        {
            // Read the questions file
            // Read the output file
            // Find matches between them

            // Load the questions
            QuestionSet questionSet = new QuestionSet(this.setType, this.txtQuestionInput.Text, Int32.Parse(txtPackets.Text));
            questionSet.LoadRealQuestions(txtQuestionInput.Text, this.setType);

            // Find where this question was matched to in the output
            string outputFile = Path.Combine(this.txtOutputDir.Text, "categories_" + DateTime.Now.ToFileTime() + ".csv");
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                bool isFirstLine = true;
                foreach (string line in File.ReadAllLines(this.txtTemplateInput.Text))
                {
                    if (isFirstLine)
                    {
                        writer.WriteLine(line);
                        isFirstLine = false;
                    }
                    else
                    {
                        List<string> columns = QuestionSet.ReadCSVLine(line);
                        List<string> outputColumns = new List<string>();
                        foreach (string column in columns)
                        {
                            bool foundMatch = false;
                            string questionText = column.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            foreach (Question question in questionSet.UnassignedTossups)
                            {
                                if (question.TossupText.Equals(questionText))
                                {
                                    foundMatch = true;
                                    outputColumns.Add(question.Category.Name);
                                    break;
                                }   
                            }

                            if (!foundMatch)
                            {
                                questionText = column.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                foreach (Question question in questionSet.UnassignedBonuses)
                                {
                                    if (question.Part1Text.Equals(questionText))
                                    {
                                        foundMatch = true;
                                        outputColumns.Add(question.Category.Name);
                                        break;
                                    }
                                }
                            }

                            if (!foundMatch)
                            {
                                Console.WriteLine(questionText);
                            }
                        }

                        writer.WriteLine(string.Join(",", outputColumns));                        
                    }
                }
            }
        }

        private void cmdCategoryOutput_Click(object sender, EventArgs e)
        {
            QuestionSet questionSet = new QuestionSet(QuestionSet.SetType.VHSL, this.txtQuestionInput.Text, Int32.Parse(txtPackets.Text));
            questionSet.LoadRealQuestions(txtQuestionInput.Text, QuestionSet.SetType.VHSL);
            questionSet.OutputCategoryFiles(this.txtSetName.Text, this.cmbFont.Text, this.chkWriterNames.Checked, this.chkCategories.Checked, this.chkComments.Checked, this.GetCommentFilters(), this.txtOutputDir.Text, this.chkSeparateSubCategories.Checked);
        }

        private void cmdFindNscBugs_Click(object sender, EventArgs e)
        {

        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "QEMS2 CSV File (*.csv)|*.csv";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtQuestionInput.Text = dialog.FileName;
            }
        }

        private void cmdAdvancedMode_Click(object sender, EventArgs e)
        {
            this.opNasat.Visible = true;
            this.opNsc.Visible = true;
            this.opVHSL.Visible = true;
            this.txtLogoFile.Visible = true;
            this.lblPackets.Visible = true;
            this.lblSetType.Visible = true;
            this.cmdGeneratePackets.Visible = true;
            this.txtPackets.Visible = true;
            this.lblLogo.Visible = true;
            this.label3.Visible = true;
            this.label5.Visible = true;
            this.txtTemplateInput.Visible = true;
            this.txtTemplateInput.Enabled = true;
            this.txtPreviousOutputFile.Visible = true;
            this.txtPreviousOutputFile.Enabled = true;
            this.chkScoresheet.Enabled = true;
            this.chkScoresheet.Visible = true;
            this.cmdQuestionLength.Visible = true;

            this.txtSetName.Text = "NSC 2018";
            this.txtLogoFile.Text = @"C:\Users\mbentley\Pictures\PACE Logo H 2018.bmp";
            this.txtQuestionInput.Text = @"C:\Users\mbentley\Downloads\packet2 (3).csv";
            this.chkScoresheet.Checked = false;

            this.txtPackets.Text = "24";
            this.chkComments.Checked = true;
            this.chkWriterNames.Checked = true;
            this.chkComments.Checked = true;
            this.opNsc.Checked = true;
        }

        private void cmdQuestionLength_Click(object sender, EventArgs e)
        {
            this.lblLength.Text = "";
            this.lblLength.Visible = true;
            QuestionSet questionSet = new QuestionSet(this.setType, this.txtQuestionInput.Text, Int32.Parse(txtPackets.Text));
            questionSet.LoadRealQuestions(txtQuestionInput.Text, this.setType);
            List<int> tossupLengths = new List<int>();
            foreach (var question in questionSet.UnassignedTossups)
            {
                tossupLengths.Add(question.Length);
            }

            List<int> bonusLengths = new List<int>();
            foreach (var question in questionSet.UnassignedBonuses)
            {
                bonusLengths.Add(question.Length);
            }

            this.lblLength.Text = $"Tossup Length: {(int)tossupLengths.Average()}, Bonus Length: {(int)bonusLengths.Average()}"; 
        }
    }
}

﻿namespace QemsPacketizer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cmdGeneratePackets = new System.Windows.Forms.Button();
            this.chkScoresheet = new System.Windows.Forms.CheckBox();
            this.chkDocx = new System.Windows.Forms.CheckBox();
            this.txtQuestionInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTemplateInput = new System.Windows.Forms.TextBox();
            this.cmdGenerateTemplates = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTemplates = new System.Windows.Forms.TextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.lblSetType = new System.Windows.Forms.Label();
            this.opNsc = new System.Windows.Forms.RadioButton();
            this.opNasat = new System.Windows.Forms.RadioButton();
            this.opVHSL = new System.Windows.Forms.RadioButton();
            this.lblLogo = new System.Windows.Forms.Label();
            this.txtLogoFile = new System.Windows.Forms.TextBox();
            this.chkWriterNames = new System.Windows.Forms.CheckBox();
            this.chkComments = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSetName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbFont = new System.Windows.Forms.ComboBox();
            this.cmdCategoryCsv = new System.Windows.Forms.Button();
            this.lblPackets = new System.Windows.Forms.Label();
            this.txtPackets = new System.Windows.Forms.TextBox();
            this.cmdCategoryOutput = new System.Windows.Forms.Button();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.chkSeparateSubCategories = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.cmdAdvancedMode = new System.Windows.Forms.Button();
            this.txtPreviousOutputFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkRespectPacket = new System.Windows.Forms.CheckBox();
            this.cmdQuestionLength = new System.Windows.Forms.Button();
            this.lblLength = new System.Windows.Forms.Label();
            this.chkCategories = new System.Windows.Forms.CheckBox();
            this.cmdConvertOphirTemplate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdGeneratePackets
            // 
            this.cmdGeneratePackets.Location = new System.Drawing.Point(181, 553);
            this.cmdGeneratePackets.Margin = new System.Windows.Forms.Padding(4);
            this.cmdGeneratePackets.Name = "cmdGeneratePackets";
            this.cmdGeneratePackets.Size = new System.Drawing.Size(160, 37);
            this.cmdGeneratePackets.TabIndex = 4;
            this.cmdGeneratePackets.Text = "Generate Packets";
            this.cmdGeneratePackets.UseVisualStyleBackColor = true;
            this.cmdGeneratePackets.Visible = false;
            this.cmdGeneratePackets.Click += new System.EventHandler(this.cmdGeneratePackets_Click);
            // 
            // chkScoresheet
            // 
            this.chkScoresheet.AutoSize = true;
            this.chkScoresheet.Enabled = false;
            this.chkScoresheet.Location = new System.Drawing.Point(397, 319);
            this.chkScoresheet.Margin = new System.Windows.Forms.Padding(4);
            this.chkScoresheet.Name = "chkScoresheet";
            this.chkScoresheet.Size = new System.Drawing.Size(183, 21);
            this.chkScoresheet.TabIndex = 6;
            this.chkScoresheet.Text = "Include NSC Scoresheet";
            this.chkScoresheet.UseVisualStyleBackColor = true;
            // 
            // chkDocx
            // 
            this.chkDocx.AutoSize = true;
            this.chkDocx.Checked = true;
            this.chkDocx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDocx.Location = new System.Drawing.Point(397, 288);
            this.chkDocx.Margin = new System.Windows.Forms.Padding(4);
            this.chkDocx.Name = "chkDocx";
            this.chkDocx.Size = new System.Drawing.Size(145, 21);
            this.chkDocx.TabIndex = 7;
            this.chkDocx.Text = "Convert rtf to docx";
            this.chkDocx.UseVisualStyleBackColor = true;
            // 
            // txtQuestionInput
            // 
            this.txtQuestionInput.Location = new System.Drawing.Point(13, 34);
            this.txtQuestionInput.Margin = new System.Windows.Forms.Padding(4);
            this.txtQuestionInput.Name = "txtQuestionInput";
            this.txtQuestionInput.Size = new System.Drawing.Size(480, 22);
            this.txtQuestionInput.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(333, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Question Input CSV (from QEMS2 export questions)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 154);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Output directory";
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.Location = new System.Drawing.Point(16, 174);
            this.txtOutputDir.Margin = new System.Windows.Forms.Padding(4);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(480, 22);
            this.txtOutputDir.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 63);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(413, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Template Input CSV (if blank, automatically generate a template)";
            this.label3.Visible = false;
            // 
            // txtTemplateInput
            // 
            this.txtTemplateInput.Enabled = false;
            this.txtTemplateInput.Location = new System.Drawing.Point(13, 82);
            this.txtTemplateInput.Margin = new System.Windows.Forms.Padding(4);
            this.txtTemplateInput.Name = "txtTemplateInput";
            this.txtTemplateInput.Size = new System.Drawing.Size(480, 22);
            this.txtTemplateInput.TabIndex = 1;
            this.txtTemplateInput.Visible = false;
            // 
            // cmdGenerateTemplates
            // 
            this.cmdGenerateTemplates.Enabled = false;
            this.cmdGenerateTemplates.Location = new System.Drawing.Point(13, 553);
            this.cmdGenerateTemplates.Margin = new System.Windows.Forms.Padding(4);
            this.cmdGenerateTemplates.Name = "cmdGenerateTemplates";
            this.cmdGenerateTemplates.Size = new System.Drawing.Size(160, 37);
            this.cmdGenerateTemplates.TabIndex = 14;
            this.cmdGenerateTemplates.Text = "Generate Templates";
            this.cmdGenerateTemplates.UseVisualStyleBackColor = true;
            this.cmdGenerateTemplates.Visible = false;
            this.cmdGenerateTemplates.Click += new System.EventHandler(this.cmdGenerateTemplates_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 602);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 17);
            this.label4.TabIndex = 15;
            this.label4.Text = "Number of Templates:";
            this.label4.Visible = false;
            // 
            // txtTemplates
            // 
            this.txtTemplates.Enabled = false;
            this.txtTemplates.Location = new System.Drawing.Point(156, 598);
            this.txtTemplates.Margin = new System.Windows.Forms.Padding(4);
            this.txtTemplates.Name = "txtTemplates";
            this.txtTemplates.Size = new System.Drawing.Size(49, 22);
            this.txtTemplates.TabIndex = 16;
            this.txtTemplates.Text = "10";
            this.txtTemplates.Visible = false;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(19, 345);
            this.lblOutput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(0, 17);
            this.lblOutput.TabIndex = 17;
            // 
            // lblSetType
            // 
            this.lblSetType.AutoSize = true;
            this.lblSetType.Location = new System.Drawing.Point(19, 260);
            this.lblSetType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSetType.Name = "lblSetType";
            this.lblSetType.Size = new System.Drawing.Size(64, 17);
            this.lblSetType.TabIndex = 18;
            this.lblSetType.Text = "Set type:";
            this.lblSetType.Visible = false;
            // 
            // opNsc
            // 
            this.opNsc.AutoSize = true;
            this.opNsc.Location = new System.Drawing.Point(17, 279);
            this.opNsc.Margin = new System.Windows.Forms.Padding(4);
            this.opNsc.Name = "opNsc";
            this.opNsc.Size = new System.Drawing.Size(57, 21);
            this.opNsc.TabIndex = 19;
            this.opNsc.Text = "NSC";
            this.opNsc.UseVisualStyleBackColor = true;
            this.opNsc.Visible = false;
            this.opNsc.CheckedChanged += new System.EventHandler(this.opNsc_CheckedChanged);
            // 
            // opNasat
            // 
            this.opNasat.AutoSize = true;
            this.opNasat.Checked = true;
            this.opNasat.Location = new System.Drawing.Point(117, 279);
            this.opNasat.Margin = new System.Windows.Forms.Padding(4);
            this.opNasat.Name = "opNasat";
            this.opNasat.Size = new System.Drawing.Size(66, 21);
            this.opNasat.TabIndex = 20;
            this.opNasat.TabStop = true;
            this.opNasat.Text = "Nasat";
            this.opNasat.UseVisualStyleBackColor = true;
            this.opNasat.Visible = false;
            this.opNasat.CheckedChanged += new System.EventHandler(this.opNasat_CheckedChanged);
            // 
            // opVHSL
            // 
            this.opVHSL.AutoSize = true;
            this.opVHSL.Location = new System.Drawing.Point(211, 279);
            this.opVHSL.Margin = new System.Windows.Forms.Padding(4);
            this.opVHSL.Name = "opVHSL";
            this.opVHSL.Size = new System.Drawing.Size(119, 21);
            this.opVHSL.TabIndex = 21;
            this.opVHSL.Text = "VHSL Regular";
            this.opVHSL.UseVisualStyleBackColor = true;
            this.opVHSL.Visible = false;
            this.opVHSL.CheckedChanged += new System.EventHandler(this.opVHSL_CheckedChanged);
            // 
            // lblLogo
            // 
            this.lblLogo.AutoSize = true;
            this.lblLogo.Location = new System.Drawing.Point(16, 202);
            this.lblLogo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(240, 17);
            this.lblLogo.TabIndex = 22;
            this.lblLogo.Text = "Logo bmp file (leave blank for none):";
            this.lblLogo.Visible = false;
            // 
            // txtLogoFile
            // 
            this.txtLogoFile.Location = new System.Drawing.Point(16, 223);
            this.txtLogoFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtLogoFile.Name = "txtLogoFile";
            this.txtLogoFile.Size = new System.Drawing.Size(480, 22);
            this.txtLogoFile.TabIndex = 3;
            this.txtLogoFile.Visible = false;
            // 
            // chkWriterNames
            // 
            this.chkWriterNames.AutoSize = true;
            this.chkWriterNames.Checked = true;
            this.chkWriterNames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWriterNames.Location = new System.Drawing.Point(397, 347);
            this.chkWriterNames.Margin = new System.Windows.Forms.Padding(4);
            this.chkWriterNames.Name = "chkWriterNames";
            this.chkWriterNames.Size = new System.Drawing.Size(116, 21);
            this.chkWriterNames.TabIndex = 24;
            this.chkWriterNames.Text = "Writer Names";
            this.chkWriterNames.UseVisualStyleBackColor = true;
            // 
            // chkComments
            // 
            this.chkComments.AutoSize = true;
            this.chkComments.Checked = true;
            this.chkComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkComments.Location = new System.Drawing.Point(396, 405);
            this.chkComments.Margin = new System.Windows.Forms.Padding(4);
            this.chkComments.Name = "chkComments";
            this.chkComments.Size = new System.Drawing.Size(96, 21);
            this.chkComments.TabIndex = 25;
            this.chkComments.Text = "Comments";
            this.chkComments.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(393, 260);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 17);
            this.label7.TabIndex = 26;
            this.label7.Text = "Packet options:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 319);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 17);
            this.label8.TabIndex = 27;
            this.label8.Text = "Question set name:";
            // 
            // txtSetName
            // 
            this.txtSetName.Location = new System.Drawing.Point(16, 347);
            this.txtSetName.Margin = new System.Windows.Forms.Padding(4);
            this.txtSetName.Name = "txtSetName";
            this.txtSetName.Size = new System.Drawing.Size(372, 22);
            this.txtSetName.TabIndex = 28;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 390);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 17);
            this.label9.TabIndex = 29;
            this.label9.Text = "Font:";
            // 
            // cmbFont
            // 
            this.cmbFont.FormattingEnabled = true;
            this.cmbFont.Items.AddRange(new object[] {
            "Georgia",
            "Garamond",
            "Times New Roman"});
            this.cmbFont.Location = new System.Drawing.Point(63, 390);
            this.cmbFont.Margin = new System.Windows.Forms.Padding(4);
            this.cmbFont.Name = "cmbFont";
            this.cmbFont.Size = new System.Drawing.Size(325, 24);
            this.cmbFont.TabIndex = 30;
            this.cmbFont.Text = "Georgia";
            // 
            // cmdCategoryCsv
            // 
            this.cmdCategoryCsv.Location = new System.Drawing.Point(13, 508);
            this.cmdCategoryCsv.Margin = new System.Windows.Forms.Padding(4);
            this.cmdCategoryCsv.Name = "cmdCategoryCsv";
            this.cmdCategoryCsv.Size = new System.Drawing.Size(199, 37);
            this.cmdCategoryCsv.TabIndex = 31;
            this.cmdCategoryCsv.Text = "Generate Category CSV";
            this.cmdCategoryCsv.UseVisualStyleBackColor = true;
            this.cmdCategoryCsv.Visible = false;
            this.cmdCategoryCsv.Click += new System.EventHandler(this.cmdCategoryCsv_Click);
            // 
            // lblPackets
            // 
            this.lblPackets.AutoSize = true;
            this.lblPackets.Location = new System.Drawing.Point(215, 602);
            this.lblPackets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPackets.Name = "lblPackets";
            this.lblPackets.Size = new System.Drawing.Size(132, 17);
            this.lblPackets.TabIndex = 32;
            this.lblPackets.Text = "Number of Packets:";
            this.lblPackets.Visible = false;
            // 
            // txtPackets
            // 
            this.txtPackets.Location = new System.Drawing.Point(363, 598);
            this.txtPackets.Margin = new System.Windows.Forms.Padding(4);
            this.txtPackets.Name = "txtPackets";
            this.txtPackets.Size = new System.Drawing.Size(49, 22);
            this.txtPackets.TabIndex = 33;
            this.txtPackets.Text = "25";
            this.txtPackets.Visible = false;
            // 
            // cmdCategoryOutput
            // 
            this.cmdCategoryOutput.Location = new System.Drawing.Point(13, 455);
            this.cmdCategoryOutput.Margin = new System.Windows.Forms.Padding(4);
            this.cmdCategoryOutput.Name = "cmdCategoryOutput";
            this.cmdCategoryOutput.Size = new System.Drawing.Size(160, 46);
            this.cmdCategoryOutput.TabIndex = 34;
            this.cmdCategoryOutput.Text = "Output to Category Files";
            this.cmdCategoryOutput.UseVisualStyleBackColor = true;
            this.cmdCategoryOutput.Click += new System.EventHandler(this.cmdCategoryOutput_Click);
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(403, 487);
            this.txtComments.Margin = new System.Windows.Forms.Padding(4);
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(219, 22);
            this.txtComments.TabIndex = 35;
            // 
            // chkSeparateSubCategories
            // 
            this.chkSeparateSubCategories.AutoSize = true;
            this.chkSeparateSubCategories.Checked = true;
            this.chkSeparateSubCategories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSeparateSubCategories.Location = new System.Drawing.Point(181, 465);
            this.chkSeparateSubCategories.Margin = new System.Windows.Forms.Padding(4);
            this.chkSeparateSubCategories.Name = "chkSeparateSubCategories";
            this.chkSeparateSubCategories.Size = new System.Drawing.Size(189, 21);
            this.chkSeparateSubCategories.TabIndex = 37;
            this.chkSeparateSubCategories.Text = "Separate Sub Categories";
            this.chkSeparateSubCategories.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(400, 428);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(216, 55);
            this.label11.TabIndex = 38;
            this.label11.Text = "Only include comments with these words (leave blank to include all):";
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(503, 34);
            this.cmdBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(85, 23);
            this.cmdBrowse.TabIndex = 39;
            this.cmdBrowse.Text = "Browse";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // cmdAdvancedMode
            // 
            this.cmdAdvancedMode.Location = new System.Drawing.Point(459, 564);
            this.cmdAdvancedMode.Margin = new System.Windows.Forms.Padding(4);
            this.cmdAdvancedMode.Name = "cmdAdvancedMode";
            this.cmdAdvancedMode.Size = new System.Drawing.Size(160, 37);
            this.cmdAdvancedMode.TabIndex = 40;
            this.cmdAdvancedMode.Text = "Advanced Mode";
            this.cmdAdvancedMode.UseVisualStyleBackColor = true;
            this.cmdAdvancedMode.Click += new System.EventHandler(this.cmdAdvancedMode_Click);
            // 
            // txtPreviousOutputFile
            // 
            this.txtPreviousOutputFile.Enabled = false;
            this.txtPreviousOutputFile.Location = new System.Drawing.Point(13, 130);
            this.txtPreviousOutputFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtPreviousOutputFile.Name = "txtPreviousOutputFile";
            this.txtPreviousOutputFile.Size = new System.Drawing.Size(480, 22);
            this.txtPreviousOutputFile.TabIndex = 41;
            this.txtPreviousOutputFile.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 111);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(461, 17);
            this.label5.TabIndex = 42;
            this.label5.Text = "Previous Output file (use this to do an update of already packetized set)";
            this.label5.Visible = false;
            // 
            // chkRespectPacket
            // 
            this.chkRespectPacket.AutoSize = true;
            this.chkRespectPacket.Checked = true;
            this.chkRespectPacket.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRespectPacket.Location = new System.Drawing.Point(400, 519);
            this.chkRespectPacket.Margin = new System.Windows.Forms.Padding(4);
            this.chkRespectPacket.Name = "chkRespectPacket";
            this.chkRespectPacket.Size = new System.Drawing.Size(219, 21);
            this.chkRespectPacket.TabIndex = 43;
            this.chkRespectPacket.Text = "Try To Respect QEMS Packet";
            this.chkRespectPacket.UseVisualStyleBackColor = true;
            // 
            // cmdQuestionLength
            // 
            this.cmdQuestionLength.Location = new System.Drawing.Point(13, 640);
            this.cmdQuestionLength.Margin = new System.Windows.Forms.Padding(4);
            this.cmdQuestionLength.Name = "cmdQuestionLength";
            this.cmdQuestionLength.Size = new System.Drawing.Size(160, 37);
            this.cmdQuestionLength.TabIndex = 44;
            this.cmdQuestionLength.Text = "Get Question Length";
            this.cmdQuestionLength.UseVisualStyleBackColor = true;
            this.cmdQuestionLength.Visible = false;
            this.cmdQuestionLength.Click += new System.EventHandler(this.cmdQuestionLength_Click);
            // 
            // lblLength
            // 
            this.lblLength.AutoSize = true;
            this.lblLength.Location = new System.Drawing.Point(199, 647);
            this.lblLength.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(56, 17);
            this.lblLength.TabIndex = 45;
            this.lblLength.Text = "Length:";
            this.lblLength.Visible = false;
            // 
            // chkCategories
            // 
            this.chkCategories.AutoSize = true;
            this.chkCategories.Checked = true;
            this.chkCategories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategories.Location = new System.Drawing.Point(396, 376);
            this.chkCategories.Margin = new System.Windows.Forms.Padding(4);
            this.chkCategories.Name = "chkCategories";
            this.chkCategories.Size = new System.Drawing.Size(98, 21);
            this.chkCategories.TabIndex = 46;
            this.chkCategories.Text = "Categories";
            this.chkCategories.UseVisualStyleBackColor = true;
            // 
            // cmdConvertOphirTemplate
            // 
            this.cmdConvertOphirTemplate.Location = new System.Drawing.Point(501, 82);
            this.cmdConvertOphirTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.cmdConvertOphirTemplate.Name = "cmdConvertOphirTemplate";
            this.cmdConvertOphirTemplate.Size = new System.Drawing.Size(115, 33);
            this.cmdConvertOphirTemplate.TabIndex = 47;
            this.cmdConvertOphirTemplate.Text = "Convert Ophir";
            this.cmdConvertOphirTemplate.UseVisualStyleBackColor = true;
            this.cmdConvertOphirTemplate.Visible = false;
            this.cmdConvertOphirTemplate.Click += new System.EventHandler(this.cmdConvertOphirTemplate_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 692);
            this.Controls.Add(this.cmdConvertOphirTemplate);
            this.Controls.Add(this.chkCategories);
            this.Controls.Add(this.lblLength);
            this.Controls.Add(this.cmdQuestionLength);
            this.Controls.Add(this.chkRespectPacket);
            this.Controls.Add(this.txtPreviousOutputFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmdAdvancedMode);
            this.Controls.Add(this.cmdBrowse);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.chkSeparateSubCategories);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.cmdCategoryOutput);
            this.Controls.Add(this.txtPackets);
            this.Controls.Add(this.lblPackets);
            this.Controls.Add(this.cmdCategoryCsv);
            this.Controls.Add(this.cmbFont);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtSetName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chkComments);
            this.Controls.Add(this.chkWriterNames);
            this.Controls.Add(this.txtLogoFile);
            this.Controls.Add(this.lblLogo);
            this.Controls.Add(this.opVHSL);
            this.Controls.Add(this.opNasat);
            this.Controls.Add(this.opNsc);
            this.Controls.Add(this.lblSetType);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.txtTemplates);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmdGenerateTemplates);
            this.Controls.Add(this.txtTemplateInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtQuestionInput);
            this.Controls.Add(this.chkDocx);
            this.Controls.Add(this.chkScoresheet);
            this.Controls.Add(this.cmdGeneratePackets);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "QEMS2 Packetizer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdGeneratePackets;
        private System.Windows.Forms.CheckBox chkScoresheet;
        private System.Windows.Forms.CheckBox chkDocx;
        private System.Windows.Forms.TextBox txtQuestionInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTemplateInput;
        private System.Windows.Forms.Button cmdGenerateTemplates;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTemplates;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Label lblSetType;
        private System.Windows.Forms.RadioButton opNsc;
        private System.Windows.Forms.RadioButton opNasat;
        private System.Windows.Forms.RadioButton opVHSL;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.TextBox txtLogoFile;
        private System.Windows.Forms.CheckBox chkWriterNames;
        private System.Windows.Forms.CheckBox chkComments;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSetName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbFont;
        private System.Windows.Forms.Button cmdCategoryCsv;
        private System.Windows.Forms.Label lblPackets;
        private System.Windows.Forms.TextBox txtPackets;
        private System.Windows.Forms.Button cmdCategoryOutput;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.CheckBox chkSeparateSubCategories;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.Button cmdAdvancedMode;
        private System.Windows.Forms.TextBox txtPreviousOutputFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkRespectPacket;
        private System.Windows.Forms.Button cmdQuestionLength;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.CheckBox chkCategories;
        private System.Windows.Forms.Button cmdConvertOphirTemplate;
    }
}


﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;


namespace PjtDailyTask
{
    public partial class Form1 : Form
    {
        private string mypath = @"S:\";        

        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenUploadShar(mypath);
        }

        private void OpenUploadShar(string mypath)
        {
            Process.Start(mypath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ManipulateRecentFiles();
            MessageBox.Show("Report Created");
            System.Windows.Forms.Application.Exit();
        }

        private void ManipulateRecentFiles()
        {
            DateTime SelectedDate = FromDateCalender.SelectionRange.Start;

            string[] filelists = System.IO.Directory.GetFiles(mypath);
            int totalfilecount = filelists.Count();
            int intcount = 0;
            string strheader = "This is a report listing the new ZipIt files: ";
            CreateReport Mainreport = new CreateReport();
            TextWriter TW = Mainreport.CreateFile("C:\\Temp\\test\\test-" + DateTime.Now.ToString("D") + ".txt", strheader);

            while (intcount < totalfilecount)
            {
                String strlastmodified = System.IO.File.GetLastWriteTime(filelists[intcount]).ToString();

                if (DateTime.Parse(strlastmodified) > SelectedDate)
                {
                    Mainreport.Write(filelists[intcount], TW);
                }
                intcount++;
            }
            TW.Close();
        }

        private void cmdUploadAce_Click(object sender, EventArgs e)
        {
            //Create Report
            ManipulateRecentFiles();
            //Navigate through each ZipIT
            string strfilepath = "C:\\Temp\\test\\test-" + DateTime.Now.ToString("D") + ".txt";
            Boolean CheckProcessed = NavigateZipIT(strfilepath);
            //Boolean CheckProcessed = NavigateZipIT("C:\\TEMP\\test\\test-Monday, November 07, 2011.txt");
            if (CheckProcessed == true)
                MessageBox.Show("Tasks Created on Ace");
            else
                MessageBox.Show("No Tasks to Upload on Ace");
            System.Windows.Forms.Application.Exit();   
        }

        private Boolean NavigateZipIT(string path)
        {
            string strline, strQQID;
            Boolean  processed = false;
            int i = 0;
            if (File.Exists(path))
            {
                StreamReader file = null;
                file = new StreamReader(path);
                
                for (i=0;(strline = file.ReadLine())!=null ;i++)
                {
                    //strline = file.ReadLine();
                    Console.WriteLine (strline);
                    int pos = strline.IndexOf("QQ", 0);
                    if (pos > 1)
                    {
                        strQQID = strline.Substring(pos, 8);
                        if (strQQID != "")
                        {
                            processed = true;
                            AceFillData(strQQID);
                        }
                    }
                }                
            }
            return processed;
        }
        
        private void AceFillData(string strQQID)
        {            
            IExplore IE = new IExplore();
            string [] UserIDs = {"LChandran","hsepulveda","NFitzgerald","RSequeira"};
            IE.show();
            IE.openWebPage("http://qqprojects.com/server01/EditTask.asp?PROJECT_ID=16");
            var TaskNo = IE.htmlDoc.getElementById("TASK_NUMBER").getAttribute("Value", 0);
            int ConvertIntTaskno = int.Parse(string.Format("{0}",TaskNo)) + 50 ;
            IE.htmlDoc.getElementById("TASK_NUMBER").innerText = ConvertIntTaskno.ToString();
            IE.htmlDoc.getElementById("TASK_RESUME").innerText = strQQID + " Desktop New Conversion";
            IE.htmlDoc.getElementById("TASK_DESC_CREATOR").innerText = "Data is located in UploadShar.";
            IE.htmlDoc.getElementById("TASK_DESC_CREATOR").style.display = "block";
            IE.htmlDoc.getElementById("TASK_DESC_CREATOR___Frame").outerHTML = "";
            
            IE.ClickElement("value", "Save + Assignment", "document");
            foreach (string UserID in UserIDs)
            {
                //string strUserID = IE.GetUserID("LChandran");
                string strUserID = IE.GetUserID(UserID);
                if (strUserID != "") 
                    IE.ClickElement("id", "ass", "incrementAssignation(this,'" + strUserID);
            }
            //IE.CloseWebPage();
        }

       
    }
}

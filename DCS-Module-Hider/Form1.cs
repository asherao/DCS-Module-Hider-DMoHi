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

/*
 * Welcome to DCS Module Hider (DMoHi). This program will enable you to select which aircraft
 * you want to display or hide on the main menu of DCS. It works for modules you have not
 * purchased, modules you have purchased, and selected mods. Enjoy!
 * 
 * 
 * 
 * 
 * 
 *               ***************To add more modules to this program all you have to do as add their 'pluginEnabled.lua'**************
 *                               ***************specific name to the "string[] modules' list.*******************
 *                               //the file location is C:\Users\UserName\Saved Games\DCS.openbeta\Config\pluginsEnabled.lua
 *                              
 *                              Todo: Add new gaz ns430
 *                               
 *                               //Last module added was: Mi-24P Hind
 */

namespace DCS_Module_Hider
{
    public partial class Form1 : Form
    {

        string appPath = Application.StartupPath;//gets the path of were the utility us running
        string fullPath_pluginsEnabledLua;
        string correctConfigFolderCheck = ("Config");
        string pathOfConfigFolder;

        public Form1()
        {
            InitializeComponent();
            // Sets up the initial objects in the CheckedListBox.
            //just the list of modules
            string[] modules = { "NS430_SA342","NS430_C-101EB", "NS430_C-101CC", "P-47D-30 by Eagle Dynamics", "MiG-19P by RAZBAM" , "MiG-21Bis by Magnitude 3 LLC",
                "NS430_Mi-8MT","NS430_L-39C","NS430", "F/A-18C","Su-27 Flanker by Eagle Dynamics", "L-39C", "Su-33 Flanker by Eagle Dynamics", 
                "VAICOM PRO by Hollywood_315", "VAICOM PRO by 315 Interactive", "Tacview by Raia Software", "Nevada", "FW-190D9 Dora by Eagle Dynamics",
                "Combined Arms by Eagle Dynamics", "Christen Eagle II by Magnitude 3 LLC", "SU-57 PAK FA by CUBANACE SIMULATIONS",
                "MiG-29 Fulcrum by Eagle Dynamics", "Yak-52 by Eagle Dynamics", "MiG-15bis by Belsimtek",
                "TheChannel", "F-15C", "Su-25A by Eagle Dynamics", "Normandy", "Fw 190 A-8 by Eagle Dynamics", "C-101 Aviojet",
                "I-16 by OctopusG", "AV-8B N/A by RAZBAM Sims", "A-10A by Eagle Dynamics", "Bf 109 K-4 by Eagle Dynamics",
                "Flaming Cliffs by Eagle Dynamics","A-10C Warthog by Eagle Dynamics" ,"AJS37 Viggen by Heatblur Simulations",
                "F-16C bl.50", "VARS Pylons 2019 by GrinnelliDesigns","Syria" , "A-10C II Warthog by Eagle Dynamics",
                "Mi-8MTV2 Hip by Belsimtek","MB-339PAN Original model by FTV" , "M-2000C by RAZBAM Sims" ,
                "F-14B by Heatblur Simulations","Supercarrier" ,"PersianGulf" ,"Spitfire LF Mk. IX by Eagle Dynamics" ,"DCS-SRS" ,
                "F-5E by Belsimtek","Su-25T by Eagle Dynamics" ,"UH-1H Huey by Belsimtek" , "JF-17 by Deka Ironwork Simulations",
                "F-86F Sabre by Belsimtek","TF-51D Mustang by Eagle Dynamics" ,"A-4E-C" ,"SA342 Gazelle by Polychop-Simulations" ,
                "Ka-50 Black Shark by Eagle Dynamics" ,"Edge540 FM by Aero" ,"P-51D Mustang by Eagle Dynamics",
                "Mi-24P by Eagle Dynamics"};
            
            checkedListBox1_modules.Items.AddRange(modules);
            checkedListBox1_modules.CheckOnClick = true;
            checkedListBox1_modules.Sorted = true;

            //if there is a settings file, load it
            if (File.Exists(appPath + @"/DCS-Module-Hider-Settings/DMoHi-UserSettings.txt"))
            {
                //MessageBox.Show("Loading...");
                System.IO.StreamReader file = new System.IO.StreamReader(appPath + @"/DCS-Module-Hider-Settings/DMoHi-UserSettings.txt");
                //read the file line by line. Assumes the lines are properly formated via the saveSettings method
                //if no custom resoution was saved, the lines will be blank, which is ok because the fields will also be blank as a result
                fullPath_pluginsEnabledLua = file.ReadLine();
                file.Close();
                //set the textboxes to the data that was read
                textBox1_luaLocation.Text = fullPath_pluginsEnabledLua;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }
        string moduleName;
        private void button2_confirmAndExport_Click(object sender, EventArgs e)//starts the process of exporting the DCS file
        {
            //check that the lua path has been set
            if (String.IsNullOrEmpty(fullPath_pluginsEnabledLua))
            //if one of the path is empty or null, the user didnt have them set
            {
                MessageBox.Show("It looks like you did not select the correct DCS Saved Games Config folder. " +
                    "Please select the correct DCS Saved Games Config folder and try again.");
                return;
            }
            else
            {
                saveLuaLocation();//saves the lua file location
                exportLua();//exports the lua to the DCS location
            }
        }

        private void saveLuaLocation()//saves the lua file location to where the utility was ran from
        {
            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-write-to-a-text-file
            string[] exportLines = { fullPath_pluginsEnabledLua };
            Directory.CreateDirectory(appPath + @"/DCS-Module-Hider-Settings");
            File.WriteAllLines(appPath + @"/DCS-Module-Hider-Settings/DMoHi-UserSettings.txt", exportLines);
        }

        private void exportLua()//exports the lua to the DCS location using some nice loops and checkbox checking
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter?redirectedfrom=MSDN&view=netcore-3.1
            using (StreamWriter sw = new StreamWriter(fullPath_pluginsEnabledLua))
            {
                //begins the file with necessarry DCS stuff
                sw.WriteLine("pluginsEnabled = ");
                sw.WriteLine("{");
                //goes through the list and identifies the checked boxes
                //then it puts the text in a variable and prints the formated line
                //then it moves to the next checked box
                foreach (object itemChecked in checkedListBox1_modules.CheckedItems)
                {
                    moduleName = itemChecked.ToString();
                    sw.WriteLine("    [\"" + moduleName + "\"] = false,");//false hides, true shows
                    //MessageBox.Show(moduleName);
                }

                //https://stackoverflow.com/questions/30240481/how-to-do-a-loop-on-all-unchecked-items-from-checkedlistbox-c
                //similar to what it does to the checked items, but for the unchecked items. Basically magic
                IEnumerable<object> notChecked = (from object item in checkedListBox1_modules.Items
                                                  where !checkedListBox1_modules.CheckedItems.Contains(item)
                                                  select item);
                foreach (object item in notChecked)
                {
                    moduleName = item.ToString();
                    sw.WriteLine("    [\"" + moduleName + "\"] = true,");//false hides, true shows
                }
                sw.WriteLine("} -- end of pluginsEnabled");//caps off the end of the file
            }
            MessageBox.Show("Your new 'pluginsEnabled.lua' file has been exported to '" + fullPath_pluginsEnabledLua + "'. If that does not " +
                        "look correct, please try again.");//tells the user that the export was successful
        }

        private void button1_selectLua_Click(object sender, EventArgs e)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.folderbrowserdialog.-ctor?view=netcore-3.1#System_Windows_Forms_FolderBrowserDialog__ctor
            //uses a folder selector because the user may not have the lua if
            //they have never used the module manager to disable a module
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = false;
            folderDlg.Description = ("Select your DCS Config Folder then click 'OK' (Hint: C:\\Users\\YOURNAME\\Saved Games\\DCS\\Config)");
            DialogResult result = folderDlg.ShowDialog();
            if(result == DialogResult.OK)
            {
                //do a check
                pathOfConfigFolder = folderDlg.SelectedPath;
                if (pathOfConfigFolder.Contains(correctConfigFolderCheck))//just makes sure that the chosen place contains "config"
                {
                    textBox1_luaLocation.Text = folderDlg.SelectedPath;
                    fullPath_pluginsEnabledLua = (pathOfConfigFolder + @"\pluginsEnabled.lua");//creates the whole lua path
                    MessageBox.Show("Your new file will be exported as '" + fullPath_pluginsEnabledLua + "'. If that does not " +
                        "look correct, please try again.");
                }
                else
                {
                    MessageBox.Show("It looks like you did not select the correct DCS Saved Games Config folder. " +
                    "Please select the correct DCS Saved Games Config folder and try again.");
                    return;
                }
            }
        }

        private void label1_selectTheModules_Click(object sender, EventArgs e)
        {
         
        }

        private void button3_deleteLua_Click(object sender, EventArgs e)
        {
            //this is basically the reset buutton

            //check that the user has a lua selected.
            if (String.IsNullOrEmpty(fullPath_pluginsEnabledLua))
            //if one of the path is empty or null, the user didnt have them set
            {
                MessageBox.Show("It looks like you did not select the correct DCS Saved Games Config folder. " +
                    "Please select the correct DCS Saved Games Config folder and try again.");
                return;
            }
            else
            {
                string deleteLuaMessageText = ("Are you sure that you want to delete '" + fullPath_pluginsEnabledLua + "'? Doing so will remove the file " +
                "from your computer. If you delete this file you WILL be able to see or access all DCS modules.");
                DialogResult dialogResult = MessageBox.Show(deleteLuaMessageText, "Are you sure?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //https://docs.microsoft.com/en-us/dotnet/api/system.io.file.delete?view=netcore-3.1
                    //do something
                    //check if file exists
                    //delete the file
                    if (File.Exists(fullPath_pluginsEnabledLua))
                    {
                        File.Delete(fullPath_pluginsEnabledLua);
                    }
                    else
                    {
                        MessageBox.Show("The file '" + fullPath_pluginsEnabledLua + "' was not detected. " +
                            "If that does not look correct, please try again.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                    return;
                }
            }




            //https://stackoverflow.com/questions/3036829/how-do-i-create-a-message-box-with-yes-no-choices-and-a-dialogresult
            
        }

        private void button4_helpReadmee_Click(object sender, EventArgs e)//readmee and help text
        {
            string helpReadmeMessage = ("Welcome to DCS Module Hider (DMoHi). This program will enable you to select " +
                "which aircraft you want to display or hide on the main menu of DCS (as well as the rest of DCS). " +
                "It works for modules you have purchased, modules you have not purchased, and selected mods. This program " +
                "modifies, creates, and deletes files on your computer. If you are not comfortable with that, do not " +
                "use this utility." + "\r\n" + "\r\n" +
                "1. Check the modules that you want to hide in the DCS main menu. (I typically check modules " +
                "that I don't own)." + "\r\n" + "\r\n" +
                "2. Select your DCS Saved Games Config folder. It is likely located at " +
                "‘C:\\Users\\YOURNAME\\SavedGames\\DCS.openbeta\\Config’." + "\r\n" + "\r\n" +
                "3. Click the ‘Confirm and Export’ button. The utility will export a new ‘pluginsEnabled.lua’ to the chosen location. " +
                " The utility will also create a folder and file where you ran the utility. This file contains the location of ‘" +
                "pluginsEnabled.lua´ so you won’t have to search for it next time you use the utility." + "\r\n" + "\r\n" +
                "4. If you want to unhide all modules in DCS press the ‘Unhide all modules’ button. Doing this will delete " +
                "‘pluginsEnabled.lua’ and sets the visible DCS modules to default." + "\r\n" + "\r\n" +

                "That’s it! Thank you for using DMoHi! If you have any questions, comments, concerns, or would just like" +
                " to say “Thanks”, feel free to contact me via Discord: Bailey#6230." + "\r\n" + "\r\n" +

                "Please feel free to donate. All donations go to making more free DCS utilities and mods for the community! " +
                " https://www.paypal.com/paypalme/asherao" + "\r\n" + "\r\n" +

                "If you would like to examine, follow, or add to DMoHi, the git is here: " +
                "https://github.com/asherao/DCS-Module-Hider-DMoHi" + "\r\n" + "\r\n" +

                "Thank you to the Hoggit community on Discord for the idea and research for this utility." + "\r\n" +
                "Enjoy!" + "\r\n" + "\r\n" +

                "~Bailey" + "\r\n" +
                "07JUL2021");

            DialogResult dialogResult = MessageBox.Show(helpReadmeMessage, "DMoHi Help / Readme", MessageBoxButtons.OK);//idk why this is here
            //oh, it puts the text in the dialog pox that pops up and presents the title.
        }
    }
}

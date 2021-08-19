using System;
using System.Collections;
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
 * purchased, modules you have purchased, and selected mods. You can also rearrange the icons. Enjoy!
 * 
 * 
 * ***************To add more modules to this program all you have to do as add their 'pluginEnabled.lua'**************
 * ***************specific name to the "string[] modules' list.*******************
 * //the file location is C:\Users\UserName\Saved Games\DCS.openbeta\Config\pluginsEnabled.lua
 *                              
 * //Todo: Add new gaz ns430
 *                               
 * //Last module added was: Mi-24P Hind
 * 
 */

/*
 * Try to implement this awesome idea for v5:
 * https://forums.eagle.ru/topic/279659-module-icon-customisation-mod-change-the-main-dcs-screen-icons-however-you-want/
 * 
 * - https://stackoverflow.com/questions/805165/reorder-a-winforms-listbox-using-drag-and-drop/805267#805267
 * - https://www.codeproject.com/Articles/321892/User-sortable-Listbox
 * 
 * - Drag and drop
 * - Does not disable the modules
 * - DCS World OpenBeta\MissionEditor\modules\plPanel.lua
 * - InsertOneItem(self,a_data,tmpData,"A-10C Warthog by Eagle Dynamics")
 * - InsertOneItem(self,a_data,tmpData,"Edge540 FM by Aero")
 */

/*
 * v6 ideas:
 * - WPF
 * - Saves/Loads the order of the Icons
 * - Fully interactable GUI
 *  - Drag and drop 
 */

namespace DCS_Module_Hider
{
    public partial class Form1 : Form
    {

        string appPath = Application.StartupPath;//gets the path of were the utility us running
        string fullPath_pluginsEnabledLua;
        string correctConfigFolderCheck = ("Config");
        string pathOfConfigFolder;


        ArrayList list = new ArrayList();


        string[] modules = { 
            "NS430_SA342","NS430_C-101EB", "NS430_C-101CC", 
            "P-47D-30 by Eagle Dynamics", "MiG-19P by RAZBAM" , 
            "MiG-21Bis by Magnitude 3 LLC",
            "NS430_Mi-8MT","NS430_L-39C",
            "NS430", "F/A-18C",
            "Su-27 Flanker by Eagle Dynamics", 
            "L-39C", "Su-33 Flanker by Eagle Dynamics",
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
        public Form1()
        {
            InitializeComponent();
            // Sets up the initial objects in the CheckedListBox.
            //just the list of modules
            button3_deleteLua.Visible = false;
            checkedListBox1_modules.Visible = false;

            listBox1.Items.AddRange(modules);
            listBox1.Sorted = false;

            checkedListBox1_modules.Items.AddRange(modules);
            //checkedListBox1_modules.CheckOnClick = true;
            //checkedListBox1_modules.Sorted = true;

            

            //sort the list of modules
            //https://www.csharp-console-examples.com/winform/sort-listbox-items-on-descending-order-in-c/
            foreach (object o in listBox1.Items)
            {
                list.Add(o);
            }
            list.Sort();
            //list.Reverse();
            listBox1.Items.Clear();
            listBox1.Items.Add("--MODULES ABOVE WILL BE SHOWN IN ORDER--");
            foreach (object o in list)
            {
                listBox1.Items.Add(o);
            }

            

            //if there is a settings file, load it
            if (File.Exists(appPath + @"/DCS-Module-Hider-Settings/DMoHi-UserSettings.txt"))
            {
                //MessageBox.Show("Loading...");
                System.IO.StreamReader file = new System.IO.StreamReader(appPath + @"/DCS-Module-Hider-Settings/DMoHi-UserSettings.txt");
                //read the file line by line. Assumes the lines are properly formated via the saveSettings method
                pathOfLuaFile = file.ReadLine();
                pathOfLuaFileCopy = file.ReadLine();
                file.Close();
                //set the textboxes to the data that was read
                textBox1_luaLocation.Text = pathOfLuaFile;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        string moduleName;

        private void button2_confirmAndExport_Click(object sender, EventArgs e)//starts the process of exporting the DCS file
        {
            //check that the lua path has been set
            if (String.IsNullOrEmpty(pathOfLuaFile))
            //if one of the path is empty or null, the user didnt have them set
            {
                MessageBox.Show("It looks like you did not select the correct DCS Install folder. " +
                    "Please select the correct DCS Install folder and try again.");
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
            string[] exportLines = { pathOfLuaFile, pathOfLuaFileCopy };
            Directory.CreateDirectory(appPath + @"/DCS-Module-Hider-Settings");
            File.WriteAllLines(appPath + @"/DCS-Module-Hider-Settings/DMoHi-UserSettings.txt", exportLines);
        }

        private void exportLua()//exports the lua to the DCS location using some nice loops and checkbox checking
        {
            if (File.Exists(pathOfLuaFileCopy))
            {
                File.Delete(pathOfLuaFileCopy);
            }
            File.Copy(pathOfLuaFile, pathOfLuaFileCopy); //we will read from the copy so that we can edit the actual one
            //https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter?redirectedfrom=MSDN&view=netcore-3.1
            using (StreamWriter sw = new StreamWriter(pathOfLuaFile))
            {
                /*
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
                */

                //sw.WriteLine("pluginsEnabled = " + "\"the word\"");

                //split the plPanel.lua into three parts (aka strings)
                //part 1 goes up until 

                //return textutil.Utf8Compare((tab1.data.name or tab1.name), (tab2.data.name or tab2.name))
                //end

                //part 2 is the middle where i fill in code

                // part 3 starts at
                //self.numEl = 0
                string[] lines = System.IO.File.ReadAllLines(pathOfLuaFileCopy);

                foreach (string line in lines)
                {
                    sw.WriteLine(line);
                    if (line.Contains("	return textutil.Utf8Compare((tab1.data.name or tab1.name), (tab2.data.name or tab2.name))"))
                        //this is right before the start of the custom section
                    {
                        //sw.WriteLine("	return textutil.Utf8Compare((tab1.data.name or tab1.name), (tab2.data.name or tab2.name))");
                        sw.WriteLine("end");

                        sw.WriteLine("");
                        sw.WriteLine("-----TAIPAN CODE START-----");
                        sw.WriteLine("local function InsertOneItem(self,a_data,tmpData, moduleID) -- NEW FUNCTION BY TAIPAN_");
                        sw.WriteLine("	--loop to get data now in a function, so can be called one item at a time in the desired sort order");
                        sw.WriteLine("");
                        sw.WriteLine("	for k,v in pairs(a_data) do");
                        sw.WriteLine("		if v.data.state ~= nil and v.data.id == moduleID then");
                        sw.WriteLine("			table.insert(tmpData, v)");
                        sw.WriteLine("	    end                                             -- TAIPAN's added line");
                        sw.WriteLine("	end");
                        sw.WriteLine("end");
                        sw.WriteLine("");
                        sw.WriteLine("function setData(self, a_data, a_callback)   -- EDITED FUNCTION BY TAIPAN_");
                        sw.WriteLine("	-- сортировка");
                        sw.WriteLine("	local tmpData = {}");
                        sw.WriteLine("	-- TAIPAN CODE START----");
                        sw.WriteLine("	-- Search DCS folder or Mods folder for \"entry.lua\" to find the module ID");
                        sw.WriteLine("	-- Saved Games\\DCS.openbeta\\Config\\pluginsEnabled.lua file also contains info");
                        sw.WriteLine("	--EDIT BELOW HERE:");

                        //InsertOneItem(self,a_data,tmpData,"UH-1H Huey by Belsimtek")

                        foreach (object module in listBox1.Items)
                        {
                            if (module.ToString().Contains("--"))// if the module has a double dash, it's the custoom limiter i put in
                            {
                                //stop exporting to the .lua file
                                break;
                            }
                            else
                            {
                                //write the module in the lua according to this format
                                sw.WriteLine("	InsertOneItem(self,a_data,tmpData,\"" + module + "\")");
                                //MessageBox.Show(module.ToString());//debug
                            }

                        }
                        sw.WriteLine("	-- TAIPAN CODE END------");
                        sw.WriteLine("");
                        break;//this stops the writing here

                    }
                    //sw.WriteLine(line); //and then continue writing lines. currently this is incorrect and should start up at "	self.numEl = 0"
                
                }

                //this takes the start point for the 3rd part and pastes it to the end of the file
                //https://stackoverflow.com/questions/21933574/reading-from-a-file-in-by-specifying-start-point-and-end-point

                var range = File.ReadLines(pathOfLuaFileCopy)
                    .SkipWhile(l => !l.TrimStart().StartsWith("self.numEl = 0"))
                    .TakeWhile(l => !l.TrimStart().StartsWith("a_buttonEl:setTooltipText(tooltip)"));

                string result = string.Join(Environment.NewLine, range);

                sw.WriteLine(result);//the paste

                //MessageBox.Show(result);//debug
                sw.WriteLine("	a_buttonEl:setTooltipText(tooltip)");
                sw.WriteLine("end");

                
            }
            MessageBox.Show("Your new file has been exported to '" + pathOfLuaFile);//tells the user that the export was successful

            //cleanup
            if (File.Exists(pathOfLuaFileCopy))
            {
                File.Delete(pathOfLuaFileCopy);
            }
        }

        string pathOfLuaFile;
        string pathOfLuaFileCopy;

        private void button1_selectLua_Click(object sender, EventArgs e)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.folderbrowserdialog.-ctor?view=netcore-3.1#System_Windows_Forms_FolderBrowserDialog__ctor
            //uses a folder selector because the user may not have the lua if
            //they have never used the module manager to disable a module
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = false;
            folderDlg.Description = ("Select your DCS Install Folder then click 'OK' (Example: C:\\Games\\DCS World Open Beta)");
            DialogResult result = folderDlg.ShowDialog();
            if(result == DialogResult.OK)
            {
                //do a check
                pathOfConfigFolder = folderDlg.SelectedPath;
                pathOfLuaFile = Path.Combine(pathOfConfigFolder, "MissionEditor\\modules\\plPanel.lua"); //https://stackoverflow.com/questions/1048129/what-is-the-best-way-to-combine-a-path-and-a-filename-in-c-net
                pathOfLuaFileCopy = Path.Combine(pathOfConfigFolder, "MissionEditor\\modules\\plPanelCopy.lua");
                if (File.Exists(pathOfLuaFile))
                {
                    MessageBox.Show("Your new file will be exported as '" + pathOfLuaFile + "'. If that does not " +
                        "look correct, please try again.");
                    textBox1_luaLocation.Text = pathOfLuaFile;
                }
                else
                {
                    MessageBox.Show("It looks like you did not select the correct DCS Install folder. " +
                    "Please select the correct DCS Install folder and try again." + "\n" +
                    "The attempted file was: " + pathOfLuaFile );
                    textBox1_luaLocation.Text = "";
                    pathOfLuaFile = "";
                    pathOfLuaFileCopy = "";
                    return;
                }

                /*
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
                */
            }
        }

        private void label1_selectTheModules_Click(object sender, EventArgs e)
        {
         
        }

        private void button3_deleteLua_Click(object sender, EventArgs e)
        {
            //this is basically the reset buutton

            //check that the user has a lua selected.
            if (String.IsNullOrEmpty(pathOfLuaFile))
            //if one of the path is empty or null, the user didnt have them set
            {
                MessageBox.Show("It looks like you did not select the correct DCS Saved Games Config folder. " +
                    "Please select the correct DCS Saved Games Config folder and try again.");
                return;
            }
            else
            {
                string deleteLuaMessageText = ("Are you sure that you want to delete '" + pathOfLuaFile + "'? Doing so will remove the file " +
                "from your computer. If you delete this file you WILL be able to see or access all DCS modules.");
                DialogResult dialogResult = MessageBox.Show(deleteLuaMessageText, "Are you sure?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //https://docs.microsoft.com/en-us/dotnet/api/system.io.file.delete?view=netcore-3.1
                    //do something
                    //check if file exists
                    //delete the file
                    if (File.Exists(pathOfLuaFile))
                    {
                        File.Delete(pathOfLuaFile);
                    }
                    else
                    {
                        MessageBox.Show("The file '" + pathOfLuaFile + "' was not detected. " +
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
                "which aircraft you want to display or hide on the main menu of DCS and in what order. " +
                "It works for modules you have purchased, modules you have not purchased, and selected mods. This program " +
                "modifies, creates, and deletes files on your computer. If you are not comfortable with that, do not " +
                "use this utility." + "\r\n" + "\r\n" +
                "1. Click and drag the module names above" + "\r\n" + "'--ABOVE MODULES WILL BE SHOWN--'." + 
                "\r\n" + "They will be displayed in the same order. " + "(Don't drag them too high!!!)" + " You can also drag the seperator if you like." +
                "\r\n" + "\r\n" +
                "2. Select your DCS Install folder. \r\nAn example is " +
                "‘C:\\Games\\DCS World Open Beta’." + "\r\n" + "\r\n" +
                "3. Click the ‘Confirm and Export’ button. The utility will export a new ‘plPanel.lua’ to the proper location. " +
                " The utility will also create a folder and file where you ran the utility. This file contains the location of ‘" +
                "plPanel.lua' so you won’t have to search for it next time you use the utility." + "\r\n" + "\r\n" +

                "4. If you want to restore all modules please use the DCS Repair utility." + "\r\n" + "\r\n" +

                "That’s it! Thank you for using DMoHi! If you have any questions, comments, concerns, or would just like" +
                " to say “Thanks”, feel free to contact me via Discord: Bailey#6230." + "\r\n" + "\r\n" +

                "Please feel free to donate. All donations go to making more free DCS utilities and mods for the community! " +
                " https://www.paypal.com/paypalme/asherao" + "\r\n" + "\r\n" +

                "If you would like to examine, follow, or add to DMoHi, the git is here: " +
                "https://github.com/asherao/DCS-Module-Hider-DMoHi" + "\r\n" + "\r\n" +

                "Thank you to the Hoggit community on Discord for the idea and research for this utility." +
                " Also thanks to TAIPAN_ for the v5 DMoHi update. Now you can select and arrange the icons without disabling them!" + "\r\n" +
                "Enjoy!" + "\r\n" + "\r\n" +

                "~Bailey" + "\r\n" +
                "18AUG2021");

            DialogResult dialogResult = MessageBox.Show(helpReadmeMessage, "DMoHi Help / Readme", MessageBoxButtons.OK);//idk why this is here
            //oh, it puts the text in the dialog pox that pops up and presents the title.
        }

        // ***** Drag and Drop Code *****

        private void checkedListBox1_modules_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.checkedListBox1_modules.SelectedItem == null) return;
            this.checkedListBox1_modules.DoDragDrop(this.checkedListBox1_modules.SelectedItem, DragDropEffects.Move);
        }

        private void checkedListBox1_modules_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void checkedListBox1_modules_DragDrop(object sender, DragEventArgs e)
        {
            Point point = checkedListBox1_modules.PointToClient(new Point(e.X, e.Y));
            int index = this.checkedListBox1_modules.IndexFromPoint(point);
            if (index < 0) index = this.checkedListBox1_modules.Items.Count - 1;
            object data = e.Data.GetData(typeof(String));
            this.checkedListBox1_modules.Items.Remove(data);
            this.checkedListBox1_modules.Items.Insert(index, data);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;
            this.listBox1.DoDragDrop(this.listBox1.SelectedItem, DragDropEffects.Move);
        }

        private void listBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            Point point = listBox1.PointToClient(new Point(e.X, e.Y));
            int index = this.listBox1.IndexFromPoint(point);
            if (index < 0) index = this.listBox1.Items.Count - 1;
            object data = e.Data.GetData(typeof(String));
            this.listBox1.Items.Remove(data);
            this.listBox1.Items.Insert(index, data);
        }
    }
}

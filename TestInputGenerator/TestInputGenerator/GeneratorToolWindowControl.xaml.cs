namespace TestInputGenerator
{
    using Microsoft.VisualBasic;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;


    /// <summary>
    /// Interaction logic for GeneratorToolWindowControl.
    /// </summary>
    public partial class GeneratorToolWindowControl : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorToolWindowControl"/> class.
        /// </summary>
        public GeneratorToolWindowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]

        private string className;
        private string methodName;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            className = Interaction.InputBox("Please enter a class name without \".cs\":", "Class name input", "");
            methodName = Interaction.InputBox("Please enter a method name:", "Method name input", "");

            classMethodNamesTextBox.Text += "\n"+className+"/"+methodName;

            JsonTools.createJson(className, methodName);

            button1.IsEnabled = false;
            button2.IsEnabled = true;
            button5.IsEnabled = true;
            button8.IsEnabled = false;
        }

        private string numOfInputs;
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            numOfInputs = Interaction.InputBox("Please enter the number of inputs:", "Number of inputs", "Any integer greater than 0");
            numOfInputTextBox.Text += numOfInputs;
            
            button2.IsEnabled = false;
            button3.IsEnabled = true;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            for (int i=1; i<=Int32.Parse(numOfInputs); i++)
            {
                string tempInput = Interaction.InputBox("Please enter input " + i + " with commas between samples.", "Input " + i);
                inputsBox.Text += "Input "+i+":"+tempInput+"\n";
                JsonTools.addInputSampleToJson(className, methodName, tempInput, i);
            }

            button3.IsEnabled = false;
            button4.IsEnabled = true;
            button6.IsEnabled = true;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            string inputToEdit = Interaction.InputBox("Please enter the number of the input you would like to change:", "Input number");
            string[] inputsText = inputsBox.Text.Split('\n');
            string sampleToAdd = Interaction.InputBox("Please enter the input sample you would like to add to input " + inputToEdit + ":", "Additional input");
            inputsText[Int32.Parse(inputToEdit)-1] += ","+sampleToAdd;
            inputsBox.Text = string.Join("\n", inputsText);

            JsonTools.addToExistingInputSampleInJson(className, methodName, sampleToAdd, Int32.Parse(inputToEdit));
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            classMethodNamesTextBox.Text = "Class Name/Method Name: ";
            numOfInputTextBox.Text = "Number of inputs: ";
            inputsBox.Text = "";
            basesBox.Text = "";
            generatedInputsBox.Text = "";
            baseNum = 1;
            button1.IsEnabled = true;
            button2.IsEnabled = false;
            button3.IsEnabled = false;
            button4.IsEnabled = false;
            button5.IsEnabled = false;
            button6.IsEnabled = false;
            button7.IsEnabled = false;
            button8.IsEnabled = true;
        }

        private int baseNum = 1;
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            string aBase = Interaction.InputBox("Please enter a base with commas between samples: ", "Base " + baseNum + " entry");
            basesBox.Text += "Base " + baseNum + ":" + aBase+"\n";
            JsonTools.addBaseToJson(className, methodName, aBase, baseNum);
            baseNum++;

            button7.IsEnabled = true;
        }
        
        private void button7_Click(object sender, RoutedEventArgs e)
        {
            List<String[]> generatedInputs = new List<String[]>();
            string[] tempInputSamples = inputsBox.Text.Split(':', '\n');
            string[] tempBases = basesBox.Text.Split(':', '\n');
            List<String> inputSamples = createSamplesOrBasesList(tempInputSamples);
            List<String> bases = createSamplesOrBasesList(tempBases);
            List<String[]> splittedSamples = splitElementsWithCommasInsideList(inputSamples);
            List<String[]> splittedBases = splitElementsWithCommasInsideList(bases);
            foreach (String[] sBase in splittedBases)
            {
                generateTestInputsForGivenBase(sBase, splittedSamples, generatedInputs);
            }
            List<String[]> uniqueGeneratedInputs = removeDuplicates(generatedInputs);
            if (!generatedInputsBox.Text.Equals(""))
            {
                generatedInputsBox.Text = "";
            }
            foreach (String[] gInput in uniqueGeneratedInputs)
            {
                generatedInputsBox.Text += "[";
                for (int i=0; i<gInput.Length; i++)
                {
                    if (i+1 == gInput.Length)
                    {
                        generatedInputsBox.Text += gInput[i];
                    }
                    else
                    {
                        generatedInputsBox.Text += gInput[i] + ",";
                    }
                }
                generatedInputsBox.Text += "]\n";
            }
            JsonTools.addGeneratedTestInputsToJson(className, methodName, uniqueGeneratedInputs);
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            JObject o1 = JsonTools.readFromJson();
            classMethodNamesTextBox.Text += "\n" + o1["Class"] + "/" + o1["Method"];
            if (o1["Input Sample 1"] != null)
            {
                int i = 1;
                while (o1["Input Sample " + i] != null)
                {
                    inputsBox.Text += "Input " + i + ":" + o1["Input Sample " + i] + "\n";
                    i++;
                }
                numOfInputTextBox.Text += (i - 1);
                numOfInputs = (i - 1).ToString();
                button4.IsEnabled = true;
                button6.IsEnabled = true;
            }
            else
            {
                button2.IsEnabled = true;
            }
            if (o1["Base 1"] != null)
            {
                int j = 1;
                while (o1["Base " + j] != null)
                {
                    basesBox.Text += "Base " + j + ":" + o1["Base " + j] + "\n";
                    j++;
                }
                baseNum = j;
                
                button7.IsEnabled = true;
            }
            className = o1["Class"].ToString();
            methodName = o1["Method"].ToString();

            //Buraya generated input boxı da dolduracak bir kod yaz.
            if (o1["Test Inputs"] != null)
            {
                JArray tempJArray = (JArray)o1["Test Inputs"];
                String[] tempGeneratedInputs = tempJArray.ToObject<string[]>();
                foreach (String s in tempGeneratedInputs)
                {
                    generatedInputsBox.Text += "[" + s + "]\n";
                }
            }

            button1.IsEnabled = false;
            button5.IsEnabled = true;
            button8.IsEnabled = false;
        }

        private List<String> createSamplesOrBasesList(string[] anArray)
        {
            List<String> desiredList = new List<String>();
            for (int i=1; i<anArray.Length; i+=2)
            {
                desiredList.Add(anArray[i]);
            }
            return desiredList;
        }

        private List<String[]> splitElementsWithCommasInsideList(List<String> aList)
        {
            List<String[]> newList = new List<String[]>();
            foreach (string s in aList)
            {
                newList.Add(s.Split(','));
            }
            return newList;
        }

        private void generateTestInputsForGivenBase(string[] aBase, List<String[]> splittedSamples, List<String[]> generatedInputs)
        {
            generatedInputs.Add(aBase);
            for (int i=0; i<aBase.Length; i++)
            {
                for (int j=0; j < splittedSamples[i].Length; j++)
                {
                    if (!splittedSamples[i][j].Equals(aBase[i]))
                    {
                        string[] tempBase = new string[aBase.Length];
                        for (int k = 0; k < aBase.Length; k++)
                        {
                            if (k == i)
                            {
                                tempBase[k] = splittedSamples[i][j];
                            }
                            else
                            {
                                tempBase[k] = aBase[k];
                            }
                        }
                        generatedInputs.Add(tempBase);
                    }
                }
            }
        }

        private List<string[]> removeDuplicates(List<string[]> items)
        {
            var result = new List<string[]>();
            var helperList = new List<string[]>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!isArrayInList(items[i], helperList))
                {
                    result.Add(items[i]);
                    helperList.Add(items[i]);
                }
            }
            return result;
        }

        private bool areArraysEqual(string[] arr1, string[] arr2)
        {
            bool flag = true;
            for (int i=0; i< arr1.Length; i++)
            {
                if (!arr1[i].Equals(arr2[i]))
                {
                    flag = false;
                }
            }
            return flag;
        }

        private bool isArrayInList(string[] arr, List<string[]> items)
        {
            bool flag = false;
            for (int i=0; i<items.Count; i++)
            {
                if (areArraysEqual(arr, items[i]))
                {
                    flag = true;
                }
            }
            return flag;
        }
    }
}
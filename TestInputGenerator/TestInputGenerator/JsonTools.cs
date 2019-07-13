using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestInputGenerator
{
    class JsonTools
    {
        private static string currentDirectory = "";

        public static void createJson(string className, string methodName)
        {
            string tempJson = @"{
                  'Test Input Generator': {
                    'Class': '',
                    'Method': ''
                  }
                }";
            JObject rss = JObject.Parse(tempJson);
            JObject o1 = (JObject)rss["Test Input Generator"];
            o1["Class"] = className;
            o1["Method"] = methodName;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Please select the folder you would like to store generator information";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                currentDirectory = fbd.SelectedPath;
            }

            string projectPath = currentDirectory + "\\"+className+"."+methodName+".json";
            File.WriteAllText(@projectPath, rss.ToString());
        }

        public static void addInputSamplesToJson(string className, string methodName, string insideInputsBox)
        {
            string[] inputSamples = insideInputsBox.Split('\n', '\r').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            string projectPath = currentDirectory + "\\" + className + "." + methodName + ".json";
            JObject rss = JObject.Parse(File.ReadAllText(@projectPath));
            JObject o1 = (JObject)rss["Test Input Generator"];

            if (o1["Input Samples"] != null)
            {
                o1.Property("Input Samples").Remove();
                File.WriteAllText(@projectPath, rss.ToString());
            }

            JArray inputsJArray = new JArray();
            foreach (string s in inputSamples)
            {
                inputsJArray.Add(s);
            }
            o1.Add(new JProperty("Input Samples", inputsJArray));
            File.WriteAllText(@projectPath, rss.ToString());
        }

        /*public static void addInputSampleToJson(string className, string methodName, string sample, int sampleId)
        {
            string projectPath = currentDirectory + "\\" + className + "." + methodName + ".json";
            JObject rss = JObject.Parse(File.ReadAllText(@projectPath));
            JObject o1 = (JObject)rss["Test Input Generator"];
            if (sampleId == 1)
            {
                o1.Property("Method").AddAfterSelf(new JProperty("Input Sample " + sampleId, sample));
            }
            else
            {
                int tempId = sampleId - 1;
                o1.Property("Input Sample " + tempId).AddAfterSelf(new JProperty("Input Sample " + sampleId, sample));
            }
            File.WriteAllText(@projectPath, rss.ToString());
        }

        public static void addToExistingInputSampleInJson(string className, string methodName, string newSample, int sampleId)
        {
            string projectPath = currentDirectory + "\\" + className + "." + methodName + ".json";
            JObject rss = JObject.Parse(File.ReadAllText(@projectPath));
            JObject o1 = (JObject)rss["Test Input Generator"];
            o1["Input Sample " + sampleId] += "," + newSample;
            File.WriteAllText(@projectPath, rss.ToString());
        }*/

        public static void addBaseToJson(string className, string methodName, string aBase, int baseId)
        {
            string projectPath = currentDirectory + "\\" + className + "." + methodName + ".json";
            JObject rss = JObject.Parse(File.ReadAllText(@projectPath));
            JObject o1 = (JObject)rss["Test Input Generator"];
            if (baseId == 1)
            {
                o1.Add(new JProperty("Base " + baseId, aBase));
            }
            else
            {
                o1.Property("Base " + (baseId - 1)).AddAfterSelf(new JProperty("Base " + baseId, aBase));
            }
            File.WriteAllText(@projectPath, rss.ToString());
        }

        public static void addGeneratedTestInputsToJson(string className, string methodName, List<String[]> testInputs)
        {
            emptyGeneratedInputsArrayInJson(className, methodName);
            string projectPath = currentDirectory + "\\" + className + "." + methodName + ".json";
            JObject rss = JObject.Parse(File.ReadAllText(@projectPath));
            JObject o1 = (JObject)rss["Test Input Generator"];

            JArray testInputsJArray = new JArray();
            foreach (string[] sArray in testInputs)
            {
                string stringToAdd = "";
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (i + 1 == sArray.Length)
                    {
                        stringToAdd += sArray[i];
                    }
                    else
                    {
                        stringToAdd += sArray[i] + ",";
                    }
                }
                testInputsJArray.Add(stringToAdd);
            }
            o1.Add(new JProperty("Test Inputs", testInputsJArray));

            File.WriteAllText(@projectPath, rss.ToString());
        }

        private static void emptyGeneratedInputsArrayInJson(string className, string methodName)
        {
            string projectPath = currentDirectory + "\\" + className + "." + methodName + ".json";
            JObject rss = JObject.Parse(File.ReadAllText(@projectPath));
            JObject o1 = (JObject)rss["Test Input Generator"];
            if (o1["Test Inputs"] != null)
            {
                o1.Property("Test Inputs").Remove();
            }
            File.WriteAllText(@projectPath, rss.ToString());
        }

        public static JObject readFromJson()
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "json Files (*.json)|*.json";
            choofdlog.FilterIndex = 1;
            choofdlog.InitialDirectory = currentDirectory;
            choofdlog.Multiselect = false;
            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                string sFileName = choofdlog.FileName;

                string[] tempDirectory = sFileName.Split('\\');
                tempDirectory[tempDirectory.Length - 1] = null;
                currentDirectory = string.Join("\\", tempDirectory);

                JObject rss = JObject.Parse(File.ReadAllText(sFileName));
                return (JObject)rss["Test Input Generator"];
            }
            else
            {
                return null;
            }            
        }
    }
}

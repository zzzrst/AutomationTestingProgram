// <copyright file="RunJavaScript.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Collections.Specialized;
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step skips to a specific line in the excel. ONLY WORKS FOR EXCEL
    /// </summary>
    public class SaveParameter : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SaveParameter";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            // value specifies the value of the key to update
            string value = this.Arguments["value"];
            // object specifies the key to store/update
            string objectValue = this.Arguments["object"];

            if (value == string.Empty)
            {
                Logger.Error("Wrong use of ActionOnObject, value should be filled");
                this.TestStepStatus.RunSuccessful = false;
                return;
            }

            if (objectValue == string.Empty)
            {
                Logger.Error("Wrong use of ActionOnObject, object should be filled");
                this.TestStepStatus.RunSuccessful = false;
                return;
            }

            // add parameter to information object run parameters key
            if (InformationObject.RunParameters.ContainsKey(objectValue))
            {
                if (this.Arguments["comment"].ToLower() == "y")
                {
                    InformationObject.RunParameters.Remove(objectValue);
                    InformationObject.RunParameters.Add(objectValue, value);
                    Logger.Info($"Successfully updated RunParameter {objectValue} to {value}");
                }
                else
                {
                    Logger.Warn($"Parameter already exists, not overwriting. Key: {objectValue} with value {InformationObject.RunParameters[objectValue]}");
                }
                //InformationObject.RunParameters[objectValue] = value;
            }
            else
            {
                InformationObject.RunParameters.Add(objectValue, value);
                Logger.Info($"Successfully set RunParameter {objectValue} to {value}");
            }

            this.TestStepStatus.RunSuccessful = true;
            this.TestStepStatus.Actual = "Successfully set dictionary value";
        }
    }
}

﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.18444
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace FSharpSpecTests
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class FSharpLeftistHeapFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "FSharpLeftistHeap.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "FSharpLeftistHeap", "In order to avoid silly mistakes\r\nAs a math idiot\r\nI want to be told the sum of t" +
                    "wo numbers", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((TechTalk.SpecFlow.FeatureContext.Current != null) 
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "FSharpLeftistHeap")))
            {
                FSharpSpecTests.FSharpLeftistHeapFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Use heap to sort a series of numbers")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "FSharpLeftistHeap")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("Heap")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("FSharp")]
        public virtual void UseHeapToSortASeriesOfNumbers()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Use heap to sort a series of numbers", new string[] {
                        "Heap",
                        "FSharp"});
#line 7
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "number"});
            table1.AddRow(new string[] {
                        "56"});
            table1.AddRow(new string[] {
                        "65"});
            table1.AddRow(new string[] {
                        "18"});
            table1.AddRow(new string[] {
                        "12"});
            table1.AddRow(new string[] {
                        "17"});
            table1.AddRow(new string[] {
                        "47"});
            table1.AddRow(new string[] {
                        "02"});
#line 8
 testRunner.Given("I have a list with the following numbers", ((string)(null)), table1, "Given ");
#line 17
 testRunner.When("I enter the list into the Heap", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "number"});
            table2.AddRow(new string[] {
                        "02"});
            table2.AddRow(new string[] {
                        "12"});
            table2.AddRow(new string[] {
                        "17"});
            table2.AddRow(new string[] {
                        "18"});
            table2.AddRow(new string[] {
                        "47"});
            table2.AddRow(new string[] {
                        "56"});
            table2.AddRow(new string[] {
                        "65"});
#line 18
 testRunner.Then("the numbers come out in this order", ((string)(null)), table2, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion

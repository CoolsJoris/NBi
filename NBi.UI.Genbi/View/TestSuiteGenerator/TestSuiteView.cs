﻿using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using NBi.Service;
using NBi.Service.Dto;
using NBi.UI.Genbi.Command;
using NBi.UI.Genbi.Interface;
using NBi.UI.Genbi.Presenter;
using NBi.UI.Genbi.Stateful;

namespace NBi.UI.Genbi.View.TestSuiteGenerator
{
    partial class TestSuiteView : Form
    {

        private TestSuiteState State { get; set; }
        private TestCasesPresenter TestCasesPresenter {get; set;}
        private TemplatePresenter TemplatePresenter { get; set; }
        private SettingsPresenter SettingsPresenter { get; set; }
        private TestListPresenter TestListPresenter { get; set; }
        private TestSuitePresenter TestSuitePresenter { get; set; }
        private MacroPresenter MacroPresenter { get; set; }


        public TestSuiteView()
        {
            State = new TestSuiteState();
            TestCasesPresenter = new TestCasesPresenter(new RenameVariableWindow(), new TestCasesManager(), State.TestCases, State.Variables);
            TemplatePresenter = new TemplatePresenter(new TemplateManager(), State.Template);
            SettingsPresenter = new SettingsPresenter(new SettingsManager(), State.Settings);
            TestListPresenter = new TestListPresenter(new TestListManager(), State.Tests, State.TestCases, State.Variables, State.Template);
            TestSuitePresenter = new TestSuitePresenter(new TestSuiteManager(), State.Tests, State.Settings);
            MacroPresenter = new MacroPresenter();

            InitializeComponent();
            DeclareBindings();            
            BindPresenter();
        }

        protected void DeclareBindings()
        {
            testCasesControl.DataBind(TestCasesPresenter);
            settingsControl.DataBind(SettingsPresenter);
            templateControl.DataBind(TemplatePresenter);
            testListControl.DataBind(TestListPresenter);

            TemplatePresenter.PropertyChanged += (sender, e) => TestListPresenter.Template = TemplatePresenter.Template;
            TestListPresenter.PropertyChanged += (sender, e) => TestSuitePresenter.RefreshCommands();

            TestSuitePresenter.TestSuiteLoaded += (sender, e) =>
                {
                    SettingsPresenter.Refresh();
                    TestListPresenter.Refresh();
                };
        }


        private void BindPresenter()
        {
            //TestCases & Variables
            CommandManager.Instance.Bindings.Add(this.TestCasesPresenter.OpenTestCasesCommand, openTestCasesToolStripMenuItem);
            CommandManager.Instance.Bindings.Add(this.TestCasesPresenter.OpenTestCasesCommand, openTestCasesToolStripButton);
            CommandManager.Instance.Bindings.Add(this.TestCasesPresenter.RemoveVariableCommand, testCasesControl.RemoveCommand);
            CommandManager.Instance.Bindings.Add(this.TestCasesPresenter.RenameVariableCommand, testCasesControl.RenameCommand);
            CommandManager.Instance.Bindings.Add(this.TestCasesPresenter.MoveLeftVariableCommand, testCasesControl.MoveLeftCommand);
            CommandManager.Instance.Bindings.Add(this.TestCasesPresenter.MoveRightVariableCommand, testCasesControl.MoveRightCommand);

            //Template
            CommandManager.Instance.Bindings.Add(this.TemplatePresenter.OpenTemplateCommand, openTemplateToolStripMenuItem);
            CommandManager.Instance.Bindings.Add(this.TemplatePresenter.OpenTemplateCommand, openTemplateToolStripButton);
            CommandManager.Instance.Bindings.Add(this.TemplatePresenter.SaveTemplateCommand, saveAsTemplateToolStripMenuItem);
            CommandManager.Instance.Bindings.Add(this.TemplatePresenter.SaveTemplateCommand, saveAsTemplateToolStripButton);

            //Settings
            CommandManager.Instance.Bindings.Add(this.SettingsPresenter.AddReferenceCommand, settingsControl.AddCommand);
            CommandManager.Instance.Bindings.Add(this.SettingsPresenter.RemoveReferenceCommand, settingsControl.RemoveCommand);

            //Tests
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.GenerateTestsXmlCommand, generateTestsToolStripMenuItem);
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.GenerateTestsXmlCommand, generateTestsToolStripButton);
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.ClearTestsXmlCommand, clearTestsToolStripMenuItem);
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.ClearTestsXmlCommand, clearTestsToolStripButton);
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.UndoGenerateTestsXmlCommand, undoGenerateTestsToolStripMenuItem);
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.UndoGenerateTestsXmlCommand, undoGenerateTestsToolStripButton);
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.DeleteTestCommand, testListControl.DeleteCommand);
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.DisplayTestCommand, testListControl.DisplayCommand);
            CommandManager.Instance.Bindings.Add(this.TestListPresenter.AddCategoryCommand, testListControl.AddCategoryCommand);

            //Test-suite
            CommandManager.Instance.Bindings.Add(this.TestSuitePresenter.OpenTestSuiteCommand, openTestSuiteToolStripMenuItem);
            CommandManager.Instance.Bindings.Add(this.TestSuitePresenter.OpenTestSuiteCommand, openTestSuiteToolStripButton);
            CommandManager.Instance.Bindings.Add(this.TestSuitePresenter.SaveAsTestSuiteCommand, saveAsTestSuiteToolStripMenuItem);
            CommandManager.Instance.Bindings.Add(this.TestSuitePresenter.SaveAsTestSuiteCommand, saveAsTestSuiteToolStripButton);

            CommandManager.Instance.Bindings.Add(this.MacroPresenter.PlayMacroCommand, playMacroToolStripMenuItem);
        }

        private void UnbindPresenter()
        {
            //CommandManager.Instance.Bindings.Remove(this.Presenter.RemoveVariableCommand, apply);
        }


        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var window = new AboutBox();
            window.ShowDialog(this);
        }

        private void generateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var view = Bootstrapper.GetRunnerConfigView();
            view.ShowDialog(this);
        }








        
    }
}
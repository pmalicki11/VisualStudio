using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ExcelLibrary.SpreadSheet;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace COMET
{
    public partial class InferenceForm : Form
    {
        #region Old variables
        /*
        List<MamdaniFuzzyRule> rules;
        MamdaniFuzzySystem fuzzySystem;
        FuzzyVariable[] systemInputVariables;*/
        #endregion

        List<CharacteristicObject> objectList;
        List<FuzzyVariableControl> inputVariables;
        Dictionary<String, List<Double>> criteria;
        Dictionary<String, List<TriangularMembershipFunction>> msFunctions;
        Dictionary<String, Plot> plots;
        int alternativesNumber = 0;
        List<AlternativeControl> alternativeControls;
        SaveFileDialog saveFileDialog;
        Double[,] alternativesValues;
        String rules;
        

        public InferenceForm(List<CharacteristicObject> list)
        {
            InitializeComponent();
            objectList = list;
            criteria = getCriteriaFromObjectList();
            msFunctions = genetareMembershipFunctions(criteria);
            generateControls();
            progressBar.Visible = false;
            progressLabel.Visible = false;
            rules = objectListToRules();
        }

        private String objectListToRules()
        {
            String result = "";
            for (int i = 0; i < objectList.Count; i++)
            {
                result += "R" + (i + 1) + ": ";
                for (int j = 0; j < objectList[0].Size(); j++)
                {
                    result += "IF " + objectList[i].names[j] + "~" + objectList[i].values[j];
                    if (j + 1 != objectList[0].Size())
                    {
                        result += " AND ";
                    }
                }
                result += " THEN " + objectList[i].Preference + "\n\n";
            }
            return result;
        }

        public int AlternativesNumber
        {
            set
            {
                alternativesNumber = value;
            }
        }

        public Double[,] AlternativesValues
        {
            set
            {
                this.alternativesValues = value;
            }
        }

        private void generateControls()
        {
            genPlots();
            genBoxesForInputValues();
        }

        private void genPlots()
        {
            #region OLD
            //List<String> keys = new List<String>(this.criteria.Keys);
            //charts = new List<Chart>();
            //for (int i = 0; i < criteria.Count; i++)
            //{
            //    Chart chart = new Chart();
            //    chart.Location = new Point(i * 310 + 10, 10);
            //    chart.Titles.Add(keys[i]);
            //    chart.Name = keys[i];
            //    ChartArea chartArea = new ChartArea();
            //    chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            //    chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            //    chartArea.AxisX.IsMarginVisible = false;
            //    chartArea.AxisY.IsMarginVisible = false;
            //    chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //    chart.ChartAreas.Add(chartArea);
            //    for (int j = 0; j < criteria[keys[i]].Count; j++)
            //    {
            //        Series series = new Series();
            //        series.ChartType = SeriesChartType.Line;
            //        setPointsToSeries(getPointsForSeries(keys[i], criteria[keys[i]][j]), ref series);
            //        chart.Series.Add(series);
            //    }
            //    charts.Add(chart);
            //    plotsPanel.Controls.Add(chart);
            //}
            #endregion

            List<String> variableNames = new List<String>(this.criteria.Keys);
            plots = new Dictionary<String, Plot>();

            for (int i = 0; i < criteria.Count; i++)
            {
                Plot plot = new Plot(variableNames[i]);
                plot.Location = new Point(i * 410 + 10, 10);
                for (int j = 0; j < criteria[variableNames[i]].Count; j++)
                {
                    Series series = new Series();
                    setPointsToSeries(getPointsForSeries(variableNames[i], criteria[variableNames[i]][j]), ref series);
                    series.Name = criteria[variableNames[i]][j].ToString();
                    plot.addSeries(series);
                }
                plots.Add(variableNames[i], plot);
                plotsPanel.Controls.Add(plot);
            }

        }

        private void genBoxesForInputValues()
        {
            inputVariables = new List<FuzzyVariableControl>();
            for (int i = 0; i < objectList[0].Size(); i++)
            {
                FuzzyVariableControl variableControl = new FuzzyVariableControl(objectList[0].names[i]);
                if (inputVariables.Capacity > 0)
                {
                    variableControl.Location = new Point(inputVariables.Last<FuzzyVariableControl>().Location.X, inputVariables.Last<FuzzyVariableControl>().Location.Y + 30);
                }
                else
                {
                    variableControl.Location = new Point(12, 12);
                }
                standardInferencePanel.Controls.Add(variableControl);
                inputVariables.Add(variableControl);
                checkedListBox.Items.Add(variableControl.NameOfVariable);
            }
        }

        private LinkedList<PointD> getPointsForSeries(String key, Double val)
        {
            LinkedList<PointD> points = new LinkedList<PointD>();
            List<String> keys = new List<String>(this.criteria.Keys);

            for (int i = 0; i < criteria[key].Count; i++)
            {
                if (criteria[key][i] == val)
                {
                    points.AddLast(new PointD(val, 1));
                }
                else
                {
                    points.AddLast(new PointD(criteria[key][i], 0));
                }
            }
            return points;
        }

        private void setPointsToSeries(LinkedList<PointD> points, ref Series series)
        {
            foreach (PointD point in points)
            {
                series.Points.AddXY(point.X, point.Y);
            }
        }

        private void checkSingleButton_Click(object sender, EventArgs e)
        {
            #region using of old functions
            /*generateFuzzySystem();

            Dictionary<FuzzyVariable, Double> inputValues = new Dictionary<FuzzyVariable,Double>();
            
            for (int i = 0; i < inputVariables.Count; i++)
            {
                inputValues.Add(fuzzySystem.InputByName(inputVariables[i].NameOfVariable), Convert.ToDouble(inputVariables[i].ValueOfVariable));
            }
            
            FuzzyVariable outputResult = fuzzySystem.OutputByName("output");

            Dictionary<FuzzyVariable, Double> result = fuzzySystem.Calculate(inputValues);
            

            resultTextBox.Text = result[outputResult].ToString("f4");*/
            #endregion
            #region old method
            //resultTextBox.Text = "";
            //removeIntersectionsFromPlots();

            //if (isEmptyField())
            //{
            //    MessageBox.Show("Any field can't be empty");
            //    return;
            //}

            //List<Double> endMfValue = new List<Double>();
            //for (int i = 0; i < objectList.Count; i++)
            //{
            //    Double criterionMfValue = 1.0;
            //    for (int j = 0; j < objectList[0].Size(); j++)
            //    {
            //        String criterionName = objectList[i].names[j];
            //        Double criterionValue = Convert.ToDouble(objectList[i].values[j]);
            //        Double inputX = Convert.ToDouble(inputVariables[j].ValueOfVariable);
            //        int indexOfCriterion = criteria[criterionName].IndexOf(criterionValue);

            //        Double criterionMin = criteria[criterionName].Min();
            //        Double criterionMax = criteria[criterionName].Max();

            //        if (inputX < criterionMin || inputX > criterionMax)
            //        {
            //            MessageBox.Show("Input value for " + criterionName + "(" + inputX + ") is out of domain (" + criterionMin + " - " + criterionMax + ")");
            //            return;
            //        }

            //        Double currentValue = msFunctions[criterionName][indexOfCriterion].getValue(inputX);
            //        criterionMfValue = criterionMfValue * currentValue;
                    
            //            plots[criterionName].addIntersections(criterionValue, inputX, currentValue);
                    
            //    }
            //    endMfValue.Add(criterionMfValue = criterionMfValue * objectList[i].Preference);
            //}
            //resultTextBox.Text = Math.Round(endMfValue.Sum(), 4).ToString();
            # endregion
            if (isEmptySingleMethodFields())
            {
                MessageBox.Show("Any field can't be empty");
                return;
            }
            resultTextBox.Text = "";
            removeIntersectionsFromPlots();
            resultTextBox.Text = inference(inputVariables, true);
        }

        private String inference(List<FuzzyVariableControl> inputVar, bool single)
        {
            List<Double> endMfValue = new List<Double>();
            for (int i = 0; i < objectList.Count; i++)
            {
                Double criterionMfValue = 1.0;
                for (int j = 0; j < objectList[0].Size(); j++)
                {
                    String criterionName = objectList[i].names[j];
                    Double criterionValue = Convert.ToDouble(objectList[i].values[j]);
                    Double inputX = Convert.ToDouble(inputVar[j].ValueOfVariable);
                    int indexOfCriterion = criteria[criterionName].IndexOf(criterionValue);

                    Double criterionMin = criteria[criterionName].Min();
                    Double criterionMax = criteria[criterionName].Max();

                    if (inputX < criterionMin || inputX > criterionMax)
                    {
                        MessageBox.Show("Input value for " + criterionName + "(" + inputX + ") is out of domain (" + criterionMin + " - " + criterionMax + ")");
                        return "Out of domain";
                    }

                    Double currentValue = msFunctions[criterionName][indexOfCriterion].getValue(inputX);
                    criterionMfValue = criterionMfValue * currentValue;

                    if (single)
                    {
                        plots[criterionName].addIntersections(criterionValue, inputX, currentValue);
                    }
                }
                endMfValue.Add(criterionMfValue = criterionMfValue * objectList[i].Preference);
            }
            return Math.Round(endMfValue.Sum(), 8).ToString();
        }

        private void removeIntersectionsFromPlots()
        {
            foreach (Plot plot in plots.Values)
            {
                plot.removeIntersections();
            }
        }

        private Dictionary<String, List<TriangularMembershipFunction>> genetareMembershipFunctions(Dictionary<String, List<Double>> criteria)
        {
            Dictionary<String, List<TriangularMembershipFunction>> msFunctions = new Dictionary<String, List<TriangularMembershipFunction>>();

            foreach (String name in criteria.Keys)
            {
                List<TriangularMembershipFunction> functions = new List<TriangularMembershipFunction>();

                for (int i = 0; i < criteria[name].Count; i++)
                {
                    Double x1 = 0.0;
                    Double x2 = 0.0;
                    Double x3 = 0.0;

                    x2 = criteria[name][i];

                    if (i == 0)
                    {
                        x1 = x2;
                    }
                    else
                    {
                        x1 = criteria[name][i - 1];
                    }

                    if (i + 1 == criteria[name].Count)
                    {
                        x3 = x2;
                    }
                    else
                    {
                        x3 = criteria[name][i + 1];
                    }
                    functions.Add(new TriangularMembershipFunction(x1, x2, x3));
                }
                msFunctions.Add(name, functions);
            }
            return msFunctions;
        }

        private Dictionary<string, List<double>> getCriteriaFromObjectList()
        {
            Dictionary<String, List<Double>> criteria;
            criteria = new Dictionary<string, List<Double>>();
            for (int i = 0; i < objectList[0].Size(); i++)
            {
                List<Double> criterionValues = new List<Double>();
                for (int j = 0; j < objectList.Count; j++)
                {
                    if (criterionValues.Count == 0)
                    {
                        criterionValues.Add(Convert.ToDouble(objectList[j].values[i]));
                    }
                    else if (!criterionValues.Contains(Convert.ToDouble(objectList[j].values[i])))
                    {
                        criterionValues.Add(Convert.ToDouble(objectList[j].values[i]));
                    }
                }
                criterionValues.Sort();
                criteria.Add(objectList[0].names[i], criterionValues);
            }
            return criteria;
        }

        private Boolean isEmptySingleMethodFields()
        {
            for (int i = 0; i < inputVariables.Count; i++)
            {
                if (inputVariables[i].ValueOfVariable == "")
                {
                    return true;
                }
            }
            return false;
        }

        private Boolean isEmptyMultipleMethodFields()
        {
            for (int i = 0; i < alternativeControls.Count; i++)
            {
                for (int j = 0; j < alternativeControls[i].Count; j++)
                {
                    if (alternativeControls[i].isEmpty(j))
                    {
                        MessageBox.Show("Any field can't be empty");
                        return true;
                    }
                }
            }
            return false;
        }

        private Boolean resultsCalculated()
        {
            for (int i = 0; i < alternativeControls.Count; i++)
            { 
                if (alternativeControls[i].Result == "")
                {
                    MessageBox.Show("Results not calculated!");
                    return false;
                }
            }
            return true;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            StartForm form = new StartForm();
            form.ShowDialog();
            this.Close();
        }

        private void multipleInference_Enter(object sender, EventArgs e)
        {
            alternativesValues = null;
            if (alternativeControls != null && multipleInferencePanel.Controls.Count > 0)
            {
                alternativeControls.Clear();
                multipleInferencePanel.Controls.Clear();
            }

            Form numOfAlternatives = new InferenceAlternatives(this);
            numOfAlternatives.ShowDialog();

            if (alternativesNumber > 0)
            {
                checkMultipleButton.Refresh();
                genBoxesForAlternatives();
                if (alternativesValues != null)
                {
                    fillAlternativesWithLoadedValues();
                }
                checkMultipleButton.Parent = multipleInferencePanel;
                saveMultipleResultsButton.Parent = multipleInferencePanel;
            }
            else
            {
                tabControl.SelectedTab = standartInference;
            }
        }

        private void genBoxesForAlternatives()
        {
            genHeaders();

            alternativeControls = new List<AlternativeControl>();
            for (int i = 0; i < alternativesNumber; i++)
            {
                AlternativeControl altControl = new AlternativeControl(criteria.Count(), i + 1);
                if (i == 0)
                {
                    altControl.Location = new Point(12, 30);
                }
                else
                {
                    altControl.Location = new Point(12, alternativeControls[i - 1].Location.Y + 30);
                }

                alternativeControls.Add(altControl);
                multipleInferencePanel.Controls.Add(altControl);
            }
        }

        private void fillAlternativesWithLoadedValues()
        {
            try
            {
                for (int i = 0; i < alternativesValues.GetLength(0); i++)
                {
                    for (int j = 0; j < alternativesValues.GetLength(1); j++)
                    {
                        alternativeControls[i].setCriterionValue(j, alternativesValues[i, j]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bad file format!" + ex.ToString());
            }
        }

        private void genHeaders()
        {
            for (int i = 0; i <= objectList[0].names.Length; i++)
            {
                
                Label label = new Label();
                if (i == 0)
                {
                    label.Text = "ID";
                    label.Location = new Point(12, 4);
                }
                else if (i == 1)
                {
                    label.Text = objectList[0].names[i - 1];
                    label.Location = new Point(63, 4);
                }
                else
                {
                    label.Text = objectList[0].names[i - 1];
                    label.Location = new Point(63 + (i - 1) * 80, 4);
                }
                label.AutoSize = true;
                label.TextAlign = ContentAlignment.MiddleCenter;
                multipleInferencePanel.Controls.Add(label);
            }
        }

        private void checkMultipleButton_Click(object sender, EventArgs e)
        {
            if (isEmptyMultipleMethodFields())
            {
                return;
            }

            removeIntersectionsFromPlots();
            for (int i = 0; i < alternativeControls.Count; i++)
            {
                alternativeControls[i].Result = inference(convertToFuzzyVariablesList(alternativeControls.ElementAt(i)), false);
            }
        }

        private List<FuzzyVariableControl> convertToFuzzyVariablesList(AlternativeControl alternativeControl)
        {
            List<FuzzyVariableControl> fuzzyVariables = new List<FuzzyVariableControl>();
            for (int i = 0; i < inputVariables.Count; i++)
            {
                FuzzyVariableControl fVar = new FuzzyVariableControl(criteria.Keys.ElementAt(i));
                fVar.ValueOfVariable = alternativeControl.getCriterionValue(i).ToString();
                fuzzyVariables.Add(fVar);
            }
            return fuzzyVariables;
        }

        private void saveMultipleResultsButton_Click(object sender, EventArgs e)
        {
            if (isEmptyMultipleMethodFields() | !resultsCalculated())
            {
                return;
            }
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Microsoft Excel Worksheet (*.xls)|*.xls";
            //saveFileDialog.Filter = "Microsoft Excel Worksheet (*.xls)|*.xls|XML File (*.xml)|*.xml";
            saveFileDialog.FileOk += saveFileDialog_FileOk;
            saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            String[,] input = new String[alternativeControls.Count + 1, criteria.Count + 2];
            
            input[0, 0] = "Id";
            for (int i = 0 ; i < criteria.Count; i++)
            {
                input[0, i + 1] = criteria.Keys.ElementAt(i);
            }
            input[0, criteria.Count + 1] = "Result";

            for (int i = 0; i < alternativeControls.Count; i++)
            {
                input[i + 1, 0] = (i + 1).ToString();
                for (int j = 0; j < criteria.Count; j++)
                {
                    input[i + 1, j + 1] = alternativeControls[i].getCriterionValue(j).ToString();
                }
                input[i + 1, criteria.Count + 1] = alternativeControls[i].Result.ToString();
            }

            try
            {
                ExcelFileManager.saveAlternativesResultsToFile(input, saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + "\nProgram restart needed");
            }
        }

        private void checkSensitivity_Click(object sender, EventArgs e)
        {
            if (resultTextBox.Text == "" || resultTextBox.Text == "Out of domain")
            {
                MessageBox.Show("Base result not calculated. Check for result first.");
                return;
            }

            SensitivityResults sResults = new SensitivityResults(getStringFromInputVariables(inputVariables), resultTextBox.Text);
            String title;

            for (int i = 0; i < inputVariables.Count; i++)
            {
                List<FuzzyVariableControl> inputVars = copyList(inputVariables);
                Double varValue = Convert.ToDouble(inputVars[i].ValueOfVariable);
                
                Double lowerVarValue = varValue - ((Convert.ToDouble(changeNumeric.Value) * varValue) / 100);
                inputVars[i].ValueOfVariable = lowerVarValue.ToString();
                String lowerResult = inference(inputVars, false);
                //if (lowerResult == "" || lowerResult == "Out of domain")
                //{
                //    return;
                //}
                title = inputVars[i].NameOfVariable + " - " + changeNumeric.Value.ToString() + "%";
                sResults.addResult(title, getStringFromInputVariables(inputVars), lowerResult);


                Double upperVarValue = varValue + ((Convert.ToDouble(changeNumeric.Value) * varValue) / 100);
                inputVars[i].ValueOfVariable = upperVarValue.ToString();
                String upperResult = inference(inputVars, false);
                //if (upperResult == "" || upperResult == "Out of domain")
                //{
                //    return;
                //}
                title = inputVars[i].NameOfVariable + " + " + changeNumeric.Value.ToString() + "%";
                sResults.addResult(title, getStringFromInputVariables(inputVars), upperResult);
            }
            sResults.StartPosition = FormStartPosition.CenterScreen;
            sResults.Show();
        }

        private String getStringFromInputVariables(List<FuzzyVariableControl> inputVars)
        {
            String res = "";
            for (int i = 0; i < inputVars.Count; i++)
            {
                res += inputVars[i].NameOfVariable + ": " + inputVars[i].ValueOfVariable + "\n";
            }
            return res;
        }

        private List<FuzzyVariableControl> copyList(List<FuzzyVariableControl> source)
        {
            List<FuzzyVariableControl> res = new List<FuzzyVariableControl>();
            for (int i = 0; i < source.Count; i++)
            {
                res.Add(new FuzzyVariableControl(source[i].NameOfVariable));
                res[i].ValueOfVariable = source[i].ValueOfVariable;
            }
            return res;
        }

        private void sensitivityGraph_Click(object sender, EventArgs e)
        {
            if (checkedListBox.CheckedIndices.Count != 2)
            {
                MessageBox.Show("Please select 2 criteria");
                return;
            }
            

            List<FuzzyVariableControl> toInference = copyList(inputVariables);
            List<Double> firstList = null;
            List<Double> secondList = null;
            int firstListIndex = 0;
            int secondListIndex = 0;

            for (int i = 0; i < inputVariables.Count; i++)
            {
                if (checkedListBox.CheckedIndices.Contains(i))
                {
                    if (firstList == null)
                    {
                        firstList = genArrayFromCriterion(i, (int)changeNumeric.Value);
                        firstListIndex = i;
                    }
                    else
                    {
                        secondList = genArrayFromCriterion(i, (int)changeNumeric.Value);
                        secondListIndex = i;
                    }
                }
                else if (inputVariables[i].ValueOfVariable.Length > 0)
                {
                    toInference[i].ValueOfVariable = inputVariables[i].ValueOfVariable;
                }
                else
                {
                    MessageBox.Show("Input variable " + inputVariables[i].NameOfVariable + " can't be empty!");
                    return;
                }
            }

            Double[,] resultsArray = new Double[firstList.Count, secondList.Count];

            progressBar.Visible = true;
            progressLabel.Visible = true;
            progressLabel.Text = "Calculating data for graph...";

            for (int i = 0; i < firstList.Count; i++)
            {
                for (int j = 0; j < secondList.Count; j++)
                {
                    toInference[firstListIndex].ValueOfVariable = firstList[i].ToString();
                    toInference[secondListIndex].ValueOfVariable = secondList[j].ToString();
                    resultsArray[i, j] = Convert.ToDouble(inference(toInference, false));
                }
                updateProgressBar(i, firstList.Count);
            }


            Excel.Application xlApp;
            Excel.Workbook xlWorkbook;
            Excel.Worksheet xlWorksheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            xlWorkbook = xlApp.Workbooks.Add(misValue);
            xlWorksheet = (Excel.Worksheet)xlWorkbook.Worksheets.get_Item(1);


            progressLabel.Text = "Preparing graph data...";
            for (int i = 0; i < firstList.Count; i++)
            {
                xlWorksheet.Cells[1, i + 2] = firstList[i];
                updateProgressBar(i, firstList.Count);
            }
            
            for (int i = 0; i < secondList.Count; i++)
            {
                xlWorksheet.Cells[i + 2, 1] = secondList[i];
                updateProgressBar(i, secondList.Count);
            }

            for (int i = 0; i < firstList.Count; i++)
            {
                for (int j = 0; j < secondList.Count; j++)
                {
                    xlWorksheet.Cells[i + 2, j + 2] = resultsArray[i, j];
                }
                updateProgressBar(i, firstList.Count);
            }


            progressLabel.Text = "Seting up graph axis...";
            Excel.Range chartRange;
            Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorksheet.ChartObjects(Type.Missing);
            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(30, 30, 600, 450);
            Excel.Chart chartPage = myChart.Chart;
            Console.WriteLine(getExcelColumnName(firstList.Count() + 1));
            chartRange = xlWorksheet.get_Range("B2", getExcelColumnName(firstList.Count + 1).ToString() + (secondList.Count + 1).ToString());
            chartPage.SetSourceData(chartRange, misValue);
            chartPage.ChartType = Excel.XlChartType.xlSurfaceWireframe;
            Excel.SeriesCollection series1 = (Excel.SeriesCollection)myChart.Chart.SeriesCollection(misValue);
            
            for (int i = 1; i <= series1.Count; i++)
            {
                series1.Item(i).Name = secondList[i - 1].ToString();
                updateProgressBar(i, series1.Count);
            }

            if (Convert.ToDouble(xlApp.Version.Replace(".", ",")) < 15)
            {
                MessageBox.Show("Version of your office is too old.\nSome chart axis descriptions may be incorrect.");
            }
            else
            {
                Excel.Range XVal_Range = xlWorksheet.get_Range("B1", getExcelColumnName(firstList.Count + 1).ToString() + "1");
                Excel.Axis xAxis = (Excel.Axis)chartPage.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
                xAxis.CategoryNames = XVal_Range;
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = inputVariables[(int)checkedListBox.CheckedIndices[0]].NameOfVariable;

                Excel.Axis yAxis = (Excel.Axis)chartPage.Axes(Excel.XlAxisType.xlSeriesAxis, Excel.XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true;
                yAxis.AxisTitle.Text = inputVariables[(int)checkedListBox.CheckedIndices[1]].NameOfVariable;
            }

            progressBar.Hide();
            progressLabel.Hide();
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Microsoft Excel Worksheet (*.xls)|*.xls";
            //saveFileDialog.Filter = "Microsoft Excel Worksheet (*.xls)|*.xls|XML File (*.xml)|*.xml";
            String patch = null;
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    patch = saveFileDialog.FileName;
                    xlWorkbook.SaveAs(patch, Excel.XlFileFormat.xlWorkbookNormal, 
                        misValue, misValue, misValue, misValue, 
                        Excel.XlSaveAsAccessMode.xlExclusive, 
                        misValue, misValue, misValue, misValue, misValue);
                    xlWorkbook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    if (MessageBox.Show("File saved. Would you like to open it?", "Saved", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.EnableRaisingEvents = false;
                        proc.StartInfo.FileName = patch;
                        proc.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private List<Double> genArrayFromCriterion(int index, int step)
        {
            Double min = criteria[inputVariables[index].NameOfVariable].Min();
            Double max = criteria[inputVariables[index].NameOfVariable].Max();
            Double range = max - min;
            List<Double> values = new List<double>();

            for (int i = 0; i <= 100; i = i + step)
            {
                if (i == 0)
                {
                    values.Add(min);
                }
                else
                {
                    values.Add(min + (range * i / 100));
                }
            }
            return values;
        }

        private string getExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        private void updateProgressBar(int i, int range)
        {
            progressBar.Value = (int)((i * 100) / range);
        }

        private void ruleBase_Click(object sender, EventArgs e)
        {
            Rule_base ruleBase = new Rule_base(rules);
            ruleBase.Show();
        }

        private void saveResults_Click(object sender, EventArgs e)
        {
            if (checkedListBox.CheckedIndices.Count != 2)
            {
                MessageBox.Show("Please select 2 criteria");
                return;
            }


            List<FuzzyVariableControl> toInference = copyList(inputVariables);
            List<Double> firstList = null;
            List<Double> secondList = null;
            int firstListIndex = 0;
            int secondListIndex = 0;

            for (int i = 0; i < inputVariables.Count; i++)
            {
                if (checkedListBox.CheckedIndices.Contains(i))
                {
                    if (firstList == null)
                    {
                        firstList = genArrayFromCriterion(i, (int)changeNumeric.Value);
                        firstListIndex = i;
                    }
                    else
                    {
                        secondList = genArrayFromCriterion(i, (int)changeNumeric.Value);
                        secondListIndex = i;
                    }
                }
                else if (inputVariables[i].ValueOfVariable.Length > 0)
                {
                    toInference[i].ValueOfVariable = inputVariables[i].ValueOfVariable;
                }
                else
                {
                    MessageBox.Show("Input variable " + inputVariables[i].NameOfVariable + " can't be empty!");
                    return;
                }
            }

            Double[,] resultsArray = new Double[firstList.Count, secondList.Count];

            progressBar.Visible = true;
            progressLabel.Visible = true;
            progressLabel.Text = "Calculating data..";

            for (int i = 0; i < firstList.Count; i++)
            {
                for (int j = 0; j < secondList.Count; j++)
                {
                    toInference[firstListIndex].ValueOfVariable = firstList[i].ToString();
                    toInference[secondListIndex].ValueOfVariable = secondList[j].ToString();
                    resultsArray[i, j] = Convert.ToDouble(inference(toInference, false));
                }
                updateProgressBar(i, firstList.Count);
            }

            progressBar.Hide();
            progressLabel.Hide();
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TXT file (*.txt)|*.txt";
            String patch = null;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    patch = saveFileDialog.FileName.Remove(saveFileDialog.FileName.Length - 4);
                    String timeNow = DateTime.Now.ToString().Replace("-", "").Replace(":", "");

                    String file1Patch = patch + "_" + checkedListBox.CheckedItems[0] + "_" + timeNow + ".txt";
                    String file2Patch = patch + "_" + checkedListBox.CheckedItems[1] + "_" + timeNow + ".txt";
                    String file3Patch = patch + "_Results_" + timeNow + ".txt";
                    System.IO.StreamWriter file1 = new System.IO.StreamWriter(file1Patch, true);
                    System.IO.StreamWriter file2 = new System.IO.StreamWriter(file2Patch, true);
                    System.IO.StreamWriter file3 = new System.IO.StreamWriter(file3Patch, true);

                    for (int i = 0; i < firstList.Count; i++)
                    {
                        for (int j = 0; j < secondList.Count; j++)
                        {
                            file1.Write(firstList[i].ToString().Replace(',', '.') + " ");
                            file2.Write(secondList[j].ToString().Replace(',', '.') + " ");
                            file3.Write(resultsArray[i, j].ToString().Replace(',', '.') + " ");
                        }
                        file1.Write("\n");
                        file2.Write("\n");
                        file3.Write("\n");
                    }
                    file1.Close();
                    file2.Close();
                    file3.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        #region Old functions
        /*private void generateFuzzySystem()
        {
            fuzzySystem = new MamdaniFuzzySystem();
            fuzzySystem.AggregationMethod = AI.Fuzzy.Library.AggregationMethod.Sum;
            generateSystemInputVariables();
            generateSystemOutputVariables();
            generateSystemRules();
        }

        private void generateSystemInputVariables()
        {
            systemInputVariables = new FuzzyVariable[objectList[0].Size()];
            for (int i = 0; i < objectList[0].Size(); i++)
            {
                systemInputVariables[i] = new FuzzyVariable(objectList[0].names[i], 0.0, 1000000.0);
                List<String> values = new List<String>();
                for (int j = 0; j < objectList.Count; j++)
                {
                    if (!values.Contains(objectList[j].values[i]))
                    {
                        values.Add(objectList[j].values[i]);
                    }
                }

                values.Sort();

                for (int j = 0; j < values.Count; j++)
                {
                    Double x1 = 0.0;
                    Double x2 = 0.0;
                    Double x3 = 0.0;

                    if (j == 0)
                    {
                        x1 = -1.0;
                        x2 = Convert.ToDouble(values[j]);
                        x3 = Convert.ToDouble(values[j + 1]);
                    }
                    else if (j == values.Count - 1)
                    {
                        x1 = Convert.ToDouble(values[j - 1]);
                        x2 = Convert.ToDouble(values[j]);
                        x3 = 1000000.0;
                    }
                    else
                    {
                        x1 = Convert.ToDouble(values[j - 1]);
                        x2 = Convert.ToDouble(values[j]);
                        x3 = Convert.ToDouble(values[j + 1]);
                    }
                    systemInputVariables[i].Terms.Add(new FuzzyTerm(values[j].Replace(",", "."), new TriangularMembershipFunction(x1, x2, x3)));
                }
                fuzzySystem.Input.Add(systemInputVariables[i]);
            }
        }

        private void generateSystemOutputVariables()
        {
            objectList = objectList.OrderBy(o => o.Preference).ToList();
            FuzzyVariable outputVariable = new FuzzyVariable("output", 0.0, 1.0);
            for (int i = 0; i < objectList.Count; i++)
            {
                Double x1 = 0.0;
                Double x2 = 0.0;
                Double x3 = 0.0;

                if (i == 0)
                {
                    x1 = -1.0;
                    x2 = objectList[i].Preference;
                    x3 = objectList[i + 1].Preference;
                }
                else if (i == objectList.Count - 1)
                {
                    x1 = objectList[i - 1].Preference;
                    x2 = objectList[i].Preference;
                    x3 = 2.0;
                }
                else
                {
                    x1 = objectList[i - 1].Preference;
                    x2 = objectList[i].Preference;
                    x3 = objectList[i + 1].Preference;
                }
                outputVariable.Terms.Add(new FuzzyTerm(objectList[i].Preference.ToString().Replace(",", "."), new TriangularMembershipFunction(x1, x2, x3)));
            }
            fuzzySystem.Output.Add(outputVariable);

        }

        private void generateSystemRules()
        {   
            rules = new List<MamdaniFuzzyRule>();
            for (int i = 0; i < objectList.Count(); i++)
            {
                String ruleString = "if ";
                for (int j = 0; j < inputVariables.Count(); j++)
                {                   
                    ruleString += "(" + objectList[0].names[j] + " is " + objectList[i].values[j].ToString() + ")";
                    if (j < inputVariables.Count() - 1)
                    {
                        ruleString += " and ";
                    }
                }
                ruleString += " then (output is " + objectList[i].Preference.ToString() + ")";
                ruleString = ruleString.Replace(",", ".");
                try
                {
                    fuzzySystem.Rules.Add(fuzzySystem.ParseRule(ruleString));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Parsing exception: {0}", ex.Message));
                }
            }
        }*/
        #endregion
    }
}
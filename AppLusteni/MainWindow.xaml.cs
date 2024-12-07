using Lusteni.Sudoku;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
//using Tesseract;
using Lusteni.Osmismerky;
using System.Runtime.CompilerServices;
using Lusteni;

[assembly: InternalsVisibleTo("LusteniUnitTests")]

namespace Osmismerky
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<OsmiSmerka> _8smerkyCollection;

        private List<Sudoku> _sudokuCollecction;

        private Dictionary<string, string> _imgFilesRegister;

        public MainWindow()
        {
            InitializeComponent();
        }

        //protected static TesseractEngine CreateEngine(string lang = "eng", EngineMode mode = EngineMode.TesseractOnly)
        //{
        //    var datapath = @"C:\Users\hynek\source\repos\Osmismerky\_tessdata";
        //    return new TesseractEngine(datapath, lang, mode);
        //}

        private string _ImageOCR_ResultFile(string imageFileName)
        {
            string _ocr_fileName = string.Empty;

            string _imgFileName = imageFileName.Substring(imageFileName.LastIndexOf(@"\"));

            string _number = Regex.Match(_imgFileName, @"\d+").Value;

                string _resultFileName = _imgFileName.Substring(_imgFileName.LastIndexOf("."));
                return String.Format("{0}.txt", _resultFileName);
            
        }


        private string _getCleanLines(string source)
        {
            var _results_clean = new List<string>();            
            string[] _splits = source.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var raw_line in _splits)
            {
                if(raw_line.Length > 3)
                {
                    _results_clean.Add(raw_line.ToString());
                }
                //raw_line.Value
            }
            return string.Join(Environment.NewLine, _results_clean);
        }

        private void _MarkImgItemWithDonePrefix(string imgFileName)
        {
            var _targetItemIndex = 0;
            bool _done = false;
            foreach(var _imgItem in LbImages.Items)
            {                
                if(_imgItem.ToString() == imgFileName)
                {
                    _done = true;
                    break;
                }
                _targetItemIndex++;
            }
            if (_done)
            {
                LbImages.Items[_targetItemIndex] = string.Format("{0} {1}", Lusteni.Const.Char_Done, imgFileName);
                LbImages.UpdateLayout();
            }
        }

        private void _MarkTheJobItemWithDonePrefix(string itemName)
        {
            var _targetItemIndex = 0;
            bool _done = false;
            foreach (var _jobItem in LbOutput.Items)
            {
                if (_jobItem.ToString() == itemName)
                {
                    _done = true;
                    break;
                }
                _targetItemIndex++;
            }
            if (_done)
            {
                LbOutput.Items[_targetItemIndex] = string.Format("{0} {1}", Lusteni.Const.Char_Done, itemName);
                LbOutput.UpdateLayout();
            }
        }
        //private void ConvertImagesUsingTesseractOCR()
        //{
        //    using (var engine = CreateEngine(/*"ces"*/))
        //    {
        //        var _imgsToConvert = new List<string>();
        //        foreach (string _imgItem in LbImages.SelectedItems)
        //        {
        //            _imgsToConvert.Add(_imgItem.ToString());
        //        }

        //        foreach (string _fileConverting in _imgsToConvert)
        //        {
        //            _imgFilesRegister.TryGetValue(_fileConverting, out string _imgFileName);

        //            var timeStamp_start = DateTime.Now.Ticks;

        //            var image = Pix.LoadFromFile(_imgFileName);
        //            var page = engine.Process(image, PageSegMode.SparseText);
        //            var text = page.GetText();
        //            var text_clean = _getCleanLines(text);

        //            var _resultFileName = _imgFileName.Replace(".jpg", ".txt").Replace("_ImagesData", "_OCR_ResultFiles");
                    
        //            var timeStamp_finish = DateTime.Now.Ticks;
                    
        //            var elapsedMilliSec = (int)TimeSpan.FromTicks(timeStamp_finish - timeStamp_start).TotalMilliseconds; //[ms]

        //            string _report = string.Format("Duration: {0} [milli Sec]{1}{1}<--OCR Data-->{1}{2}", elapsedMilliSec, Environment.NewLine, text_clean);

        //            File.WriteAllText(_resultFileName, _report);

        //            // mark the item with ✔️ prefix
        //            _MarkImgItemWithDonePrefix(_fileConverting);

        //            //Thread.Sleep(15000);
        //            page.Dispose();
        //        }
        //    }

        //}

        private bool _is8smerkaSelectedToResolveNow(OsmiSmerka _8smerka)
        {
            var _isSelected = false;
            foreach(string _selItem in LbOutput.SelectedItems)
            {
                if (_selItem.Contains(Lusteni.Const.Char_Done)) continue;
                if(_selItem == _8smerka.Name) { _isSelected = true; break; }
            }
            return _isSelected;
        }

        private bool _isSudokuSelectedToResolveNow(Sudoku sudoku)
        {
            var _isSelected = false;
            foreach (string _selItem in LbOutput.SelectedItems)
            {
                if (_selItem.Contains(Lusteni.Const.Char_Done)) continue;
                if (_selItem == sudoku.Name) { _isSelected = true; break; }
            }
            return _isSelected;
        }

        //private void TestIronOCR()
        //{
        //    IronTesseract IronOcr = new IronTesseract();
        //    IronOcr.Language = OcrLanguage.Czech;
        //    var Result = IronOcr.Read(@"C:\Users\hynek\source\repos\Osmismerky\_ImagesData\osmi_8_core.jpg");
        //    var text = Result.Text;
        //    File.WriteAllText(@"C:\Users\hynek\source\repos\Osmismerky\_OutputTexts\osmi_8_core_ironOCR.txt", text);
        //}

        private void BtnConvertImages_Click(object sender, RoutedEventArgs e)
        {
            //ConvertImagesUsingTesseractOCR();
        }

        private void BtnDoTheJob_Click(object sender, RoutedEventArgs e)
        {
            if(chb8smerky.IsChecked == true)
            {
                Solve8Smerky();
            }
            else if(chbSudoku.IsChecked == true)
            {
                SolveSudoku();
            }
        }

        private void Solve8Smerky()
        {
            var timeStamp_t0 = DateTime.Now.Ticks;
            string _reportOverall = string.Empty;
            int _osmiIterator = 1;

            var selected8smerky = Get8smerkyListToBeSolved();

            Parallel.ForEach(selected8smerky, osmi =>
            //foreach (var _osmi in selected8smerky)
            {

                var timeStamp_start = DateTime.Now.Ticks;

                osmi.LoadCoreData();
                osmi.LoadWordsDictionary();
                var result = osmi.Resolve();

                var timeStamp_finish = DateTime.Now.Ticks;
                long elapsedTicksMs = (timeStamp_finish - timeStamp_start) / 10; //[mikro sec]

                string _report = String.Format("Duration: {0} [{3}s],{1}{1}Tajenka: {2}", elapsedTicksMs, Environment.NewLine, osmi.Result, Const.Char_Micro);

                File.WriteAllText(Path.Combine(Lusteni.Tools.TryGetSolutionDirectoryInfo(), "_ResultFiles", osmi.ResultFile), _report);

                _reportOverall += String.Format("{0}. {1}\t{2}\t{3} [{4}s]\n", _osmiIterator++, osmi.Name, result.ToString(), elapsedTicksMs, Const.Char_Micro);

            });

            var timeStamp_done = DateTime.Now;
            long durationAllMs = (timeStamp_done.Ticks - timeStamp_t0) / 10; //[mikro sec]
            _reportOverall += String.Format("Duration All: {0} [{1}s]\n", durationAllMs, Const.Char_Micro);
            string reportOverall_filename = $"{timeStamp_done.Year}-{timeStamp_done.Month}-{timeStamp_done.Day}_{timeStamp_done.Hour}h{timeStamp_done.Minute}m{timeStamp_done.Second}s_reportOverall.txt";
            File.WriteAllText(Path.Combine(Lusteni.Tools.TryGetSolutionDirectoryInfo(), "_ResultFiles", reportOverall_filename), _reportOverall);

            foreach (var osmi in selected8smerky)
            {
                if (osmi.ResultStatus == OsmiStatus.Completed)
                    _MarkTheJobItemWithDonePrefix(osmi.Name);
            }
        }

        private IList<Sudoku> GetSudokuListToBeSolved()
        {
            var sudokuList = new List<Sudoku>();

            foreach (var s in _sudokuCollecction)
            {
                if(s.IsValid && _isSudokuSelectedToResolveNow(s))
                {
                    sudokuList.Add(s);
                }
            }
            return sudokuList;
        }

        private IList<OsmiSmerka> Get8smerkyListToBeSolved()
        {
            var osmiList = new List<OsmiSmerka>();

            foreach (var osmi in _8smerkyCollection)
            {
                if (osmi.IsValid && _is8smerkaSelectedToResolveNow(osmi))
                {
                    osmiList.Add(osmi);
                }
            }
            return osmiList;
        }

        private void SolveSudoku()
        {
            var timeStamp_t0 = DateTime.Now.Ticks;
            string _reportOverall = string.Empty;
            int _sudokuIterator = 1;

            var selectedSudoku = GetSudokuListToBeSolved();
            //foreach (var sudoku in _sudokuCollecction)
            Parallel.ForEach(selectedSudoku, sudoku =>
            {
                var timeStamp_start = DateTime.Now.Ticks;

                var result = sudoku.Solve();

                var timeStamp_finish = DateTime.Now.Ticks;

                long elapsedTicksMs = (timeStamp_finish - timeStamp_start) / 10; //[mikro sec]

                string _report = String.Format("Duration: {0} [{1}s]\n", elapsedTicksMs, Const.Char_Micro);
                _report += String.Format("Status: {0}\n\n", result.ToString());
                _report += String.Format("Řešení:\n{0}", sudoku.GetResult());

                File.WriteAllText(Path.Combine(Lusteni.Tools.TryGetSolutionDirectoryInfo(), "_ResultFiles", sudoku.ResultFileName), _report);

                _reportOverall += String.Format("{0}. {1}\t{2}\t{3} [{4}s]\n", _sudokuIterator++, sudoku.Name, result.ToString(), elapsedTicksMs, Const.Char_Micro);
            });


            var timeStamp_done = DateTime.Now;
            long durationAllMs = (timeStamp_done.Ticks - timeStamp_t0) / 10; //[mikro sec]
            _reportOverall += String.Format("Duration All: {0} [{1}s]\n", durationAllMs, Const.Char_Micro);
            string reportOverall_filename = $"{timeStamp_done.Year}-{timeStamp_done.Month}-{timeStamp_done.Day}_{timeStamp_done.Hour}h{timeStamp_done.Minute}m{timeStamp_done.Second}s_reportOverall.txt";
            File.WriteAllText(Path.Combine(Lusteni.Tools.TryGetSolutionDirectoryInfo(), "_ResultFiles", reportOverall_filename), _reportOverall);

            foreach (var sudoku in selectedSudoku)
            {
                if(sudoku.IsResolved)
                    _MarkTheJobItemWithDonePrefix(sudoku.Name);
            }
        }


        private void BtnSetSource_Click(object sender, RoutedEventArgs e)
        {
            if (chb8smerky.IsChecked == true)
            {
                Set8smerkyData();
            }
            else if (chbSudoku.IsChecked == true)
            {
                SetSudokuInputData();
            }
        }

        private void Set8smerkyData()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = @"C:\Users\hynek\source\repos\Osmismerky\_inputs_2024-10_OCR\chat_GPT",
                Filter = "8-smerky (*.txt;)|*.TXT",
                Title = "Select 8-smerky files"
            };

            openFileDialog.ShowDialog();
            LbOutput.Items.Clear();

            _8smerkyCollection = new List<OsmiSmerka>();
            OsmiSmerka _8smerka = new OsmiSmerka();

            for (int itemIndex = 0; itemIndex < openFileDialog.FileNames.Count(); itemIndex++)
            {
                int startIndex = openFileDialog.FileNames[itemIndex].LastIndexOf(@"\");

                var _path = openFileDialog.FileNames[itemIndex].Substring(0, startIndex);
                var _fileName = openFileDialog.FileNames[itemIndex].Substring(startIndex + 1);

                _8smerka.AssignFile(_fileName, _path);

                if (_8smerka.IsValid)
                {
                    _8smerkyCollection.Add(_8smerka);
                    LbOutput.Items.Add(_8smerka.Name);
                    _8smerka = new OsmiSmerka();
                }
            }
        }

        private void SetSudokuInputData()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = @"C:\Users\hynek\source\repos\Osmismerky\Sudoku\SudokuData",
                Filter = "Sudoku (*.txt;)|*.TXT",
                Title = "Select Sudoku files"
            };

            openFileDialog.ShowDialog();
            LbOutput.Items.Clear();

            _sudokuCollecction = new List<Sudoku>();         
            for (int itemIndex = 0; itemIndex < openFileDialog.FileNames.Count(); itemIndex++)
            {
                int startIndex = openFileDialog.FileNames[itemIndex].LastIndexOf(@"\");

                //var _path = openFileDialog.FileNames[itemIndex].Substring(0, startIndex);
                var _fileName = openFileDialog.FileNames[itemIndex].Substring(startIndex + 1);

                Sudoku sudoku = new Sudoku(openFileDialog.FileNames[itemIndex]);

                _sudokuCollecction.Add(sudoku);
                LbOutput.Items.Add(_fileName);

            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = @"C:\Users\hynek\source\repos\Osmismerky\_ImagesData",
                Filter = "Images (*.jpg)|*.JPG|(*.*;)|*.*",
                Title = "Select Image files"
            };

            openFileDialog.ShowDialog();
            LbImages.Items.Clear();
            
            _imgFilesRegister = new Dictionary<string, string>();

            foreach (var fileName in openFileDialog.FileNames)
            {
                _imgFilesRegister.Add(Path.GetFileName(fileName), fileName);
                LbImages.Items.Add(Path.GetFileName(fileName));
            }
            
        }

        private void CheckBox_CZ_Changed(object sender, RoutedEventArgs e)
        {
            AppConfig.IsCZElanguage = Convert.ToBoolean(CheckBox_CZ.IsChecked);
        }

        private void CheckBox_SelectAllImages_Checked(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(CheckBox_SelectAllImages.IsChecked))
            {
                LbImages.SelectAll();
            }
            else
            {
                LbImages.UnselectAll();
            }

        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            CheckBox_CZ_Changed(sender, new RoutedEventArgs());
        }

        private void CheckBox_SelectAllOsmi_Checked(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(CheckBox_SelectAllOsmi.IsChecked))
            {
                LbOutput.SelectAll();
            }
            else
            {
                LbOutput.UnselectAll();
            }
        }

        private void chb8smerky_Checked(object sender, RoutedEventArgs e)
        {
            chbSudoku.IsChecked = !chb8smerky.IsChecked;
        }

        private void chbSudoku_Checked(object sender, RoutedEventArgs e)
        {
            chb8smerky.IsChecked = !chbSudoku.IsChecked;
        }
    }
}

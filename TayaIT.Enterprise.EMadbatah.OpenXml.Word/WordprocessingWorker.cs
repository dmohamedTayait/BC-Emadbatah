using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Validation;
using System.IO;
using System.Xml;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Xml.Linq;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using OVML = DocumentFormat.OpenXml.Vml.Office;
using V = DocumentFormat.OpenXml.Vml;
using System.Text.RegularExpressions;
using System.Threading;

namespace TayaIT.Enterprise.EMadbatah.OpenXml.Word
{
    public class WordprocessingWorker : IDisposable
    {
        //http://stackoverflow.com/questions/3308299/replace-bookmark-text-in-word-file-using-open-xml-sdk
        private WordprocessingDocument _currentDoc;
        private MainDocumentPart _docMainPart;

        private StyleDefinitionsPart _docStylePart;
        private DocumentSettingsPart _docSettingsPart;
        private FooterPart _docFooterPart;
        private HeaderPart _docHeaderPart;
        private string _docHeaderString;
        private ThemePart _themePart;
        private WebSettingsPart _webSettingsPart;
        private ExtendedFilePropertiesPart _extendedFilePropertiesPart;
        private NumberingDefinitionsPart _numberingDefinitionsPart;
        private EndnotesPart _endnotesPart;
        private FontTablePart _fontTablePart;
        private FootnotesPart _footnotesPart;
        private Footnotes _footNotes;
        private CoreFilePropertiesPart _coreFilePropertiesPart;
        private DocFileOperation _docOperation;

        public MainDocumentPart DocMainPart
        {
            get
            {
                return _docMainPart;
            }
        }
        public string DocHeaderString
        {
            get
            {
                return _docHeaderString;
            }
            set
            {
                _docHeaderString = value;
            }
        }

        //private Body _body;
        private MemoryStream _inMemoryStream;
        private string _outputPath;


        public String DocPath { get; set; }
        public bool IsOpen
        {
            get { return (_currentDoc != null); }
        }

        private DocXmlParts _docXmlParts;

        public WordprocessingWorker(string docPath, DocXmlParts docXmlParts, DocFileOperation fileOperation)
        {
            DocPath = docPath;
            _docXmlParts = docXmlParts;
            _docOperation = fileOperation;
            _footNotes = new Footnotes();
            if (fileOperation == DocFileOperation.CreateNew)
            {
                _currentDoc = WordprocessingDocument.Create(DocPath, WordprocessingDocumentType.Document);
                _docMainPart = _currentDoc.AddMainDocumentPart();
                _docMainPart.Document = MakeEmpyDocument();
                InitializeDocumentPartsFromXml();
                InitializeDocumentStyles();
            }
            else
            {
                Open(docPath);
            }
            //_docMainPart.Document = new Document();
            //_body = new Body();
            //InitializeDocumentPartsFromXml();
            //InitializeDocumentStyles();
        }

        public WordprocessingWorker(string docPath, DocXmlParts docXmlParts, DocFileOperation fileOperation,bool loadStyles)
        {
            DocPath = docPath;
            _docXmlParts = docXmlParts;
            _docOperation = fileOperation;
            _footNotes = new Footnotes();
            if (fileOperation == DocFileOperation.CreateNew)
            {
                _currentDoc = WordprocessingDocument.Create(DocPath, WordprocessingDocumentType.Document);
                _docMainPart = _currentDoc.AddMainDocumentPart();
                _docMainPart.Document = MakeEmpyDocument();
                InitializeDocumentPartsFromXml();
                InitializeDocumentStyles();
            }
            else
            {
                Open(docPath);
                if (loadStyles)
                    InitializeDocumentStyles();
            }
            SectionProperties sectionProps = new SectionProperties();
            PageMargin pageMargin = new PageMargin() { Top = 1440, Right = (UInt32Value)1440U, Bottom = 1440, Left = (UInt32Value)1440U, Header = (UInt32Value)850U, Footer = (UInt32Value)130U, Gutter = (UInt32Value)0U };//def; 1440
            PageSize pageSize = new PageSize() { Width = (UInt32Value)11908U, Height = (UInt32Value)16833U };
            //Spacing space = new Spacing (){ 
            sectionProps.Append(pageMargin);
            sectionProps.Append(pageSize);
            _docMainPart.Document.Body.Append(sectionProps);

            //_docMainPart.Document = new Document();
            //_body = new Body();
            //InitializeDocumentPartsFromXml();
            //InitializeDocumentStyles();
        }


        public WordprocessingWorker(string docPath,
    DocFileOperation fileOperation)
        {
            DocPath = docPath;

            _docOperation = fileOperation;

            _footNotes = new Footnotes();
            if (fileOperation == DocFileOperation.CreateNew)
            {
                _currentDoc = WordprocessingDocument.Create(DocPath, WordprocessingDocumentType.Document);
                // _currentDoc.FileOpenAccess = FileAccess.ReadWrite;
                _docMainPart = _currentDoc.AddMainDocumentPart();
                _docMainPart.Document = MakeEmpyDocument();
            }
            else
            {
                Open(docPath);
            }

            SectionProperties sectionProps = new SectionProperties();
            PageMargin pageMargin = new PageMargin() { Top = 1440, Right = (UInt32Value)1440U, Bottom = 1440, Left = (UInt32Value)1440U, Header = (UInt32Value)850U, Footer = (UInt32Value)130U, Gutter = (UInt32Value)0U };
            PageSize pageSize = new PageSize() { Width = (UInt32Value)11908U, Height = (UInt32Value)16833U };
            //Spacing space = new Spacing (){ 
            sectionProps.Append(pageMargin);
            sectionProps.Append(pageSize);
            _docMainPart.Document.Body.Append(sectionProps);


        }


        public static DocXmlParts GetDocParts(string resfolderpath)
        {

            DocXmlParts xmlFilesPaths = new DocXmlParts();
            xmlFilesPaths.CoreFilePropertiesPart = resfolderpath + "core.xml";
            xmlFilesPaths.EndNotes = resfolderpath + "endnotes.xml";
            xmlFilesPaths.FilePropPartPath = resfolderpath + "app.xml";
           // xmlFilesPaths.FontTablePart = resfolderpath + "fontTable.xml";
            xmlFilesPaths.FooterPartPath = resfolderpath + "footer1.xml";
            xmlFilesPaths.HeaderPartPath = resfolderpath + "header.xml";
            xmlFilesPaths.FootnotesPart = resfolderpath + "footnotes.xml";
            xmlFilesPaths.NumberingDefinitionsPartPath = resfolderpath + "numbering.xml";
            xmlFilesPaths.SettingsPartPath = resfolderpath + "settings.xml";
            xmlFilesPaths.StylePartPath = resfolderpath + "styles.xml";
            xmlFilesPaths.ThemePartPath = resfolderpath + "theme1.xml";
            xmlFilesPaths.WebSettingsPartPath = resfolderpath + "webSettings.xml";
            return xmlFilesPaths;
        }


        private void InitializeDocumentPartsFromXml()
        {

            //app.xml
            //XmlDocument filePropPartXml = new XmlDocument();
            //filePropPartXml.Load(_docXmlParts.FilePropPartPath);
            //_extendedFilePropertiesPart = _docMainPart.AddNewPart<ExtendedFilePropertiesPart>();
            //using (Stream outputStream = _extendedFilePropertiesPart.GetStream())
            //{
            //    using (StreamWriter ts = new StreamWriter(outputStream))
            //    {
            //        ts.Write(filePropPartXml.InnerXml);
            //    }
            //}


            //numbering.xml
            XmlDocument numberingPartXML = new XmlDocument();
            numberingPartXML.Load(_docXmlParts.NumberingDefinitionsPartPath);
            _numberingDefinitionsPart = _docMainPart.AddNewPart<NumberingDefinitionsPart>(); //("rId9");
            using (Stream outputStream = _numberingDefinitionsPart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write(numberingPartXML.InnerXml);
                }
            }


            //endnotes.xml
            XmlDocument endNotesPartXML = new XmlDocument();
            endNotesPartXML.Load(_docXmlParts.EndNotes);
            _endnotesPart = _docMainPart.AddNewPart<EndnotesPart>("rId6");
            using (Stream outputStream = _endnotesPart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write(endNotesPartXML.InnerXml);
                }
            }


            //fontTable.xml
            XmlDocument fontTablePartXML = new XmlDocument();
            fontTablePartXML.Load(_docXmlParts.FontTablePart);
            _fontTablePart = _docMainPart.AddNewPart<FontTablePart>("rId11");
            using (Stream outputStream = _fontTablePart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write(fontTablePartXML.InnerXml);
                }
            }


            //footnotes.xml
            XmlDocument footnotePartXML = new XmlDocument();
            footnotePartXML.Load(_docXmlParts.FootnotesPart);
            _footnotesPart = _docMainPart.AddNewPart<FootnotesPart>("rId5");
            using (Stream outputStream = _footnotesPart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write(footnotePartXML.InnerXml);
                }
            }


            //_coreFilePropertiesPart
            //XmlDocument coreFilePropertiesPartXml = new XmlDocument();
            //coreFilePropertiesPartXml.Load(_docXmlParts.CoreFilePropertiesPart);
            //_coreFilePropertiesPart = _docMainPart.AddNewPart<CoreFilePropertiesPart>();
            //using (Stream outputStream = _coreFilePropertiesPart.GetStream())
            //{
            //    using (StreamWriter ts = new StreamWriter(outputStream))
            //    {
            //        ts.Write(coreFilePropertiesPartXml.InnerXml);
            //    }
            //}


            XmlDocument settingsPartXml = new XmlDocument();
            settingsPartXml.Load(_docXmlParts.SettingsPartPath);
            _docSettingsPart = _docMainPart.AddNewPart<DocumentSettingsPart>("rId3");
            using (Stream outputStream = _docSettingsPart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write(settingsPartXml.InnerXml);
                }
            }

            XmlDocument themePartXml = new XmlDocument();
            themePartXml.Load(_docXmlParts.ThemePartPath);
            _themePart = _docMainPart.AddNewPart<ThemePart>("rId12");
            using (Stream outputStream = _themePart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write(themePartXml.InnerXml);
                }
            }

            XmlDocument footerPartXml = new XmlDocument();
            footerPartXml.Load(_docXmlParts.FooterPartPath);
            _docFooterPart = _docMainPart.AddNewPart<FooterPart>("rId8");
            using (Stream outputStream = _docFooterPart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write("footer");
                }
            }


            XmlDocument headerPartXml = new XmlDocument();
            headerPartXml.Load(_docXmlParts.HeaderPartPath);
            _docHeaderPart = _docMainPart.AddNewPart<HeaderPart>("rId2");
            using (Stream outputStream = _docHeaderPart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write("header");
                }
            }

            XmlDocument webSettingsPartXml = new XmlDocument();
            webSettingsPartXml.Load(_docXmlParts.WebSettingsPartPath);
            _webSettingsPart = _docMainPart.AddNewPart<WebSettingsPart>("rId4");
            using (Stream outputStream = _webSettingsPart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write(webSettingsPartXml.InnerXml);
                }
            }

        }

        public WordprocessingWorker()
        {
            _footNotes = new Footnotes();
            //_docMainPart = _currentDoc.AddMainDocumentPart();
            //_docMainPart.Document = new Document();
            //_body = new Body();

            //InitializeDocumentStyles();
            //InitializeFootNotesPart();
        }


        /// <summary>
        /// Opens the specified Document file.
        /// This loads it in memory for later modification.
        /// </summary>
        /// <param name="docPath"></param>
        public void Open(string docPath)
        {
            //if the document is already Open well close it first to free up resources.
            if (IsOpen)
            {
                Close();
            }

            DocPath = docPath;

            //populate a MemoryStream with byte array of the Document
            byte[] sourceBytes = File.ReadAllBytes(DocPath);
            _inMemoryStream = new MemoryStream();
            _inMemoryStream.Write(sourceBytes, 0, (int)sourceBytes.Length);

            //Create the in-memory Document from the stream for modification
            _currentDoc = WordprocessingDocument.Open(docPath, true);//_inMemoryStream, true);
            _docMainPart = _currentDoc.MainDocumentPart;

            _themePart = _docMainPart.ThemePart;
            _docStylePart = _docMainPart.StyleDefinitionsPart;
            _docSettingsPart = _docMainPart.DocumentSettingsPart;
            _docHeaderPart = _docMainPart.HeaderParts.FirstOrDefault<HeaderPart>();
            _docFooterPart = _docMainPart.FooterParts.FirstOrDefault<FooterPart>();
            _webSettingsPart = _docMainPart.WebSettingsPart;

            _numberingDefinitionsPart = _docMainPart.NumberingDefinitionsPart;
            _endnotesPart = _docMainPart.EndnotesPart;
            _fontTablePart = _docMainPart.FontTablePart;
            _footnotesPart = _docMainPart.FootnotesPart;
            _coreFilePropertiesPart = _currentDoc.CoreFilePropertiesPart;
            _extendedFilePropertiesPart = _currentDoc.ExtendedFilePropertiesPart;
            //_numberingDefinitionsPart = from p in _currentDoc.Parts
            //                            where typeof(p) is NumberingDefinitionsPart
            //                            select p;


        }


        public void LoadDocumentFromHTML(string htmlpath)
        {
            //http://notesforhtml2openxml.codeplex.com/documentation?referringTitle=Home
        }

        public void SaveDocumentAsHTML(string htmlpath)
        {
            //http://msdn.microsoft.com/en-us/library/ff628051%28v=office.14%29.aspx
        }

        //public static int CountPagesUsingOpenXML(WordprocessingWorker doc, string docPath, DocXmlParts xmlFilesPaths, string serverMapPath)
        //{

        //    /*doc.Save();
        //    doc.Close();
        //    doc.Dispose();

        //    int count = 0;
        //    string tmpFileName = docPath.Replace(".docx", "_temp.docx");
        //    File.Copy(docPath, tmpFileName, true);
        //    FileInfo fi=new FileInfo(docPath);

        //    PdfMaker.PrintToPdf(fi.DirectoryName, serverMapPath, tmpFileName, tmpFileName.Replace(".docx", ".pdf"));
        //    count = TayaIT.Enterprise.EMadbatah.Pdf2Word.pdf2ImageConvert.getNumberOfPdfPages(tmpFileName.Replace(".docx", ".pdf"));
        //    File.Delete(tmpFileName);
        //    File.Delete(tmpFileName.Replace(".docx", ".pdf"));
        //    doc = new WordprocessingWorker(docPath, xmlFilesPaths, DocFileOperation.Open);
        //    return count;*/
        //    return 0;
        //}

        public int CountPagesUsingOpenXML(WordprocessingWorker doc, string docPath, DocXmlParts xmlFilesPaths, string serverMapPath, out WordprocessingWorker worker)
        {
            string tmpFileName = "";
            int count = 0;
            try
            {
                doc.Save();
                doc.Dispose();

                tmpFileName = docPath.Replace(".docx", "_temp.docx");
                using (WordprocessingWorker tmpDoc = new WordprocessingWorker(docPath, DocFileOperation.Open))
                {
                    tmpDoc.SaveAs(tmpFileName);
                }
                count = WordCom.GetDocumentNumPages(tmpFileName);
                int counter = 0;
                //Thread.Sleep(1000);
                while (true)
                {
                    bool WordExists = false;
                    Thread.Sleep(200);
                    foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcesses())
                    {
                        if (thisproc.ProcessName.StartsWith("WINWORD"))
                        {
                            WordExists = true;
                            counter += 10;
                            if (counter == 1000)
                            {
                                thisproc.Kill();
                                WordExists = false;
                                Util.LogHelper.LogMessage("process killed by me", "page num", System.Diagnostics.TraceEventType.Information);
                            }
                            break;
                        }
                    }
                    if (!WordExists)
                        break;
                }
            }
            catch (Exception ex)
            {
                Util.LogHelper.LogException(ex, "CountPagesUsingOpenXML");
                Thread.ResetAbort();
            }

            File.Delete(tmpFileName);
            worker = new WordprocessingWorker(docPath, DocFileOperation.Open);
            return count;
        }

        public long CountLineNum(WordprocessingWorker doc, string docPath,DocXmlParts xmlFilesPaths, string serverMapPath, out WordprocessingWorker worker)
        {
            long lnCount = 0;
            string tmpFileName = docPath.Replace(".docx", "_temp.docx");
            try
            {
                doc.Save();
                doc.Dispose();

                using (WordprocessingWorker tmpDoc = new WordprocessingWorker(docPath, DocFileOperation.Open))
                {
                    tmpDoc.SaveAs(tmpFileName);
                }

                lnCount = WordCom.GetDocumentLineNum(tmpFileName, 0);
                int counter = 0;
                while (true)
                {
                    bool WordExists = false;
                    Thread.Sleep(200);
                    foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcesses())
                    {
                        if (thisproc.ProcessName.StartsWith("WINWORD"))
                        {
                            WordExists = true;
                            counter += 10;
                            if (counter == 1000)
                            {
                                thisproc.Kill();
                                WordExists = false;
                                Util.LogHelper.LogMessage("process killed by me", "line num", System.Diagnostics.TraceEventType.Information);
                            }
                            break;
                        }
                    }
                    if (!WordExists)
                        break;
                }
            }
            catch (Exception ex)
            {
                Util.LogHelper.LogException(ex, "CountLineNum");
                Thread.ResetAbort();
                lnCount = 0;
            }
            File.Delete(tmpFileName);
            worker = new WordprocessingWorker(docPath, DocFileOperation.Open);
            return lnCount;
        }

        public int GetCurrentPageNumber()
        {
            //int pageCount = (int)document.ExtendedFilePropertiesPart.Properties.Pages.Text;
            //return _docMainPart.Document.Descendants<SectionProperties>().Count();
            return _docMainPart.Document.Descendants<SectionProperties>().Count();

            /*FileInfo fi = new FileInfo(docPath);
            PdfMaker.PrintToPdf(fi.DirectoryName, serverMapPath, docPath, docPath.Replace(".docx", ".pdf"));
            int count = TayaIT.Enterprise.EMadbatah.Pdf2Word.pdf2ImageConvert.getNumberOfPdfPages(docPath.Replace(".docx", ".pdf"));
            File.Delete(docPath.Replace(".docx", ".pdf"));

            return 0;*/
        }
        public void AddBreak()
        {

            if (_docMainPart == null)
                _docMainPart = _currentDoc.AddMainDocumentPart();
            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();

            Paragraph paragraph = new Paragraph();
            Run run = new Run(new Break() { });


            paragraph.Append(run);

            _docMainPart.Document.Body.Append(paragraph);

            Save();
        }

        public void AddPageBreak()
        {
           /* if (_docMainPart == null)
                _docMainPart = _currentDoc.AddMainDocumentPart();
            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();*/

            Paragraph paragraph = new Paragraph();
            Run run = new Run(new Break() { Type=BreakValues.Page });
            paragraph.Append(run);

            _docMainPart.Document.Body.Append(paragraph);

            Save();
        }

        public void AddCarriageReturn()
        {

            if (_docMainPart == null)
                _docMainPart = _currentDoc.AddMainDocumentPart();
            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();

            Paragraph paragraph = new Paragraph();
            Run run = new Run(new CarriageReturn() { });


            paragraph.Append(run);

            _docMainPart.Document.Body.Append(paragraph);

            Save();
        }

        public void AddTextToWordDocument(string paragraphText, ParagraphStyle paragraphType, ParagrapJustification textDirection)
        {
            if (_docMainPart == null)
                _docMainPart = _currentDoc.AddMainDocumentPart();
            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();
            //Run run = new Run(new Text(paragraphText) { Space = SpaceProcessingModeValues.Preserve });
            Run run = new Run();// { RsidRunProperties = "006B5D2A" };

            // Assign a reference to the existing document body.
            //Body body = _currentDoc.MainDocumentPart.Document.Body;
            //var sdtContentRunElements =
            //    from sdtContentRun in _currentDoc.MainDocumentPart.RootElement.Descendants<SdtContentRun>()
            //    select sdtContentRun;

            Paragraph currentParagraph = _currentDoc.MainDocumentPart.Document.Body.Elements<Paragraph>().Last();
            //Paragraph currentParagraph = (Paragraph)_currentDoc.MainDocumentPart.RootElement.Descendants<Paragraph>().Last();
            //string modifiedString = Regex.Replace(pp.InnerText, currentString, reusableContentString);
            //currentParagraph.RemoveAllChildren<Run>();

            switch (paragraphType)
            {
                case ParagraphStyle.ParagraphTitle:
                    RunProperties runParagraphTitleTextProp = new RunProperties();
                    RunFonts runParagraphTitleTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
                    Bold bold = new Bold();
                    BoldComplexScript boldComplexScript = new BoldComplexScript();
                    FontSize fontParagraphTitleTextSize = new FontSize() { Val = "40" };
                    FontSizeComplexScript fontParagraphTitleComplexScriptSize = new FontSizeComplexScript() { Val = "40" };
                    RightToLeftText rtlTextParagraphTitle = new RightToLeftText();
                    runParagraphTitleTextProp.Append(runParagraphTitleTextFonts);
                    runParagraphTitleTextProp.Append(bold);
                    runParagraphTitleTextProp.Append(boldComplexScript);
                    runParagraphTitleTextProp.Append(fontParagraphTitleTextSize);
                    runParagraphTitleTextProp.Append(fontParagraphTitleComplexScriptSize);
                    runParagraphTitleTextProp.Append(rtlTextParagraphTitle);
                    run.Append(runParagraphTitleTextProp);

                    break;
                case ParagraphStyle.UnderLineParagraphTitle:
                    RunProperties runUnderlineParagraphTitleTextProp = new RunProperties();
                    RunFonts runUnderlineParagraphTitleTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
                 //   Bold Underlinebold = new Bold();
                    Underline underline = new Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single };
                 //   BoldComplexScript boldUnderlineComplexScript = new BoldComplexScript();
                    FontSize fontUnderlineParagraphTitleTextSize = new FontSize() { Val = "40" };
                    FontSizeComplexScript fontUnderlineParagraphTitleComplexScriptSize = new FontSizeComplexScript() { Val = "40" };
                    RightToLeftText rtlUnderlineTextParagraphTitle = new RightToLeftText();
                    runUnderlineParagraphTitleTextProp.Append(runUnderlineParagraphTitleTextFonts);
                  //  runUnderlineParagraphTitleTextProp.Append(Underlinebold);
                    runUnderlineParagraphTitleTextProp.Append(underline);
                  //  runUnderlineParagraphTitleTextProp.Append(boldUnderlineComplexScript);
                    runUnderlineParagraphTitleTextProp.Append(fontUnderlineParagraphTitleTextSize);
                    runUnderlineParagraphTitleTextProp.Append(fontUnderlineParagraphTitleComplexScriptSize);
                    runUnderlineParagraphTitleTextProp.Append(rtlUnderlineTextParagraphTitle);
                    run.Append(runUnderlineParagraphTitleTextProp);
                    break;
                case ParagraphStyle.NormalArabic:
                    RunProperties runNormalTextProp = new RunProperties();
                    RunFonts runNormalTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla", AsciiTheme = ThemeFontValues.MinorBidi, HighAnsiTheme = ThemeFontValues.MinorBidi };
                    FontSize fontNormalTextSize = new FontSize() { Val = "36" };
                    FontSizeComplexScript fontComplexNormalTextSize = new FontSizeComplexScript() { Val = "36" };
                    RightToLeftText rightToLeftText = new RightToLeftText();
                    //BiDi biDi1 = new BiDi();
                    //Languages languages = new Languages() { EastAsia = "ar-SA", Bidi = "ar-SA" };

                    runNormalTextProp.Append(runNormalTextFonts);
                    runNormalTextProp.Append(fontNormalTextSize);
                    runNormalTextProp.Append(fontComplexNormalTextSize);
                    runNormalTextProp.Append(rightToLeftText);
                    run.Append(runNormalTextProp);
                    break;
                case ParagraphStyle.Footer:
                    RunProperties runFooterProp = new RunProperties();
                    RunFonts runFooterFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla", AsciiTheme = ThemeFontValues.MinorBidi, HighAnsiTheme = ThemeFontValues.MinorBidi };
                    FontSize fontFooterSize = new FontSize() { Val = "28" };
                    FontSizeComplexScript fontComplexFooterSize = new FontSizeComplexScript() { Val = "28" };
                    RightToLeftText rightToLeftFooterText = new RightToLeftText();
                    //BiDi biDi1 = new BiDi();
                    //Languages languages = new Languages() { EastAsia = "ar-SA", Bidi = "ar-SA" };

                    runFooterProp.Append(runFooterFonts);
                    runFooterProp.Append(fontFooterSize);
                    runFooterProp.Append(fontComplexFooterSize);
                    runFooterProp.Append(rightToLeftFooterText);
                    run.Append(runFooterProp);
                    break;
                case ParagraphStyle.ParagraphItalic:
                    RunProperties runParagraphItalicTextProp = new RunProperties();
                    RunFonts runParagraphItalicTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
                    Italic italic = new Italic();
                    BoldComplexScript boldComplexScriptItalic = new BoldComplexScript();
                    ItalicComplexScript italicComplexScript = new ItalicComplexScript();
                    FontSize fontParagraphItalicTextSize = new FontSize() { Val = "36" };
                    FontSizeComplexScript fontParagraphItalicComplexScriptSize = new FontSizeComplexScript() { Val = "36" };
                    RightToLeftText rtlTextParagraphItalic = new RightToLeftText();
                    runParagraphItalicTextProp.Append(runParagraphItalicTextFonts);
                    runParagraphItalicTextProp.Append(italic);
                    runParagraphItalicTextProp.Append(italicComplexScript);
                    runParagraphItalicTextProp.Append(fontParagraphItalicTextSize);
                    runParagraphItalicTextProp.Append(fontParagraphItalicComplexScriptSize);
                    runParagraphItalicTextProp.Append(rtlTextParagraphItalic);
                    run.Append(runParagraphItalicTextProp);
                    break;
                case ParagraphStyle.ParagraphBoldItalic:
                    RunProperties runParagraphBoldItalicTextProp = new RunProperties();
                    RunFonts runParagraphBoldItalicTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
                    Italic italicBoldItalic = new Italic();
                    Bold boldBoldItalic = new Bold();
                    BoldComplexScript boldComplexScriptBoldItalic = new BoldComplexScript();
                    ItalicComplexScript italicComplexScriptBoldItalic = new ItalicComplexScript();
                    FontSize fontParagraphBoldItalicTextSize = new FontSize() { Val = "36" };
                    FontSizeComplexScript fontParagraphBoldItalicComplexScriptSize = new FontSizeComplexScript() { Val = "36" };
                    RightToLeftText rtlTextParagraphBoldItalic = new RightToLeftText();
                    runParagraphBoldItalicTextProp.Append(runParagraphBoldItalicTextFonts);
                    runParagraphBoldItalicTextProp.Append(italicBoldItalic);
                    runParagraphBoldItalicTextProp.Append(italicComplexScriptBoldItalic);
                    runParagraphBoldItalicTextProp.Append(boldBoldItalic);
                    runParagraphBoldItalicTextProp.Append(boldComplexScriptBoldItalic);
                    runParagraphBoldItalicTextProp.Append(fontParagraphBoldItalicTextSize);
                    runParagraphBoldItalicTextProp.Append(fontParagraphBoldItalicComplexScriptSize);
                    runParagraphBoldItalicTextProp.Append(rtlTextParagraphBoldItalic);
                    run.Append(runParagraphBoldItalicTextProp);
                    break;
                case ParagraphStyle.FootnoteText:
                    // footnote = new Footnote() { Type = FootnoteEndnoteValues.Separator, Id = -1 };
                    break;
                case ParagraphStyle.FootnoteReference:
                    //     FootnoteReference footnoteRef = new FootnoteReference() { CustomMarkFollows = true, Id = 1 };
                    //  run.Append(footnoteRef);
                    break;
                case ParagraphStyle.SeparatorMark:
                    SeparatorMark separatorMark = new SeparatorMark();
                    run.Append(separatorMark);
                    break;
                default:
                    break;
            }
            run.Append(new Text(paragraphText) { Space = SpaceProcessingModeValues.Preserve });
            currentParagraph.AppendChild<Run>(run);


        }

        public void AddParagraph(string paragraphText, ParagraphStyle paragraphType, ParagrapJustification textDirection, bool appendFootNote, string footNoteText)
        {
            paragraphText = System.Web.HttpUtility.HtmlDecode(paragraphText).Replace(";psbn&", " ").Replace("&nbsp;", " ");

            if (_docMainPart == null)
                _docMainPart = _currentDoc.AddMainDocumentPart();
            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();

            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProp = new ParagraphProperties();
            BiDi biDi1 = new BiDi();
            Justification justification = new Justification();

            switch (textDirection)
            {
                case ParagrapJustification.RTL:
                    justification.Val = JustificationValues.LowKashida;
                    paragraphProp.Append(biDi1);
                    break;
                case ParagrapJustification.LTR:
                    justification.Val = JustificationValues.Left;
                    break;
                case ParagrapJustification.Center:
                    justification.Val = JustificationValues.Center;
                    break;
                case ParagrapJustification.Both:
                    justification.Val = JustificationValues.Both;
                    paragraphProp.Append(biDi1);
                    break;
                default:
                    break;
            }

            paragraphProp.Append(justification);
            paragraphProp.ParagraphStyleId = new ParagraphStyleId() { Val = paragraphType.ToString() }; // we set the style
            paragraph.Append(paragraphProp);    //HeadingArabic

            paragraph.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines() { After = "1", Line = "240", LineRule = LineSpacingRuleValues.Auto };
            Run run = new Run(new Text(paragraphText) { Space = SpaceProcessingModeValues.Preserve });

            switch (paragraphType)
            {
                case ParagraphStyle.ParagraphTitle:
                    break;
                case ParagraphStyle.NormalArabic:
                    paragraphProp.Append(new Indentation() { FirstLine = "720" });
                    break;
                case ParagraphStyle.FootnoteText:
                    // footnote = new Footnote() { Type = FootnoteEndnoteValues.Separator, Id = -1 };
                    break;
                case ParagraphStyle.FootnoteReference:
                    //     FootnoteReference footnoteRef = new FootnoteReference() { CustomMarkFollows = true, Id = 1 };
                    //  run.Append(footnoteRef);
                    break;
                case ParagraphStyle.SeparatorMark:
                    SeparatorMark separatorMark = new SeparatorMark();
                    run.Append(separatorMark);
                    break;
                default:
                    break;
            }

            if (appendFootNote)
            {
                FootnoteEndnoteReferenceType reference;
                reference = new FootnoteReference() { Id = AddFootnoteReference(footNoteText) };

                List<OpenXmlElement> elements = new List<OpenXmlElement>();
                elements.Add(
                new Run(
                    new RunProperties(new FootnotePosition() { Val = FootnotePositionValues.BeneathText },
                        new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript },
                        new FontSizeComplexScript() { Val = "24" },
                        new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" },
                        new RightToLeftText()),
                    reference)
                );

                paragraph.Append(elements);
                paragraph.Append(run);
               
                _docMainPart.Document.Body.Append(paragraph);

            }
            else
            {
                paragraph.Append(run);
                _docMainPart.Document.Body.Append(paragraph);
            }

           /* List<Break> breaks = _docMainPart.Document.Descendants<Break>().ToList();
            foreach (Break b in breaks)
            {
                b.Remove();
            }*/
            Save();
        }

        public void AddParagraph(List<string> paragraphTextArr, ParagraphStyle paragraphType, ParagrapJustification textDirection, bool appendFootNote, List<string> footNoteTextArr)
        {
            if (_docMainPart == null)
                _docMainPart = _currentDoc.AddMainDocumentPart();
            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();

            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProp = new ParagraphProperties();
            BiDi biDi1 = new BiDi();
            Justification justification = new Justification();

            switch (textDirection)
            {
                case ParagrapJustification.RTL:
                    justification.Val = JustificationValues.LowKashida;
                    paragraphProp.Append(biDi1);
                    break;
                case ParagrapJustification.LTR:
                    justification.Val = JustificationValues.Left;
                    break;
                case ParagrapJustification.Center:
                    justification.Val = JustificationValues.Center;
                    break;
                case ParagrapJustification.Both:
                    justification.Val = JustificationValues.Both;
                    paragraphProp.Append(biDi1);
                    break;
                default:
                    break;
            }


            paragraphProp.Append(justification);
            paragraphProp.ParagraphStyleId = new ParagraphStyleId() { Val = paragraphType.ToString() }; // we set the style
            paragraph.Append(paragraphProp);    //HeadingArabic

            paragraph.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines() { After = "1", Line = "240", LineRule = LineSpacingRuleValues.Auto };
            switch (paragraphType)
            {
                case ParagraphStyle.ParagraphTitle:
                    break;
                case ParagraphStyle.NormalArabic:
                    paragraphProp.Append(new Indentation() { FirstLine = "720" });
                    break;
                default:
                    break;
            }

            int i = 0;
            bool emptyStr = false;
            bool newParag = true;
            foreach (string paragraphTxt in paragraphTextArr)
            {
                if (i < footNoteTextArr.Count && !String.IsNullOrEmpty(footNoteTextArr[i]))
                {
                    string footNoteText = footNoteTextArr[i];
                    FootnoteReference reference = new FootnoteReference() { Id = AddFootnoteReference(footNoteText + " .") };
                    List<OpenXmlElement> elements = new List<OpenXmlElement>();
                    elements.Add(
                    new Run(
                        new RunProperties(new FootnotePosition() { Val = FootnotePositionValues.BeneathText },
                            new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript },
                            new FontSizeComplexScript() { Val = "24" },
                            new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" },
                            new RightToLeftText()),
                        reference)
                    );
                    paragraph.Append(elements);
                    newParag = false;
                }

                string[] sep = new string[1] { "#!#!#!" };
                string[] pTextArr = System.Web.HttpUtility.HtmlDecode(paragraphTxt).Replace(";psbn&", " ").Replace("&nbsp;", " ").Split(sep, StringSplitOptions.None);
                int j = 0;
                foreach (string p in pTextArr)
                {
                    if (!emptyStr || (p.Trim() != "" && emptyStr))
                    {
                        if (p.Trim() == "")
                            emptyStr = true;
                        else emptyStr = false;

                        Run run = new Run(new Text(p.Trim() + " ") { Space = SpaceProcessingModeValues.Preserve });
                        paragraph.Append(run);
                        newParag = false;

                        if (pTextArr.Length != 1 && (j != pTextArr.Length - 1 || p.Trim() == ""))
                        {
                            _docMainPart.Document.Body.Append(paragraph);
                            Save();

                            paragraph = new Paragraph();
                            paragraphProp = new ParagraphProperties();
                            biDi1 = new BiDi();
                            justification = new Justification();
                            justification.Val = JustificationValues.LowKashida;
                            paragraphProp.Append(biDi1);
                            paragraphProp.Append(justification);
                            paragraphProp.ParagraphStyleId = new ParagraphStyleId() { Val = paragraphType.ToString() };
                            paragraph.Append(paragraphProp);

                            paragraph.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines() { After = "1", Line = "240", LineRule = LineSpacingRuleValues.Auto };
                            newParag = true;
                        }
                        j++;
                    }
                }
                i++;
            }

            if (!newParag)
            {
                _docMainPart.Document.Body.Append(paragraph);
                Save();
            }
        }

        public void DeleteLastParagraph(string text)
        {
            IEnumerable<OpenXmlElement> elems = _docMainPart.Document.Body.Descendants().ToList();
            List<Paragraph> paragraphsToDelete = new List<Paragraph>();
            foreach (OpenXmlElement elem in elems)
            {
                if (elem is Paragraph && elem.InnerText == text)
                {
                    Paragraph p = (Paragraph)elem;
                    paragraphsToDelete.Add(p);
                }
            }

            foreach (var p in paragraphsToDelete)
            {
                p.RemoveAllChildren();
                p.Remove();
            }
        }

        public void InsertImage(string fileName,int i)
        {
            ImageWriter.AddImage(_docMainPart.Document.Body, _docMainPart, fileName, "rId" + i);
            Save();
        }

        public void Save()
        {
            if (!IsOpen)
                Open(DocPath);
            try
            {
                _docMainPart.Document.Save();// Save changes to the main document part.             
            }
            catch
            {
                if (_docMainPart == null)
                    _docMainPart = _currentDoc.AddMainDocumentPart();
                if (_docMainPart.Document == null)
                    _docMainPart.Document = MakeEmpyDocument();
              
                _docMainPart.Document.Save();
            }
        }

        public void AddTable(string[,] data)
        {
            var doc = _currentDoc.MainDocumentPart.Document;

            Table table = new Table();

            TableProperties props = new TableProperties(new TableStyle() { Val = "styleTableGrid" },
                new TableWidth() { Width = "4000", Type = TableWidthUnitValues.Pct },
                new TableLook() {Val = "04A0",FirstRow = false,LastRow = false,NoHorizontalBand = false,NoVerticalBand = true},
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.None)},
                    new LeftBorder  { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.None)},
                    new InsideVerticalBorder {Val = new EnumValue<BorderValues>(BorderValues.None)}
                ));

            TableJustification tblJustification = new TableJustification();
            tblJustification.Val = TableRowAlignmentValues.Center;
            props.Append(tblJustification);
            table.AppendChild<TableProperties>(props);

            for (var i = 0; i <= data.GetUpperBound(0); i++)
            {
                var tr = new TableRow();
                for (var j = 0; j <= data.GetUpperBound(1); j++)
                {
                    var tc = new TableCell();

                    BiDi biDi1 = new BiDi();
                    tc.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(),new Justification() { Val = JustificationValues.LowKashida }), new Run(new Text(data[i, j]))));

                    // Assume you want columns that are automatically sized.
                    tc.Append(new TableCellProperties(
                        new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Pct }));

                    tr.Append(tc);
                }
                table.Append(tr);
            }
            doc.Body.Append(table);
        }

        public void AddTable(List<string> data)
        {
            try
            {
                if (_docMainPart == null)
                    _docMainPart = _currentDoc.AddMainDocumentPart();
                if (_docMainPart.Document == null)
                    _docMainPart.Document = MakeEmpyDocument();

                Table table = new Table();

                TableProperties props = new TableProperties(new TableStyle() { Val = "styleTableGrid" },
                    new TableWidth() { Width = "4250", Type = TableWidthUnitValues.Pct },
                    new TableLook() { Val = "04A0", FirstRow = false, LastRow = false, NoHorizontalBand = false, NoVerticalBand = true },
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.None) },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.None) }),
                    new TableCellMarginDefault(
                       new TopMargin() { Width = "71.4", Type = TableWidthUnitValues.Dxa },//0.05
                       new TableCellLeftMargin() { Width = 71, Type = TableWidthValues.Dxa },
                       new BottomMargin() { Width = "71.4", Type = TableWidthUnitValues.Dxa },
                       new TableCellRightMargin() { Width = 71, Type = TableWidthValues.Dxa }));

                TableJustification tblJustification = new TableJustification();
                tblJustification.Val = TableRowAlignmentValues.Center;
                props.Append(tblJustification);
                table.AppendChild<TableProperties>(props);
                string[] sep = new string[1] { "," };
                for (var i = 0; i < data.Count(); i++)
                {
                    var tr = new TableRow();

                    string[] names = data[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    if (names.Count() != 1)
                    {
                        for (int j = 0; j < names.Count(); j++)
                        {
                            var tc1 = new TableCell();
                            tc1.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(), new SpacingBetweenLines() { After = "0", Before = "0", Line = "276", LineRule = LineSpacingRuleValues.Auto }, new Justification() { Val = JustificationValues.LowKashida }), new Run(new Text(names[j]))));
                            tc1.Append(new TableCellProperties(new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));
                            tr.Append(tc1);
                        }
                    }
                    else
                    {
                        var tc1 = new TableCell();
                        tc1.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(), new SpacingBetweenLines() { After = "0", Before = "0", Line = "276", LineRule = LineSpacingRuleValues.Auto }, new Justification() { Val = JustificationValues.LowKashida }), new Run(new Text(data[i]))));
                        tc1.Append(new TableCellProperties(new HorizontalMerge() { Val = MergedCellValues.Restart }, new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));

                        var tc2 = new TableCell();
                        tc2.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(), new SpacingBetweenLines() { After = "0", Before = "0", Line = "276", LineRule = LineSpacingRuleValues.Auto }, new Justification() { Val = JustificationValues.LowKashida }), new Run(new Text(""))));
                        tc2.Append(new TableCellProperties(new HorizontalMerge() { Val = MergedCellValues.Continue }, new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));

                        tr.Append(tc1);
                        tr.Append(tc2);
                    }
                    table.Append(tr);
                }

                _docMainPart.Document.Body.Append(table);
                Save();
            }
            catch (Exception ex)
            {
                ;
            }
        }
        public void AddCustomTable(List<Model.SessionMembersVote> membersVoteLst)
        {
            if (_docMainPart == null)
                _docMainPart = _currentDoc.AddMainDocumentPart();
            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();

            Table table = new Table();

            TableProperties tableProp = new TableProperties(new TableStyle() { Val = "styleTableGrid" },
                    new TableIndentation() { Width = 0,Type = TableWidthUnitValues.Dxa },
                    new TableJustification() { Val = TableRowAlignmentValues.Right },
                    new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },//in percentage makes 70%
                    new TableLook() { Val = "04A0", FirstRow = true, LastRow = false, NoHorizontalBand = false, NoVerticalBand = true },
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "ffffff", Size = (UInt32Value)6U , Space = (UInt32Value)0U},
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "ffffff", Size = (UInt32Value)6U, Space = (UInt32Value)0U },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "ffffff", Size = (UInt32Value)6U, Space = (UInt32Value)0U },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "ffffff", Size = (UInt32Value)6U, Space = (UInt32Value)0U },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "ffffff", Size = (UInt32Value)6U, Space = (UInt32Value)0U },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "ffffff", Size = (UInt32Value)6U, Space = (UInt32Value)0U }),
                   new TableCellMarginDefault(
                       new TopMargin() { Width = "20.5", Type = TableWidthUnitValues.Dxa },//0.07
                       new TableCellLeftMargin() { Width = 20, Type = TableWidthValues.Dxa },
                       new BottomMargin() { Width = "20.5", Type = TableWidthUnitValues.Dxa },
                       new TableCellRightMargin() { Width = 20, Type = TableWidthValues.Dxa })
                       );

            table.Append(tableProp);

           /* TableRow th = new TableRow();
            var thc1 = new TableCell();
            thc1.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(), new Justification() { Val = JustificationValues.Center },new SpacingBetweenLines() { After = "0",Before="0", Line = "240", LineRule = LineSpacingRuleValues.Auto }), new Run(new Text("م".ToString()) { Space = SpaceProcessingModeValues.Preserve })));
            thc1.Append(new TableCellProperties(new Shading() { Color = "auto", Fill = "d0d0d0", Val = ShadingPatternValues.Clear }, new TableCellWidth { Width = "500", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));

            var thc2 = new TableCell();
            thc2.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(), new Justification() { Val = JustificationValues.Center }, new SpacingBetweenLines() { After = "0", Before = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto }) , new Run(new Text(" اسم العضو ") { Space = SpaceProcessingModeValues.Preserve })));
            thc2.Append(new TableCellProperties(new Shading() { Color = "auto", Fill = "d0d0d0", Val = ShadingPatternValues.Clear }, new TableCellWidth { Width = "2000", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));

            var thc3 = new TableCell();
            thc3.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(), new Justification() { Val = JustificationValues.Center }, new SpacingBetweenLines() { After = "0", Before = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto }), new Run(new Text("التصويت") { Space = SpaceProcessingModeValues.Preserve })));
            thc3.Append(new TableCellProperties(new Shading() { Color = "auto", Fill = "d0d0d0", Val = ShadingPatternValues.Clear }, new TableCellWidth { Width = "1000", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));

            th.Append(thc3);
            th.Append(thc2);
            th.Append(thc1);
          
            table.Append(th);*/

            int i = 0;
            foreach (Model.SessionMembersVote member in membersVoteLst)
            {
                string voteStr = "";
                string color = "fff";
                switch (member.MemberVoteValue)
                {
                    case 1:
                         voteStr = "موافق";
                         color = "fff";
                      //  color = "9be19b";
                        break;
                    case 2:
                        voteStr = "غير موافق";
                        color = "fff";
                       // color = "fb969e";
                        break;
                    case 3:
                        voteStr = "ممتنع";
                        break;
                    default:
                        voteStr = "غير موجود";
                        break;
                }
                var tr = new TableRow();

                var tc2 = new TableCell();
                tc2.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "NormalArabic" }, new BiDi(), new Justification() { Val = JustificationValues.LowKashida }, new SpacingBetweenLines() { After = "0", Before = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto }), new Run(new Text("  " + member.AttendantName) { Space = SpaceProcessingModeValues.Preserve }) { RsidRunProperties = "003213CC" }) { RsidParagraphAddition = "003213CC", RsidParagraphProperties = "00704A9B", RsidParagraphMarkRevision = "003213CC", RsidRunAdditionDefault = "003213CC" });
                tc2.Append(new TableCellProperties(new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));
      
                var tc3 = new TableCell();
                tc3.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "NormalArabic" }, new BiDi(), new Justification() { Val = JustificationValues.LowKashida }, new SpacingBetweenLines() { After = "0", Before = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto }), new Run(new Text(voteStr) { Space = SpaceProcessingModeValues.Preserve })));
                tc3.Append(new TableCellProperties(new Shading() { Color = "auto", Fill = color, Val = ShadingPatternValues.Clear }, new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));

                tr.Append(tc3);
                tr.Append(tc2);
              
                table.Append(tr);
                i++;
            }
            _docMainPart.Document.Body.Append(table);
            Save();
        }

        public void AddStatistcsTable(string[,] data)
        {
            try
            {
                if (_docMainPart == null)
                    _docMainPart = _currentDoc.AddMainDocumentPart();
                if (_docMainPart.Document == null)
                    _docMainPart.Document = MakeEmpyDocument();


                //Container table
                Table bigtable = new Table();

                TableProperties bigtableProp = new TableProperties(new TableStyle() { Val = "styleTableGrid" },
                        new TableIndentation() { Width = 0, Type = TableWidthUnitValues.Dxa },
                        new TableJustification() { Val = TableRowAlignmentValues.Right },
                        new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },
                        new TableLook() { Val = "04A0", FirstRow = false, LastRow = false, NoHorizontalBand = false, NoVerticalBand = true });


                bigtable.Append(bigtableProp);
                TableRow tr = new TableRow();

                for (var i = 0; i <= data.GetUpperBound(0); i++)
                {
                    Table table1 = new Table( new TableProperties(new TableStyle() { Val = "styleTableGrid" },
                            new TableIndentation() { Width = 0, Type = TableWidthUnitValues.Dxa },
                            new TableJustification() { Val = TableRowAlignmentValues.Right },
                            new TableWidth() { Width = "4000", Type = TableWidthUnitValues.Pct },
                            new TableLook() { Val = "04A0", FirstRow = false, LastRow = false, NoHorizontalBand = false, NoVerticalBand = true },
                            new TableBorders(
                                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "d0d0d0", Size = (UInt32Value)8U, Space = (UInt32Value)0U },
                                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "d0d0d0", Size = (UInt32Value)8U, Space = (UInt32Value)0U },
                                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "d0d0d0", Size = (UInt32Value)8U, Space = (UInt32Value)0U },
                                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "d0d0d0", Size = (UInt32Value)8U, Space = (UInt32Value)0U },
                                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "d0d0d0", Size = (UInt32Value)12U, Space = (UInt32Value)0U },
                                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Color = "d0d0d0", Size = (UInt32Value)12U, Space = (UInt32Value)0U }),
                           new TableCellMarginDefault(
                               new TopMargin() { Width = "67.5", Type = TableWidthUnitValues.Dxa },
                               new TableCellLeftMargin() { Width = 67, Type = TableWidthValues.Dxa },
                               new BottomMargin() { Width = "67.5", Type = TableWidthUnitValues.Dxa },
                               new TableCellRightMargin() { Width = 67, Type = TableWidthValues.Dxa })
                               ));
                    TableRow tpl1tr1 = new TableRow();

                    for (var j = 0; j <= data.GetUpperBound(1); j++)
                    {
                        var tpl1Tc1 = new TableCell();
                        tpl1Tc1.Append(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "0",Before="0", Line = "240", LineRule = LineSpacingRuleValues.Auto },new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(), new Justification() { Val = JustificationValues.LowKashida }), new Run(new Text(data[i, j]))));
                        tpl1tr1.Append(tpl1Tc1);
                    }
                    table1.Append(tpl1tr1);

                    var tc1 = new TableCell();
                    tc1.Append(table1);
                    tc1.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "ParagraphTitle" }, new BiDi(), new Justification() { Val = JustificationValues.LowKashida }), new Run(new Text(""))));
                    tc1.Append(new TableCellProperties(new TableCellWidth { Width = "1250", Type = TableWidthUnitValues.Pct }, new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));
                    tr.Append(tc1);
                }

                bigtable.Append(tr);

                _docMainPart.Document.Body.Append(bigtable);
                Save();
            }
            catch(Exception ex){

            }
        }
        private void InitializeDocumentStyles()
        {
            // Create a style part and add it to the document
            XmlDocument stylesXml = new XmlDocument();
            stylesXml.Load(_docXmlParts.StylePartPath);

            _docStylePart = _docMainPart.AddNewPart<StyleDefinitionsPart>();
            //  Copy the style.xml content into the new part...
            using (Stream outputStream = _docStylePart.GetStream())
            {
                using (StreamWriter ts = new StreamWriter(outputStream))
                {
                    ts.Write(stylesXml.InnerXml);
                }
            }


            if (_docMainPart.StyleDefinitionsPart == null)
                _docStylePart = _docMainPart.AddNewPart<StyleDefinitionsPart>();
            else
                _docStylePart = _docMainPart.StyleDefinitionsPart;


            //Run runNormalText = new Run() { RsidRunProperties = "007A6E3F" };
            RunProperties runNormalTextProp = new RunProperties();
            RunFonts runNormalTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla", AsciiTheme = ThemeFontValues.MinorBidi, HighAnsiTheme = ThemeFontValues.MinorBidi };
            FontSize fontNormalTextSize = new FontSize() { Val = "36" };
            FontSizeComplexScript fontComplexNormalTextSize = new FontSizeComplexScript() { Val = "36" };
            RightToLeftText rightToLeftText = new RightToLeftText();
            //BiDi biDi1 = new BiDi();
            //Languages languages = new Languages() { EastAsia = "ar-SA", Bidi = "ar-SA" };

            runNormalTextProp.Append(runNormalTextFonts);
            runNormalTextProp.Append(fontNormalTextSize);
            runNormalTextProp.Append(fontComplexNormalTextSize);
            runNormalTextProp.Append(rightToLeftText);
            //runNormalTextProp.Append(biDi1);
            //runNormalTextProp.Append(languages);

            Style styleNormalText = new Style();
            styleNormalText.StyleId = "NormalArabic"; //this is the ID of the style
            styleNormalText.Append(new Name() { Val = "Taya Madbatah Normal Text" }); //this is name                
            styleNormalText.Append(new BasedOn() { Val = "Normal" });// our style based on Normal style                
            styleNormalText.Append(new NextParagraphStyle() { Val = "Normal" });// the next paragraph is Normal type
            styleNormalText.Append(runNormalTextProp);//we are adding properties previously defined


            // Run runParagraphTitleText = new Run() { RsidRunProperties = "00CD365A" };
            RunProperties runParagraphTitleTextProp = new RunProperties();
            RunFonts runParagraphTitleTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
            Bold bold = new Bold();
            BoldComplexScript boldComplexScript = new BoldComplexScript();
            FontSize fontParagraphTitleTextSize = new FontSize() { Val = "40" };
            FontSizeComplexScript fontParagraphTitleComplexScriptSize = new FontSizeComplexScript() { Val = "40" };
            RightToLeftText rtlTextParagraphTitle = new RightToLeftText();
            runParagraphTitleTextProp.Append(runParagraphTitleTextFonts);
            runParagraphTitleTextProp.Append(bold);
            runParagraphTitleTextProp.Append(boldComplexScript);
            runParagraphTitleTextProp.Append(fontParagraphTitleTextSize);
            runParagraphTitleTextProp.Append(fontParagraphTitleComplexScriptSize);
            runParagraphTitleTextProp.Append(rtlTextParagraphTitle);
            //runParagraphTitleTextProp.Append(languages);
            //runParagraphTitleText.Append(runParagraphTitleTextProp);
            Style styleParagraphTitle = new Style();
            styleParagraphTitle.StyleId = "ParagraphTitle";
            styleParagraphTitle.Append(new Name() { Val = "Taya Madbatah Paragraph Title Text" });
            styleParagraphTitle.Append(new BasedOn() { Val = "Normal" });
            styleParagraphTitle.Append(new NextParagraphStyle() { Val = "Normal" });
            styleParagraphTitle.Append(runParagraphTitleTextProp);




            // Run runParagraphTitleText = new Run() { RsidRunProperties = "00CD365A" };
            RunProperties runUnderlineParagraphTitleTextProp = new RunProperties();
            RunFonts runUnderlineParagraphTitleTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
          //  Bold Underlinebold = new Bold();
            Underline underline = new Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single };
          //  BoldComplexScript UnderlineboldComplexScript = new BoldComplexScript();
            FontSize fontUnderlineParagraphTitleTextSize = new FontSize() { Val = "36" };
            FontSizeComplexScript fontUnderlineParagraphTitleComplexScriptSize = new FontSizeComplexScript() { Val = "36" };
            RightToLeftText UnderlinertlTextParagraphTitle = new RightToLeftText();
            runUnderlineParagraphTitleTextProp.Append(runUnderlineParagraphTitleTextFonts);
          //  runUnderlineParagraphTitleTextProp.Append(Underlinebold);
            runUnderlineParagraphTitleTextProp.Append(underline);
          //  runUnderlineParagraphTitleTextProp.Append(UnderlineboldComplexScript);
            runUnderlineParagraphTitleTextProp.Append(fontUnderlineParagraphTitleTextSize);
            runUnderlineParagraphTitleTextProp.Append(fontUnderlineParagraphTitleComplexScriptSize);
            runUnderlineParagraphTitleTextProp.Append(UnderlinertlTextParagraphTitle);
            //runParagraphTitleTextProp.Append(languages);
            //runParagraphTitleText.Append(runParagraphTitleTextProp);
            Style styleUnderlineParagraphTitle = new Style();
            styleUnderlineParagraphTitle.StyleId = "UnderLineParagraphTitle";
            styleUnderlineParagraphTitle.Append(new Name() { Val = "Taya Madbatah UnderLine Paragraph Title Text" });
            styleUnderlineParagraphTitle.Append(new BasedOn() { Val = "Normal" });
            styleUnderlineParagraphTitle.Append(new NextParagraphStyle() { Val = "Normal" });
            styleUnderlineParagraphTitle.Append(runUnderlineParagraphTitleTextProp);

            RunProperties runFootNoteTextProp = new RunProperties();
            RunFonts runFootNoteTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
            FontSize fontFootNoteTextSize = new FontSize() { Val = "28" };
            FontSizeComplexScript fontFootNoteComplexScriptSize = new FontSizeComplexScript() { Val = "28" };
            RightToLeftText rtlTextFootNote = new RightToLeftText();
            runFootNoteTextProp.Append(runFootNoteTextFonts);
            runFootNoteTextProp.Append(fontFootNoteTextSize);
            runFootNoteTextProp.Append(fontFootNoteComplexScriptSize);
            runFootNoteTextProp.Append(rtlTextFootNote);
           
            Style styleFootNoteTitle = new Style();
            styleFootNoteTitle.StyleId = "FootNote";
            styleFootNoteTitle.Append(new Name() { Val = "Taya Madbatah FootNote Text" });
            styleFootNoteTitle.Append(new BasedOn() { Val = "Normal" });
            styleFootNoteTitle.Append(new NextParagraphStyle() { Val = "Normal" });
            styleFootNoteTitle.Append(runFootNoteTextProp);


            RunProperties runFooterTextProp = new RunProperties();
            RunFonts runFooterTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
            FontSize fontFooterTextSize = new FontSize() { Val = "28" };
            FontSizeComplexScript fontFooterComplexScriptSize = new FontSizeComplexScript() { Val = "28" };
            RightToLeftText rtlTextFooter = new RightToLeftText();
            runFootNoteTextProp.Append(runFooterTextFonts);
            runFootNoteTextProp.Append(fontFooterTextSize);
            runFootNoteTextProp.Append(fontFooterComplexScriptSize);
            runFootNoteTextProp.Append(rtlTextFooter);

            Style styleFooterTitle = new Style();
            styleFooterTitle.StyleId = "Footer";
            styleFooterTitle.Append(new Name() { Val = "Taya Madbatah Footer Text" });
            styleFooterTitle.Append(new BasedOn() { Val = "Normal" });
            styleFooterTitle.Append(new NextParagraphStyle() { Val = "Normal" });
            styleFooterTitle.Append(runFooterTextProp);

                    
            Style styleTableGrid = new Style() { Type = StyleValues.Table, StyleId = "styleTableGrid" };
            StyleName styleName1 = new StyleName() { Val = "Table Grid" };
            BasedOn basedOn1 = new BasedOn() { Val = "TableNormal" };
            UIPriority uIPriority1 = new UIPriority() { Val = 59 };
            Rsid rsid1 = new Rsid() { Val = "005F1CC5" };


            RunProperties runTableParagraphTitleTextProp = new RunProperties();
            RunFonts runTableParagraphTitleTextFonts = new RunFonts() { Ascii = "Sakkal Majalla", HighAnsi = "Sakkal Majalla", ComplexScript = "Sakkal Majalla" };
           // Bold Tablebold = new Bold();
           // BoldComplexScript TableBoldComplexScript = new BoldComplexScript();
            FontSize fontTableParagraphTitleTextSize = new FontSize() { Val = "36" };
            FontSizeComplexScript fontTableParagraphTitleComplexScriptSize = new FontSizeComplexScript() { Val = "36" };
            RightToLeftText TableRtlTextParagraphTitle = new RightToLeftText();
            Justification justification = new Justification();
            justification.Val = JustificationValues.Right;
            runTableParagraphTitleTextProp.Append(runTableParagraphTitleTextFonts);
          //  runTableParagraphTitleTextProp.Append(Tablebold);
          //  runTableParagraphTitleTextProp.Append(TableBoldComplexScript);
            runTableParagraphTitleTextProp.Append(fontTableParagraphTitleTextSize);
            runTableParagraphTitleTextProp.Append(fontTableParagraphTitleComplexScriptSize);
            runTableParagraphTitleTextProp.Append(TableRtlTextParagraphTitle);
            runTableParagraphTitleTextProp.Append(justification);

            StyleParagraphProperties styleParagraphProperties1 = new StyleParagraphProperties();
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines()
            {
                After = "0",
                Line = "240",
                LineRule = LineSpacingRuleValues.Auto
            };

            styleParagraphProperties1.Append(spacingBetweenLines1);

            StyleTableProperties styleTableProperties1 = new StyleTableProperties();
            TableIndentation tableIndentation1 = new TableIndentation()
            {
                Width = 0,
                Type = TableWidthUnitValues.Dxa
            };

            TableBorders tableBorders1 = new TableBorders();
            TopBorder topBorder1 = new TopBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            LeftBorder leftBorder1 = new LeftBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            BottomBorder bottomBorder1 = new BottomBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            RightBorder rightBorder1 = new RightBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            InsideHorizontalBorder insideHorizontalBorder1 = new InsideHorizontalBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            InsideVerticalBorder insideVerticalBorder1 = new InsideVerticalBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };

            tableBorders1.Append(topBorder1);
            tableBorders1.Append(leftBorder1);
            tableBorders1.Append(bottomBorder1);
            tableBorders1.Append(rightBorder1);
            tableBorders1.Append(insideHorizontalBorder1);
            tableBorders1.Append(insideVerticalBorder1);


            styleTableProperties1.Append(tableIndentation1);
            styleTableProperties1.Append(tableBorders1);


            styleTableGrid.Append(styleName1);
            styleTableGrid.Append(basedOn1);
            styleTableGrid.Append(uIPriority1);
            styleTableGrid.Append(rsid1);
            styleTableGrid.Append(styleParagraphProperties1);
            styleTableGrid.Append(styleTableProperties1);
            styleTableGrid.Append(runTableParagraphTitleTextProp);

            _docStylePart.Styles = new Styles();
            _docStylePart.Styles.Append(styleNormalText);
            _docStylePart.Styles.Append(styleParagraphTitle);
            _docStylePart.Styles.Append(styleUnderlineParagraphTitle);
          //  _docStylePart.Styles.Append(styleTableGrid);
            _docStylePart.Styles.Append(styleFootNoteTitle);
            _docStylePart.Styles.Append(styleFooterTitle);
            //_docStylePart.Styles.Append(styleFootNote);
            _docStylePart.Styles.Save();

           

            // we have to add style that we have created to the StylePart
            // we save the style part
        }

        public void Validate()
        {
            if (!IsOpen)
            {
                throw new DocumentNotOpenException("The object must be Open before calling Validate()");
            }

            if (_currentDoc != null)
            {
                OpenXmlValidator validator = new OpenXmlValidator();
                validator.Validate(_currentDoc);

            }
        }

        public void SaveAs(string fileName)
        {
            if (!IsOpen)
            {
                throw new DocumentNotOpenException("The object must be Open before calling SaveAs()");
            }

            _outputPath = fileName;
            if (!string.IsNullOrEmpty(_outputPath))
            {
                //Close the in-memory document to ensure the memory stream is ready for saving
                CloseInMemoryDocument();
                try
                {

                    //Now save the stream to file
                    using (FileStream fileStream = new FileStream(_outputPath, System.IO.FileMode.Create))
                    {
                        _inMemoryStream.WriteTo(fileStream);
                    }

                }
                finally
                {
                    //Now close the memory stream. 
                    //The in-memory document has already been closed above 
                    //and there's no point having one without the other!
                    CloseInMemoryStream();
                }
            }
        }
        //private Footer MakeFooterPart(string FooterText)
        //{
        //    var element =
        //      new Footer(
        //        new Paragraph(
        //          new ParagraphProperties(
        //            new ParagraphStyleId() { Val = "Footer" }),
        //          new Run(
        //            new Text(FooterText))
        //        )
        //      );

        //    return element;
        //}


        //public void AppendFooter(string sFtText)
        //{
        //    //using (WordprocessingDocument wdDoc = WordprocessingDocument.Open(sFile, true))
        //    //{
        //    FooterPart newFtPart = _docMainPart.AddNewPart<FooterPart>();
        //    string ft_ID = _docMainPart.GetIdOfPart(newFtPart);

        //        MakeFooterPart(sFtText).Save(newFtPart);
        //        foreach (SectionProperties sectProperties in
        //                _docMainPart.Document.Descendants<SectionProperties>())
        //        {
        //            FooterReference newFtReference =
        //             new FooterReference() { Id = ft_ID, Type = HeaderFooterValues.Default };
        //            sectProperties.Append(newFtReference);
        //        }
        //        //  Save the changes to the main document part.
        //        _docMainPart.Document.Save();
        //    //}
        //}


        /// <summary>
        /// 
        /// </summary>
        public void ApplyHeaderAndFooter()
        {
            if (!IsOpen)
            {
                throw new DocumentNotOpenException("The object must be Open before calling ApplyHeaderAndFooter()");
            }


            if (_docMainPart == null)
            {
                _docMainPart = _currentDoc.AddMainDocumentPart();
                _docMainPart.Document = MakeEmpyDocument();
            }
            else
            {
                // Delete the existing header and Footer parts.
                _docMainPart.DeleteParts(_docMainPart.HeaderParts);
                _docMainPart.DeleteParts(_docMainPart.FooterParts);
            }

            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();

            //Multi-sectioned documents seem to have their SectionProperties hidden away inside paragraphs.
            //Go through all these first, and set a flag if we find any.
            bool paragraphSectionPropertiesFound = false;
            IEnumerable<Paragraph> paragraphElements = _docMainPart.Document.Body.Elements<Paragraph>().AsEnumerable();
            foreach (Paragraph paragraph in paragraphElements)
            {
                ParagraphProperties paragraphProperties = paragraph.Elements<ParagraphProperties>().LastOrDefault();
                if (paragraphProperties != null)
                {
                    SectionProperties sectionProps = paragraphProperties.Elements<SectionProperties>().LastOrDefault();
                    if (sectionProps != null)
                    {
                        ApplyHeaderToSectionProperties(sectionProps);
                        ApplyFooterToSectionProperties(sectionProps);
                        paragraphSectionPropertiesFound = true;
                    }
                }
            }

            //documents can also have SectionProperties under the body. Particularly single section Documents.
            SectionProperties bodySectionProperties = _docMainPart.Document.Body.Elements<SectionProperties>().LastOrDefault();

            //if no SectionProperties were found in paragraphs, we will populate the SectionProperties found in the Body instead
            if (paragraphSectionPropertiesFound)
            {
                //SectionProperties were set in the paragraph(s) which should be sufficient, 
                //We'll just remove any header/footer references we find in the Body SectionProperties
                if (bodySectionProperties != null)
                {
                    bodySectionProperties.RemoveAllChildren<HeaderReference>();
                    bodySectionProperties.RemoveAllChildren<FooterReference>();
                }
            }
            else
            {
                if (bodySectionProperties == null)
                {
                    //not there? create it
                    bodySectionProperties = new SectionProperties();
                    ApplyHeaderToSectionProperties(bodySectionProperties);
                    ApplyFooterToSectionProperties(bodySectionProperties);
                    _docMainPart.Document.Body.Append(bodySectionProperties);
                }
                else
                {
                    ApplyHeaderToSectionProperties(bodySectionProperties);
                    ApplyFooterToSectionProperties(bodySectionProperties);
                }
            }

            Save();
        }
        public void ApplyFooter()
        {
            if (!IsOpen)
            {
                throw new DocumentNotOpenException("The object must be Open before calling ApplyHeaderAndFooter()");
            }


            if (_docMainPart == null)
            {
                _docMainPart = _currentDoc.AddMainDocumentPart();
                _docMainPart.Document = MakeEmpyDocument();
            }
            else
            {
                // Delete the existing header and Footer parts.
                 _docMainPart.DeleteParts(_docMainPart.FooterParts);
            }

            if (_docMainPart.Document == null)
                _docMainPart.Document = MakeEmpyDocument();

            //Multi-sectioned documents seem to have their SectionProperties hidden away inside paragraphs.
            //Go through all these first, and set a flag if we find any.
            bool paragraphSectionPropertiesFound = false;
            IEnumerable<Paragraph> paragraphElements = _docMainPart.Document.Body.Elements<Paragraph>().AsEnumerable();
            foreach (Paragraph paragraph in paragraphElements)
            {
                ParagraphProperties paragraphProperties = paragraph.Elements<ParagraphProperties>().LastOrDefault();
                if (paragraphProperties != null)
                {
                    SectionProperties sectionProps = paragraphProperties.Elements<SectionProperties>().LastOrDefault();
                    if (sectionProps != null)
                    {
                         ApplyFooterToSectionProperties(sectionProps);
                        paragraphSectionPropertiesFound = true;
                    }
                }
            }

            //documents can also have SectionProperties under the body. Particularly single section Documents.
            SectionProperties bodySectionProperties = _docMainPart.Document.Body.Elements<SectionProperties>().LastOrDefault();

            //if no SectionProperties were found in paragraphs, we will populate the SectionProperties found in the Body instead
            if (paragraphSectionPropertiesFound)
            {
                //SectionProperties were set in the paragraph(s) which should be sufficient, 
                //We'll just remove any header/footer references we find in the Body SectionProperties
                if (bodySectionProperties != null)
                {
                    bodySectionProperties.RemoveAllChildren<FooterReference>();
                }
            }
            else
            {
                if (bodySectionProperties == null)
                {
                    //not there? create it
                    bodySectionProperties = new SectionProperties();
                    ApplyFooterToSectionProperties(bodySectionProperties);
                    _docMainPart.Document.Body.Append(bodySectionProperties);
                }
                else
                {
                    ApplyFooterToSectionProperties(bodySectionProperties);
                }
            }

            Save();
        }

        private void ApplyHeaderToSectionProperties(SectionProperties sectionProps)
        {
            // Create a new header part and grab its id for the header reference.
            HeaderPart headerPart = _docMainPart.AddNewPart<HeaderPart>();
            string rId = _docMainPart.GetIdOfPart(headerPart);

            //create our Header reference
            HeaderReference headerRef = new HeaderReference();
            headerRef.Id = rId;

            sectionProps.RemoveAllChildren<HeaderReference>();
            sectionProps.Append(headerRef);

            //Now populate the header contents
            headerPart.Header = MakeHeader();
            headerPart.Header.Save();
        }

        private void ApplyFooterToSectionProperties(SectionProperties sectionProps)
        {
            // Create a new Footer part and grab its id for the Footer reference.
            FooterPart footerPart = _docMainPart.AddNewPart<FooterPart>();
            string rId = _docMainPart.GetIdOfPart(footerPart);

            //create our Footer reference
            FooterReference footerRef = new FooterReference();
            footerRef.Id = rId;

            sectionProps.RemoveAllChildren<FooterReference>();
            sectionProps.Append(footerRef);

            //Now populate the Footer contents
            footerPart.Footer = MakeFooter();
            footerPart.Footer.Save();

        }

        public void writeImage(int ii,string text,string img_path)
        {
            Text textPlaceHolder = _docMainPart.Document.Body.Descendants<Text>()
                .Where((x) => x.Text == text).First();

            if (textPlaceHolder == null)
            {
                Console.WriteLine("Text holder not found!");
            }
            else
            {
                var parent = textPlaceHolder.Parent;

                if (!(parent is Run))  // Parent should be a run element.
                {
                    Console.Out.WriteLine("Parent is not run");
                }
                else
                {
                    // Insert image (the image created with your function) after text place holder. 
                   // DocumentFormat.OpenXml.Office.Drawing.Drawing element = new DocumentFormat.OpenXml.Office.Drawing.Drawing(img_path);
                  //  ImageWriter.InsertImageAfterPlaceHolder(_docMainPart.Document.Body, _docMainPart, img_path, "rId" + ii, textPlaceHolder);
                   // textPlaceHolder.Parent.InsertAfter<DocumentFormat.OpenXml.Office.Drawing.Drawing>(element, textPlaceHolder);
                    // Remove text place holder.
                    textPlaceHolder.Remove();
                }
            }
        }

        public void yy()
        {
            Text textPlaceHolder = _docMainPart.Document.Body.Descendants<Text>()
                .Where((x) => x.Text == "$image_tag$").First();

            if (textPlaceHolder == null)
            {
                Console.WriteLine("Text holder not found!");
            }
            else
            {
                var parent = textPlaceHolder.Parent;

                if (!(parent is Run))  // Parent should be a run element.
                {
                    Console.Out.WriteLine("Parent is not run");
                }
                else
                {
                    // Insert image (the image created with your function) after text place holder. 
                    DocumentFormat.OpenXml.Office.Drawing.Drawing element = new DocumentFormat.OpenXml.Office.Drawing.Drawing("C:\\Users\\Desktop\\TestPdfs\\Signature1.png");
                    textPlaceHolder.Parent.InsertAfter<DocumentFormat.OpenXml.Office.Drawing.Drawing>(element, textPlaceHolder);
                    // Remove text place holder.
                    textPlaceHolder.Remove();
                }
            }
        }

        public static Document MakeEmpyDocument()
        {
            Document doc = new Document();

            Body body = new Body();
            //Paragraph paragraph = new Paragraph();
            //Run run = new Run();
            //Text text = new Text("");
            //run.Append(text);
            //paragraph.Append(run);
            //body.Append(paragraph);
            doc.Append(body);

            return doc;
        }

        public Header MakeHeader()
        {
            Header header = new Header();
            Paragraph paragraph = new Paragraph();
            Run run = new Run(new Text(_docHeaderString) { Space = SpaceProcessingModeValues.Preserve });
            ParagraphProperties paragraphProp = new ParagraphProperties();
            BiDi biDi1 = new BiDi();
            Justification justification = new Justification();
            justification.Val = JustificationValues.Center;
            paragraphProp.Append(justification);
            paragraphProp.ParagraphStyleId = new ParagraphStyleId() { Val = ParagraphStyle.NormalArabic.ToString() }; // we set the style
            paragraph.Append(paragraphProp);    //HeadingArabic
            paragraph.Append(run);

            header.Append(paragraph);
            return header;
        }

        public Footer MakeFooter()
        {
            Footer footer = new Footer();

            ParagraphProperties paragraphProperties = new ParagraphProperties(new RightToLeftText(),
                            new ParagraphStyleId() { Val = "Footer" }, new RunProperties(new RightToLeftText()),
                            new Tabs(
                                new TabStop() { Val = TabStopValues.Clear, Position = 4320 },
                                new TabStop() { Val = TabStopValues.Clear, Position = 8640 },
                                new TabStop() { Val = TabStopValues.Center, Position = 4820 },
                                new TabStop() { Val = TabStopValues.Right, Position = 6639 }));
            ParagraphProperties paragraphProperties2 = new ParagraphProperties(new RightToLeftText(),
                         new ParagraphStyleId() { Val = "Footer" }, new RunProperties(new RightToLeftText()));

            paragraphProperties.ParagraphStyleId = new ParagraphStyleId() { Val = ParagraphStyle.Footer.ToString() };

            Paragraph paragraph1 = new Paragraph(

                       paragraphProperties,
                         new Run(
                           new Text(" ------") { Space = SpaceProcessingModeValues.Preserve }, new RunProperties(new RightToLeftText())
                         )
                   );

            Paragraph paragraph2 = new Paragraph(

                        paragraphProperties2,
                        new Run(
                            new FieldChar() { FieldCharType = FieldCharValues.Begin }, new RunProperties(new RightToLeftText())),
                        new Run(
                            new FieldCode(" TITLE   \\* MERGEFORMAT ") { Space = SpaceProcessingModeValues.Preserve }, new RunProperties(new RightToLeftText())
                        ),
                        new Run(
                            new FieldChar() { FieldCharType = FieldCharValues.End }, new RunProperties(new RightToLeftText())),
                        new Run(
                            new Text(" - ") { Space = SpaceProcessingModeValues.Preserve }, new RunProperties(new RightToLeftText())
                        ),
                        new SimpleField(
                            new Run(
                                new RunProperties(
                                    new NoProof(), new RightToLeftText()),
                                new Text("1")
                            )
                        ) { Instruction = " PAGE   \\* MERGEFORMAT " },
                        new Run(
                            new Text(" - ") { Space = SpaceProcessingModeValues.Preserve }, new RunProperties(new RightToLeftText())
                        ),
                        new Run(
                            new Text(" الأمانة العامة لمجلس الأمة | قطاع الجلسات | إدارة المضابط ") { Space = SpaceProcessingModeValues.Preserve }, new RunProperties(new RightToLeftText())
                        )
                    );

            footer.Append(paragraph1);
            footer.Append(paragraph2);

            return footer;
        }

        /// <summary>
        /// Closes and disposes the in-memory document
        /// </summary>

        private void CloseInMemoryDocument()
        {
            if (_currentDoc != null)
            {

                _currentDoc.Close();
                _currentDoc.Dispose();
                _currentDoc = null;
            }



        }
        /// <summary>
        /// Closes and disposes the memory stream
        /// </summary>
        private void CloseInMemoryStream()
        {
            if (_inMemoryStream != null)
            {
                _inMemoryStream.Close();
                _inMemoryStream.Dispose();
                _inMemoryStream = null;
            }
        }

        public void Close()
        {
            CloseInMemoryDocument();
            CloseInMemoryStream();
        }

        public void Dispose()
        {
            Close();


        }

        
        public static byte[] OpenAndCombine(IList<byte[]> documents)
        {
            MemoryStream mainStream = new MemoryStream();

            mainStream.Write(documents[0], 0, documents[0].Length);
            mainStream.Position = 0;

            int pointer = 1;
            byte[] ret;
            try
            {
                using (WordprocessingDocument mainDocument = WordprocessingDocument.Open(mainStream, true))
                {

                    XElement newBody = XElement.Parse(mainDocument.MainDocumentPart.Document.Body.OuterXml);

                    for (pointer = 1; pointer < documents.Count; pointer++)
                    {
                        WordprocessingDocument tempDocument = WordprocessingDocument.Open(new MemoryStream(documents[pointer]), true);
                        XElement tempBody = XElement.Parse(tempDocument.MainDocumentPart.Document.Body.OuterXml);

                        newBody.Add(tempBody);
                        mainDocument.MainDocumentPart.Document.Body = new Body(newBody.ToString());
                        mainDocument.MainDocumentPart.Document.Save();
                        mainDocument.Package.Flush();
                    }
                }
            }
            catch (OpenXmlPackageException oxmle)
            {
                //throw new OfficeMergeControlException(string.Format(CultureInfo.CurrentCulture, "Error while merging files. Document index {0}", pointer), oxmle);
            }
            catch (Exception e)
            {
                //throw new OfficeMergeControlException(string.Format(CultureInfo.CurrentCulture, "Error while merging files. Document index {0}", pointer), e);
            }
            finally
            {
                ret = mainStream.ToArray();
                mainStream.Close();
                mainStream.Dispose();
            }
            return (ret);
        }
        static void AppendPageBreak(WordprocessingDocument myDoc)
        {
            MainDocumentPart mainPart = myDoc.MainDocumentPart;
            OpenXmlElement last = myDoc.MainDocumentPart.Document
                .Body
                .Elements()
                .LastOrDefault(e => e is Paragraph || e is AltChunk);
            last.InsertAfterSelf(new Paragraph(
                new Run(
                    new Break() { Type = BreakValues.Page })));
        }
        public static void MergeWithAltChunk(string outPath, string[] files)
        {
            //using (WordprocessingWorker myDoc = new WordprocessingWorker(outPath,GetDocParts(@"E:\FNC\EMadbatah\TayaIT.Enterprise.EMadbatah.Web\resources\"), DocFileOperation.Open))
            using (WordprocessingDocument myDoc = WordprocessingDocument.Open(outPath, true))
            {
                MainDocumentPart mainPart = myDoc.MainDocumentPart;
                /*if (mainPart == null)
                    mainPart = myDoc.AddMainDocumentPart();
                if (mainPart.Document == null)
                    mainPart.Document = MakeEmpyDocument();
                */
                for (int i = 0; i < files.Length; i++)
                {
                    string altChunkId = "AltChunkId" + i;
                    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                    AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                    using (FileStream fileStream = File.Open(files[i], FileMode.Open))
                        chunk.FeedData(fileStream);
                    AltChunk altChunk = new AltChunk();
                    altChunk.Id = altChunkId;
                    //Ibrahim
                    //AltChunkProperties acp = new AltChunkProperties() { MatchSource = new MatchSource() { Val = OnOffValue.FromBoolean(true) } };
                    //altChunk.Append(acp);
                    //Paragraph  para = new Paragraph(new Run(new Break()));
                    //altChunk.Append(para);

                    mainPart.Document.Body.InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());
                    if (i != files.Length - 1)
                        AppendPageBreak(myDoc);
                    //mainPart.Document.Append(para);
                    //mainPart.Document.Body.InsertAfter(para, mainPart.Document.Body.Elements<AltChunk>().Last());

                }
                mainPart.Document.Save();
            }

            /*
            using (WordprocessingDocument myDoc = WordprocessingDocument.Open(doc1, true))
             {
                 string altChunkId = "AltChunkId2";
                 MainDocumentPart mainPart = myDoc.MainDocumentPart;
                 AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                 AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                 using (FileStream fileStream = File.Open(doc2, FileMode.Open))
                 chunk.FeedData(fileStream);
                 AltChunk altChunk = new AltChunk();
                 altChunk.Id = altChunkId;
                 mainPart.Document.Body.InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());
                 mainPart.Document.Save();
             }
           */
        }
        public static void InsertAPicture(string document, string fileName)
        {
            using (WordprocessingDocument wordprocessingDocument =
                WordprocessingDocument.Open(document, true))
            {

                MainDocumentPart mainPart = wordprocessingDocument.MainDocumentPart;
                //if (mainPart == null)
                //    mainPart = wordprocessingDocument.AddMainDocumentPart();
                //if (mainPart.Document == null)
                //    mainPart.Document = MakeEmpyDocument();

                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                AddImageToBody(wordprocessingDocument, mainPart.GetIdOfPart(imagePart));
            }
        }

        public static void InsertAPicture(string document, string[] fileNames)
        {
            using (WordprocessingDocument wordprocessingDocument =
                WordprocessingDocument.Open(document, true))
            {
                MainDocumentPart mainPart = wordprocessingDocument.MainDocumentPart;
                if (mainPart == null)
                    mainPart = wordprocessingDocument.AddMainDocumentPart();
                if (mainPart.Document == null)
                    mainPart.Document = MakeEmpyDocument();
                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                foreach (string fileName in fileNames)
                    using (FileStream stream = new FileStream(fileName, FileMode.Open))
                    {
                        imagePart.FeedData(stream);
                    }

                AddImageToBody(wordprocessingDocument, mainPart.GetIdOfPart(imagePart));
            }
        }

        public void InsertPictures(string[] fileNames)
        {
            try
            {
                MainDocumentPart mainPart = _currentDoc.MainDocumentPart;
                if (mainPart == null)
                    mainPart = _currentDoc.AddMainDocumentPart();
                if (mainPart.Document == null)
                    mainPart.Document = MakeEmpyDocument();
                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                foreach (string fileName in fileNames)
                {
                    FileStream stream = new FileStream(fileName, FileMode.Open);
                    imagePart.FeedData(stream);
                }
                AddImageToBody(_currentDoc, mainPart.GetIdOfPart(imagePart));
            }
            catch (Exception ex)
            {
            }
        }

        private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {

            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                       "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         ) { Preset = A.ShapeTypeValues.Rectangle }))
                             ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }
        public void AddFootnote(string text)
        {
            FootnoteEndnoteReferenceType reference;
            reference = new FootnoteReference() { Id = AddFootnoteReference(text) };
            Run run;
            List<OpenXmlElement> elements = new List<OpenXmlElement>();
            elements.Add(
            run = new Run(
                new RunProperties(
                    new RunStyle() { Val = "Normal" }),
                reference)
            );
            Paragraph paragraph = new Paragraph();


            paragraph.Append(elements);

            _docMainPart.Document.Body.Append(paragraph);


            //_docMainPart.Document.Body.Append(run);

        }
        private Int32 footnotesRef = 1, endnotesRef = 1, figCaptionRef = -1;
        private int AddFootnoteReference(string description)
        {
            FootnotesPart fpart = _docMainPart.FootnotesPart;
            if (fpart == null)
                fpart = _docMainPart.AddNewPart<FootnotesPart>();

            if (fpart.Footnotes == null)
            {
                // Insert a new Footnotes reference
                new Footnotes(
                    new Footnote(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId() { Val = "FootNote" }, new Justification() { Val = JustificationValues.Right },
                                new SpacingBetweenLines() { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto }),
                            new Run(new RightToLeftText(),
                                new SeparatorMark() , new NumberingRestart() { Val = RestartNumberValues.EachPage })
                        )
                    ) { Type = FootnoteEndnoteValues.Separator, Id = -1 },
                    new Footnote(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId() { Val = "FootNote" }, new Justification() { Val = JustificationValues.Right },
                                new SpacingBetweenLines() { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto }),
                            new Run(new RightToLeftText(),
                                new ContinuationSeparatorMark(), new NumberingRestart() { Val = RestartNumberValues.EachPage })
                        )
                    ) { Type = FootnoteEndnoteValues.ContinuationSeparator, Id = 0 }).Save(fpart);
                footnotesRef = 1;
            }
            else
            {
                // The footnotesRef Id is a required field and should be unique. You can assign yourself some hard-coded
                // value but that's absolutely not safe. We will loop through the existing Footnote
                // to retrieve the highest Id.
                foreach (var p in fpart.Footnotes.Elements<Footnote>())
                {
                    if (p.Id.HasValue && p.Id > footnotesRef) footnotesRef = (int)p.Id.Value;
                }
                footnotesRef++;
            }

            Run markerRun;
            fpart.Footnotes.Append(
                new Footnote(
                    new Paragraph(
                        new ParagraphProperties(new Justification() { Val = JustificationValues.Right },
                            new ParagraphStyleId() { Val = "FootNote" }),//htmlStyles.GetStyle("footnote text", false) }),
                        markerRun = new Run(
                            new RunProperties(new RightToLeftText(),
                                new RunStyle() { Val = "Normal" }),//htmlStyles.GetStyle("footnote reference", true) }),
                            new FootnoteReferenceMark()),
                        new Run(
                // Word insert automatically a space before the definition to separate the reference number
                // with its description
                            new Text(" " + description) { Space = SpaceProcessingModeValues.Preserve })
                    )
                ) { Id = footnotesRef });

            /*if (!htmlStyles.DoesStyleExists("footnote reference"))
            {
                // Force the superscript style because if the footnote text style does not exists,
                // the rendering will be awful.
                markerRun.InsertInProperties(new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript });
            }
            fpart.Footnotes.Save();
            */
            return footnotesRef;
        }

        public static void SaveDOCX(string fileName, string BodyText, bool isLandScape, double rMargin, double lMargin, double bMargin, double tMargin)
        {
            WordprocessingDocument _currentDoc = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document);

            MainDocumentPart _docMainPart = _currentDoc.AddMainDocumentPart();
            _docMainPart.Document = MakeEmpyDocument();
            //InitializeDocumentPartsFromXml();
            // InitializeDocumentStyles();

            MainDocumentPart mainDocumenPart = _currentDoc.MainDocumentPart;

            //Place the HTML String into a MemoryStream Object
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"><html><head></head><body>" + BodyText + "</body></html>"));

            //Assign an HTML Section for the String Text
            string htmlSectionID = "Sect1";

            // Create alternative format import part.
            AlternativeFormatImportPart formatImportPart = mainDocumenPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, htmlSectionID);

            // Feed HTML data into format import part (chunk).
            formatImportPart.FeedData(ms);
            AltChunk altChunk = new AltChunk();
            altChunk.Id = htmlSectionID;

            //Clear out the Document Body and Insert just the HTML string.  (This prevents an empty First Line)
            mainDocumenPart.Document.Body.RemoveAllChildren();
            mainDocumenPart.Document.Body.Append(altChunk);

            /*
             Set the Page Orientation and Margins Based on Page Size
             inch equiv = 1440 (1 inch margin)
             */
            double width = 8.27 * 1440;
            double height = 11.69 * 1440;

            SectionProperties sectionProps = new SectionProperties();
            PageSize pageSize;
            if (isLandScape)
                pageSize = new PageSize() { Width = (UInt32Value)height, Height = (UInt32Value)width, Orient = PageOrientationValues.Landscape };
            else
                pageSize = new PageSize() { Width = (UInt32Value)width, Height = (UInt32Value)height, Orient = PageOrientationValues.Portrait };

            rMargin = rMargin * 1440;
            lMargin = lMargin * 1440;
            bMargin = bMargin * 1440;
            tMargin = tMargin * 1440;

            //Top = 1440, Right = (UInt32Value)1440U, Bottom = 1440, Left = (UInt32Value)1440U, Header = (UInt32Value)850U, Footer = (UInt32Value)130U, Gutter = (UInt32Value)0U 
            PageMargin pageMargin = new PageMargin() { Top = (Int32)tMargin, Right = (UInt32Value)rMargin, Bottom = (Int32)bMargin, Left = (UInt32Value)lMargin, Header = (UInt32Value)850U, Footer = (UInt32Value)130U, Gutter = (UInt32Value)0U };

            sectionProps.Append(pageSize);
            sectionProps.Append(pageMargin);
            mainDocumenPart.Document.Body.Append(sectionProps);

            //Saving/Disposing of the created word Document
            _currentDoc.MainDocumentPart.Document.Save();
            _currentDoc.Dispose();
        }

    }
}

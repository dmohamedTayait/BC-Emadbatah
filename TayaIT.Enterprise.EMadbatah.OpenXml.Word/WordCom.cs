using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TayaIT.Enterprise.EMadbatah.Word;

namespace TayaIT.Enterprise.EMadbatah.OpenXml.Word
{
    public class WordCom
    {
        public static int GetDocumentNumPages(string docPath)
        {
            int numPages = 0;
            try
            {
                WordDocument doc = new WordDocument();
                doc.Open(docPath, false);
                numPages = doc.GetNumPages();
                doc.Quit();
            }
            catch (Exception ex)
            {
                Util.LogHelper.LogException(ex, "WordCom.GetDocumentNumPages");
                System.Threading.Thread.ResetAbort();
            }
            return numPages;
        }

        public static long GetDocumentLineNum(string docPath,bool end)
        {
            WordDocument doc = new WordDocument();
            doc.Open(docPath, false);
            long numPages = doc.GetCurrentPageLineNumber(end);
            doc.Quit();

            return numPages;
        }

        public static long GetDocumentLineNum(string docPath, int page)
        {
            long numPages = 0;
            try
            {
                WordDocument doc = new WordDocument();
                doc.Open(docPath, false);
                numPages = doc.GetCurrentPageLineNumber(page);
                doc.Quit();
            }
            catch (Exception ex)
            {
                Util.LogHelper.LogException(ex, "WordCom.GetDocumentLineNum");
                System.Threading.Thread.ResetAbort();
            }
            return numPages;
        }

        public static bool ConvertDocument(string sourceDocPath, string targetFilePath, EMadbatah.Model.TargetFormat format)
        {
            return WordConverter.ConvertDocument(sourceDocPath, targetFilePath, format);
        }
    }
}

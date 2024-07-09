using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using TechTalk.SpecFlow;

namespace ImportFile
{
    [Binding]
    public class ReadFile
    {
        private string _filePath;
        private string _fileName;
        FeatureContext _context;
        public ReadFile(FeatureContext _fc)
        {
            _context = _fc;
        }


        [Given(@"I have a PDF File")]
        public void GivenIHaveAPDFFile()
        {
            //string fc = _context["FileName"] as string;
            string completeFilePath = _context["FilePath"] as string;
            string fileNameInDirectory = _context["FileNameWExtension"] as string;
            string downloadedFileName = _context["FileName"] as string;
            _filePath = completeFilePath;
            _fileName = fileNameInDirectory;
            // Ensure the downloaded file name matches the expected file name in the directory
            if (completeFilePath.Contains(fileNameInDirectory))
            {
                _fileName = fileNameInDirectory;
            }
            else
            {
                throw new ArgumentException($"Mismatch between downloaded file name '{downloadedFileName}' and file name in directory '{fileNameInDirectory}'.");
            }

            //Assert.IsTrue(File.Exists(_filePath), $"File {_filePath} does not exist.");
        }

        private bool FileWithPartialNameExists(string filePathDirectory, string fileName)
        {
            string[] files = Directory.GetFiles(filePathDirectory);
            return files.Any(file => Path.GetFileName(file).Contains(fileName));
        }


        [When(@"I read the file")]
        public void WhenIReadTheFile()
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                throw new InvalidOperationException("File path is not set.");
            }
            var fileExtension = Path.GetExtension(_fileName).ToLower();
            if (fileExtension == ".pdf")
            {
                ReadPdfFile();
            }
            else
            {
                Assert.Fail("Unsupported file format");
            }
        }

        private void ReadPdfFile()
        {
            using (PdfReader pdfReader = new PdfReader(_filePath))
            {
                using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
                {
                    int numberOfPages = pdfDocument.GetNumberOfPages();
                    for (int page = 1; page <= numberOfPages; page++)
                    {
                        var strategy = new SimpleTextExtractionStrategy();
                        string currentPageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                        Console.WriteLine(currentPageText);
                    }
                }
            }
        }

        [Then(@"I display contents of PDF file")]
        public void ThenIDisplayContentsOfPDFFile()
        {
            // The contents are already displayed in ReadPdfFile() method
        }
    }
}

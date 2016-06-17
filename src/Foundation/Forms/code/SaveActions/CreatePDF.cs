namespace Sitecore.Foundation.Forms.SaveActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Sitecore.Diagnostics;
    using Sitecore.Foundation.SitecoreExtensions.Services;
    using Sitecore.WFFM.Abstractions.Actions;
    using Sitecore.WFFM.Actions.Base;
    using iTextSharp.text.pdf;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Sitecore.Data;
    using Sitecore.WFFM.Abstractions.Actions;

    public class CreatePdf : WffmSaveAction
    {
        //public string BasePdf { get; set; }
        //public string DownloadFileName { get; set; }
        private const string PdfExtension = ".pdf";
        //private const string RelativePdfPath = "/Content/files/";
        public string BasePdfFile { get; set; }
        public string PdfRelativePath { get; set; }


        public override void Execute(ID formId, AdaptedResultList adaptedFields, ActionCallContext actionCallContext = null, params object[] data)
        {
            if (string.IsNullOrEmpty(this.BasePdfFile) || string.IsNullOrEmpty(this.PdfRelativePath))
            {
                Log.Warn("Can't create a PDF. BasePDF or relative path isn't set", this);
                ;
            }
            Dictionary<string, string> substitutions = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            foreach (AdaptedControlResult adaptedField in adaptedFields)
            {
                substitutions.Add(RemoveWhitespace(adaptedField.FieldName), adaptedField.Value);
            }
            string pdfPath = this.StampPdfAndGetPath(substitutions);
        }

        private string StampPdfAndGetPath(Dictionary<string, string> substitutions)
        {
            string appRootDir = HttpContext.Current.Server.MapPath("~");
            //var finalFileName = "/" + this.BasePdf.Replace(".pdf", "-");


            string startFile = appRootDir + this.PdfRelativePath + this.BasePdfFile;
            string finalFileRelativePath = this.PdfRelativePath ;
            string finalFile = appRootDir + finalFileRelativePath + Guid.NewGuid().ToString() + PdfExtension;
            string tmp = Path.GetTempFileName();
            tmp = Path.ChangeExtension(tmp, PdfExtension);
            //FileInfo tmpFile = new FileInfo(tmp) {Attributes = FileAttributes.Temporary};

            //if (!File.Exists(tmp))
            //{
                // create the PDF again only if it doesn't exist already
                using (FileStream existingFileStream = new FileStream(startFile, FileMode.Open))
                {
                    using (FileStream newFileStream = new FileStream(tmp, FileMode.Create))
                    {
                        // PDF read
                        PdfReader pdfReader = new PdfReader(existingFileStream);

                        PdfStamper stamper = new PdfStamper(pdfReader, newFileStream);

                        //Replace variables
                        var form = stamper.AcroFields;
                       // var fieldKeys = form.Fields.Keys;
                        //foreach (var kvp in substitutions)
                        //{
                        //    if (fieldKeys.Contains(kvp.Key))
                        //    {
                        //        form.SetField(kvp.Key, kvp.Value);
                        //    }
                        //}

                        //case insensitive comparison
                        //using actual key from the pdf form field
                        foreach (string key in form.Fields.Keys)
                        {
                            if (substitutions.ContainsKey(key))
                            {
                                form.SetField(key, substitutions[key]);
                            }
                        }

                        stamper.FormFlattening = true;

                        stamper.Close();
                        pdfReader.Close();
                    }
                //}
            }
            return finalFileRelativePath;
        }

        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        //private ActionResult GetFinalFile(string appRootDir, string pdfRelativePath, )
        //{
        //    string fileName = appRootDir + pdfRelativePath;
        //    if (System.IO.File.Exists(fileName))
        //    {
        //        return File(fileName, "pdf/application", this.DownloadFileName);
        //    }

        //}
    }
}
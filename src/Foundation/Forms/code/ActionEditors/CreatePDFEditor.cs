namespace Sitecore.Foundation.Forms.ActionEditors
{
    using System;
    using Sitecore.Data;
    using Sitecore.Foundation.SitecoreExtensions.Services;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Sheer;

    public class CreatePdfEditor : BaseActionEditor
    {
        public CreatePdfEditor(ISheerService sheerService) : base(sheerService)
        {
        }

        public CreatePdfEditor() : this(new SheerService())
        {
        }

        public DataContext ItemDataContext { get; set; }
        public Edit EbFileName { get; set; }
        public Edit EbRelativePath { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                //  var outcomeId = this.Parameters[Constants.OutcomeParameter];
                //  if (!string.IsNullOrEmpty(outcomeId) && ID.IsID(outcomeId))
                //  {
                //    this.ItemDataContext.DefaultItem = outcomeId;
                //  }

                var nameValueCollection = this.Parameters;
                if (nameValueCollection != null)
                {
                    this.EbFileName.Value = nameValueCollection[Constants.PdfFileParameter];
                    this.EbRelativePath.Value = nameValueCollection[Constants.PdfRelativePathParameter];
                }
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            //var item = this.ItemLister?.GetSelectionItem();
            //if (item == null || item.TemplateID != Constants.OutcomeTemplateId)
            //{
            //  SheerResponse.Alert("Please, select outcome");
            //  return;
            //}

            //this.Parameters.Set(Constants.OutcomeParameter, item.ID.ToString());
            if (this.EbFileName.Value.Length < 4)
            {
                SheerResponse.Alert("Please Enter a PDF filename");
            }
            else
            {
                this.Parameters.Set(Constants.PdfFileParameter, this.EbFileName.Value);
            }
            if (this.EbRelativePath.Value.Length < 1)
            {
                SheerResponse.Alert("Please Enter a Relative Path");
            }
            else
            {
                this.Parameters.Set(Constants.PdfRelativePathParameter, this.EbRelativePath.Value);
            }
            base.OnOK(sender, args);
        }
    }
}
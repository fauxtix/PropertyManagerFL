using System;

namespace PropertyManagerFL.Application.ViewModels.MailMerge
{
    public class MailMergePreviewModel
    {
        public string TemplateBase64 { get; set; } = string.Empty;
        public string[] MergeFields { get; set; } = Array.Empty<string>();
        public string[] ValuesFields { get; set; } = Array.Empty<string>();
    }
}

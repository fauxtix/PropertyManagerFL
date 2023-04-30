﻿using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Application.ViewModels.MailMerge
{
    public class MailMergeModel
    {
        public int CodContrato { get; set; }
        public string? WordDocument { get; set; }
        public string[]? MergeFields { get; set; }
        public string[]? ValuesFields { get; set; }
        public string? DocumentHeader { get; set; }
        public bool Referral { get; set; }
        public bool SaveFile { get; set; }
        public DocumentoEmitido TipoDocumentoEmitido { get; set; }
    }
}

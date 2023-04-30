namespace PropertyManagerFL.Application.ViewModels.Arrendamentos
{
    public class ArrendamentoVM
    {
        public int Id { get; set; }
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Fim { get; set; }
        public DateTime Data_Saida { get; set; }
        public DateTime Data_Pagamento { get; set; }
        public decimal SaldoInicial { get; set; }
        public bool Fiador { get; set; } = true;
        public int Prazo_Meses { get; set; }
        public int Prazo { get; set; } // em anos (1/20)
        public decimal Valor_Renda { get; set; }
        public bool Caucao { get; set; } = true;
        public bool Doc_IRS { get; set; } = true;
        public bool Doc_Vencimento { get; set; } = true;
        public string? Notas { get; set; }
        public int ID_Fracao { get; set; }
        public int ID_Inquilino { get; set; }
        public int ID_Fiador { get; set; }
        public bool ContratoEmitido { get; set; } = false;
        public string? DocumentoGerado { get; set; }
        public bool EnvioCartaAtualizacaoRenda { get; set; }
        public DateTime DataEnvioCartaAtualizacao { get; set; }
        public string? DocumentoAtualizacaoGerado { get; set; }
        public bool EnvioCartaRevogacao { get; set; }
        public bool RespostaCartaRevogacao { get; set; }
        public string? DocumentoRevogacaoGerado { get; set; } // Atraso no pagamento de renda / Revogação
        public DateTime DataEnvioCartaRevogacao { get; set; }
        public DateTime DataRespostaCartaRevogacao { get; set; }

        public bool EnvioCartaAtrasoRenda { get; set; }
        public string? DocumentoAtrasoRendaGerado { get; set; }
        public DateTime DataEnvioCartaAtraso { get; set; }
        public bool RespostaCartaAtrasoRenda { get; set; }
        public DateTime DataRespostaCartaAtrasoRenda { get; set; }

        public int FormaPagamento { get; set; }
        public bool Ativo { get; set; }
        public string LeiVigente { get; set; } = string.Empty;
        public bool ArrendamentoNovo { get; set; }
        public string? EstadoPagamento { get; set; }
        public bool RenovacaoAutomatica { get; set; }

        public string? NomeInquilino { get; set; }
        public string? Fracao { get; set; }
        public string? Porta { get; set; }
    }
}

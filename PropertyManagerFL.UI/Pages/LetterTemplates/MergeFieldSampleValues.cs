using System;

namespace PropertyManagerFL.UI.Pages.LetterTemplates;

/// <summary>
/// Valores realistas mas fictícios usados apenas na pré-visualização de modelos de carta.
/// Permite verificar texto traduzido, quebras de linha e layout sem gravar documentos.
/// </summary>
public static class MergeFieldSampleValues
{
    private static readonly string Today = DateTime.Now.ToString("dd/MM/yyyy");

    public static string For(string code) => code switch
    {
        "NomeOutorgante_1" => "João Silva",
        "NomeOutorgante_2" => "Maria Silva",
        "NomeOutorgante_3" => "Ana Silva",
        "DtNasc_O_1" => "01/01/1980",
        "DtNasc_O_2" => "02/02/1985",
        "DtNasc_O_3" => "03/03/2010",
        "MoradaOutorgante_1" => "Rua das Flores, 12, 1.º Esq., 1000-001 Lisboa",
        "MoradaOutorgante_2" => "Rua das Flores, 12, 1.º Esq., 1000-001 Lisboa",
        "MoradaOutorgante_3" => "Rua das Flores, 12, 1.º Esq., 1000-001 Lisboa",
        "NaturalidadeOutorgante_1" => "Lisboa",
        "NaturalidadeOutorgante_2" => "Porto",
        "NaturalidadeOutorgante_3" => "Lisboa",
        "EC_Outorgante_1" => "Casado",
        "EC_Outorgante_2" => "Casada",
        "EC_Outorgante_3" => "Solteira",
        "CC_Outorgante_1" => "12345678",
        "CC_Outorgante_2" => "87654321",
        "CC_Outorgante_3" => "11223344",
        "ValidadeCC_Outorgante_1" => "31/12/2030",
        "ValidadeCC_Outorgante_2" => "30/06/2029",
        "ValidadeCC_Outorgante_3" => "15/05/2031",
        "NIF_Outorgante_1" => "123456789",
        "NIF_Outorgante_2" => "987654321",
        "NIF_Outorgante_3" => "112233445",

        "Letra" => "A",
        "Andar" => "1.º",
        "Lado" => "Esquerdo",
        "MoradaImovel" => "Rua das Flores, 12",
        "Freguesia" => "Santo António",
        "Concelho" => "Lisboa",
        "Artigo" => "1234",
        "LicencaHabitacao" => "LH-2020/123",
        "DataLicencaHabitacao" => "15/06/2020",
        "Quartos" => "2",
        "MesesContrato" => "12",
        "DataInicioContrato" => "01/01/2026",
        "DiaInicioContrato" => "01",
        "MesInicioContrato" => "01",
        "AnoInicioContrato" => "2026",
        "InicioContrato" => "01/01/2026",
        "FimContrato" => "31/12/2026",

        "LocalEmissao" => "Lisboa",
        "DataEmissao" => Today,
        "NomeSenhorio" => "João Senhorio",
        "MoradaSenhorio" => "Rua do Senhorio, 5, 1000-002 Lisboa",
        "CodigoPostalSenhorio" => "1000-002",
        "NomeInquilino" => "Ana Inquilina",
        "MoradaInquilino" => "Rua do Inquilino, 8, 2.º Dto., 1000-003 Lisboa",
        "CodigoPostalInquilino" => "1000-003",
        "MoradaFracao" => "Rua das Flores, 12, 1.º Esq., 1000-001 Lisboa",

        "ValorRenda" => "650,00 €",
        "ValorRendaExtenso" => "seiscentos e cinquenta euros",
        "NovoValorRenda" => "663,00 €",
        "ExtensoNovoValor" => "seiscentos e sessenta e três euros",
        "MontanteRendasEmAtraso" => "1.300,00 €",
        "ValorExtenso" => "mil e trezentos euros",
        "RendasEmAtraso" => "2",
        "PrazoEmDias" => "15",
        "Coeficiente" => "1,02",
        "Lei" => "Lei n.º 6/2006",
        "DataPublicacao" => "15/03/2006",
        "AnoPublicacao" => "2006",
        "MatrizPredial" => "1234",
        "DiaAPartirDe" => "01",
        "MesAPartirDe" => "01",
        "AnoAtualizacao" => "2026",

        _ => $"[{code}]"
    };
}

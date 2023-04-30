namespace PropertyManagerFL.Application.Shared.Enums
{
    public static class AppDefinitions
    {
        public enum Roles
        {
            Admin = 1,
            Utilizador = 2,
        }

        public enum SituacaoFracao
        {
            Alugada = 1,
            Livre = 2,
            Reservada = 3,
            Vendida = 4,
            Contencioso = 5
        }

        public enum EstadoConservacaoImovel
        {
            Bom,
            PrecisaObras,
            Degradado
        }

        public enum EstadoConservacaoFracao
        {
            Bom,
            PrecisaObras,
            Degradado
        }

        public enum TipoBackup
        {
            SqLiteBackup,
            AccessBackup,
            SqlServerBackup
        }

        /// <summary>
        /// Tabelas a carregar nas comboboxes
        /// </summary>
        public enum ComboBoxItem
        {
            TipoDespesas,
            Role,
            TipoContacto,
            TipoPropriedade,
            TipoRecebimento,
            Utilizadores
        }

        public enum AlertMessageType
        {
            Error, Warning, Success, Info
        }

        /// <summary>
        /// Operações a efetuar sobre os registos
        /// </summary>
        public enum OpcoesRegisto
        {
            Inserir,
            Gravar,
            Apagar,
            Duplicar,
            Navegar,
            CriandoNovoRegisto,
            Backup,
            Zip,
            Warning,
            Info,
            Error
        }

        public enum Modules
        {
            Inquilinos,
            Fiadores,
            Fracoes,
            Imoveis,
            Recebimentos,
            Pagamentos,
            Contactos,
            Arrendamentos,
            PdfViewer
        }

        /// <summary>
        /// User roles supported by system
        /// </summary>
        public enum UserRole
        {
            Admin, GeneralUser
        }

        public enum OpcaoCRUD
        {
            Inserir,
            Atualizar,
            Anular,
            Ler
        }

        public enum TipoBD
        {
            SqLite,
            Access,
            SqlServer
        }

        public enum Idioma
        {
            Portugues,
            Espanhol,
            Ingles,
            Frances
        }

        public enum OpcaoSeguranca
        {
            Backup,
            Restore
        }

        public enum ImputacaoDespesa
        {
            Imovel = 1,
            Fracao = 2,
            Geral = 3
        }

        public enum OpcaoAlteracao
        {
            Permitir,
            Inibir
        }

        public enum EstadoPagamento
        {
            Pago = 1,
            PagoParcialmente = 2,
            EmDivida = 3
        }

        public enum DocumentoEmitido
        {
            ContratoArrendamento,
            AtualizacaoRendas,
            OposicaoRenovacaoContrato,
            RendasEmAtraso
        }

        public enum TipoDocumento
        {
            Word,
            Pdf,
            Excel
        }
    }
}

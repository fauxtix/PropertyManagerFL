using PropertyManagerFL.Application.Formatting;
using System.Configuration;

namespace PropertyManagerFLApplication.Configurations
{
	public static class ApplicationConfiguration
	{
		const string DEFAULT_CONNECTION_KEY = "PMConnection";
		const string LOGGING_ENABLED_KEY = "logging";
		const string TRACING_ENABLED_KEY = "tracing";
		const string SUPPORT_MAIL_KEY = "supportMail";
		const string SUPPORT_URL_KEY = "supportURL";
		const string PASTA_BD_SQLITE = "PastaBDSqLite";
		const string PASTA_BACKUP_SQLITE = "SqLiteBackupFolder";
		const string PASTA_IMAGEM_FUNDO = "PastaImagemFundo";
		const string PASTA_LISTAGENS = "PastaListagens";
		const string PASTA_DOCUMENTOS = "PastaDocumentos";
		const string FICHEIRO_BACKUP = "FicheiroBackup";
		const string PASTA_DOCUMENTOS_SCRIPTS = "PastaScripts";
		const string FIRST_TIME = "first_time";
		const string LANGUAGE = "language";
		const string NOTIFIER = "UsaNotifier";
		const string TEMPO_NOTIFIER = "TempoNotifier";
		const string ISCLIENT = "Is_Client";
		const string LOGO_IMAGE = "LogoImage";
		const string DOCSeCAUCAO = "DocumentosFiadorECaucaoObrigatorios";


		/// <summary>
		/// Gets default connection being used.
		/// </summary>
		public static string? DefaultConnection => ConfigurationManager.AppSettings[DEFAULT_CONNECTION_KEY];

		/// <summary>
		/// Gets DB provider from app.config file
		/// </summary>
		public static string DBProvider => ConfigurationManager.ConnectionStrings[DefaultConnection].ProviderName;

		public static bool? DocsECaucaoObrigatorios => DataFormat.GetBoolean(ConfigurationManager.AppSettings[DOCSeCAUCAO]);

		public static bool? Is_Client => DataFormat.GetBoolean(ConfigurationManager.AppSettings[ISCLIENT]);
		/// <summary>
		/// Gets connection string from app.config file
		/// </summary>
		public static string ConnectionString => ConfigurationManager.ConnectionStrings[0].ConnectionString;


		public static string? LogoImage => DataFormat.GetString(ConfigurationManager.AppSettings[LOGO_IMAGE]);
		/// <summary>
		/// Gets whether logging is enabled or not
		/// </summary>
		public static bool? LoggingEnabled => DataFormat.GetBoolean(ConfigurationManager.AppSettings[LOGGING_ENABLED_KEY]);

		public static string? PastaBackupSqLite => DataFormat.GetString(ConfigurationManager.AppSettings[PASTA_BACKUP_SQLITE]);

		public static string? PastaBDSqLite => DataFormat.GetString(ConfigurationManager.AppSettings[PASTA_BD_SQLITE]);

		public static string? PastaDocumentos => DataFormat.GetString(ConfigurationManager.AppSettings[PASTA_DOCUMENTOS]);

		public static string? PastaImagemFundo => DataFormat.GetString(ConfigurationManager.AppSettings[PASTA_IMAGEM_FUNDO]);

		public static string? PastaListagens => DataFormat.GetString(ConfigurationManager.AppSettings[PASTA_LISTAGENS]);

		public static string? Language => DataFormat.GetString(ConfigurationManager.AppSettings[LANGUAGE]);

		public static string? UsaNotifier => DataFormat.GetString(ConfigurationManager.AppSettings[NOTIFIER]);

		public static int Tempo_Notifier => Convert.ToInt32(DataFormat.GetString(ConfigurationManager.AppSettings[TEMPO_NOTIFIER]));

		public static string? FicheiroBackup => DataFormat.GetString(ConfigurationManager.AppSettings[FICHEIRO_BACKUP]);



		public static string? PastaScripts => DataFormat.GetString(ConfigurationManager.AppSettings[PASTA_DOCUMENTOS_SCRIPTS]);

		/// <summary>
		/// Gets whether tracing is enabled or not
		/// </summary>
		public static bool? TracingEnabled => DataFormat.GetBoolean(ConfigurationManager.AppSettings[TRACING_ENABLED_KEY]);

		/// <summary>
		/// Type of Logging/ Tracing
		/// </summary>
		public enum LogTraceType
		{
			SingleFile,
			DateWise
		}

		/// <summary>
		/// Gets support mail from app.config
		/// </summary>
		public static string? SupportMail => DataFormat.GetString(ConfigurationManager.AppSettings[SUPPORT_MAIL_KEY]);


		/// <summary>
		/// Gets support URL
		/// </summary>
		public static string? SupportURL => DataFormat.GetString(ConfigurationManager.AppSettings[SUPPORT_URL_KEY]);

		/// <summary>
		/// 1ª utilização do programa
		/// </summary>
		public static string? Primeira_Vez => DataFormat.GetString(ConfigurationManager.AppSettings[FIRST_TIME]);

	}
}
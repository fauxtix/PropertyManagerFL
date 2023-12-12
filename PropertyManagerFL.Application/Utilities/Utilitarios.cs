using Microsoft.Win32;
using PropertyManagerFLApplication.Configurations;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PropertyManagerFLApplication.Utilities
{
	public class Utilitarios
	{
		public static int GetMonthsBetweenDates(DateTime dInicio, DateTime dFim)
		{
			var iMeses = dFim.Subtract(dInicio).Days / (365.2425 / 12);
			return Convert.ToInt32(iMeses);
		}

		/// <summary>
		/// Verifica se email é valido
		/// </summary>
		/// <param name="strEmail"></param>
		/// <returns></returns>
		public bool IsEmailValid(string strEmail)
		{
			string s1;
			string s2;
			long i;
			bool blnIsItValid;
			blnIsItValid = true;
			string dummy;
			dummy = strEmail.Replace("@", "");
			i = strEmail.Length - dummy.Length;
			if (i != 1)
			{
				return false;
			}

			s1 = strEmail.Substring(strEmail.IndexOf("@") + 1);
			s2 = strEmail.Substring(0, strEmail.IndexOf("@"));
			if (s1.Length == 0 || s2.Length == 0)
			{
				return false;
			}

			for (int iLoop = 0; iLoop < s1.Length - 1; iLoop++)
			{
				dummy = s1.Substring(iLoop, 0);
				dummy = dummy.ToLower();
				int xxx = dummy.IndexOf("abcdefghijklmnopqrstuvwxyz_-.");
				if (xxx <= 0)
					blnIsItValid = false;

				dummy = s2.Substring(iLoop, 0);
				dummy = dummy.ToLower();
				xxx = dummy.IndexOf("abcdefghijklmnopqrstuvwxyz_-.");
				if (xxx <= 0)
					blnIsItValid = false;
				if (s1.Substring(0, 1) == "." || s2.Substring(0, 1) == ".")
					blnIsItValid = false;
				if (s1.Substring(s1.Length - 1) == "" || s2.Substring(s2.Length - 1, 1) == ".")
					blnIsItValid = false;
			}


			return blnIsItValid;

		}

		/// <summary>
		/// Valida e-mail
		/// </summary>
		/// <param name="Email"></param>
		/// <returns></returns>
		public bool ValidateEmail(string Email)
		{
			try
			{
				if ((Email.IndexOf('@') != Email.LastIndexOf('@')) || Email.IndexOf('@') == -1) return false;

				if (Email[Email.IndexOf('@') + 1] == '.') return false;

				if ((Email.LastIndexOf(".") == Email.Length - 1) || Email.LastIndexOf(".") == -1) return false;

				if (Email.Contains(" ") || Email.Contains("\\") || Email.Contains("/") || Email.Contains("*") || Email.Contains(";")) return false;

				return true;
			}
			catch //(System.Exception ex)
			{
				return false;
				//                throw new ApplicationException("E-Mail inválido");
				//throw;
			}

		}

		/// <summary>
		/// Calcula idade através da data de nascimento passada
		/// </summary>
		/// <param name="DataNascimento"></param>
		/// <returns></returns>
		public int CalculaIdade(DateTime DataNascimento)
		{
			int anos = DateTime.Now.Year - DataNascimento.Year;
			if (DateTime.Now.Month < DataNascimento.Month ||
				(DateTime.Now.Month == DataNascimento.Month && DateTime.Now.Day < DataNascimento.Day))
				anos--;
			return anos;
		}




		/// <summary>
		/// Justificação de texto
		/// </summary>
		/// <param name="s"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static String Justify(String s, Int32 count)
		{
			if (count <= 0)
				return s;

			Int32 middle = s.Length / 2;
			IDictionary<Int32, Int32> spaceOffsetsToParts = new Dictionary<Int32, Int32>();
			String[] parts = s.Split(' ');
			for (Int32 partIndex = 0, offset = 0; partIndex < parts.Length; partIndex++)
			{
				spaceOffsetsToParts.Add(offset, partIndex);
				offset += parts[partIndex].Length + 1; // +1 to count space that was removed by Split 
			}
			foreach (var pair in spaceOffsetsToParts.OrderBy(entry => Math.Abs(middle - entry.Key)))
			{
				count--;
				if (count < 0)
					break;
				parts[pair.Value] += ' ';
			}
			return String.Join(" ", parts);
		}

		/// <summary>
		/// Confirm that an email address is valid
		/// in format
		/// </summary>
		/// <param name="emailAddress">Full email address to validate</param>
		/// <returns>True if email address is valid</returns>
		public static bool ValidateEmailAddress(string emailAddress)
		{
			try
			{
				string TextToValidate = emailAddress;
				Regex expression = new Regex(@"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}");

				// test email address with expression
				if (expression.IsMatch(TextToValidate))
				{
					// is valid email address
					return true;
				}
				else
				{
					// is not valid email address
					return false;
				}
			}
			catch (System.Exception)
			{
				throw;
			}
		}



		//Começam aqui os Métodos de Compatibilazação com VB 6 .........Left() Right() Mid()
		public static string Left(string param, int length)
		{
			//we start at 0 since we want to get the characters starting from the
			//left and with the specified lenght and assign it to a variable
			if (param == "")
				return "";
			string result = param.Substring(0, length);
			//return the result of the operation
			return result;
		}

		public static string Right(string param, int length)
		{
			//start at the index based on the lenght of the sting minus
			//the specified lenght and assign it a variable
			if (param == "")
				return "";
			string result = param.Substring(param.Length - length, length);
			//return the result of the operation
			return result;
		}

		public static string Mid(string param, int startIndex, int length)
		{
			//start at the specified index in the string ang get N number of
			//characters depending on the lenght and assign it to a variable
			string result = param.Substring(startIndex, length);
			//return the result of the operation
			return result;
		}

		public static string Mid(string param, int startIndex)
		{
			//start at the specified index and return all characters after it
			//and assign it to a variable
			string result = param.Substring(startIndex);
			//return the result of the operation
			return result;
		}

		/// <summary>
		/// Verifica se sistema operativo é de 64 bits
		/// </summary>
		/// <param name="hProcess"></param>
		/// <param name="lpSystemInfo"></param>
		/// <returns></returns>
		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

		public bool Is64Bit()
		{

			IsWow64Process(Process.GetCurrentProcess().Handle, out bool retVal);

			return retVal;
		}

		public static bool Is64Bits()
		{
			if (IntPtr.Size == 8)
			{
				return true;
			}
			else
				return false;

		}

		/// <summary>
		/// Verifica se motor de dados usado no programa é o SQL Server
		/// </summary>
		/// <returns></returns>
		public bool IsSQLServer()
		{
			string connectionString = ApplicationConfiguration.DefaultConnection.ToString();

			if (connectionString.ToLower().Contains("sqlserver"))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Verifica se motor de dados usado no programa é o SQL Server
		/// </summary>
		/// <returns></returns>
		public bool IsAccess()
		{
			string connectionString = ApplicationConfiguration.DefaultConnection.ToString();

			if (connectionString.ToLower().StartsWith("msaccess"))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Verifica se motor de dados usado no programa é o MySQL
		/// </summary>
		/// <returns></returns>
		public bool IsMySQLServer()
		{
			string connectionString = ApplicationConfiguration.DefaultConnection.ToString();

			if (connectionString.ToLower().StartsWith("mysql"))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Verifica se motor de dados usado no programa é o MySQL
		/// </summary>
		/// <returns></returns>
		public bool IsSQLite()
		{
			string connectionString = ApplicationConfiguration.DefaultConnection.ToString();

			if (connectionString.ToLower().StartsWith("sqlite"))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Verifica se o sqlserver está instalado
		/// </summary>
		/// <returns>sucesso?</returns>
		public bool IsSqlServerInstalled()
		{
			bool blnInstalled = false;

			RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
			using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
			{
				RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
				if (instanceKey != null)
				{
					blnInstalled = true;
					foreach (var instanceName in instanceKey.GetValueNames())
					{
						Console.WriteLine(Environment.MachineName + @"\" + instanceName);
					}
				}
				return blnInstalled;
			}
		}

		/// <summary>
		/// Obtém info sobre o sistema operativo
		/// </summary>
		/// <returns></returns>
		public string GetOSInfo()
		{
			//Get Operating system information.
			OperatingSystem os = Environment.OSVersion;
			//Get version information about the os.
			System.Version vs = os.Version;

			//Variable to hold our return value
			string operatingSystem = "";

			if (os.Platform == PlatformID.Win32Windows)
			{
				//This is a pre-NT version of Windows
				switch (vs.Minor)
				{
					case 0:
						operatingSystem = "95";
						break;
					case 10:
						if (vs.Revision.ToString() == "2222A")
							operatingSystem = "98SE";
						else
							operatingSystem = "98";
						break;
					case 90:
						operatingSystem = "Me";
						break;
					default:
						break;
				}
			}
			else if (os.Platform == PlatformID.Win32NT)
			{
				switch (vs.Major)
				{
					case 3:
						operatingSystem = "NT 3.51";
						break;
					case 4:
						operatingSystem = "NT 4.0";
						break;
					case 5:
						if (vs.Minor == 0)
							operatingSystem = "2000";
						else
							operatingSystem = "XP";
						break;
					case 6:
						if (vs.Minor == 0)
							operatingSystem = "Vista";
						else
							operatingSystem = "7";
						break;
					default:
						break;
				}
			}
			//Make sure we actually got something in our OS check
			//We don't want to just return " Service Pack 2" or " 32-bit"
			//That information is useless without the OS version.
			if (operatingSystem != "")
			{
				//Got something.  Let's prepend "Windows" and get more info.
				operatingSystem = "Windows " + operatingSystem;
				//See if there's a service pack installed.
				if (os.ServicePack != "")
				{
					//Append it to the OS name.  i.e. "Windows XP Service Pack 3"
					operatingSystem += " " + os.ServicePack;
				}
				//Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
				operatingSystem += " " + GetOSArchitecture().ToString() + "-bit";
			}
			//Return the information we've gathered.
			return operatingSystem;
		}

		private static int GetOSArchitecture()
		{
			string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
			return ((String.IsNullOrEmpty(pa) || String.Compare(pa, 0, "x86", 0, 3, true) == 0) ? 32 : 64);
		}

		public bool IsAccessDatabaseEngineInstalled()
		{
			bool llretval = false;
			RegistryKey rkACDBKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products");
			if (rkACDBKey != null)
			{
				//int lnSubKeyCount = 0;
				//lnSubKeyCount =rkACDBKey.SubKeyCount; 
				foreach (string subKeyName in rkACDBKey.GetSubKeyNames())
				{
					using (RegistryKey RegSubKey = rkACDBKey.OpenSubKey(subKeyName))
					{
						foreach (string valueName in RegSubKey.GetValueNames())
						{
							if (valueName.ToUpper() == "PRODUCTNAME")
							{
								string AccessDBAsValue = (string)RegSubKey.GetValue(valueName.ToUpper());
								if (AccessDBAsValue.Contains("Access database engine"))
								{
									llretval = true;
									break;
								}
							}
						}
					}
					if (llretval)
					{
						break;
					}
				}
			}
			return llretval;
		}


		public bool IsAccessInstalled()
		{
			// Verifica se o Access está instalado neste PC,
			// ao verificar a chave HKEY_CLASSES_ROOT\Access.Application.
			using (var regAccess = Registry.ClassesRoot.OpenSubKey("Access.Application"))
			{
				if (regAccess == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}

		}

		public bool IsWordFileOpened()
		{
			System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
			if (processes != null)
			{
				if (processes.Length > 0)
					return true;
				else
					return false;
			}
			else
				return false;
		}
		public bool IsWordInstalled()
		{
			// Verifica se o Winword está instalado neste PC,
			// ao verificar a chave HKEY_CLASSES_ROOT\Word.Application.
			using (var regWord = Registry.ClassesRoot.OpenSubKey("Word.Application"))
			{
				if (regWord == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}

		}

		public bool IsExcelInstalled()
		{
			// Verifica se o Excel está instalado neste PC,
			// ao verificar a chave HKEY_CLASSES_ROOT\Excel.Application.
			using (var regWord = Registry.ClassesRoot.OpenSubKey("Excel.Application"))
			{
				if (regWord == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}

		}


		//public bool IsSQLServerRunning()
		//{
		//    SqlDataSourceEnumerator sqldatasourceenumerator1 = SqlDataSourceEnumerator.Instance;
		//    System.Data.DataTable datatable1 = sqldatasourceenumerator1.GetDataSources();
		//    //foreach (DataRow row in datatable1.Rows)
		//    //{
		//    //    Console.WriteLine("****************************************");
		//    //    Console.WriteLine("Server Name:" + row["ServerName"]);
		//    //    Console.WriteLine("Instance Name:" + row["InstanceName"]);
		//    //    Console.WriteLine("Is Clustered:" + row["IsClustered"]);
		//    //    Console.WriteLine("Version:" + row["Version"]);
		//    //    Console.WriteLine("****************************************");
		//    //}
		//    if (datatable1.Rows.Count > 0)
		//        return true;
		//    else
		//        return false;
		//}


		//public void DisableUnwantedExportFormat(ReportViewer ReportViewerID, string strFormatName)
		//{
		//    FieldInfo info;
		//    foreach (RenderingExtension extension in ReportViewerID.LocalReport.ListRenderingExtensions())
		//    {
		//        if (extension.Name == strFormatName)
		//        {
		//            info = extension.GetType().GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
		//            info.SetValue(extension, false);
		//        }
		//    }
		//}

		public static bool UsaNotifier()
		{
			string sUsaNotifier = ApplicationConfiguration.UsaNotifier ?? string.Empty;
			if (sUsaNotifier.ToLower() == "sim")
				return true;
			else
				return false;
		}

		public static int Tempo_Notifier()
		{
			int sUsaNotifier = ApplicationConfiguration.Tempo_Notifier;
			return sUsaNotifier;
		}

		public static string PastaListagens()
		{
			string sPastaImagens = ApplicationConfiguration.PastaListagens ?? string.Empty;
			return sPastaImagens;
		}

		public static string DefaultConnection()
		{
			string sConnection = ApplicationConfiguration.DefaultConnection ?? string.Empty;
			return sConnection;

		}

		public void UpdateConfig(string key, string value, string fileName)
		{
			var configFile = ConfigurationManager.OpenExeConfiguration(fileName);
			configFile.AppSettings.Settings[key].Value = value;
			try
			{
				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("appSettings/" + key);
				ConfigurationManager.OpenExeConfiguration(fileName);

				//configFile.Save();

			}
			catch
			{
				throw;
			}
		}

		//public static bool IsSQLServerExecutando()
		//{
		//    SqlDataSourceEnumerator sqldatasourceenumerator1 = SqlDataSourceEnumerator.Instance;
		//    System.Data.DataTable datatable1 = sqldatasourceenumerator1.GetDataSources();
		//    return datatable1.Rows.Count > 0;
		//}

		/// <summary>
		/// Verifica se base de dados (SQL Server) existe
		/// </summary>
		/// <param name="databaseName"></param>
		/// <returns></returns>
		public bool CheckOtherBDExists()
		{
			string connectString;
			string sTipoBD = "";
			Utilitarios Utils = new Utilitarios();

			if (Utils.IsSQLite())
				sTipoBD = "SqLite";
			if (Utils.IsAccess())
				sTipoBD = "MSAccessCon";

			connectString = ConfigurationManager.ConnectionStrings[sTipoBD].ToString();
			String[] parts = connectString.Split(';');

			string sDataSource;
			if (Utils.IsAccess())
				sDataSource = parts[1];
			else
				sDataSource = parts[0];

			sDataSource = sDataSource.Replace(@"|DataDirectory|",
				AppDomain.CurrentDomain.BaseDirectory);

			int iPos = sDataSource.IndexOf("=");
			string sFileLocation = sDataSource.Substring(iPos + 1);
			bool bRet;
			if (File.Exists(sFileLocation))
				bRet = true;
			else
				bRet = false;
			//else if (sTipoBD == "SqLite")
			//{
			//    CriaBD_Livros_SqLite(sFileLocation);
			//    bRet = true;
			//}
			return bRet;
		}

		public static bool IsUrlValid(string sUrlName)
		{
			try
			{
				Uri uriNew;
				uriNew = GetUri(sUrlName);
				string sUrl = uriNew.AbsoluteUri;

				bool result = Uri.TryCreate(sUrl, UriKind.Absolute, out Uri uriResult)
					&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
				return result;

			}
			catch
			{
				return false;
			}
		}

		private static Uri GetUri(string s)
		{
			return new UriBuilder(s).Uri;
		}

		public List<string> PopulateUrlList()
		{
			string regKey = "Software\\Microsoft\\Internet Explorer\\TypedURLs";
			RegistryKey subKey = Registry.CurrentUser.OpenSubKey(regKey);
			List<string> urlList = new List<string>();
			int counter = 1;
			while (true)
			{
				string sValName = "url" + counter.ToString();
				string url = (string)subKey.GetValue(sValName) ;
				if (url is null)
				{
					break; // TODO: might not be correct. Was : Exit While
				}
				urlList.Add(url);
				counter += 1;
			}
			return urlList;
		}

		public DataTable ConvertToDataTable<T>(IList<T> data)
		{
			PropertyDescriptorCollection properties =
			   TypeDescriptor.GetProperties(typeof(T));
			DataTable table = new DataTable();
			foreach (PropertyDescriptor prop in properties)
				table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
			foreach (T item in data)
			{
				DataRow row = table.NewRow();
				foreach (PropertyDescriptor prop in properties)
					row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
				table.Rows.Add(row);
			}
			return table;

		}

        public static string ValorPorExtenso(decimal valor)
        {
            int cent = 0;
            try
            {
                if (valor == 0)
                {
                    return "Zero Euros";
                }

                cent = Convert.ToInt32(decimal.Round((valor - (int)valor) * 100, MidpointRounding.ToEven));
                valor = (int)valor;

                if (cent > 0)
                {
                    return (valor, cent) switch
                    {
                        (1, _) => $"Um Euro e {getDecimal(Convert.ToByte(cent))} Cêntimos",
                        (0, _) => $"{getDecimal(Convert.ToByte(cent))} Cêntimos",
                        _ => $"{getInteger(valor)} Euros e {getDecimal(Convert.ToByte(cent))} Cêntimos",
                    };
                }
                else
                {
                    return (valor) switch
                    {
                        (1) => "Um Euro",
                        _ => $"{getInteger(valor)} Euros",
                    };
                }
            }
            catch
            {
                return "";
            }
        }

        public static string getDecimal(byte number)
        {
            try
            {
                return number switch
                {
                    0 => "",
                    >= 1 and <= 19 => new[]
                    {
                "Um", "Dois", "Três", "Quatro", "Cinco", "Seis", "Sete", "Oito", "Nove", "Dez",
                "Onze", "Doze", "Treze", "Quatorze", "Quinze", "Dezasseis", "Dezassete", "Dezoito", "Dezanove"
            }[number - 1] + " ",
                    >= 20 and <= 99 when (number % 10) == 0 => new[] { "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" }[number / 10 - 2],
                    >= 20 and <= 99 => $"{new[] { "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" }[number / 10 - 2]} e {getDecimal(Convert.ToByte(number % 10))}",
                    _ => "",
                };
            }
            catch
            {
                return "";
            }
        }

        public static string getInteger(decimal number)
        {
            try
            {
                number = (int)number;

                return number switch
                {
                    < 0 => $"-{getInteger(-number)}",
                    0 => "",
                    >= 1 and <= 19 => new[]
                    {
                "Um", "Dois", "Três", "Quatro", "Cinco", "Seis", "Sete", "Oito", "Nove", "Dez",
                "Onze", "Doze", "Treze", "Quatorze", "Quinze", "Dezasseis", "Dezassete", "Dezoito", "Dezanove"
            }[Convert.ToInt32(number) - 1] + " ",
                    >= 20 and <= 99 when (number % 10) == 0 => new[] { "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" }[Convert.ToInt32(number) / 10 - 2] + " ",
                    >= 20 and <= 99 => $"{new[] { "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" }[Convert.ToInt32(number) / 10 - 2]} e {getInteger(number % 10)}",
                    100 => "Cem ",
                    >= 101 and <= 999 when (number % 100) == 0 => new[] { "Cento", "Duzentos", "Trezentos", "Quatrocentos", "Quinhentos", "Seiscentos", "Setecentos", "Oitocentos", "Novecentos" }[Convert.ToInt32(number) / 100 - 1],
                    >= 101 and <= 999 => $"{new[] { "Cento", "Duzentos", "Trezentos", "Quatrocentos", "Quinhentos", "Seiscentos", "Setecentos", "Oitocentos", "Novecentos" }[Convert.ToInt32(number) / 100 - 1]} e {getInteger(number % 100)}",
                    >= 1000 and <= 1999 when (number % 1000 == 0) => "Mil ",
                    >= 1000 and <= 1999 => number % 100 <= 100 ? "Mil e " + getInteger(number % 1000) : "Mil, " + getInteger(number % 1000),
                    >= 2000 and <= 999999 when (number % 1000 == 0) => getInteger(number / 1000) + "Mil ",
                    >= 2000 and <= 999999 => number % 100 <= 100 ? getInteger(number / 1000) + "Mil e " + getInteger(number % 1000) : getInteger(number / 1000) + "Mil, " + getInteger(number % 1000),
                    >= 1000000 and <= 1999999 when (number % 1000000 == 0) => "Um Milhão ",
                    >= 1000000 and <= 1999999 => number % 100 <= 100 ? getInteger(number / 1000000) + "Milhão e " + getInteger(number % 1000000) : getInteger(number / 1000000) + "Milhão, " + getInteger(number % 1000000),
                    >= 2000000 and <= 999999999 when (number % 1000000 == 0) => getInteger(number / 1000000) + " Milhões ",
                    >= 2000000 and <= 999999999 => number % 100 <= 100 ? getInteger(number / 1000000) + "Milhões e " + getInteger(number % 1000000) : getInteger(number / 1000000) + "Milhões, " + getInteger(number % 1000000),
                    >= 1000000000 and <= 1999999999 when (number % 1000000000 == 0) => "Um Bilião ",
                    >= 1000000000 and <= 1999999999 => number % 100 <= 100 ? getInteger(number / 1000000000) + "Bilião e " + getInteger(number % 1000000000) : getInteger(number / 1000000000) + "Bilião, " + getInteger(number % 1000000000),
                    _ => getInteger(number / 1000000000) + " Biliões ",
                };
            }
            catch
            {
                return "";
            }
        }
    }

}

using System.Data;
using System.Globalization;

namespace PropertyManagerFLApplication.Utilities
{
	public static class Helpers
	{
		/// <summary>
		/// GetDataTableFrom2DArray
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="reportData"></param>
		/// <returns>Dados em forma de DataTable</returns>
		public static DataTable GetDataTableFrom2DArray(string[] columnName, string[,] reportData)
		{
			DataTable table = new DataTable();
			int cols = columnName.Length;
			DataColumn[] dataColumn = new DataColumn[cols];

			for (int c = 0; c < cols; c++)
			{
				dataColumn[c] = new DataColumn(columnName[c], typeof(string));
				table.Columns.Add(dataColumn[c]);
			}

			string[,] reportArray = reportData;

			for (int i = 0; i < reportArray.GetUpperBound(0) + 1; i++)
			{
				DataRow row = table.NewRow();

				for (int columns = 0; columns < dataColumn.Length; columns++)
				{
					row[dataColumn[columns]] = reportArray[i, columns];
				}

				table.Rows.Add(row);
			}
			return table;
		}

		/// <summary>
		/// Lê ficheiro de texto (acerca de...)
		/// </summary>
		/// <returns>Dados sobre 'acerca de...'</returns>
		public static string ReadeTxtFile()
		{
			string filePath = System.Environment.CurrentDirectory + "\\AboutPubliSoft.txt";
			string aboutData = string.Empty;

			if (File.Exists(filePath))
			{
				try
				{
					using (StreamReader reader = new StreamReader(filePath))
					{

						while (reader.Peek() != -1)
						{
							aboutData = aboutData + reader.ReadLine().Trim() + "\n";
						}
					}
				}
				catch (Exception ex)
				{
					//Logger.WriteLog(ex);
					throw new ApplicationException(ex.ToString());
				}
			}
			else
			{
				return "Não foi possível obter informação sobre PropertyManagerFL.";
			}
			return aboutData;
		}

		/// <summary>
		/// Descodifica Mes/Ano
		/// </summary>
		/// <param name="month"></param>
		/// <param name="year"></param>
		/// <returns>MesAno</returns>
		public static string DecodeMonthYear(string month, string year)
		{
			year = year.Substring(2);
			string monthYear = string.Empty;

			switch (month)
			{
				case "Jan":
					monthYear = "01" + year;
					break;
				case "Feb":
					monthYear = "02" + year;
					break;
				case "Mar":
					monthYear = "03" + year;
					break;
				case "Apr":
					monthYear = "04" + year;
					break;
				case "May":
					monthYear = "05" + year;
					break;
				case "Jun":
					monthYear = "06" + year;
					break;
				case "Jul":
					monthYear = "07" + year;
					break;
				case "Aug":
					monthYear = "08" + year;
					break;
				case "Sep":
					monthYear = "09" + year;
					break;
				case "Oct":
					monthYear = "10" + year;
					break;
				case "Nov":
					monthYear = "11" + year;
					break;
				case "Dec":
					monthYear = "12" + year;
					break;
			}

			return monthYear;
		}

		public static string DecodeMonthYear(int month, int year)
		{
			string OutputDate = "";
			switch(month)
			{
                case 1:
                    OutputDate = "Janeiro";
                    break;
                case 2:
                    OutputDate = "Fevereiro";
                    break;
                case 3:
                    OutputDate = "Março";
                    break;
                case 4:
                    OutputDate = "Abril";
                    break;
                case 5:
                    OutputDate = "Maio";
                    break;
                case 6:
                    OutputDate = "Junho";
                    break;
                case 7:
                    OutputDate = "Julho";
                    break;
                case 8:
                    OutputDate = "Agosto";
                    break;
                case 9:
                    OutputDate = "Setembro";
                    break;
                case 10:
                    OutputDate = "Outubro";
                    break;
                case 11:
                    OutputDate = "Novembro";
                    break;
                case 12:
                    OutputDate = "Dezembro";
                    break;
            }

			return $"{OutputDate} de {year}";
        }

		/// <summary>
		/// Obtém mês anterior
		/// </summary>
		/// <param name="month">mês</param>
		/// <returns>Iniciais do mês</returns>
		public static string GetPreviousMonth(string month)
		{
			string previousMonth = string.Empty;

			switch (month)
			{
				case "Jan":
					previousMonth = "Dec";
					break;
				case "Feb":
					previousMonth = "Jan";
					break;
				case "Mar":
					previousMonth = "Feb";
					break;
				case "Apr":
					previousMonth = "Mar";
					break;
				case "May":
					previousMonth = "Apr";
					break;
				case "Jun":
					previousMonth = "May";
					break;
				case "Jul":
					previousMonth = "Jun";
					break;
				case "Aug":
					previousMonth = "Jul";
					break;
				case "Sep":
					previousMonth = "Aug";
					break;
				case "Oct":
					previousMonth = "Sep";
					break;
				case "Nov":
					previousMonth = "Oct";
					break;
				case "Dec":
					previousMonth = "Nov";
					break;
			}

			return previousMonth;
		}

		/// <summary>
		/// Obtém mês seguinte
		/// </summary>
		/// <param name="month">mês</param>
		/// <returns>Iniciais do mês</returns>
		public static string GetNextMonth(string month)
		{
			string nextMonth = string.Empty;

			switch (month)
			{
				case "Jan":
					nextMonth = "Fev";
					break;
				case "Feb":
					nextMonth = "Mar";
					break;
				case "Mar":
					nextMonth = "Abr";
					break;
				case "Apr":
					nextMonth = "Mai";
					break;
				case "May":
					nextMonth = "Jun";
					break;
				case "Jun":
					nextMonth = "Jul";
					break;
				case "Jul":
					nextMonth = "Ago";
					break;
				case "Aug":
					nextMonth = "Set";
					break;
				case "Sep":
					nextMonth = "Out";
					break;
				case "Oct":
					nextMonth = "Nov";
					break;
				case "Nov":
					nextMonth = "Dez";
					break;
				case "Dec":
					nextMonth = "Jan";
					break;
			}

			return nextMonth;
		}

		/// <summary>
		/// GetPrevMonthsYear
		/// </summary>
		/// <param name="month"></param>
		/// <param name="year"></param>
		/// <returns></returns>
		public static string GetPrevMonthsYear(string month, string year)
		{
			int prevMonthsYear = 0;
			if (month.Equals("Jan"))
				prevMonthsYear = Convert.ToInt16(year) - 1;
			else
				prevMonthsYear = Convert.ToInt16(year);

			return prevMonthsYear.ToString();
		}

		/// <summary>
		/// GetNextMonthsYear
		/// </summary>
		/// <param name="month"></param>
		/// <param name="year"></param>
		/// <returns></returns>
		public static string GetNextMonthsYear(string month, string year)
		{
			int nextMonthsYear = 0;
			if (month.Equals("Dec"))
				nextMonthsYear = Convert.ToInt16(year) + 1;
			else
				nextMonthsYear = Convert.ToInt16(year);

			return nextMonthsYear.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="month"></param>
		/// <returns></returns>
		public static int GetMonth(string month)
		{
			int iMonth = 0;
			switch (month)
			{
				case "Jan":
					iMonth = 1;
					break;
				case "Feb":
					iMonth = 2;
					break;
				case "Mar":
					iMonth = 3;
					break;
				case "Apr":
					iMonth = 4;
					break;
				case "May":
					iMonth = 5;
					break;
				case "Jun":
					iMonth = 6;
					break;
				case "Jul":
					iMonth = 7;
					break;
				case "Aug":
					iMonth = 8;
					break;
				case "Sep":
					iMonth = 9;
					break;
				case "Oct":
					iMonth = 10;
					break;
				case "Nov":
					iMonth = 11;
					break;
				case "Dec":
					iMonth = 12;
					break;
			}

			return iMonth;
		}

		public static string ToTitleCase(this string str)
		{
			var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
			return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
		}

		/// <summary>
		/// Overload which uses the culture info with the specified name
		/// </summary>
		public static string ToTitleCase(this string str, string cultureInfoName)
		{
			var cultureInfo = new CultureInfo(cultureInfoName);
			return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
		}

		/// <summary>
		/// Overload which uses the specified culture info
		/// </summary>
		public static string ToTitleCase(this string str, CultureInfo cultureInfo)
		{
			return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
		}
	}
}

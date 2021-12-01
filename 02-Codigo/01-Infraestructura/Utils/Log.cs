using System;
using System.IO;

namespace Utils
{
    /// <summary>
    /// Clase para recoger y mostrar los errores de la aplicación
    /// </summary>
    public class Log
    {
        //--------------------------------------------------------------------
        #region Variables y constantes
        private const string FileLog = "Log.txt";
        private static string Path = @"{0}\{1}";
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procedimientos y funciones
        /// <summary>
        /// Crea una entrada en el log de errores
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="Critico"></param>
        public static void SetLog(Exception ex, bool Critico)
        {
            //1.- Comprobamos que el directorio / archivo existen. Si no existen, creamos...
            Path = string.Format(Path, AppDomain.CurrentDomain.BaseDirectory, FileLog);
            if (!File.Exists(Path))
            {
                var myFile = File.Create(Path);
                myFile.Close();
            }
            else
            {
                RemoveContentFile(Path);
            }

            GC.Collect();

            //2.- Recogemos valores
            string sFecha = "{0} - {1}:{2}";
            sFecha = string.Format(sFecha, FuncUtils.GetDateFormatDDMMAAAA(DateTime.Now), DateTime.Now.Hour, DateTime.Now.Minute);

            string sCritic = "No";
            if (Critico)
            {
                sCritic = "Si";
            }

            string sOrigen = ex.Source;
            string sErr = ex.Message;

            string StackTrace = ex.StackTrace;

            //3.- Configuramos línea
            string sLine = "Fecha y hora: {0} - Error crítico: {1} - Origen del error: {2} - Descripción del error {3} - Traza del error: {4}";
            sLine = string.Format(sLine, sFecha, sCritic, sOrigen, sErr, StackTrace);

            using (StreamWriter w = File.AppendText(Path))
            {
                w.WriteLine(sLine);
                w.WriteLine("*******************************************************************");
            }
        }

        /// <summary>
        /// Vacía el fichero de log si este sobrepasa las 5mb
        /// </summary>
        /// <param name="Path"></param>
        private static void RemoveContentFile(string Path)
        {
            const long LengMax = 5000000;
            var myFile = File.Open(Path, FileMode.Open);
            bool RemoveContent = false;

            if (myFile.Length >= LengMax)
            {
                RemoveContent = true;
            }

            myFile.Dispose();

            if (RemoveContent)
            {
                File.WriteAllText(Path, string.Empty);
            }
        }
        #endregion
        //--------------------------------------------------------------------
    }
}

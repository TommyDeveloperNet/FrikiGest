using System.Text.RegularExpressions;
using System.Net.Mail;
using System;
using System.Globalization;

namespace Utils
{
    /// <summary>
    /// Clase de funciones genéricas
    /// </summary>
    public class FuncUtils
    {
        /// <summary>
        /// Valida si una cadena de texto es alfanumerica (solo se permiten letras y números)
        /// </summary>
        /// <param name="sVal">Cadena de texto a validar</param>
        /// <returns>TRUE si la cadena es alfanumerica; FALSE si no lo es</returns>
        public static bool IsAlphanumeric(string sVal)
        {
            //Regex rg = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$");
            //Regex rg = new Regex(@"^[a-zA-Z][a-zA-Z0-9][a-zA-Z0-9(?=.*[\.@_-])]*$");
            //return rg.IsMatch(sVal);
            Regex rg = new Regex(@"(?=.*[#$%])"); //Caracteres especiales no permitidos: # $ %
            if (rg.IsMatch(sVal))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Valida si una cadena de texto es un email válido o no
        /// </summary>
        /// <param name="sVal">Cadena de texto a validar</param>
        /// <returns>TRUE si la cadena es un email válido; FALSE si no lo es</returns>
        public static bool IsEmail(string sVal)
        {
            try
            {
                MailAddress objMail = new MailAddress(sVal);
                return true;
            }
            catch (Exception e)
            {
                Log.SetLog(e, false);
                return false;
            }
        }

        /// <summary>
        /// Valida si un valor concreto es un valor numérico valido o no.
        /// </summary>
        /// <param name="sVal">Cadena a validar</param>
        /// <param name="NumDec">Número de decimales. Si este valor es 0, el valor introducido es un Entero; de lo contrario es un decimal</param>
        /// <returns></returns>
        public static bool IsNumeric(string sVal, int NumDec = 0)
        {
            //Declaración
            bool bResult = false;
            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            string SeparadorDecimal = nfi.NumberDecimalSeparator;
            string sRegex = @"[0-9]";

            //Código
            sRegex = string.Format(sRegex, SeparadorDecimal);
            Regex rg = new Regex(sRegex);
            if (rg.IsMatch(sVal))
            {
                if (NumDec == 0)
                {
                    if (int.TryParse(sVal, out int iNumber))
                    {
                        bResult = true;
                    }
                }
                else
                {
                    if (double.TryParse(sVal, out double dNumber))
                    {
                        if (CountDec(sVal) <= NumDec)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                bResult = false;
            }

            //Resultado
            return bResult;
        }

        /// <summary>
        /// Cuenta el nº de decimales
        /// </summary>
        /// <param name="sVal"></param>
        /// <returns></returns>
        private static int CountDec(string sVal)
        {
            //Declaración
            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            string SeparadorDecimal = nfi.NumberDecimalSeparator;
            int iDec = 0;

            //Código
            if (sVal.Contains(SeparadorDecimal))
            {
                float numDecimal = float.Parse(sVal.Split(SeparadorDecimal[0])[1]);
                iDec = numDecimal.ToString().Length;
            }

            //Resultado
            return iDec;
        }

        /// <summary>
        /// Devuelve un valor de tipo Double
        /// </summary>
        /// <param name="sVal">Cadena de texto</param>
        /// <param name="RoundDec">Nº decimales a redondear</param>
        public static double GetDouble(string sVal, int RoundDec = 2)
        {
            //Declaración
            double dResult = 0;

            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            string SeparadorDecimal = nfi.NumberDecimalSeparator;

            //Código
            sVal = sVal.Replace(".", SeparadorDecimal);

            if (!double.TryParse(sVal, out dResult))
            {
                dResult = 0;
            }
            else
            {
                //Redondeamos
                dResult = Math.Round(dResult, RoundDec);
            }

            //Resultado
            return dResult;
        }

        /// <summary>
        /// Devuelve un valor formateado con x decimales
        /// </summary>
        /// <param name="dValue">Valor numérico a formatear</param>
        /// <param name="RoundDec">Nº de decimales</param>
        /// <returns></returns>
        public static string GetDoubleFormat(double dValue, int RoundDec = 2)
        {
            return dValue.ToString("N" + RoundDec);
        }

        /// <summary>
        /// Devuelve un valor de fecha formateado en DD/MM/AAAA
        /// </summary>
        /// <param name="dtValue"></param>
        /// <returns></returns>
        public static string GetDateFormatDDMMAAAA(DateTime dtValue)
        {
            //Declaracíon
            string sResult;

            //Código
            sResult = dtValue.ToString("dd/MM/yyyy");

            //Resultado
            return sResult.Trim();
        }
    }
}

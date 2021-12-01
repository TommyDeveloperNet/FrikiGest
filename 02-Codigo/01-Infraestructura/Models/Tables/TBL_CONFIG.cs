using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Tables
{
    [Table("TBL_CONFIG")]
    [Serializable()]
    public class TBL_CONFIG
    {
        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }

        [Required()]
        [StringLength(100)]
        [Column(Order = 1)]
        public string DEFINICION { get; set; }

        [Column(Order = 2)]
        public int HEIGHT { get; set; }

        [Column(Order = 3)]
        public int WIDTH { get; set; }

        [Column(Order = 4)]
        public bool PANTALLA_COMPLETA { get; set; }

        /// <summary>
        /// Crea un objeto de tipo TBL_CONFIG con los elementos a añadir / actualizar
        /// </summary>
        /// <param name="sDefinicion">OBLIGATORIO - Definición del elemento de configuración</param>
        /// <param name="iHeight">OBLIGATORIO - Alto del formulario principal</param>
        /// <param name="iWidth">OBLIGATORIO - Ancho del formulario principal</param>
        /// <param name="bPantallaCompleta">OBLIGATORIO - Pantalla completa si (true) o no (false)</param>
        /// <param name="iID">OPCIONAL - ID del elemento / Registro. Si es 0 (valor por defecto) es el elemento o registro es para un insert. Si es distinto de 0 es para un update</param>
        /// <returns></returns>
        public static TBL_CONFIG Create(string sDefinicion, int iHeight, int iWidth, bool bPantallaCompleta, int iID = 0)
        {
            //Declaración
            TBL_CONFIG objModel;

            //Código
            //-Validaciones...

            //-Asignamos datos
            objModel = new TBL_CONFIG()
            {
                ID = iID,
                DEFINICION = sDefinicion,
                HEIGHT = iHeight,
                WIDTH = iWidth,
                PANTALLA_COMPLETA = bPantallaCompleta
            };

            //Resultado
            return objModel;
        }
    }
}
